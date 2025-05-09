using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace MakaleYuklemeSistemi
{
    public partial class HakemSayfasi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GenerateHakemButtons(); // Butonlar her postback’te oluşturulmalı
        }

        private void GenerateHakemButtons()
        {
            pnlHakemler.Controls.Clear(); // Paneli temizleyerek butonların çakışmasını önle

            string connectionString = "Data Source=LAPTOP-KH2PNG3N\\SQLEXPRESS02;Initial Catalog=MakaleSistemi;Integrated Security=True";
            string query = "SELECT ID, Isim FROM Hakemler";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                int i = 0;
                while (reader.Read() && i < 16)
                {
                    string hakemID = reader["ID"].ToString();
                    string hakemIsim = reader["Isim"].ToString();

                    Button btnHakem = new Button
                    {
                        ID = "btnHakem" + hakemID,
                        Text = hakemIsim,
                        CssClass = "hakem-button",
                        CommandArgument = hakemID
                    };

                    btnHakem.Click += HakemButton_Click; // Tıklama eventini bağla

                    pnlHakemler.Controls.Add(btnHakem);
                    i++;
                }

                reader.Close();
            }
        }

        protected void HakemButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string hakemID = clickedButton.CommandArgument;

            if (!string.IsNullOrEmpty(hakemID))
            {
                Response.Redirect("HakemDegerlendirme.aspx?ID=" + hakemID);
            }
        }
    }
}
