using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MakaleYuklemeSistemi
{
    public partial class KullaniciyaGeriIletme : System.Web.UI.Page
    {
        private readonly string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";
        private readonly string pythonPath = @"C:\Users\EZGI\AppData\Local\Programs\Python\Python311\python.exe";
        private readonly string scriptPath = @"C:\Users\EZGI\OneDrive\Masaüstü\yazlab2proje1\MakaleYuklemeSistemi\Scripts\pdf_decrypt.py";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MakaleListele();
            }
        }

        private void MakaleListele()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT TakipNumarasi, Email, SifrelenmisDosyaninYolu FROM KullaniciDosya WHERE Durumu = 'Hakem Değerlendirdi, Yönetici Onayında'";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gvMakaleler.DataSource = dt;
                    gvMakaleler.DataBind();
                }
            }
        }

        protected void btnAc_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            string dosyaYolu = btn.CommandArgument;
            if (!string.IsNullOrEmpty(dosyaYolu))
            {
                string dosyaAdi = Path.GetFileName(dosyaYolu);
                string downloadUrl = "~/Uploads/" + dosyaAdi;
                Response.Redirect(downloadUrl);
            }
        }

        protected void btnDesifreleVeIlet_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            string sifrelenmisDosyaYolu = btn.CommandArgument;

            if (!string.IsNullOrEmpty(sifrelenmisDosyaYolu))
            {
                try
                {
                    string dizin = Path.GetDirectoryName(sifrelenmisDosyaYolu);
                    string jsonDosyaYolu = Path.ChangeExtension(sifrelenmisDosyaYolu, ".json");
                    string dosyaAdi = Path.GetFileNameWithoutExtension(sifrelenmisDosyaYolu) + "_decrypted.pdf";
                    string desifreDosyaYolu = Path.Combine(dizin, dosyaAdi);

                    if (!File.Exists(sifrelenmisDosyaYolu))
                        throw new FileNotFoundException("Şifrelenmiş PDF dosyası bulunamadı!");

                    if (!File.Exists(jsonDosyaYolu))
                        throw new FileNotFoundException("Şifreleme bilgilerini içeren JSON dosyası bulunamadı!");

                    // Python betiğini çalıştır
                    bool success = RunPythonScript(sifrelenmisDosyaYolu, desifreDosyaYolu, jsonDosyaYolu);

                    if (success && File.Exists(desifreDosyaYolu))
                    {
                        lblMessage.Text = "✅ Makale başarıyla deşifre edildi.";
                        lblMessage.CssClass = "alert alert-success text-center w-100";
                        lblMessage.Visible = true;

                        // Veritabanına deşifre dosya yolunu kaydet
                        SaveDecryptedFilePath(sifrelenmisDosyaYolu, desifreDosyaYolu);

                        // İndirme linkini oluştur
                        string downloadUrl = "~/Uploads/" + dosyaAdi;
                        //lnkDownload.NavigateUrl = downloadUrl;
                        //lnkDownload.Text = "📂 PDF'yi İndir";
                        //lnkDownload.Visible = true;
                    }
                    else
                    {
                        lblMessage.Text = "❌ Deşifreleme işlemi başarısız oldu!";
                        lblMessage.CssClass = "alert alert-danger text-center w-100";
                        lblMessage.Visible = true;
                        //lnkDownload.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "⚠ Hata: " + ex.Message;
                    lblMessage.CssClass = "alert alert-danger text-center w-100";
                    lblMessage.Visible = true;
                }
            }
        }


        private bool RunPythonScript(string encryptedFilePath, string decryptedFilePath, string jsonFilePath)
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"\"{scriptPath}\" \"{encryptedFilePath}\" \"{decryptedFilePath}\" \"{jsonFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = start })
                {
                    process.Start();
                    string result = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        lblMessage.Text = "⚠ Python hatası: " + error;
                        lblMessage.CssClass = "alert alert-warning text-center w-100";
                        lblMessage.Visible = true;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "⚠ Hata oluştu: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger text-center w-100";
                lblMessage.Visible = true;
                return false;
            }
        }

        private void SaveDecryptedFilePath(string encryptedFilePath, string decryptedFilePath)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE KullaniciDosya SET DesifrelenmisDosyaninYolu = @DesifreDosyaYolu WHERE SifrelenmisDosyaninYolu = @SifreDosyaYolu";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DesifreDosyaYolu", decryptedFilePath);
                cmd.Parameters.AddWithValue("@SifreDosyaYolu", encryptedFilePath);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
