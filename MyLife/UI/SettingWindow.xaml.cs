﻿using MyLife.Helper;
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
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }

        private void bthMailSet_Click(object sender, RoutedEventArgs e)
        {
            MailHelper mailHelper = new MailHelper();
        }

        private void btnMailKeyword_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
