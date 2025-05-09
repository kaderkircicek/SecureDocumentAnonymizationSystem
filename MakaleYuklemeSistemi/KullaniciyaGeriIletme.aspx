<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KullaniciyaGeriIletme.aspx.cs" Inherits="MakaleYuklemeSistemi.KullaniciyaGeriIletme" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Kullanıcıya Geri İletme</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <div class="container mt-5">
            <h2 class="text-center text-primary">Kullanıcıya İletilecek Makaleler</h2>

            <!-- Kullanıcıya mesaj göstermek için Label -->
            <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info text-center w-100" Visible="false"></asp:Label>

            <asp:GridView ID="gvMakaleler" runat="server" CssClass="table table-bordered table-striped mt-3"
                AutoGenerateColumns="False" AllowPaging="true" PageSize="10" class="table-responsive">
                <Columns>
                    <asp:BoundField DataField="TakipNumarasi" HeaderText="Takip Numarası" />
                    <asp:BoundField DataField="Email" HeaderText="E-Mail" />
                    
                    <asp:TemplateField HeaderText="Makaleyi Aç">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnAc" runat="server" CssClass="btn btn-primary btn-sm" 
                                CommandArgument='<%# Eval("SifrelenmisDosyaninYolu") %>'
                                OnClick="btnAc_Click">Aç</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Deşifrele ve Kullanıcıya İlet">
                        <ItemTemplate>
                            <asp:Button ID="btnDesifrele" runat="server" Text="Deşifrele ve Kullanıcıya İlet"
                                CssClass="btn btn-success btn-sm"
                                CommandArgument='<%# Eval("SifrelenmisDosyaninYolu") %>'
                                OnClick="btnDesifreleVeIlet_Click" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>
