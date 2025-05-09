using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MakaleYuklemeSistemi
{
    public partial class HakemSecimi : System.Web.UI.Page
    {
        private string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";
        private readonly string pythonPath = @"C:\Users\EZGI\AppData\Local\Programs\Python\Python311\python.exe";
        private readonly string scriptPath = @"C:\Users\EZGI\OneDrive\Masaüstü\yazlab2proje1\MakaleYuklemeSistemi\Scripts\keyword_extraction.py";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MakaleleriListele();
            }
        }

        private void MakaleleriListele()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT TakipNumarasi, Email FROM KullaniciDosya WHERE Durumu = 'Hakeme İletilme Aşamasında'";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        gvMakaleler.DataSource = dt;
                        gvMakaleler.DataBind();
                    }
                }
            }
        }

        protected void btnHakemeIlet_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            string takipNumarasi = row.Cells[0].Text;

            string dosyaYolu = GetMakaleDosyaYolu(takipNumarasi);
            if (!string.IsNullOrEmpty(dosyaYolu))
            {
                string keywords = RunPythonScript(dosyaYolu);
                lblSonuc.Text = "Anahtar Kelimeler: " + keywords;

                var keywordList = keywords.Split(new[] { ", " }, StringSplitOptions.None).ToList();
                List<int> matchedIds = GetMatchingKonuIds(keywordList);

                if (matchedIds.Count > 0)
                {
                    var uniqueMatchedIds = matchedIds.Distinct().ToList();
                    lblSonuc.Text += "<br/>Eşleşen Konu ID'leri: " + string.Join(", ", uniqueMatchedIds);

                    List<string> hakemIsimleri = GetHakemIsimleriByIlgiAlaniIds(uniqueMatchedIds, out int selectedHakemId);
                    if (hakemIsimleri.Count > 0)
                    {
                        lblSonuc.Text += "<br/>İlgili Hakemler: " + string.Join(", ", hakemIsimleri);
                        lblSonuc.Text += "<br/>Seçilen Hakem: " + hakemIsimleri.FirstOrDefault(h => GetHakemIdByName(h) == selectedHakemId);

                        int makaleId = GetMakaleIdByTakipNumarasi(takipNumarasi);
                        if (makaleId > 0 && selectedHakemId > 0)
                        {
                            AssignMakaleToHakem(makaleId, selectedHakemId);
                            UpdateMakaleStatusAndDate(makaleId);
                            UpdateLogsTable(makaleId);
                        }
                    }
                    else
                    {
                        lblSonuc.Text += "<br/>İlgili Hakem Bulunamadı.";
                    }
                }
                else
                {
                    lblSonuc.Text += "<br/>Eşleşme Yok.";
                }
            }
            else
            {
                lblSonuc.Text = "Dosya yolu bulunamadı.";
            }
        }

        private void UpdateMakaleStatusAndDate(int makaleId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "UPDATE KullaniciDosya SET IncelemeTarihi = @IncelemeTarihi, Durumu = @Durumu WHERE ID = @MakaleID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IncelemeTarihi", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Durumu", "Hakem İncelemekte");
                    cmd.Parameters.AddWithValue("@MakaleID", makaleId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private string GetMakaleDosyaYolu(string takipNumarasi)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT DosyaYolu FROM KullaniciDosya WHERE TakipNumarasi = @TakipNumarasi";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : string.Empty;
                }
            }
        }

        private string RunPythonScript(string filePath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = pythonPath;
                psi.Arguments = $"\"{scriptPath}\" \"{filePath}\"";
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                return "Hata: " + ex.Message;
            }
        }

        private List<int> GetMatchingKonuIds(List<string> keywords)
        {
            List<int> matchedIds = new List<int>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Konu FROM IlgiAlanlari";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string konu = reader["Konu"].ToString().ToLower();

                            // Her anahtar kelimeyi konu ile kontrol et
                            foreach (var keyword in keywords)
                            {
                                if (konu.Contains(keyword.ToLower()))
                                {
                                    matchedIds.Add(Convert.ToInt32(reader["ID"]));
                                    break;  // Bu anahtar kelime ile eşleşme bulundu, diğer anahtar kelimelerle eşleşmeyi durdur
                                }
                            }
                        }
                    }
                }
            }

            return matchedIds;
        }

        private List<string> GetHakemIsimleriByIlgiAlaniIds(List<int> ilgiAlaniIds, out int selectedHakemId)
        {
            List<string> hakemIsimleri = new List<string>();
            List<int> hakemIds = new List<int>();
            selectedHakemId = 0;

            if (ilgiAlaniIds.Count == 0)
                return hakemIsimleri;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT DISTINCT h.ID, h.Isim
                         FROM Hakemler h
                         JOIN Hakem_IlgiAlani hi ON h.ID = hi.HakemID
                         WHERE hi.IlgiAlaniID IN (" + string.Join(",", ilgiAlaniIds.Select((s, i) => "@Id" + i)) + ")";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    for (int i = 0; i < ilgiAlaniIds.Count; i++)
                    {
                        cmd.Parameters.AddWithValue("@Id" + i, ilgiAlaniIds[i]);
                    }

                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hakemIsimleri.Add(reader["Isim"].ToString());
                            hakemIds.Add(Convert.ToInt32(reader["ID"]));
                        }
                    }
                }
            }

            if (hakemIds.Count > 0)
            {
                Random rnd = new Random();
                int index = rnd.Next(hakemIds.Count);
                selectedHakemId = hakemIds[index];
            }

            return hakemIsimleri;
        }

        private int GetHakemIdByName(string hakemIsmi)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ID FROM Hakemler WHERE Isim = @Isim";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Isim", hakemIsmi);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private void AssignMakaleToHakem(int makaleId, int hakemId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Makale_Hakem (MakaleID, HakemID) VALUES (@MakaleID, @HakemID)";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@MakaleID", makaleId);
                    cmd.Parameters.AddWithValue("@HakemID", hakemId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private int GetMakaleIdByTakipNumarasi(string takipNumarasi)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT ID FROM KullaniciDosya WHERE TakipNumarasi = @TakipNumarasi";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }


        private void UpdateLogsTable(int makaleId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // KullaniciDosya tablosundan Email ve TakipNumarasi'ni al
                string querySelect = "SELECT Email, TakipNumarasi FROM KullaniciDosya WHERE ID = @MakaleID";

                string email = "";
                string takipNumarasi = "";

                using (SqlCommand cmdSelect = new SqlCommand(querySelect, con))
                {
                    cmdSelect.Parameters.AddWithValue("@MakaleID", makaleId);
                    con.Open();
                    using (SqlDataReader reader = cmdSelect.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            email = reader["Email"].ToString();
                            takipNumarasi = reader["TakipNumarasi"].ToString();
                        }
                    }
                }

                // Logs tablosuna yeni bir kayıt ekle
                string queryInsert = "INSERT INTO Logs (DegisiklikTarihi, Durumu, Email, TakipNumarasi) " +
                                     "VALUES (@DegisiklikTarihi, @Durumu, @Email, @TakipNumarasi)";

                using (SqlCommand cmdInsert = new SqlCommand(queryInsert, con))
                {
                    cmdInsert.Parameters.AddWithValue("@DegisiklikTarihi", DateTime.Now);
                    cmdInsert.Parameters.AddWithValue("@Durumu", "Hakem İncelemekte");
                    cmdInsert.Parameters.AddWithValue("@Email", email);
                    cmdInsert.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                    cmdInsert.ExecuteNonQuery();
                }
            }
        }


    }
}
