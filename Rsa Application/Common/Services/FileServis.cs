using GemBox.Document;
using Rsa_Application.Infrastructure.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace Rsa_Application.Common.Services
{
    internal class FileServis : IFileService
    {
        public void Open(string path, ref string fileText, out bool isCrypto, out string opt, out string keyE, out string keyN)
        {
            opt = String.Empty; keyE = String.Empty; keyN = String.Empty; 

            var document = DocumentModel.Load(path); //открываем файл
            string text = document.Content.ToString(); //считываем содержимое

            string[] paragraphs = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            if (paragraphs.Length >= 4)
            {
                if (Regex.IsMatch(paragraphs[1], "^e=\\d{3,}$") &&
                    Regex.IsMatch(paragraphs[2], "^n=\\d{5,}$"))
                {
                    isCrypto = true;
                    opt = paragraphs[0];
                    keyE = paragraphs[1][2..];
                    keyN = paragraphs[2][2..];
                    for (int i = 3; i < paragraphs.Length; i++)
                        fileText += $"{paragraphs[i]}\n";
                    return;
                }
            }

            isCrypto = false;
            fileText = text;
        }

        public void Save(string path, string text)
        {
            var document = new DocumentModel();

            document.Sections.Add(
                new Section(document,
                new Paragraph(document, text)));

            document.Save(path);
        }

        public void Save(string path, string keyE, string keyN, string opt, string ciphertext)
        {                      
            var document = new DocumentModel();

            document.Sections.Add(
                new Section(document,
                new Paragraph(document, opt),
                new Paragraph(document, $"e={keyE}"),
                new Paragraph(document, $"n={keyN}"),
                new Paragraph(document, ciphertext))); 
            
            document.Save(path);
        }
    }
}
