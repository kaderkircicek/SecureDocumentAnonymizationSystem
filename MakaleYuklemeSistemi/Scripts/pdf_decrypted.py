import os
import json
import fitz
import logging
from cryptography.hazmat.primitives.ciphers import Cipher, algorithms, modes
from cryptography.hazmat.backends import default_backend
import base64

# AES anahtarı (pdf_encrypt.py ile aynı olmalı)
AES_KEY = b"0123456789abcdef"  # Gerçek projende burayı değiştir!

# AES ile şifre çözme fonksiyonu
def decrypt_data(encrypted_text, key):
    try:
        encrypted_bytes = bytes.fromhex(encrypted_text)
        iv = encrypted_bytes[:16]  # İlk 16 bayt IV'dir
        cipher = Cipher(algorithms.AES(key), modes.CBC(iv), backend=default_backend())
        decryptor = cipher.decryptor()
        decrypted_padded = decryptor.update(encrypted_bytes[16:]) + decryptor.finalize()
        
        # PKCS7 padding kaldırma
        pad_len = decrypted_padded[-1]
        decrypted_text = decrypted_padded[:-pad_len].decode("utf-8", errors="ignore")
        return decrypted_text

    except Exception as e:
        logging.error(f"Şifre çözme hatası: {str(e)}", exc_info=True)
        return "[DEŞİFRE HATASI]"

# PDF deşifreleme fonksiyonu
def decrypt_pdf(encrypted_pdf, decrypted_pdf):
    try:
        encrypted_pdf = os.path.abspath(encrypted_pdf)
        decrypted_pdf = os.path.abspath(decrypted_pdf)
        
        # 🔹 JSON dosya adını PDF dosya adıyla aynı yap, sadece .json uzantılı olsun
        json_path = encrypted_pdf.replace(".pdf", ".json")

        # 🔹 JSON dosyasının var olup olmadığını kontrol et
        if not os.path.exists(json_path):
            print(f"⚠ Uyarı: JSON dosyası bulunamadı! Beklenen konum: {json_path}")
            logging.error(f"Şifreleme bilgilerini içeren JSON dosyası eksik! Dosya: {json_path}")
            raise FileNotFoundError("Şifrelenmiş verileri içeren JSON dosyası bulunamadı!")

        # 🔹 JSON dosyasını oku
        with open(json_path, 'r', encoding="utf-8") as json_file:
            encrypted_data = json.load(json_file)

        # 🔹 PDF dosyasını aç
        doc = fitz.open(encrypted_pdf)

        for page_num in range(len(doc)):
            page = doc.load_page(page_num)

            for key, encrypted_value in encrypted_data.items():
                if f"_{page_num}_" in key:
                    decrypted_value = decrypt_data(encrypted_value, AES_KEY)

                    # 🔹 PDF'ye çözülmüş metni ekle
                    page.insert_text((50, 50), decrypted_value, fontsize=12, color=(1, 0, 0))

        # 🔹 Deşifrelenmiş PDF'i kaydet
        doc.save(decrypted_pdf)
        doc.close()

        logging.info(f"✅ Deşifrelenmiş PDF başarıyla oluşturuldu: {decrypted_pdf}")
        print(f"✅ Deşifrelenmiş PDF oluşturuldu: {decrypted_pdf}")

    except FileNotFoundError as fnf_error:
        print(f"🚨 Dosya bulunamadı: {fnf_error}")
        logging.error(f"Dosya bulunamadı: {fnf_error}", exc_info=True)
    
    except Exception as e:
        logging.error(f"Deşifreleme sırasında hata oluştu: {str(e)}", exc_info=True)
        raise
