using MyLife.BLL;
using MyLife.Helper;
using MyLife.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace MyLife
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string DiaryTime = DateTime.Now.ToString("yyyy-MM-dd");
        private static int countAll = 0;

        public MainWindow()
        {
            InitializeComponent();
            //判断时间切换样式
            if (DateTime.Now.Hour < 21 && DateTime.Now.Hour > 7)
            {
                ApplyStyle("/Style/Light.xaml");
            }
        }

        private void EditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //连接数据库
            if (CommonHelper.InitSql())
            {
                rtbEdit.AppendText("输入密码，按回车键进入\r\n密码：");
                pswLogin.Focus();
                EditWindow.MouseDown -= EditWindow_MouseDown;
                EditWindow.MouseDown += PasswordFocus;
                pswLogin.PreviewKeyDown += PasswordEnter;
            }
        }
        private void EditWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!tbSearch.IsKeyboardFocusWithin)
            {
                rtbEdit.Focus();
                e.Handled = true;
            }
            else
            {
                rtbEdit.Focus();
            }
        }//焦点始终在编辑区内，除非用户点了搜索
        private void EditWindow_Closed(object sender, EventArgs e)
        {
            if (SQLiteHelper.ConStr.Password.Length > 0)
            {
                SaveEdit();
            }
        }

        private void PasswordFocus(object sender, MouseButtonEventArgs e)
        {
            if (!tbSearch.IsKeyboardFocusWithin)
            {
                pswLogin.Focus();
                e.Handled = true;
            }
            else
            {
                pswLogin.Focus();
            }
        }
        private void PasswordEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SQLiteHelper.ConStr.Password = pswLogin.Password;
                e.Handled = true;
                try
                {
                    BindTree();
                    rtbEdit.Document.Blocks.Clear();
                    gridEdit.Children.Remove(pswLogin);
                    InitData();
                    this.rtbEdit.Focus();

                    EditWindow.MouseDown -= PasswordFocus;
                    EditWindow.MouseDown += EditWindow_MouseDown;
                    rtbEdit.PreviewKeyDown += rtbEdit_PreviewKeyDown;
                    gridToolBar.MouseEnter += gridToolBar_MouseEnter;
                    gridToolBar.MouseLeave += gridToolBar_MouseLeave;
                    gridSideBar.MouseEnter += girdSideBar_MouseEnter;
                    gridSideBar.MouseLeave += gridSideBar_MouseLeave;
                }
                catch
                {
                    MessageBox.Show("密码错误请重新输入");
                    pswLogin.Clear();
                }
            }
        }

        private void rtbEdit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & (ModifierKeys.Control)) == (ModifierKeys.Control))
            {
                if (Clipboard.ContainsImage())
                {
                    e.Handled = true;
                    BitmapSource bitmap = Clipboard.GetImage();
                    PngBitmapEncoder pE = new PngBitmapEncoder();
                    string tempFile = Path.GetTempFileName();
                    pE.Frames.Add(BitmapFrame.Create(bitmap));
                    using (Stream stream = File.Create(tempFile))
                    {
                        pE.Save(stream);
                    }

                    ImageHelper.InsertImg(rtbEdit, tempFile);
                    File.Delete(tempFile);
                }
            }
            else
            {
                string strText = new TextRange(rtbEdit.Document.ContentStart, rtbEdit.Document.ContentEnd).Text;
                if (strText.LastIndexOf("\r\n") > 0)
                {
                    strText = strText.Remove(strText.LastIndexOf("\r\n"));
                }
                if (strText.Length <= 0) return;
                int titleEnd = strText.IndexOf("\r\n");
                if (titleEnd > 31) titleEnd = 0;

                strText = strText.Remove(0, titleEnd);
                int count = Regex.Matches(strText, @"[^\s]").Count;
                if (count <= 0) return;

                tbCount.Text = count + "";

                if (count > countAll + 10 || count < countAll - 10)
                {
                    countAll = count;
                    SaveEdit();
                }
            }

        }
        private void rtbEdit_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        private void rtbEdit_PreviewDrop(object sender, DragEventArgs e)
        {

            Array arr = (Array)e.Data.GetData(DataFormats.FileDrop);
            if (arr == null) return;

            foreach (string item in arr)
            {
                ImageHelper.InsertImg(rtbEdit, item);
            }
            e.Handled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            ////测试用的，暂时不删
            //TextRange textRange = new TextRange(rtbEdit.Document.ContentStart, rtbEdit.Document.ContentEnd);
            //string xw = System.Windows.Markup.XamlWriter.Save(rtbEdit.Document);

            SaveEdit();
        }
        private void btnLight_Click(object sender, RoutedEventArgs e)
        {
            if (btnLight.Background.ToString() == "#FFF9F9F5")
            {
                ApplyStyle("/Style/Dark.xaml");
            }
            else
            {
                ApplyStyle("/Style/Light.xaml");
            }
        }
        private void btnHelper_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            UI.SettingWindow swin = new UI.SettingWindow();
            swin.ShowDialog();

        }

        private void girdSideBar_MouseEnter(object sender, MouseEventArgs e)
        {
            ThicknessAnimation ta = new ThicknessAnimation();
            ta.From = new Thickness(0, 0, 300, 0);
            ta.To = new Thickness(0, 0, 0, 0);

            SideBarAnimation(SideBar, 0, 1, ta);
            SideBar.Visibility = Visibility.Visible;
        }
        private void gridSideBar_MouseLeave(object sender, MouseEventArgs e)
        {
            ThicknessAnimation ta = new ThicknessAnimation();
            ta.From = new Thickness(0, 0, 0, 0);
            ta.To = new Thickness(0, 0, 300, 0);

            SideBarAnimation(SideBar, 1, 0, ta);
        }
        private void gridToolBar_MouseEnter(object sender, MouseEventArgs e)
        {
            ThicknessAnimation ta = new ThicknessAnimation();
            ta.From = new Thickness(300, 0, 0, 0);
            ta.To = new Thickness(0, 0, 0, 0);

            SideBarAnimation(ToolBar, 0, 1, ta);
            ToolBar.Visibility = Visibility.Visible;

        }
        private void gridToolBar_MouseLeave(object sender, MouseEventArgs e)
        {
            ThicknessAnimation ta = new ThicknessAnimation();
            ta.From = new Thickness(0, 0, 0, 0);
            ta.To = new Thickness(300, 0, 0, 0);

            SideBarAnimation(ToolBar, 1, 0, ta);
        }
        private void tvSideBar_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            object selectItem = tvSideBar.SelectedItem;
            TreeModel model = (TreeModel)selectItem;
            if (model.PID > 0)
            {
                new DiaryBLL().Save(this.rtbEdit, DiaryTime);
                rtbEdit.Document.Blocks.Clear();
                DiaryTime = new DiaryBLL().SelectTimeById(model.DiaID).ToString("yyyy-MM-dd");
                InitData();
            }
        }

        private void InitData()
        {
            //初始化
            this.Title = DiaryTime;
            //加载内容
            new DiaryBLL().TodayContent(this.rtbEdit, DiaryTime);
            TextRange textRange = new TextRange(rtbEdit.Document.ContentStart, rtbEdit.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, rtbEdit.Foreground);
        }
        private void BindTree()
        {
            List<TreeModel> treeView = new TreeBLL().CreateTree();
            this.tvSideBar.ItemsSource = treeView;
            Helper.CommonHelper.ExpandLastNode(tvSideBar);
        }
        private void ApplyStyle(string stylePath)
        {
            ResourceDictionary myResourceDictionary = MyLife.App.LoadComponent(new Uri(stylePath, UriKind.Relative)) as ResourceDictionary;
            if (stylePath.Equals("/Style/Dark.xaml"))
            {
                MyLife.App.Current.Resources.MergedDictionaries.Clear();
            }
            MyLife.App.Current.Resources.MergedDictionaries.Add(myResourceDictionary);

            TextRange textRange = new TextRange(rtbEdit.Document.ContentStart, rtbEdit.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, rtbEdit.Foreground);
        }
        private void SaveEdit()
        {
            bool isok = new DiaryBLL().Save(this.rtbEdit, DiaryTime);
            BindTree();
        }
        private static void SideBarAnimation(Panel sideBar, int from, int to, ThicknessAnimation ta)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = from;
            da.To = to;
            da.Duration = TimeSpan.FromSeconds(0.4);
            sideBar.BeginAnimation(TextBlock.OpacityProperty, da);

            ta.Duration = TimeSpan.FromSeconds(0.2);
            sideBar.BeginAnimation(TextBlock.MarginProperty, ta);
        }//侧边栏动画

    }
}
