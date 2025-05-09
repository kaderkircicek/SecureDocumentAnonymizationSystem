using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace MakaleYuklemeSistemi
{
    public partial class MakaleYukleme : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fuPdf.HasFile && Path.GetExtension(fuPdf.FileName).ToLower() == ".pdf")
            {
                string email = txtEmail.Text.Trim();

                // 4 basamaklı benzersiz takip numarası oluştur
                Random rnd = new Random();
                string takipNumarasi = rnd.Next(1000, 9999).ToString();

                string dosyaAdi = takipNumarasi + "_" + Path.GetFileName(fuPdf.FileName);
                string sanalYol = "~/Uploads/" + dosyaAdi;  // Sanal yol (web için)
                string fizikiYol = Server.MapPath(sanalYol); // Gerçek dosya yolu (sunucuda)

                if (!Directory.Exists(Server.MapPath("~/Uploads/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Uploads/"));
                }

                fuPdf.SaveAs(fizikiYol);  // Dosyayı kaydet

                string connStr = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    // Gerçek dosya yolu veritabanına kaydediliyor
                    string query1 = "INSERT INTO KullaniciDosya (Email, DosyaYolu, TakipNumarasi, YuklemeTarihi, Durumu) VALUES (@Email, @DosyaYolu, @TakipNumarasi, GETDATE(), 'Yönetici Onayına Sunuldu.')";

                    using (SqlCommand cmd1 = new SqlCommand(query1, conn))
                    {
                        cmd1.Parameters.AddWithValue("@Email", email);
                        cmd1.Parameters.AddWithValue("@DosyaYolu", fizikiYol); // Gerçek dosya yolunu kaydet
                        cmd1.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                        cmd1.ExecuteNonQuery();
                    }

                    string query2 = "INSERT INTO Logs (DegisiklikTarihi, Durumu, Email, TakipNumarasi) VALUES (GETDATE(), 'Yönetici Onayına Sunuldu.', @Email, @TakipNumarasi)";

                    using (SqlCommand cmd2 = new SqlCommand(query2, conn))
                    {
                        cmd2.Parameters.AddWithValue("@Email", email);
                        cmd2.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                        cmd2.ExecuteNonQuery();
                    }
                }

                lblMessage.Text = "Dosyanız başarıyla yüklendi. Takip Numaranız: " + takipNumarasi;
                lblMessage.CssClass = "text-success";

                btnViewPdf.Visible = true;
                btnViewPdf.CommandArgument = fizikiYol; // Fiziksel yolu butona ekle
            }
            else
            {
                lblMessage.Text = "Lütfen yalnızca PDF formatında bir dosya yükleyiniz.";
                lblMessage.CssClass = "text-danger";
            }
        }

        protected void btnViewPdf_Click(object sender, EventArgs e)
        {
            string fizikiYol = ((System.Web.UI.WebControls.Button)sender).CommandArgument;

            if (!string.IsNullOrEmpty(fizikiYol))
            {
                if (File.Exists(fizikiYol))
                {
                    Response.ContentType = "application/pdf";
                    Response.AppendHeader("Content-Disposition", "inline; filename=" + Path.GetFileName(fizikiYol));
                    Response.TransmitFile(fizikiYol);
                    Response.End();
                }
                else
                {
                    lblMessage.Text = "PDF dosyası bulunamadı.";
                    lblMessage.CssClass = "text-danger";
                }
            }
        }

        /// <summary>
        /// PDF'nin fiziksel dosya yolunu getirir.
        /// </summary>
        public string GetDosyaYolu(string takipNumarasi)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MakaleSistemi"].ConnectionString;
            string fizikselYol = "";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT DosyaYolu FROM KullaniciDosya WHERE TakipNumarasi = @TakipNumarasi";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        fizikselYol = result.ToString();
                    }
                }
            }

            return fizikselYol;
        }
    }
}
