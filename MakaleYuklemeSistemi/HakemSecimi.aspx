<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HakemSecimi.aspx.cs" Inherits="MakaleYuklemeSistemi.HakemSecimi" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hakem Seçimi</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #ffe4e1;
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
            background-color: #ff1493;
            color: white;
            border: none;
            padding: 7px 12px;
            border-radius: 5px;
            cursor: pointer;
        }
        button:hover {
            background-color: #c71585;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h2>Hakem Seçimi</h2>
            
            <asp:GridView ID="gvMakaleler" runat="server" AutoGenerateColumns="False" CssClass="table">
                <Columns>
                    <asp:BoundField DataField="TakipNumarasi" HeaderText="Takip Numarası" />
                    <asp:BoundField DataField="Email" HeaderText="E-posta" />
                    <asp:TemplateField HeaderText="İşlem">
                        <ItemTemplate>
                            <asp:Button ID="btnHakemeIlet" runat="server" Text="Hakeme İlet" CssClass="btn btn-primary" OnClick="btnHakemeIlet_Click" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            
            <asp:Label ID="lblSonuc" runat="server" ForeColor="Red"></asp:Label>
        </div>
    </form>
</body>
</html>
