<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AnaSayfa.aspx.cs"  Inherits="MakaleYuklemeSistemi.AnaSayfa" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Makale Değerlendirme Sistemi</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            background: linear-gradient(135deg, #f3f7ff, #d3e0ff);
            text-align: center;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }
        .container {
            background: white;
            padding: 50px;
            border-radius: 15px;
            box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
            width: 600px;
            text-align: center;
        }
        h1 {
            color: #333;
            font-size: 32px;
            font-weight: bold;
            margin-bottom: 30px;
        }
        .button-container {
            display: flex;
            flex-direction: column;
            gap: 25px;
        }
        .button-container .nav-button {
            width: 100%;
            padding: 20px;
            font-size: 22px;
            font-weight: bold;
            border: none;
            border-radius: 10px;
            cursor: pointer;
            transition: 0.3s ease-in-out;
            box-shadow: 0 3px 8px rgba(0, 0, 0, 0.15);
        }
        #btnHakem {
            background: #5dade2;
            color: white;
        }
        #btnHakem:hover {
            background: #3498db;
        }
        #btnEditor {
            background: #ff85a2;
            color: white;
        }
        #btnEditor:hover {
            background: #ff4d6a;
        }
        #btnMakaleYukle {
            background: #6a5acd;
            color: white;
        }
        #btnMakaleYukle:hover {
            background: #483d8b;
        }
        #btnMakaleSorgula {
            background: #20b2aa;
            color: white;
        }
        #btnMakaleSorgula:hover {
            background: #178f85;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Makale Değerlendirme Sistemi</h1>
            <div class="button-container">
                <asp:Button ID="btnHakem" runat="server" Text="Yönetici Girişi" OnClick="btnHakem_Click" CssClass="nav-button" />
                <asp:Button ID="btnEditor" runat="server" Text="Hakem Girişi" OnClick="btnEditor_Click" CssClass="nav-button" />
                <asp:Button ID="btnMakaleYukle" runat="server" Text="Makale Yükleme" OnClick="btnMakaleYukle_Click" CssClass="nav-button" />
                <asp:Button ID="btnMakaleSorgula" runat="server" Text="Makale Durumu Sorgulama" OnClick="btnMakaleSorgula_Click" CssClass="nav-button" />
            </div>
        </div>
    </form>
</body>
</html>
