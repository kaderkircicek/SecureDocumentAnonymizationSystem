<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MakaleYukleme.aspx.cs" Inherits="MakaleYuklemeSistemi.MakaleYukleme" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Makale Yükleme</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.3.0/css/bootstrap.min.css" />
    <style>
        body {
            background: linear-gradient(135deg, #f8cdda, #1c92d2);
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            font-family: 'Arial', sans-serif;
        }
        .upload-container {
            background: white;
            padding: 40px;
            border-radius: 15px;
            box-shadow: 0px 5px 15px rgba(0, 0, 0, 0.3);
            width: 600px;
        }
        .upload-container h3 {
            color: #1c92d2;
            font-size: 28px;
            font-weight: bold;
            text-align: center;
            margin-bottom: 25px;
        }
        .form-label {
            font-weight: bold;
            color: #444;
            font-size: 16px;
        }
        .form-control {
            font-size: 16px;
            padding: 10px;
        }
        .btn-primary {
            background-color: #1c92d2;
            border: none;
            font-size: 20px;
            padding: 14px;
            border-radius: 8px;
        }
        .btn-primary:hover {
            background-color: #0d6efd;
        }
        .btn-success {
            background-color: #ff69b4;
            border: none;
            font-size: 20px;
            padding: 14px;
            border-radius: 8px;
            margin-top: 15px;
            width: 100%;
        }
        .btn-success:hover {
            background-color: #ff1493;
        }
        .text-danger {
            font-size: 15px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="upload-container">
            <asp:ScriptManager runat="server"></asp:ScriptManager>

            <h3>Makale Yükleme</h3>

            <div class="mb-4">
                <label for="txtEmail" class="form-label">E-Posta Adresi</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" placeholder="@gmail.com ile bitmelidir" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    CssClass="text-danger" ErrorMessage="E-posta zorunludur." Display="Dynamic" ValidationGroup="UploadGroup" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    CssClass="text-danger" ValidationExpression="^[a-zA-Z0-9._%+-]+@gmail\.com$"
                    ErrorMessage="Lütfen geçerli bir Gmail adresi giriniz." Display="Dynamic" ValidationGroup="UploadGroup" />
            </div>

            <div class="mb-4">
                <label for="fuPdf" class="form-label">PDF Dosyası Yükle</label>
                <asp:FileUpload ID="fuPdf" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvFile" runat="server" ControlToValidate="fuPdf"
                    CssClass="text-danger" ErrorMessage="Lütfen bir PDF yükleyin." Display="Dynamic" ValidationGroup="UploadGroup" />
            </div>

            <asp:Button ID="btnUpload" runat="server" Text="Yükle" CssClass="btn btn-primary w-100"
                OnClick="btnUpload_Click" ValidationGroup="UploadGroup" />

            <asp:Button ID="btnViewPdf" runat="server" Text="PDF Aç" CssClass="btn btn-success" OnClick="btnViewPdf_Click" Visible="false" />

            <div class="mt-4 text-center">
                <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
