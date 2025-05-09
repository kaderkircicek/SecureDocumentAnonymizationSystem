<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MakaleSifreleme.aspx.cs" Inherits="MakaleYuklemeSistemi.MakaleSifreleme" Async="true" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Şifrelenecek Makaleler</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f8f9fa;
            text-align: center;
        }
        .container {
            width: 80%;
            margin: auto;
            background: #fff;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        th, td {
            border: 1px solid #ddd;
            padding: 10px;
            text-align: center;
        }
        th {
            background-color: #ff69b4;
            color: white;
        }
        button {
            background-color: #007bff;
            color: white;
            border: none;
            padding: 5px 10px;
            border-radius: 5px;
            cursor: pointer;
        }
        button:hover {
            background-color: #0056b3;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Şifrelenecek Makaleler</h2>
            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Visible="false"></asp:Label>
            <asp:GridView ID="gvMakaleler" runat="server" AutoGenerateColumns="False" CssClass="table">
                <Columns>
                    <asp:BoundField DataField="TakipNumarasi" HeaderText="Takip Numarası" />
                    <asp:BoundField DataField="Email" HeaderText="Mail Adresi" />
                    
                    <asp:TemplateField HeaderText="PDF'i Aç">
    <ItemTemplate>
        <asp:HyperLink ID="lnkPdf" runat="server" Text="Aç" Target="_blank" 
            NavigateUrl='<%# GetFileUrl(Eval("DosyaYolu")) %>'></asp:HyperLink>
    </ItemTemplate>
</asp:TemplateField>

                    <asp:TemplateField HeaderText="Şifrelenecek Bilgileri Seç">
                        <ItemTemplate>
                            <asp:CheckBoxList ID="chkSecenekler" runat="server">
                                <asp:ListItem Text="Yazar Ad-Soyad" Value="AdSoyad" />
                                <asp:ListItem Text="Yazar İletişim Bilgileri" Value="Iletisim" />
                                <asp:ListItem Text="Yazar Kurum Bilgileri" Value="Kurum" />
                            </asp:CheckBoxList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Şifrele">
                        <ItemTemplate>
                            <asp:Button ID="btnSifrele" runat="server" Text="Şifrele" CssClass="btn btn-primary" OnClick="btnSifrele_Click" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Şifrelenmiş Dosyayı Aç">
                        <ItemTemplate>
                            <asp:Button ID="btnSifreliAc" runat="server" Text="Şifrelenmiş Dosyayı Aç" CssClass="btn btn-success" Visible="false" OnClick="btnSifreliAc_Click"/>
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>

</html>