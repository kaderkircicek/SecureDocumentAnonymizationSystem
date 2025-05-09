<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MakaleSorgulama.aspx.cs" Inherits="MakaleYuklemeSistemi.MakaleSorgulama" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Makale Sorgulama</title>
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .content-container { margin-top: 50px; }
        .panel { background-color: #f4f6f9; border-radius: 8px; box-shadow: 0 0 15px rgba(0, 0, 0, 0.1); padding: 20px; }
        .btn-custom { background-color: #007bff; color: white; border-radius: 5px; }
        .btn-custom:hover { background-color: #0056b3; }
        .message-box { display: none; margin-top: 20px; }
    </style>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container content-container">
            <div class="panel">
                <h2 class="text-center">Makale Sorgulama</h2>
                <div class="form-group">
                    <label for="email">E-posta Adresi:</label>
                    <input type="email" id="email" class="form-control" placeholder="E-posta adresinizi girin" required />
                </div>
                <div class="form-group">
                    <label for="takipNumarasi">Takip Numarası:</label>
                    <input type="text" id="takipNumarasi" class="form-control" placeholder="Takip numaranızı girin" required />
                </div>
                <button type="button" class="btn btn-custom" id="btnSorgula">Sorgula</button>

                <div class="mt-4">
                    <h3>Makalenizin Güncel Durumu: <span id="durum"></span></h3>
                </div>

                <div class="mt-4">
                    <h4>Detaylı Bilgi</h4>
                    <table class="table table-striped" id="logsTable">
                        <thead>
                            <tr>
                                <th>Değişiklik Tarihi</th>
                                <th>Durum</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>

                <button class="btn btn-info mt-3" type="button" id="btnShowMessageBox">Hakeme Mesaj Gönder</button>

                <div class="message-box">
                    <h5>Mesajınızı Yazın</h5>
                    <textarea class="form-control" id="hakemMesaji" rows="4" placeholder="Mesajınızı yazın..."></textarea>
                    <button class="btn btn-custom mt-2" type="button" id="btnSendMessage">Gönder</button>
                </div>
            </div>
        </div>
    </form>

    <script>
        $(document).ready(function () {
            $("#btnShowMessageBox").click(function () {
                $(".message-box").toggle();
            });

            $("#btnSorgula").click(function () {
                var email = $("#email").val();
                var takipNumarasi = $("#takipNumarasi").val();

                $.ajax({
                    type: "POST",
                    url: "MakaleSorgulama.aspx/GetMakaleDurumu",
                    data: JSON.stringify({ email: email, takipNumarasi: takipNumarasi }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        var data = response.d; // Dönen veri burada
                        $("#durum").text(data.durum); // Durum kısmı
                        $("#logsTable tbody").empty(); // Önceki satırları temizle

                        // Logs verisini döngü ile tabloya ekle
                        data.logs.forEach(function (log) {
                            var logDurumu = log.durum !== undefined && log.durum !== null ? log.durum : ""; // Durum verisini kontrol et
                            $("#logsTable tbody").append(
                                `<tr><td>${log.degisiklikTarihi}</td><td>${logDurumu}</td></tr>`
                            );
                        });
                    },
                    error: function () {
                        alert("Sorgulama sırasında hata oluştu.");
                    }
                });
            });

            $("#btnSendMessage").click(function () {
                var message = $("#hakemMesaji").val();
                var email = $("#email").val();
                var takipNumarasi = $("#takipNumarasi").val();

                $.ajax({
                    type: "POST",
                    url: "MakaleSorgulama.aspx/SendHakemMessage",
                    data: JSON.stringify({ email: email, takipNumarasi: takipNumarasi, message: message }),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function () {
                        alert("Mesajınız başarıyla gönderildi.");
                    },
                    error: function () {
                        alert("Mesaj gönderme sırasında hata oluştu.");
                    }
                });
            });
        });
    </script>
</body>
</html>
