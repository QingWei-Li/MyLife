using MyLife.Helper;
using System.Collections.Generic;
using System.Windows;

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
            Init();
        }

        private void Init()
        {
            BLL.SettingBLL bll = new BLL.SettingBLL();
            Dictionary<string, string> dic = bll.GetMailSet();
            tbPOP.Text = dic["POP"];
            tbPort.Text = dic["Port"];
            tbMail.Text = dic["Mail"];
            tbMailPwd.Password = dic["MailPwd"];
            tbKeyword.Text = dic["Keyword"];
        }

        private void btnMailSave_Click(object sender, RoutedEventArgs e)
        {
            MailHelper mailHelper = new MailHelper();
            bool isok = mailHelper.TryMail(tbPOP.Text, tbPort.Text, tbMail.Text, tbMailPwd.Password);
            if (isok)
            {
                BLL.SettingBLL bll = new BLL.SettingBLL();
                bll.SaveMailSet(tbPOP.Text, tbPort.Text, tbMail.Text, tbMailPwd.Password, tbKeyword.Text);
                this.Close();
            }
        }
        private void btnPwdSet_Click(object sender, RoutedEventArgs e)
        {
            if (tbNewPwd.Password.Equals(tbNewPwd2.Password))
            {
                if (!Helper.SQLiteHelper.ChangePassword(tbNewPwd.Password, tbOldPwd.Password))
                {
                    MessageBox.Show("原密码不正确", "密码错误");
                    tbOldPwd.Password = string.Empty;
                    tbOldPwd.Focus();
                    return;
                }
                else
                {
                    MessageBox.Show("修改成功", "完成");
                    Helper.SQLiteHelper.ConStr.Password = tbNewPwd.Password;
                    tbOldPwd.Password = string.Empty;
                    tbNewPwd.Password = string.Empty;
                    tbNewPwd2.Password = string.Empty;
                    return;
                }
            }
            else
            {
                MessageBox.Show("两次输入的密码不一致", "密码错误");
                tbNewPwd.Password = string.Empty;
                tbNewPwd2.Password = string.Empty;
                tbNewPwd.Focus();
                return;
            }
        }

        private void tbMail_LostFocus(object sender, RoutedEventArgs e)
        {
            CommonHelper comHelper = new CommonHelper();
            string[] pop = comHelper.GetPOP(tbMail.Text);
            if (null != pop)
            {
                tbPOP.Text = pop[0];
                tbPort.Text = pop[1];
            }
            else
            {
                tbPOP.Text = string.Empty;
                tbPort.Text = string.Empty;
            }
        }

    }
}
