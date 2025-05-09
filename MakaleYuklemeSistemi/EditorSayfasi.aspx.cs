using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MakaleYuklemeSistemi
{
    public partial class EditorSayfasi : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnMakaleLog_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakaleLog.aspx");
        }

        protected void btnMakaleSifreleme_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakaleSifreleme.aspx");
        }

        protected void btnHakemSecimi_Click(object sender, EventArgs e)
        {
            Response.Redirect("HakemSecimi.aspx");
        }

        protected void btnKullaniciyaIlet_Click(object sender, EventArgs e)
        {
            Response.Redirect("KullaniciyaGeriIletme.aspx");
        }
    }
}