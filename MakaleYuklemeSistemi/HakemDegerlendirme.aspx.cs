using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;

namespace MakaleYuklemeSistemi
{
    public partial class HakemDegerlendirme : System.Web.UI.Page
    {
        private readonly string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";
        private readonly string pythonPath = @"C:\\Users\\EZGI\\AppData\\Local\\Programs\\Python\\Python311\\python.exe";
        private readonly string scriptPath = @"C:\\Users\\EZGI\\OneDrive\\Masaüstü\\yazlab2proje1\\MakaleYuklemeSistemi\\Scripts\\evaluation.py";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string hakemID = Request.QueryString["ID"];
                if (!string.IsNullOrEmpty(hakemID))
                {
                    lblHakemIsim.Text = "Hakem ID: " + hakemID;
                    LoadPdfFiles(hakemID);
                }
            }
        }

        private void LoadPdfFiles(string hakemID)
        {
            string query = @"
                SELECT k.Id, k.SifrelenmisDosyaninYolu 
                FROM Makale_Hakem mh 
                JOIN KullaniciDosya k ON mh.MakaleID = k.Id 
                WHERE mh.HakemID = @HakemID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@HakemID", hakemID);
                SqlDataReader reader = cmd.ExecuteReader();

                gvMakaleDosyalar.DataSource = reader;
                gvMakaleDosyalar.DataBind();
                reader.Close();
            }
        }

        protected void gvMakaleDosyalar_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "OpenFile")
            {
                string fullPath = e.CommandArgument.ToString();
                string fileName = Path.GetFileName(fullPath);
                Response.Redirect($"MakaleGoruntule.aspx?file={fileName}");
            }
            else if (e.CommandName == "Evaluate")
            {
                string makaleID = e.CommandArgument.ToString();
                ViewState["MakaleID"] = makaleID;
                pnlDegerlendirme.Visible = true;
            }
            else if (e.CommandName == "ViewMessage")
            {
                string id = e.CommandArgument.ToString();
                string query = "SELECT Mesaj FROM Mesajlar WHERE MakaleID = @MakaleID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MakaleID", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    lblMesaj.Text = "";
                    while (reader.Read())
                    {
                        lblMesaj.Text += reader["Mesaj"].ToString() + "<br/><hr/>";
                    }
                    reader.Close();
                    lblMesaj.Visible = true;
                }
            }
        }



        protected void btnGonder_Click(object sender, EventArgs e)
        {
            try
            {
                string makaleID = ViewState["MakaleID"].ToString();
                string yorum = txtDegerlendirme.Text;
                string dosyaYolu = GetFilePath(makaleID);

                // KullaniciDosya tablosundan makaleID ile eşleşen veriyi al
                string query = "SELECT Email, TakipNumarasi FROM KullaniciDosya WHERE Id = @MakaleID";

                // İlk SqlConnection'ı oluşturun ve açın
                using (SqlConnection con1 = new SqlConnection(connectionString))
                {
                    con1.Open();
                    SqlCommand cmd = new SqlCommand(query, con1);
                    cmd.Parameters.AddWithValue("@MakaleID", makaleID);

                    // SqlDataReader kullanın
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string email = reader["Email"].ToString();
                            string takipNumarasi = reader["TakipNumarasi"].ToString();

                            // KullaniciDosya tablosunda Durumu güncelle
                            string updateQuery = "UPDATE KullaniciDosya SET Durumu = 'Hakem Değerlendirdi, Yönetici Onayında' WHERE Id = @MakaleID";

                            // İkinci SqlConnection'ı oluşturun ve açın
                            using (SqlConnection con2 = new SqlConnection(connectionString))
                            {
                                con2.Open();
                                SqlCommand updateCmd = new SqlCommand(updateQuery, con2);
                                updateCmd.Parameters.AddWithValue("@MakaleID", makaleID);
                                updateCmd.ExecuteNonQuery();
                            }

                            // Logs tablosuna yeni bir kayıt ekle
                            string insertLogQuery = "INSERT INTO Logs (DegisiklikTarihi, Durumu, Email, TakipNumarasi) VALUES (@DegisiklikTarihi, @Durumu, @Email, @TakipNumarasi)";

                            // Üçüncü SqlConnection'ı oluşturun ve açın
                            using (SqlConnection con3 = new SqlConnection(connectionString))
                            {
                                con3.Open();
                                SqlCommand insertLogCmd = new SqlCommand(insertLogQuery, con3);
                                insertLogCmd.Parameters.AddWithValue("@DegisiklikTarihi", DateTime.Now);
                                insertLogCmd.Parameters.AddWithValue("@Durumu", "Hakem Değerlendirdi, Yönetici Onayında");
                                insertLogCmd.Parameters.AddWithValue("@Email", email);
                                insertLogCmd.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                                insertLogCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                // Python script'ini çalıştır
                RunPythonScript(dosyaYolu, yorum);
            }
            catch (Exception ex)
            {
                // Hata mesajını kullanıcıya göster
                //lblError.Text = $"An error occurred: {ex.Message}";
                // Hata detaylarını loglayabilirsiniz
                //Console.WriteLine($"Error in btnGonder_Click: {ex.StackTrace}");
            }
        }


        private string GetFilePath(string makaleID)
        {
            string filePath = "";
            string query = "SELECT SifrelenmisDosyaninYolu FROM KullaniciDosya WHERE Id = @MakaleID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MakaleID", makaleID);
                filePath = cmd.ExecuteScalar()?.ToString();
            }
            return filePath;
        }

        private void RunPythonScript(string filePath, string yorum)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = pythonPath;
                psi.Arguments = $"\"{scriptPath}\" \"{filePath}\" \"{yorum}\"";  // Dosya yolu ve yorum parametre olarak gönderiliyor.
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;
                psi.CreateNoWindow = true;

                Process process = new Process();
                process.StartInfo = psi;
                process.Start();

                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Hata varsa, sadece hata mesajını göster
                if (!string.IsNullOrEmpty(error))
                {
                    lblError.Text = $"Python script error: {error}";
                }
                // Başarılı ise sadece başarılı mesajı göster
                else
                {
                    lblSuccess.Text = $"Python script executed successfully: {result}";
                }
            }
            catch (Exception ex)
            {
                // Hata mesajını kullanıcıya göster
                lblError.Text = $"Error running Python script: {ex.Message}";
                // Hata detaylarını loglayabilirsiniz
                Console.WriteLine($"Error in RunPythonScript: {ex.StackTrace}");
            }
        }


        protected void btnMesajGoruntule_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string id = btn.CommandArgument.ToString();
            string query = "SELECT Mesaj FROM Mesajlar WHERE MakaleID = @MakaleID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MakaleID", id);
                SqlDataReader reader = cmd.ExecuteReader();

                lblMesaj.Text = "";
                while (reader.Read())
                {
                    lblMesaj.Text += reader["Mesaj"].ToString() + "<br/><hr/>";
                }
                reader.Close();
                lblMesaj.Visible = true;
            }
        }
    }

}