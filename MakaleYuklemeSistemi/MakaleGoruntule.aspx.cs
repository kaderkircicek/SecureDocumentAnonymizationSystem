using System;
using System.IO;
using System.Web;

namespace MakaleYuklemeSistemi
{
    public partial class MakaleGoruntule : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string fullFilePath = Request.QueryString["file"];

                if (!string.IsNullOrEmpty(fullFilePath))
                {
                    // Veritabanından gelen tam yol: "C:\Users\EZGI\OneDrive\Masaüstü\...\8113_örnek_makale1.pdf"
                    string fileName = Path.GetFileName(fullFilePath);  // Sadece "8113_örnek_makale1.pdf" alınır.

                    // Sunucuya uygun hale getirilen dosya yolu
                    string serverFilePath = Server.MapPath($"~/Uploads/{fileName}");

                    if (File.Exists(serverFilePath))
                    {
                        // Tarayıcıda PDF açılması için gereken ayarlar
                        Response.ContentType = "application/pdf";
                        Response.AppendHeader("Content-Disposition", "inline; filename=" + fileName);
                        Response.WriteFile(serverFilePath);
                        Response.End();
                    }
                    else
                    {
                        Response.Write("Dosya sunucuda bulunamadı.");
                    }
                }
            }
        }
    }
}
