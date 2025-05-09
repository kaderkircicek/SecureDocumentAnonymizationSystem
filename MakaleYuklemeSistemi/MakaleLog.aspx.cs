using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace MakaleYuklemeSistemi
{
    public partial class MakaleLog : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLogs();
            }
        }

        private void LoadLogs()
        {
            string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";
            string query = "SELECT Id, DegisiklikTarihi, Durumu, Email, TakipNumarasi FROM Logs ORDER BY TakipNumarasi, DegisiklikTarihi";

            // Veritabanına bağlantı açma
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // SQL komutunu çalıştırma
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var logs = new List<LogEntry>();
                        while (reader.Read())
                        {
                            logs.Add(new LogEntry
                            {
                                TakipNumarasi = reader["TakipNumarasi"].ToString(),
                                DegisiklikTarihi = Convert.ToDateTime(reader["DegisiklikTarihi"]),
                                Durumu = reader["Durumu"].ToString(),
                                Email = reader["Email"].ToString()
                            });
                        }

                        // Logs'u gruplama ve HTML oluşturma
                        var groupedLogs = logs
                            .GroupBy(log => log.TakipNumarasi)
                            .OrderBy(group => group.Key)
                            .ToList();

                        StringBuilder htmlContent = new StringBuilder();
                        foreach (var group in groupedLogs)
                        {
                            htmlContent.Append("<div class='log-card-group'>");
                            htmlContent.Append($"<div class='log-group-header'>Takip Numarası: {group.Key}</div>");

                            foreach (var log in group.OrderBy(log => log.DegisiklikTarihi))
                            {
                                htmlContent.Append($@"
                                    <div class='log-card'>
                                        <div class='log-detail'><span>Email:</span> {log.Email}</div>
                                        <div class='log-detail'><span>Durumu:</span> {log.Durumu}</div>
                                        <div class='log-detail'><span>Degisiklik Tarihi:</span> {log.DegisiklikTarihi.ToString("yyyy-MM-dd HH:mm:ss")}</div>
                                    </div>");
                            }

                            htmlContent.Append("</div>");
                        }

                        litLogs.Text = htmlContent.ToString();
                    }
                }
            }
        }

        // Log entry veri yapısı
        public class LogEntry
        {
            public string TakipNumarasi { get; set; }
            public DateTime DegisiklikTarihi { get; set; }
            public string Durumu { get; set; }
            public string Email { get; set; }
        }
    }
}
