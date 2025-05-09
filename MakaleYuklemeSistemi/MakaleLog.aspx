<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MakaleLog.aspx.cs" Inherits="MakaleYuklemeSistemi.MakaleLog" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MAKALE DURUMLARI</title>
    <style>
        /* Ana konteyner */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
            background: linear-gradient(to right, #ff7eb9, #ff65a3);
            border-radius: 15px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
        }

        /* Başlık kısmı */
        .log-header h1 {
            text-align: center;
            font-size: 2.5rem;
            color: white;
            margin-bottom: 20px;
            font-family: 'Arial', sans-serif;
        }

        /* Log kartları grubu */
        .log-card-group {
            margin-bottom: 30px;
            background-color: rgba(255, 255, 255, 0.8);
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
        }

        /* Grup başlık */
        .log-group-header {
            font-size: 1.5rem;
            font-weight: bold;
            color: #ff4b6e;
            margin-bottom: 15px;
            text-transform: uppercase;
            letter-spacing: 1px;
        }

        /* Kart detayları */
        .log-card {
            background-color: #fff;
            padding: 15px;
            margin-bottom: 15px;
            border-radius: 8px;
            box-shadow: 0 3px 6px rgba(0, 0, 0, 0.1);
        }

        .log-detail {
            margin-bottom: 10px;
            font-size: 1rem;
        }

        .log-detail span {
            font-weight: bold;
            color: #ff65a3;
        }

        body {
            background: #f0f0f0;
            font-family: 'Arial', sans-serif;
            margin: 0;
            padding: 0;
        }

        .log-card:last-child {
            margin-bottom: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="log-header">
                <h1>Makale Logları</h1>
            </div>
            <div id="logsContainer" runat="server">
                <asp:Literal ID="litLogs" runat="server"></asp:Literal>
            </div>
        </div>
    </form>
</body>
</html>
