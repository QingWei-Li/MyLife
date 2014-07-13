using System.Collections.Generic;

namespace MyLife.BLL
{
    class SettingBLL
    {
        internal void SaveMailSet(string pop, string port, string mail, string password, string keyword)
        {
            DAL.SettingDAL dal = new DAL.SettingDAL();
            Model.SettingModel model = new Model.SettingModel();
            model.Key = "POP";
            model.Value = pop;
            dal.Update(model);
            model.Key = "Port";
            model.Value = port;
            dal.Update(model);
            model.Key = "Mail";
            model.Value = mail;
            dal.Update(model);
            model.Key = "MailPwd";
            model.Value = password;
            dal.Update(model);
            model.Key = "Keyword";
            model.Value = keyword;
            dal.Update(model);
        }

        internal Dictionary<string, string> GetMailSet()
        {
            DAL.SettingDAL dal = new DAL.SettingDAL();
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add("POP", dal.GetByKey("POP").Value);
            dic.Add("Port", dal.GetByKey("Port").Value);
            dic.Add("Mail", dal.GetByKey("Mail").Value);
            dic.Add("MailPwd", dal.GetByKey("MailPwd").Value);
            dic.Add("Keyword", dal.GetByKey("Keyword").Value);

            return dic;
        }
    }
}
