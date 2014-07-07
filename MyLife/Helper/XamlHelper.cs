using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyLife.Helper
{
    public static class XamlHelper
    {
        public static string ToXaml(RichTextBox richTextBox)
        {
            MemoryStream s = new MemoryStream();
            TextRange tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            tr.Save(s, DataFormats.XamlPackage);
            return Convert.ToBase64String(s.ToArray());
        }

        public static void FromXaml(RichTextBox richTextBox, string buffer)
        {
            MemoryStream s = new MemoryStream(Convert.FromBase64String(buffer));
            TextRange tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            tr.Load(s, DataFormats.XamlPackage);
        }
    }
}
