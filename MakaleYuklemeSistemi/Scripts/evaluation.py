from PyPDF2 import PdfReader, PdfWriter  # PdfReader ve PdfWriter'ı import et
from reportlab.lib.pagesizes import letter
from reportlab.pdfgen import canvas
import io
import sys

# Yorumları PDF'e eklemek için bir fonksiyon
def add_review_to_pdf(pdf_path, review_text):
    try:
        # Dosya yolunun doğru olduğunu kontrol et
        print(f"Received PDF path: {pdf_path}")  # Dosya yolunu ekrana yazdır
        
        # PDF okuma
        reader = PdfReader(pdf_path)  # PdfReader ile PDF dosyasını oku
        writer = PdfWriter()

        # Tüm mevcut sayfaları yaz
        for page in reader.pages:
            writer.add_page(page)

        # Yeni sayfa oluşturma
        packet = io.BytesIO()
        c = canvas.Canvas(packet, pagesize=letter)
        c.drawString(100, 750, "Hakem Yorumları:")
        c.drawString(100, 730, review_text)
        c.save()

        # Yeni sayfayı PDF'e ekle
        packet.seek(0)
        new_pdf = PdfReader(packet)  # Yeni sayfayı oku
        writer.add_page(new_pdf.pages[0])  # Yeni sayfayı ekle

        # Orijinal dosyaya üzerine yaz
        with open(pdf_path, "wb") as output_file:
            writer.write(output_file)

        print(f"Evaluation added successfully. Updated file: {pdf_path}")

    except Exception as e:
        print(f"Error in adding review to PDF: {e}")

# Python script'i çalıştırma
if __name__ == "__main__":
    pdf_path = sys.argv[1]  # Python betiğine iletilen ilk parametre (dosya yolu)
    review_text = sys.argv[2]  # Python betiğine iletilen ikinci parametre (yorum)
    
    # Dosya yolunun doğru alındığını doğrulamak için
    print(f"Received PDF path: {pdf_path}")
    
    add_review_to_pdf(pdf_path, review_text)