using System;
using System.Collections.Generic;
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
    /// CountWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CountWindow : Window
    {
        public CountWindow()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            DAL.DiaryDAL dal = new DAL.DiaryDAL();
            List<Model.DiaryModel> list = dal.ListAll().ToList();
            totalDiaryCount.Text = list.Count.ToString() + " 篇";

            DateTime startDateTime = Helper.TimesTampHelper.GetTime(list[0].PubTime);
            DateTime endDateTime = Helper.TimesTampHelper.GetTime(list[list.Count - 1].PubTime);
            TimeSpan ts = startDateTime - endDateTime;
            string num = ((float)list.Count / (float)(ts.Days + 1)).ToString("0.0");
            avgDiaryTime.Text = num + " 天";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
