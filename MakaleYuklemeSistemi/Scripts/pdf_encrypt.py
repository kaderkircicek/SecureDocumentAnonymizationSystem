import fitz  # PyMuPDF
import cv2
import numpy as np
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad
import json
import os
import spacy
import re
import sys
import logging

logging.basicConfig(filename="C:\\Users\\EZGI\\OneDrive\\Masaüstü\\pdf_encrypt.log",
                    level=logging.INFO,
                    format="%(asctime)s - %(levelname)s - %(message)s")

# Yüz algılama için Haar Cascade dosyasını yükleme
face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + "haarcascade_frontalface_default.xml")

# Sabit AES anahtarı (16 bayt)
AES_KEY = b'16BytesFixedKey!'

def encrypt_data(data, key):
    cipher = AES.new(key, AES.MODE_CBC, iv=b'16BytesFixedIV!!')
    ct_bytes = cipher.encrypt(pad(data.encode(), AES.block_size))
    return ct_bytes.hex()

def find_sensitive_info(text, adSoyad, iletisim, kurum):
    nlp = spacy.load("en_core_web_sm")
    doc = nlp(text)
    sensitive_info = []

    if adSoyad:
        sensitive_info.extend(re.findall(r'\b[A-Z][a-z]+ [A-Z][a-z]+\b', text))
    
    if iletisim:
        sensitive_info.extend(re.findall(r'\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b', text))
        sensitive_info.extend(re.findall(r'\b\d{3}[-.\s]?\d{3}[-.\s]?\d{4}\b', text))
    
    if kurum:
        sensitive_info.extend(re.findall(r'\b[A-Z][a-z]+ (?:University|Hospital|Company)\b', text))
    
    return list(set(sensitive_info))
def blur_faces(image):
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    faces = face_cascade.detectMultiScale(gray, scaleFactor=1.1, minNeighbors=5, minSize=(30, 30))
    
    blurred_faces = []
    
    # Her yüzü bulanıklaştırma işlemi
    for (x, y, w, h) in faces:
        face_roi = image[y:y+h, x:x+w]
        blurred_face = cv2.GaussianBlur(face_roi, (55, 55), 30)
        image[y:y+h, x:x+w] = blurred_face
        blurred_faces.append((x, y, w, h))  # Yüzlerin koordinatlarını sakla
    
    return image, blurred_faces  # Yüzlerin koordinatlarını döndürüyoruz

def process_pdf(input_file, output_file, adSoyad, iletisim, kurum):
    try:
        input_file = os.path.abspath(input_file)
        output_file = os.path.abspath(output_file)
        json_path = output_file.replace(".pdf", ".json")

        if not os.path.exists(input_file):
            raise FileNotFoundError(f"Dosya bulunamadı: {input_file}")

        data_to_encrypt = {}
        doc = fitz.open(input_file)
        
        for page_num in range(len(doc)):
            page = doc.load_page(page_num)
            text = page.get_text("text")
            sensitive_info = find_sensitive_info(text, adSoyad, iletisim, kurum)
            
            for info in sensitive_info:
                areas = page.search_for(info)
                for area in areas:
                    page.add_redact_annot(area, fill=(1, 1, 1))
                    data_to_encrypt[f"{info}_{page_num}_{area}"] = encrypt_data(info, AES_KEY)
            
            page.apply_redactions()
            
            if page_num >= len(doc) - 2:  # Son iki sayfa için resim işleme
                img_list = page.get_images(full=True)
                for img_index, img in enumerate(img_list):
                    xref = img[0]
                    base_image = doc.extract_image(xref)
                    img_bytes = base_image["image"]
                    img_np = np.frombuffer(img_bytes, dtype=np.uint8)
                    image = cv2.imdecode(img_np, cv2.IMREAD_COLOR)
                    blurred_image, faces = blur_faces(image)

                    # Yüzlerin her biri için bulanıklaştırılmış yüzleri yerleştirme
                    for (x, y, w, h) in faces:
                        # Yüz görselinin bulanıklaştırılmış kısmını alıyoruz
                        face_img = blurred_image[y:y+h, x:x+w]
                        success, encoded_image = cv2.imencode(".png", face_img)
                        if success:
                            img_stream = encoded_image.tobytes()
                            # Yüzün sayfada bulunan koordinatlarına göre yüzü yerleştiriyoruz
                            rect = fitz.Rect(x, y, x + w, y + h)  # Yüzün bulunduğu dikdörtgen alan
                            page.insert_image(rect, stream=img_stream)  # Bu alana yüzü yerleştiriyoruz
        
        doc.save(output_file)
        doc.close()

        with open(json_path, 'w') as json_file:
            json.dump(data_to_encrypt, json_file)

        logging.info(f"Şifrelenmiş PDF şuraya kaydedildi: {output_file}")
        logging.info(f"JSON dosyası şuraya kaydedildi: {json_path}")
        print(f"Şifrelenmiş PDF şuraya kaydedildi: {output_file}")
        print(f"JSON dosyası şuraya kaydedildi: {json_path}")
    
    except Exception as e:
        logging.error(f"Hata: {str(e)}", exc_info=True)
        print(f"Hata: {str(e)}")
        raise

if __name__ == "__main__":
    try:
        if len(sys.argv) != 6:
            print("Kullanım: python pdf_encrypt.py <input_file> <output_file> <adSoyad> <iletisim> <kurum>")
            sys.exit(1)

        input_file = sys.argv[1]
        output_file = sys.argv[2]
        adSoyad = sys.argv[3].lower() == 'true'
        iletisim = sys.argv[4].lower() == 'true'
        kurum = sys.argv[5].lower() == 'true'

        process_pdf(input_file, output_file, adSoyad, iletisim, kurum)
    
    except Exception as e:
        logging.error(f"Script çalıştırılırken hata oluştu: {str(e)}", exc_info=True)
        print(f"Script çalıştırılırken hata oluştu: {str(e)}")
        sys.exit(1)
