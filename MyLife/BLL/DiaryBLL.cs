using MyLife.DAL;
using MyLife.Helper;
using MyLife.Model;
using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyLife.BLL
{
    class DiaryBLL
    {
        internal bool Save(RichTextBox rtb, string diaryTime)
        {
            bool isok = false;

            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            DiaryModel diaOld = getTodayDiary(Convert.ToDateTime(diaryTime));
            DiaryDAL diaryDal = new DiaryDAL();
            TreeBLL treeBll = new TreeBLL();

            //如果内容清空表示删除日记
            if (textRange.Text.Length <= 0)
            {
                if (diaOld != null)
                {
                    diaryDal.DeleteById(diaOld.ID);
                    treeBll.Delete(diaOld.ID);
                }
                return true;
            }

            //保存日记内容
            DiaryModel diaNew = new DiaryModel();
            string contents = XamlHelper.ToXaml(rtb);
            int pubTime = TimesTampHelper.ConvertDateTimeInt(Convert.ToDateTime(diaryTime));
            string title = textRange.Text.Substring(0, textRange.Text.IndexOf("\r\n"));

            diaNew.Title = title;
            diaNew.PubTime = pubTime;
            diaNew.Contents = contents;
            diaNew.IsUpload = 0;
            if (title.Length > 30)
            {
                diaNew.Title = diaryTime + " " + DateTime.Now.DayOfWeek;
            }

            //查找是否有当天内容
            if (diaOld != null)
            {
                diaNew.ID = diaOld.ID;
                diaNew.PubTime = diaOld.PubTime;
                diaNew.IsUpload = diaOld.IsUpload;
                return diaryDal.Update(diaNew);
            }

            int lastID = diaryDal.Insert(diaNew);
            isok = treeBll.Save(lastID);
            return isok;
        }

        //获得当天的日记
        internal DiaryModel getTodayDiary(DateTime diaryTime)
        {
            int pubTime = TimesTampHelper.ConvertDateTimeInt(diaryTime);
            DataTable dt = SQLiteHelper.ExecuteDataTable("select * from Diaries where PubTime=" + pubTime);

            if (dt.Rows.Count == 1)
            {
                return new DiaryDAL().ToModel(dt.Rows[0]);
            }
            else if (dt.Rows.Count > 1)
            {
                throw new Exception("more than 1 row was found");
            }
            else
            {
                return null;
            }
        }

        internal void TodayContent(RichTextBox rtb, string DiaryTime)
        {
            DiaryModel diaOld = getTodayDiary(Convert.ToDateTime(DiaryTime));

            if (diaOld != null)
            {
                XamlHelper.FromXaml(rtb, diaOld.Contents);
            }
            else
            {
                //设置日记标题
                rtb.AppendText(DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.DayOfWeek);
                rtb.Document.Blocks.Add(new Paragraph());
            }
            //设置默认焦点位置
            rtb.CaretPosition = rtb.CaretPosition.DocumentEnd;
        }

        internal DateTime SelectTimeById(int? id)
        {
            int diaID = Convert.ToInt32(id);
            DiaryModel model = new DiaryDAL().GetById(diaID);
            return TimesTampHelper.GetTime(model.PubTime);
        }
    }
}
