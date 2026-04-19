import zipfile
import xml.etree.ElementTree as ET
import sys
import os

def read_docx(file_path):
    try:
        with zipfile.ZipFile(file_path, 'r') as docx:
            xml_content = docx.read('word/document.xml')
            tree = ET.XML(xml_content)
            
            WORD_NAMESPACE = '{http://schemas.openxmlformats.org/wordprocessingml/2006/main}'
            PARA = WORD_NAMESPACE + 'p'
            TEXT = WORD_NAMESPACE + 't'
            
            paragraphs = []
            for paragraph in tree.iter(PARA):
                texts = [node.text
                         for node in paragraph.iter(TEXT)
                         if node.text]
                if texts:
                    paragraphs.append(''.join(texts))
            
            return '\n'.join(paragraphs)
    except Exception as e:
        return f"Error reading {file_path}: {e}"

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Usage: python read_docx.py <input.docx> <output.txt>")
        sys.exit(1)
        
    file_path = sys.argv[1]
    out_path = sys.argv[2]
    content = read_docx(file_path)
    with open(out_path, 'w', encoding='utf-8') as f:
        f.write(content)
