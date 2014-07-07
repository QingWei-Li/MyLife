using OpenPop.Common.Logging;
using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Message = OpenPop.Mime.Message;

namespace MyLife.Helper
{
    class MailHelper
    {
        private readonly Pop3Client pop3Client = new Pop3Client();
        private readonly Dictionary<int, Message> messages = new Dictionary<int, Message>();

        public void ReceiveMails(string pop, string port, string mailname, string mailpassword)
        {
            try
            {
                if (pop3Client.Connected)
                    pop3Client.Disconnect();
                pop3Client.Connect(pop, int.Parse(port), true);
                pop3Client.Authenticate(mailname, mailpassword);
                int count = pop3Client.GetMessageCount();
                messages.Clear();

                for (int i = count; i >= 1; i -= 1)
                {
                    try
                    {
                        Message message = pop3Client.GetMessage(i);
                        if (message.Headers.Subject.StartsWith("123"))
                        {
                            messages.Add(i, message);
                        }
                    }
                    catch (Exception e)
                    {
                    }

                }

            }
            catch (InvalidLoginException)
            {
                MessageBox.Show("The server did not accept the user credentials!", "POP3 Server Authentication");
            }
            catch (PopServerNotFoundException)
            {
                MessageBox.Show("The server could not be found", "POP3 Retrieval");
            }
            catch (PopServerLockedException)
            {
                MessageBox.Show("The mailbox is locked. It might be in use or under maintenance. Are you connected elsewhere?", "POP3 Account Locked");
            }
            catch (LoginDelayException)
            {
                MessageBox.Show("Login not allowed. Server enforces delay between logins. Have you connected recently?", "POP3 Account Login Delay");
            }
            catch (Exception e)
            {
                MessageBox.Show("Error occurred retrieving mail. " + e.Message, "POP3 Retrieval");
            }

        }
    }
}
