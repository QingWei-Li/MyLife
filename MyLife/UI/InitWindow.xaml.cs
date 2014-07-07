using MyLife.Helper;
using MyLife.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyLife.UI
{
    /// <summary>
    /// InitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InitWindow : Window
    {
        public InitWindow()
        {
            InitializeComponent();
        }

        private void bthSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbSavePath.Text.Length <= 0 || tbUserName.Text.Length <= 0)
            {
                MessageBox.Show("输入文件名并选择文件的保存目录");
                return;
            }
            if (pswNew.Password.Length <= 0 || pswNew.Password.Length <= 0)
            {
                MessageBox.Show("请输入密码");
                return;
            }
            if (pswNew.Password != pswTwo.Password)
            {
                MessageBox.Show("两次输入密码不一致，请重试");
                return;
            }

            string path = tbSavePath.Text + System.IO.Path.DirectorySeparatorChar + tbUserName.Text + ".mylife";

            try
            {
                SQLiteHelper.CreateFile(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            Settings set = new Settings();
            set.DBPath = path;
            set.Save();

            CommonHelper.InitSql();
            SQLiteHelper.InitDB();
            SQLiteHelper.ChangePassword(pswNew.Password);
            this.Closed -= Window_Closed;
            this.Close();
        }

        private void tbSavePath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox texbBox = (TextBox)sender;
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
                texbBox.Text = fbd.SelectedPath;
        }
      
        private void FilePath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox texbBox = (TextBox)sender;
            System.Windows.Forms.OpenFileDialog fbd = new System.Windows.Forms.OpenFileDialog();
            fbd.Multiselect = false;
            fbd.CheckFileExists = true;
            fbd.Title = "选择日记文件以.mylife结尾";
            fbd.Filter = "日记文件|*.mylife";
            fbd.ShowDialog();
            if (fbd.FileName != string.Empty)
                texbBox.Text = fbd.FileName;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePath.Text;
            if (File.Exists(filePath))
            {
                SQLiteHelper.ConStr.Password = FilePassword.Password;
                SQLiteHelper.ConStr.DataSource = filePath;

                try
                {
                    SQLiteHelper.ExecuteZip();

                    Settings set = new Settings();
                    set.DBPath = filePath;
                    set.Save();

                    this.Closed -= Window_Closed;
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("密码错误");
                    FilePassword.Clear();
                }
            }

        }
    }
}
