using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyLife.Helper
{
    class SearchHelper
    {
        private static List<TextRange> FindAllMatchedTextRanges(RichTextBox richBox, string keyword)
        {
            List<TextRange> trList = new List<TextRange>();
            TextPointer position = richBox.Document.ContentStart;
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string text = position.GetTextInRun(LogicalDirection.Forward);
                    int index = 0;
                    while (index < text.Length)
                    {
                        index = text.IndexOf(keyword, index);
                        if (index == -1)
                        {
                            break;
                        }
                        else
                        {
                            TextPointer start = position.GetPositionAtOffset(index);
                            TextPointer end = start.GetPositionAtOffset(keyword.Length);
                            trList.Add(new TextRange(start, end));
                            index += keyword.Length;
                        }
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }
            return trList;
        }

        public static void ReplaceKeywordColor(RichTextBox richBox, string keyword)
        {
            List<TextRange> trList = FindAllMatchedTextRanges(richBox, keyword);
            foreach (TextRange tr in trList)
            {
                tr.ApplyPropertyValue(TextElement.BackgroundProperty, System.Windows.Media.Brushes.IndianRed);
            }
        }
    }
}
