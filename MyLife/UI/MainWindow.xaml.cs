using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
        private BackgroundWorker backgroundWorker = new BackgroundWorker();
        private System.Timers.Timer timer = new System.Timers.Timer();
        const int COUNTDOWN = 3 * 60 * 1000;
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
            if (Helper.CommonHelper.InitSql())
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
            if (Helper.SQLiteHelper.ConStr.Password.Length > 0)
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
                Helper.SQLiteHelper.ConStr.Password = pswLogin.Password;
                backgroundWorker.DoWork += backgroundWorker_DoWork;
                backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
                backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
                backgroundWorker.WorkerReportsProgress = true;
                pbMail.Visibility = Visibility.Visible;
                e.Handled = true;
                try
                {
                    BindTree();
                    rtbEdit.Document.Blocks.Clear();
                    gridEdit.Children.Remove(pswLogin);
                    InitData();
                    backgroundWorker.RunWorkerAsync();
                    SaveEdit();
                    this.rtbEdit.Focus();
                    timer.Interval = COUNTDOWN;
                    timer.Elapsed += timer_Elapsed;
                    timer.AutoReset = false;
                    timer.Start();

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

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //超过规定时间无内容输入就重启程序，达到锁定程序的目的
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.Dispatcher.Invoke(new Action(() => { Application.Current.Shutdown(); }));
        }


        private void rtbEdit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            timer.Interval = COUNTDOWN;

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

                    Helper.ImageHelper.InsertImg(rtbEdit, tempFile);
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
                if (titleEnd > 31 || titleEnd < 0) titleEnd = 0;

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
                Helper.ImageHelper.InsertImg(rtbEdit, item);
            }
            e.Handled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
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
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            SaveEdit();
            UI.ExportWindow expwin = new UI.ExportWindow();
            expwin.ShowDialog();
        }
        private void btnCount_Click(object sender, RoutedEventArgs e)
        {
            UI.CountWindow win = new UI.CountWindow();
            win.ShowDialog();
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
            Model.TreeModel model = (Model.TreeModel)tvSideBar.SelectedItem;

            if (model.PID > 0)
            {
                rtbEdit.Document.Blocks.Clear();
                DiaryTime = new BLL.DiaryBLL().SelectTimeById(model.DiaID).ToString("yyyy-MM-dd");
                InitData();

                if (tbSearch.Text.Trim() != String.Empty)
                {
                    Helper.SearchHelper.ReplaceKeywordColor(rtbEdit.Document, tbSearch.Text.Trim());
                }
            }
        }
        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbSearch.Text.Trim().Length <= 0)
            {
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Enter)
            {
                BindTree();
                TextRange textRange = new TextRange(rtbEdit.Document.ContentStart, rtbEdit.Document.ContentEnd);
                textRange.ApplyPropertyValue(TextElement.BackgroundProperty, rtbEdit.Background);
                Helper.SearchHelper.ReplaceKeywordColor(rtbEdit.Document, tbSearch.Text.Trim());

                //搜索其他时间带有关键字的日记
                DAL.DiaryDAL dal = new DAL.DiaryDAL();
                List<Model.DiaryModel> listModel = dal.ListAll().ToList();
                List<Model.TreeModel> listTreeView = tvSideBar.ItemsSource as List<Model.TreeModel>;
                List<Model.TreeModel> listTreeSrc = new DAL.TreeDAL().ListAll().ToList();

                foreach (Model.DiaryModel model in listModel)
                {
                    FlowDocument fd = Helper.XamlHelper.FromXaml(model.Contents);
                    bool isFind = Helper.SearchHelper.isFindMatchedTextRanges(fd, tbSearch.Text.Trim());

                    if (isFind)
                    {
                        Model.TreeModel nodeSrc = listTreeSrc.Find(delegate(Model.TreeModel t) { return t.DiaID == model.ID; });
                        Model.TreeModel tvModel = listTreeView.Find(delegate(Model.TreeModel t) { return t.ID == nodeSrc.PID; });
                        Model.TreeModel tvNodeModel = tvModel.Nodes.Find(delegate(Model.TreeModel t) { return t.DiaID == nodeSrc.DiaID; });

                        TreeViewItem currentContainer = tvSideBar.ItemContainerGenerator.ContainerFromItem(tvModel) as TreeViewItem;
                        currentContainer.SetValue(TreeViewItem.IsExpandedProperty, true);
                        currentContainer.UpdateLayout();

                        TreeViewItem tvitem = currentContainer.ItemContainerGenerator.ContainerFromItem(tvNodeModel) as TreeViewItem;
                        tvitem.Background = System.Windows.Media.Brushes.IndianRed;

                    }

                }

            }
        }
        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            tbSearch.Text = String.Empty;
        }

        private void InitData()
        {
            //初始化
            this.Title = DiaryTime;
            //加载内容
            rtbEdit.Document = new BLL.DiaryBLL().GetDoc(DiaryTime);
            rtbEdit.CaretPosition = rtbEdit.CaretPosition.DocumentEnd;
            TextRange textRange = new TextRange(rtbEdit.Document.ContentStart, rtbEdit.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, rtbEdit.Foreground);
        }
        private void BindTree()
        {
            List<Model.TreeModel> treeView = new BLL.TreeBLL().CreateTree();
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
            TextRange textRange = new TextRange(rtbEdit.Document.ContentStart, rtbEdit.Document.ContentEnd);
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, rtbEdit.Background);
            bool isok = new BLL.DiaryBLL().Save(this.rtbEdit.Document, DiaryTime);
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
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gridMain.Children.Remove(pbMail);
            InitData();
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Helper.MailHelper.ReceiveMails(backgroundWorker);
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbMail.Value = e.ProgressPercentage;
        }

    }
}
