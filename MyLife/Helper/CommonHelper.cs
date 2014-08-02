using MyLife.Properties;
using MyLife.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyLife.Helper
{
    class CommonHelper
    {
        public string Week(DateTime dt)
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(dt.DayOfWeek)];
            return week;
        }

        public string[] GetPOP(string mail)
        {
            if (mail.IndexOf('@') < 0) return null;

            string[] mailfix = mail.Split('@');
            string mailsuffix = mailfix[1].ToLower();

            string[] pop = new string[2];

            switch (mailsuffix)
            {
                case "gmail.com":
                    pop[0] = "pop.gmail.com";
                    pop[1] = "995";
                    break;
                case "qq.com":
                    pop[0] = "pop.qq.com";
                    pop[1] = "995";
                    break;
                case "126.com":
                    pop[0] = "pop.126.com";
                    pop[1] = "110";
                    break;
                case "hotmail.com":
                    pop[0] = "pop.live.com";
                    pop[1] = "995";
                    break;
                case "sina.com":
                    pop[0] = "pop3.sina.com.cn";
                    pop[1] = "110";
                    break;
                case "tom.com":
                    pop[0] = "pop.tom.com";
                    pop[1] = "110";
                    break;
                case "163.com":
                    pop[0] = "pop.163.com";
                    pop[1] = "110";
                    break;
                case "263.net":
                    pop[0] = "pop3.263.net";
                    pop[1] = "110";
                    break;
                case "yahoo.com":
                    pop[0] = "pop.mail.yahoo.com";
                    pop[1] = "110";
                    break;
                case "yahoo.com.cn":
                    pop[0] = "pop.mail.yahoo.com.cn";
                    pop[1] = "995";
                    break;
                case "21cn.com":
                    pop[0] = "pop.21cn.com";
                    pop[1] = "110";
                    break;
                case "sohu.com":
                    pop[0] = "pop3.sohu.com";
                    pop[1] = "110";
                    break;
                case "foxmail.com":
                    pop[0] = "pop.foxmail.com";
                    pop[1] = "110";
                    break;
                case "139.com":
                    pop[0] = "pop.139.com";
                    pop[1] = "110";
                    break;
                default:
                    pop = null;
                    break;
            }
            return pop;
        }

        /// <summary>
        /// 初始化数据库连接字符串
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool InitSql(string password = "")
        {
            string dbpath = new Settings().DBPath;
            if (dbpath.Length <= 0 || !File.Exists(dbpath))
            {
                InitWindow initWin = new InitWindow();
                initWin.ShowDialog();
                dbpath = new Settings().DBPath;
            }
            if (!File.Exists(dbpath)) return false;
            SQLiteHelper.ConStr.DataSource = dbpath;
            SQLiteHelper.ConStr.Password = password;

            return true;
        }

        /// <summary>
        /// 展开最后一个节点
        /// </summary>
        /// <param name="treeView"></param>
        public static void ExpandLastNode(TreeView treeView)
        {
            if (treeView.Items.Count > 0)
            {
                var lastModel = treeView.Items[treeView.Items.Count - 1];
                TreeViewItem currentContainer = treeView.ItemContainerGenerator.ContainerFromItem(lastModel) as TreeViewItem;
                currentContainer.IsExpanded = true;
            }
        }

        public static FlowDocument MergeFlowDocument(FlowDocument fd, FlowDocument fdNew)
        {
            FlowDocument FlowDocument = new FlowDocument();
            List<Block> flowDocumetnBlocks1 = new List<Block>(fd.Blocks);
            List<Block> flowDocumetnBlocks2 = new List<Block>(fdNew.Blocks);

            foreach (Block item in flowDocumetnBlocks1)
                FlowDocument.Blocks.Add(item);
            foreach (Block item in flowDocumetnBlocks2)
                FlowDocument.Blocks.Add(item);

            return FlowDocument;
        }
    }
}