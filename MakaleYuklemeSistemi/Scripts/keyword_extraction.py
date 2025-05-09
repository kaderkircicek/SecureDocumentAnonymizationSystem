import sys
import fitz  # PyMuPDF
import re
import collections
from nltk.corpus import stopwords
import nltk

# Stopwords'leri indir
nltk.download('stopwords')

def extract_keywords(pdf_path):
    try:
        with fitz.open(pdf_path) as doc:
            text = " ".join([page.get_text("text") for page in doc])

        # Kelimeleri ayıkla
        words = re.findall(r'\b[a-zA-Z]{3,}\b', text.lower())  # Küçük harfe çevir

        # İngilizce stopwords'leri çıkar
        stop_words = set(stopwords.words('english'))
        filtered_words = [word for word in words if word not in stop_words]

        # En sık geçen 20 kelimeyi bul
        word_counts = collections.Counter(filtered_words)
        common_words = word_counts.most_common(20)

        # Sonucu yazdır
        result = ", ".join([word for word, count in common_words])
        print(result)

    except Exception as e:
        print(f"Hata: {str(e)}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Dosya yolu eksik!")
    else:
        extract_keywords(sys.argv[1])
