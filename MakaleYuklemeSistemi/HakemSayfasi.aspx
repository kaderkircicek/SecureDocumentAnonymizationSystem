<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HakemSayfasi.aspx.cs" Inherits="MakaleYuklemeSistemi.HakemSayfasi" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Hakem Sayfası</title>
    <style>
        /* Sayfanın genel stili */
        body {
            font-family: Arial, sans-serif;
            background-color: #f7f7f7;
            margin: 0;
            padding: 0;
        }

        /* Butonların genel stili */
        .hakem-button {
            margin: 10px;
            padding: 20px;
            font-size: 18px;
            color: white;
            border: none;
            cursor: pointer;
            width: 150px;
            height: 60px;
            font-weight: bold;
            border-radius: 10px;
            transition: transform 0.3s, background-color 0.3s;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
        }

        /* Buton hover etkisi */
        .hakem-button:hover {
            transform: scale(1.1);
            opacity: 0.9;
        }

        /* Butonların arka plan renkleri, canlı ve dikkat çekici tonlar */
        .hakem-button:nth-child(1) { background-color: #FF69B4; } /* Pembe */
        .hakem-button:nth-child(2) { background-color: #1E90FF; } /* Mavi */
        .hakem-button:nth-child(3) { background-color: #8A2BE2; } /* Mor */
        .hakem-button:nth-child(4) { background-color: #FF1493; } /* Derin Pembe */
        .hakem-button:nth-child(5) { background-color: #00BFFF; } /* Aqua Mavi */
        .hakem-button:nth-child(6) { background-color: #BA55D3; } /* Orkide Moru */
        .hakem-button:nth-child(7) { background-color: #FF6347; } /* Domates Kırmızısı */
        .hakem-button:nth-child(8) { background-color: #20B2AA; } /* Deniz Mavisi */
        .hakem-button:nth-child(9) { background-color: #C71585; } /* Orkide Pembe */
        .hakem-button:nth-child(10) { background-color: #4682B4; } /* Çelik Mavi */
        .hakem-button:nth-child(11) { background-color: #9932CC; } /* Koyu Mor */
        .hakem-button:nth-child(12) { background-color: #00CED1; } /* Açık Deniz Mavisi */
        .hakem-button:nth-child(13) { background-color: #FF4500; } /* Turuncu Kırmızı */
        .hakem-button:nth-child(14) { background-color: #8B008B; } /* Koyu Mor */
        .hakem-button:nth-child(15) { background-color: #FF8C00; } /* Koyu Turuncu */
        .hakem-button:nth-child(16) { background-color: #7B68EE; } /* Orta Mor */

        /* Grid yapısının stili */
        .button-container {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 15px;
            margin: 20px auto;
            max-width: 800px;
            justify-items: center;
            padding: 20px;
        }

        /* Sayfa içerisindeki genel stil */
        .container {
            text-align: center;
            margin: 0 auto;
            padding: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <asp:Panel ID="pnlHakemler" runat="server" CssClass="button-container"></asp:Panel>
        </div>
    </form>
</body>
</html>
