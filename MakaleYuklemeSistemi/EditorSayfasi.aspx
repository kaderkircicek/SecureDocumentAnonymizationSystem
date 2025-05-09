<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditorSayfasi.aspx.cs" Inherits="MakaleYuklemeSistemi.EditorSayfasi" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Yönetici Ekranı</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: Arial, sans-serif;
            background: #f3f7ff;
            text-align: center;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }
        .container {
            background: white;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
            width: 450px;
        }
        h1 {
            color: #333;
            font-size: 28px;
            margin-bottom: 20px;
        }
        .button-container {
            display: flex;
            flex-direction: column;
            gap: 15px;
        }
        .button-container .nav-button {
            width: 100%;
            padding: 15px;
            font-size: 18px;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: 0.3s ease-in-out;
        }
        #btnMakaleLog {
            background: #48C9B0; 
            color: white;
        }
        #btnMakaleLog:hover {
            background: #1ABC9C; 
        }
        #btnMakaleSifreleme {
            background: #FF69B4; 
            color: white;
        }
        #btnMakaleSifreleme:hover {
            background: #FF1493; 
        }
        #btnHakemSecimi {
            background: #8A2BE2; 
            color: white;
        }
        #btnHakemSecimi:hover {
            background: #6A5ACD; 
        }
        #btnKullaniciyaIlet {
            background: #32CD32; 
            color: white;
        }
        #btnKullaniciyaIlet:hover {
            background: #2E8B57; 
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>Yönetici Ekranı</h1>
            <div class="button-container">
                <asp:Button ID="btnMakaleLog" runat="server" Text="Makaleler ve Durumları" CssClass="nav-button" OnClick="btnMakaleLog_Click" />
                <asp:Button ID="btnMakaleSifreleme" runat="server" Text="Anonimleştirilecek Makaleler" CssClass="nav-button" OnClick="btnMakaleSifreleme_Click" />
                <asp:Button ID="btnHakemSecimi" runat="server" Text="Hakeme İletilecek Makaleler" CssClass="nav-button" OnClick="btnHakemSecimi_Click" />
                <asp:Button ID="btnKullaniciyaIlet" runat="server" Text="Kullanıcıya İletilecek Makaleler" CssClass="nav-button" OnClick="btnKullaniciyaIlet_Click" />
            </div>
        </div>
    </form>
</body>
</html>
