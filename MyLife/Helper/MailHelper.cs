using OpenPop.Mime;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using Message = OpenPop.Mime.Message;

namespace MyLife.Helper
{
    class MailHelper
    {
        private static readonly Pop3Client pop3Client = new Pop3Client();

        public static void ReceiveMails(System.ComponentModel.BackgroundWorker backgroundWorker)
        {
            string pop, port, mailname, mailpassword, keyword;
            try
            {
                BLL.SettingBLL bll = new BLL.SettingBLL();
                Dictionary<string, string> dic = bll.GetMailSet();
                pop = dic["POP"];
                port = dic["Port"];
                mailname = dic["Mail"];
                mailpassword = dic["MailPwd"];
                keyword = dic["Keyword"];

                if (pop3Client.Connected)
                    pop3Client.Disconnect();
                pop3Client.Connect(pop, int.Parse(port), true);
                pop3Client.Authenticate(mailname, mailpassword);
                int count = pop3Client.GetMessageCount();

                for (int i = 1; i <= count; i++)
                {
                    backgroundWorker.ReportProgress((int)(((double)i / count) * 100));
                    /*
                    * 1.接收邮件，筛选出带有关键字开头的邮件
                    * 2.将邮件格式转成xaml便于读取
                    * 3.判断邮件时间，如果当天邮件则进行合并和显示操作；如果是以前的时间，就进行合并和保存操作
                    * 4.删除已读取的邮件
                    */
                    Message message = pop3Client.GetMessage(i);
                    if (message.Headers.Subject.StartsWith(keyword))
                    {
                        SaveMail(i, message);
                        pop3Client.DeleteMessage(i);
                        continue;
                    }
                }

            }
            catch
            {
                return;
            }
            //提交删除的更改信息
            TryMail(pop, port, mailname, mailpassword);
        }

        private static void SaveMail(int i, Message message)
        {
            MessagePart plainHtmlPart = message.FindFirstHtmlVersion();
            MessagePart plainTextPart = message.FindFirstPlainTextVersion();
            string textMail = null;
            if (plainHtmlPart == null && plainTextPart != null)
            {
                textMail = plainTextPart.GetBodyAsText();
            }
            else if (plainHtmlPart == null && plainTextPart == null)
            {
                List<MessagePart> textVersions = message.FindAllTextVersions();
                if (textVersions.Count >= 1)
                    textMail = textVersions[0].GetBodyAsText();
            }
            else if (plainHtmlPart != null)
            {
                textMail = plainHtmlPart.GetBodyAsText();
            }

            string xamlMail = HTMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(textMail, true);
            StringReader sr = new StringReader(xamlMail);
            XmlReader xr = XmlReader.Create(sr);
            FlowDocument fdMail = (FlowDocument)XamlReader.Load(xr);
            FlowDocument fd = new FlowDocument();
            DateTime dateMail = Convert.ToDateTime(message.Headers.Date);
            BLL.DiaryBLL dbll = new BLL.DiaryBLL();

            fd = dbll.GetDoc(dateMail.Date.ToString());
            fd = CommonHelper.MergeFlowDocument(fd, fdMail);
            dbll.Save(fd, dateMail.Date.ToString());

        }

        public static bool TryMail(string pop, string port, string mailname, string mailpassword)
        {
            try
            {
                if (pop3Client.Connected)
                    pop3Client.Disconnect();
                pop3Client.Connect(pop, int.Parse(port), true);
                pop3Client.Authenticate(mailname, mailpassword);
                return true;
            }
            catch (InvalidLoginException)
            {
                MessageBox.Show("用户名或密码错误", "POP3 Server Authentication");
                return false;

            }
            catch (PopServerNotFoundException)
            {
                MessageBox.Show("服务器无法找到，可能是pop或端口填写错误，或是无网络连接", "POP3 Retrieval");
                return false;

            }
            catch (PopServerLockedException)
            {
                MessageBox.Show("邮箱被锁定，这可能是正在被使用或是服务商在维护", "POP3 Account Locked");
                return false;

            }
            catch (LoginDelayException)
            {
                MessageBox.Show("登录不被允许，可能是连接超时，检查下网络后再试一次", "POP3 Account Login Delay");
                return false;

            }
            catch (Exception e)
            {
                MessageBox.Show("请检查信息是否填写正确或网络是否可用\n\n错误描述： " + e.Message, "连接出错");
                return false;

            }

        }
    }
}
