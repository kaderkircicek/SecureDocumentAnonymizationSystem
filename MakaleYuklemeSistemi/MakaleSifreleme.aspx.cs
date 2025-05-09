using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MakaleYuklemeSistemi
{
    public partial class MakaleSifreleme : System.Web.UI.Page
    {
        private readonly string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";
        private readonly string pythonPath = @"C:\Users\EZGI\AppData\Local\Programs\Python\Python311\python.exe";
        private readonly string scriptPath = @"C:\Users\EZGI\OneDrive\Masaüstü\yazlab2proje1\MakaleYuklemeSistemi\Scripts\pdf_encrypt.py";

        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await MakaleListeleAsync();
            }
        }

        private async Task MakaleListeleAsync()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "SELECT TakipNumarasi, Email, DosyaYolu FROM KullaniciDosya WHERE Durumu = @Durum";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Durum", "Yönetici Onayına Sunuldu.");

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            gvMakaleler.DataSource = reader;
                            gvMakaleler.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Hata (MakaleListeleAsync): " + ex.Message);
            }
        }


        protected void btnSifrele_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;
                string takipNumarasi = row.Cells[0].Text;
                string dosyaYolu = GetDosyaYolu(takipNumarasi);
                CheckBoxList chkSecenekler = (CheckBoxList)row.FindControl("chkSecenekler");

                // En az bir seçenek seçildi mi?
                if (chkSecenekler.SelectedItem == null)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Lütfen en az bir şifreleme seçeneği seçin!');", true);
                    return;
                }

                bool sifreleAdSoyad = chkSecenekler.Items.FindByValue("AdSoyad").Selected;
                bool sifreleIletisim = chkSecenekler.Items.FindByValue("Iletisim").Selected;
                bool sifreleKurum = chkSecenekler.Items.FindByValue("Kurum").Selected;

                string encryptedFilePath = dosyaYolu.Replace(".pdf", "_encrypted.pdf");

                string scriptOutput = RunPythonScript(dosyaYolu, encryptedFilePath, sifreleAdSoyad, sifreleIletisim, sifreleKurum);

                if (!string.IsNullOrEmpty(scriptOutput))
                {
                    UpdateDatabase1(takipNumarasi, encryptedFilePath);
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Şifreleme işlemi başarıyla tamamlandı.');", true);
                    Button btnSifreliAc = (Button)row.FindControl("btnSifreliAc");
                    btnSifreliAc.Visible = true;
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Şifreleme işlemi başarısız oldu.');", true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Hata (btnSifrele_Click): " + ex.Message);
            }
        }

        private string RunPythonScript(string inputFilePath, string outputFilePath, bool adSoyad, bool iletisim, bool kurum)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"\"{scriptPath}\" \"{inputFilePath}\" \"{outputFilePath}\" {adSoyad} {iletisim} {kurum}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                using (StreamReader reader = process.StandardOutput)
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Hata (RunPythonScript): " + ex.Message);
                return null;
            }
        }

        private void UpdateDatabase1(string takipNumarasi, string encryptedFilePath)
        {
            string email = GetEmail(takipNumarasi);  // Email'i buradan alıyoruz

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE KullaniciDosya SET SifrelenmisDosyaninYolu = @path WHERE TakipNumarasi = @numara";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@path", encryptedFilePath);
                    cmd.Parameters.AddWithValue("@numara", takipNumarasi);
                    cmd.ExecuteNonQuery();
                }
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "UPDATE KullaniciDosya SET SifrelenmisDosyaninYolu = @path, Durumu = @durum WHERE TakipNumarasi = @numara";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@path", encryptedFilePath);
                    cmd.Parameters.AddWithValue("@durum", "Hakeme İletilme Aşamasında");
                    cmd.Parameters.AddWithValue("@numara", takipNumarasi);
                    cmd.ExecuteNonQuery();
                }

                string logQuery = "INSERT INTO Logs (DegisiklikTarihi, Durumu, Email, TakipNumarasi) VALUES (@tarih, @durum, @mail, @numara)";
                using (SqlCommand logCmd = new SqlCommand(logQuery, conn))
                {
                    logCmd.Parameters.AddWithValue("@tarih", DateTime.Now);
                    logCmd.Parameters.AddWithValue("@durum", "Hakeme İletilme Aşamasında");
                    logCmd.Parameters.AddWithValue("@mail", email);  // Email'i burada kullanıyoruz
                    logCmd.Parameters.AddWithValue("@numara", takipNumarasi);
                    logCmd.ExecuteNonQuery();
                }
            }
        }

        private string GetDosyaYolu(string takipNumarasi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT DosyaYolu FROM KullaniciDosya WHERE TakipNumarasi = @numara";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@numara", takipNumarasi);
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        private string GetEmail(string takipNumarasi)  // Email'i almak için yeni bir metod ekledik
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT Email FROM KullaniciDosya WHERE TakipNumarasi = @numara";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@numara", takipNumarasi);
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        protected void btnSifreliAc_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            GridViewRow row = (GridViewRow)btn.NamingContainer;
            string takipNumarasi = row.Cells[0].Text;
            string sifreliDosyaYolu = GetSifrelenmisDosyaYolu(takipNumarasi);

            if (!string.IsNullOrEmpty(sifreliDosyaYolu))
            {
                // Dosya yolunu istemciye uygun hale getirelim
                string fileUrl = ConvertToClientPath(sifreliDosyaYolu);
                Response.Redirect(fileUrl);
            }
        }

        private string GetSifrelenmisDosyaYolu(string takipNumarasi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT SifrelenmisDosyaninYolu FROM KullaniciDosya WHERE TakipNumarasi = @numara";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@numara", takipNumarasi);
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        private string ConvertToClientPath(string serverFilePath)
        {
            // Sunucu yolu örneği: C:\Users\EZGI\OneDrive\...\MakaleYuklemeSistemi\Uploads\7773_ornek_makale2.pdf
            if (serverFilePath.StartsWith("C:\\"))
            {
                // Dosya yolundaki sunucu yolunu "~/" ile değiştiriyoruz
                string fileName = Path.GetFileName(serverFilePath); // Sadece dosya adı al
                string clientFilePath = "~/Uploads/" + fileName; // İstemciye uygun yolu oluştur
                return clientFilePath;
            }

            // Eğer yol zaten istemciye uygunsa olduğu gibi döndür
            return serverFilePath;
        }


        protected string GetFileUrl(object filePath)
            {
                string filePathStr = filePath.ToString();

                // Eğer dosya yolu sunucu yolu formatında ise, bu yolu istemciye uygun hale getirelim
                if (filePathStr.StartsWith("C:\\"))
                {
                    // Dosya yolunu sunucu köküne göre düzeltiyoruz
                    string fileName = Path.GetFileName(filePathStr); // Dosya adını al
                    return "~/Uploads/" + fileName; // Sunucudaki "Uploads" klasörüne yönlendirme yapıyoruz
                }

                // Veritabanı yolu zaten istemciye uygun ise, olduğu gibi döndür
                return filePathStr;
            }

        
    }
}
