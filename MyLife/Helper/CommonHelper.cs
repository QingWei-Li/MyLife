using MyLife.Properties;
using MyLife.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MyLife.Helper
{
    class CommonHelper
    {
        public string Week()
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            return week;
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

    }
}