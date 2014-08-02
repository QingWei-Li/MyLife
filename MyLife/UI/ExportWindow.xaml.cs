using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace MyLife.UI
{
    /// <summary>
    /// ExportWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExportWindow : Window
    {
        private static string filepath = null;
        private int count = 0;

        public ExportWindow()
        {
            InitializeComponent();
        }

        private string SelectPath()
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
                return fbd.SelectedPath;
            return null;
        }
        private static string SaveHtml(string filepath, DateTime filetime)
        {
            BLL.DiaryBLL dbll = new BLL.DiaryBLL();
            Model.DiaryModel dmodel = dbll.getTodayDiary(filetime);
            FlowDocument fd = Helper.XamlHelper.FromXaml(dmodel.Contents);

            string strDoc = System.Windows.Markup.XamlWriter.Save(fd);
            string strHtml = HTMLConverter.HtmlFromXamlConverter.ConvertXamlToHtml(strDoc);
            filepath = filepath + System.IO.Path.DirectorySeparatorChar + Helper.TimesTampHelper.GetTime(dmodel.PubTime).ToString("yyyy-MM-dd") + ".html";
            strHtml = strHtml.Replace("#F1F1F1;", "#2C3E50");
            File.WriteAllText(filepath, strHtml);
            return filepath;
        }
        private static string SaveRtf(string filepath, DateTime filetime)
        {
            BLL.DiaryBLL dbll = new BLL.DiaryBLL();
            Model.DiaryModel dmodel = dbll.getTodayDiary(filetime);
            filepath = filepath + System.IO.Path.DirectorySeparatorChar + Helper.TimesTampHelper.GetTime(dmodel.PubTime).ToString("yyyy-MM-dd") + ".rtf";

            using (FileStream stream = File.OpenWrite(filepath))
            {
                FlowDocument fd = Helper.XamlHelper.FromXaml(dmodel.Contents);
                TextRange tr = new TextRange(fd.ContentStart, fd.ContentEnd);
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Black);
                tr.Save(stream, DataFormats.Rtf);
            }
            return filepath;
        }

        private void btnTodayHtml_Click(object sender, RoutedEventArgs e)
        {
            filepath = SelectPath();
            if (filepath != null)
            {
                filepath = SaveHtml(filepath, DateTime.Now.Date);
                MessageBox.Show("文件已保存在" + filepath, "保存成功");
            }
        }
        private void bthTodayRtf_Click(object sender, RoutedEventArgs e)
        {
            filepath = SelectPath();
            if (filepath != null)
            {
                filepath = SaveRtf(filepath, DateTime.Now.Date);
                MessageBox.Show("文件已保存在" + filepath, "保存成功");
            }
        }
        private void btnHtmlAll_Click(object sender, RoutedEventArgs e)
        {
            filepath = SelectPath();
            if (filepath != null)
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += btnHtmlAll_Dowork;
                bgw.RunWorkerCompleted += RunWorkerCompleted;
                bgw.ProgressChanged += btnHtmlAll_ProgressChanged;
                bgw.WorkerReportsProgress = true;
                bgw.RunWorkerAsync();
            }
        }
        private void bthRtfAll_Click(object sender, RoutedEventArgs e)
        {
            filepath = SelectPath();
            if (filepath != null)
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += btnRtfAll_Dowork;
                bgw.RunWorkerCompleted += RunWorkerCompleted;
                bgw.ProgressChanged += btnRtfAll_ProgressChanged;
                bgw.WorkerReportsProgress = true;
                bgw.RunWorkerAsync();
            }
        }
        private void btnHtmlAlone_Click(object sender, RoutedEventArgs e)
        {
            filepath = SelectPath();
            if (filepath != null)
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += btnHtmlAlone_DoWork;
                bgw.RunWorkerCompleted += RunWorkerCompleted;
                bgw.ProgressChanged += btnHtmlAlone_ProgressChanged;
                bgw.WorkerReportsProgress = true;
                bgw.RunWorkerAsync();
            }
        }
        private void bthRtfAlone_Click(object sender, RoutedEventArgs e)
        {
            filepath = SelectPath();
            if (filepath != null)
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += btnRtfAlone_Dowork;
                bgw.RunWorkerCompleted += RunWorkerCompleted;
                bgw.ProgressChanged += btnRtfAlone_ProgressChanged;
                bgw.WorkerReportsProgress = true;
                bgw.RunWorkerAsync();
            }
        }

        private void btnRtfAll_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbRtfAll.Value = e.ProgressPercentage;
        }
        private void btnRtfAll_Dowork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            DAL.DiaryDAL dal = new DAL.DiaryDAL();
            List<Model.DiaryModel> listModel = dal.ListAll().ToList();
            FlowDocument fdAll = new FlowDocument();
            count = 0;
            filepath = filepath + System.IO.Path.DirectorySeparatorChar + "mylife.rtf";

            using (FileStream stream = File.OpenWrite(filepath))
            {
                foreach (Model.DiaryModel model in listModel)
                {
                    FlowDocument fd = Helper.XamlHelper.FromXaml(model.Contents);
                    fdAll = Helper.CommonHelper.MergeFlowDocument(fd, fdAll);
                    count++;
                    bgw.ReportProgress((int)((float)count / (float)listModel.Count * 100));
                }

                TextRange tr = new TextRange(fdAll.ContentStart, fdAll.ContentEnd);
                tr.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Black);
                tr.Save(stream, DataFormats.Rtf);
            }

        }
        private void btnHtmlAll_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbHtmlAll.Value = e.ProgressPercentage;
        }
        private void btnHtmlAll_Dowork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgw = (BackgroundWorker)sender;
            DAL.DiaryDAL dal = new DAL.DiaryDAL();
            List<Model.DiaryModel> listModel = dal.ListAll().ToList();
            string strHtml = "";
            count = 0;
            filepath = filepath + System.IO.Path.DirectorySeparatorChar + "mylife.html";

            foreach (Model.DiaryModel model in listModel)
            {
                FlowDocument fd = Helper.XamlHelper.FromXaml(model.Contents);
                string strDoc = System.Windows.Markup.XamlWriter.Save(fd);
                strHtml += HTMLConverter.HtmlFromXamlConverter.ConvertXamlToHtml(strDoc);
                count++;
                bgw.ReportProgress((int)((float)count / (float)listModel.Count * 100));
            }

            strHtml = strHtml.Replace("#F1F1F1;", "#2C3E50");
            File.WriteAllText(filepath, strHtml);

        }
        private void btnRtfAlone_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbRtfAlone.Value = e.ProgressPercentage;
        }
        private void btnRtfAlone_Dowork(object sender, DoWorkEventArgs e)
        {
            filepath = filepath + System.IO.Path.DirectorySeparatorChar + "MyLife-RTF" + System.IO.Path.DirectorySeparatorChar;
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);

            BackgroundWorker bgw = (BackgroundWorker)sender;
            DAL.DiaryDAL dal = new DAL.DiaryDAL();
            List<Model.DiaryModel> listModel = dal.ListAll().ToList();
            count = 0;
            foreach (Model.DiaryModel model in listModel)
            {
                DateTime filetime = Helper.TimesTampHelper.GetTime(model.PubTime);
                SaveRtf(filepath, filetime);
                count++;
                bgw.ReportProgress((int)((float)count / (float)listModel.Count * 100));
            }
        }
        private void btnHtmlAlone_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbHtmlAlone.Value = e.ProgressPercentage;
        }
        private void btnHtmlAlone_DoWork(object sender, DoWorkEventArgs e)
        {
            filepath = filepath + System.IO.Path.DirectorySeparatorChar + "MyLife-HTML" + System.IO.Path.DirectorySeparatorChar;
            if (!Directory.Exists(filepath))
                Directory.CreateDirectory(filepath);

            BackgroundWorker bgw = (BackgroundWorker)sender;
            DAL.DiaryDAL dal = new DAL.DiaryDAL();
            List<Model.DiaryModel> listModel = dal.ListAll().ToList();
            count = 0;
            foreach (Model.DiaryModel model in listModel)
            {
                DateTime filetime = Helper.TimesTampHelper.GetTime(model.PubTime);
                SaveHtml(filepath, filetime);
                count++;
                bgw.ReportProgress((int)((float)count / (float)listModel.Count * 100));
            }
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("文件已保存在" + filepath, "保存成功");
        }
    }

}
