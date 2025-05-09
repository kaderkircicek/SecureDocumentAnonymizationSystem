using System;
using System.Data.SqlClient;
using System.Web.Services;
using System.Collections.Generic;

namespace MakaleYuklemeSistemi
{
    public partial class MakaleSorgulama : System.Web.UI.Page
    {
        private static string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";

        [WebMethod]
        public static object GetMakaleDurumu(string email, string takipNumarasi)
        {
            List<object> logs = new List<object>();
            string durum = "Bilinmiyor"; // Başlangıç durumu

            // Veritabanı bağlantısı
            string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // KullaniciDosya tablosundaki Durum bilgisini almak için sorgu
                string query = "SELECT Durumu FROM KullaniciDosya WHERE TakipNumarasi = @TakipNumarasi AND Email = @Email";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                cmd.Parameters.AddWithValue("@Email", email);

                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    durum = result.ToString(); // Durumu al
                }
                else
                {
                    // Eğer geçersiz takip numarası veya e-posta varsa, hata mesajı döndür
                    return new { durumu = "Geçersiz takip numarası veya e-posta.", logs };
                }

                // Logs tablosundan takip numarasına göre değişiklik tarihlerini ve durumlarını almak için sorgu
                string logsQuery = "SELECT DegisiklikTarihi, Durumu FROM Logs WHERE TakipNumarasi = @TakipNumarasi ORDER BY DegisiklikTarihi ASC";
                SqlCommand logsCmd = new SqlCommand(logsQuery, connection);
                logsCmd.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);

                SqlDataReader reader = logsCmd.ExecuteReader();
                while (reader.Read())
                {
                    string logDurumu = reader["Durumu"] != DBNull.Value ? reader["Durumu"].ToString() : "";  // Durumu boş veya null ise boş bırakıyoruz

                    // Logs tablosundan alınan verileri listele
                    logs.Add(new
                    {
                        degisiklikTarihi = reader["DegisiklikTarihi"].ToString(),
                        durum = logDurumu // Durumu burada düzgün şekilde alıyoruz
                    });
                }
            }

            // Sonuçları döndür
            return new { durum, logs };
        }


        [WebMethod]
        public static void SendHakemMessage(string email, string takipNumarasi, string message)
        {
            // SQL connection string
            string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True"; // Bağlantı stringini buraya ekleyin

            // SQL bağlantısı ve komutları
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // TakipNumarasi ile KullaniciDosya tablosundaki eşleşen veriyi buluyoruz
                    string query = "SELECT Id FROM KullaniciDosya WHERE TakipNumarasi = @TakipNumarasi";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TakipNumarasi", takipNumarasi);
                    int kullaniciDosyaId = (int)cmd.ExecuteScalar();

                    // HakemID'yi almak için Hakem_Makale tablosu sorgusu
                    query = "SELECT HakemID FROM Makale_Hakem WHERE MakaleID = @KullaniciDosyaId";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@KullaniciDosyaId", kullaniciDosyaId);
                    var hakemIDResult = cmd.ExecuteScalar();

                    if (hakemIDResult == null || hakemIDResult == DBNull.Value)
                    {
                        throw new Exception("Belirtilen makaleye atanmış hakem bulunamadı. Mesaj gönderilemedi.");
                    }
                    int hakemID = (int)hakemIDResult;

                    // Mesajı Mesajlar tablosuna kaydediyoruz
                    query = "INSERT INTO Mesajlar (MakaleID, HakemID, Mesaj) VALUES (@MakaleID, @HakemID, @Mesaj)";
                    cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MakaleID", kullaniciDosyaId);  // MakaleID burada KullaniciDosya tablosundan alınan Id ile eşleşiyor
                    cmd.Parameters.AddWithValue("@HakemID", hakemID);
                    cmd.Parameters.AddWithValue("@Mesaj", message);

                    cmd.ExecuteNonQuery(); // Veriyi kaydediyoruz
                }
                catch (Exception ex)
                {
                    // Hata mesajı loglama işlemi (Opsiyonel)
                    // Log veya başka bir hata yönetimi yapılabilir
                    throw new Exception("Mesaj gönderme sırasında bir hata oluştu.", ex);
                }
            }
        }
    }
}



 