using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MakaleYuklemeSistemi
{
    public partial class AnaSayfa : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnHakem_Click(object sender, EventArgs e)
        {
           Response.Redirect("EditorSayfasi.aspx");
        }

        protected void btnEditor_Click(object sender, EventArgs e)
        {
            Response.Redirect("HakemSayfasi.aspx");
        }

        protected void btnMakaleYukle_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakaleYukleme.aspx");
        }

        protected void btnMakaleSorgula_Click(object sender, EventArgs e)
        {
            Response.Redirect("MakaleSorgulama.aspx");
        }
    }
}