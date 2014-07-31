using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyLife.Helper
{
    public static class XamlHelper
    {
        public static string ToXaml(FlowDocument fd)
        {
            MemoryStream s = new MemoryStream();
            TextRange tr = new TextRange(fd.ContentStart, fd.ContentEnd);
            tr.Save(s, DataFormats.XamlPackage);
            return Convert.ToBase64String(s.ToArray());
        }

        public static FlowDocument FromXaml(string buffer)
        {
            FlowDocument fd = new FlowDocument();
            MemoryStream s = new MemoryStream(Convert.FromBase64String(buffer));
            TextRange tr = new TextRange(fd.ContentStart, fd.ContentEnd);
            tr.Load(s, DataFormats.XamlPackage);
            return fd;
        }

    }
}
