<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HakemDegerlendirme.aspx.cs" Inherits="MakaleYuklemeSistemi.HakemDegerlendirme" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hakem Değerlendirme</title>
    <style>
        body {
            font-family: 'Arial', sans-serif;
            background: linear-gradient(to right, #fbc2eb, #a6c1ee);
            margin: 0;
            padding: 0;
            display: flex;
            justify-content: center;
            align-items: flex-start;
            height: 100vh;
            overflow: auto;
        }

        .container {
            background-color: #ffffff;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
            width: 85%;
            max-width: 1200px;
            text-align: center;
            margin-top: 20px;
        }

        h3 {
            color: #333;
            font-size: 28px;
            margin-bottom: 20px;
        }

        label {
            color: #555;
            font-size: 18px;
            margin-bottom: 8px;
            display: block;
        }

        .input-text {
            width: 100%;
            padding: 15px;
            margin: 10px 0;
            border: 1px solid #ddd;
            border-radius: 8px;
            font-size: 18px;
        }

        .button {
            padding: 12px 30px;
            font-size: 18px;
            color: #fff;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: 0.3s;
        }

        .button:hover {
            transform: scale(1.05);
            opacity: 0.9;
        }

        .button-open { background-color: #FF69B4; }
        .button-evaluate { background-color: #1E90FF; }
        .button-submit { background-color: #8A2BE2; }

        .evaluation-panel {
            background-color: #f9f9f9;
            padding: 30px;
            border-radius: 10px;
            border: 1px solid #ddd;
            margin-top: 20px;
        }

        .message {
            font-size: 18px;
            font-weight: bold;
            margin-top: 15px;
        }

        .message-success { color: green; }
        .message-error { color: red; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h3>Makale Değerlendirme</h3>

            <asp:Label ID="lblHakemIsim" runat="server" Text="Hakem Adı" CssClass="label"></asp:Label>
            <br />

            <asp:GridView ID="gvMakaleDosyalar" runat="server" AutoGenerateColumns="False" 
                CellPadding="4" ForeColor="#333" GridLines="None" 
                OnRowCommand="gvMakaleDosyalar_RowCommand">
                <Columns>
                    <asp:BoundField DataField="Id" HeaderText="Id" SortExpression="Id" />
                    <asp:TemplateField HeaderText="Makaleyi Aç">
                        <ItemTemplate>
                            <asp:Button ID="btnAç" runat="server" Text="Makaleyi Aç" 
                                CommandName="OpenFile" CommandArgument='<%# Eval("SifrelenmisDosyaninYolu") %>' 
                                CssClass="button button-open" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Değerlendir">
                        <ItemTemplate>
                            <asp:Button ID="btnDegerlendir" runat="server" Text="Değerlendir" 
                                CommandName="Evaluate" CommandArgument='<%# Eval("Id") %>' 
                                CssClass="button button-evaluate" />
                        </ItemTemplate>
                    </asp:TemplateField>
<asp:TemplateField HeaderText="Mesajı Görüntüle">
    <ItemTemplate>
        <asp:Button ID="btnMesajGoruntule" runat="server" Text="Mesajı Görüntüle"
            CommandName="ViewMessage" CommandArgument='<%# Eval("Id") %>' 
            CssClass="button button-submit" />
    </ItemTemplate>
</asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Panel ID="pnlDegerlendirme" runat="server" Visible="false" CssClass="evaluation-panel">
                <asp:Label ID="lblDegerlendirme" runat="server" Text="Değerlendirme Notu:" CssClass="label"></asp:Label>
                <br />
                <asp:TextBox ID="txtDegerlendirme" runat="server" TextMode="MultiLine" Rows="6" CssClass="input-text"></asp:TextBox>
                <br />
                <asp:Button ID="btnGonder" runat="server" Text="Gönder" OnClick="btnGonder_Click" CssClass="button button-submit" />
                <asp:Label ID="lblError" runat="server" CssClass="message message-error"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" CssClass="message message-success"></asp:Label>
            </asp:Panel>

            <asp:Label ID="lblMesaj" runat="server" CssClass="message message-error" Visible="false"></asp:Label>
            <asp:Panel ID="pnlMesaj" runat="server" Visible="false" CssClass="evaluation-panel">
                <asp:Label ID="lblPanelMesaj" runat="server" Text="Mesaj İçeriği:" CssClass="label"></asp:Label>
                <br />
                <asp:TextBox ID="txtMesaj" runat="server" TextMode="MultiLine" Rows="6" CssClass="input-text"></asp:TextBox>
                <br />
            </asp:Panel>
        </div>
    </form>
</body>
</html>
