using MyLife.DAL;
using MyLife.Helper;
using MyLife.Model;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyLife.BLL
{
    class DiaryBLL
    {
        internal bool Save(FlowDocument fd, string diaryTime)
        {
            bool isok = false;

            DateTime datetime = Convert.ToDateTime(diaryTime);
            TextRange textRange = new TextRange(fd.ContentStart, fd.ContentEnd);
            DiaryModel diaOld = getTodayDiary(datetime);
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
            string contents = XamlHelper.ToXaml(fd);
            int pubTime = TimesTampHelper.ConvertDateTimeInt(datetime);
            string title = datetime.ToString("yyyy-MM-dd") + " " + datetime.DayOfWeek;

            diaNew.Title = title;
            diaNew.PubTime = pubTime;
            diaNew.Contents = contents;

            //查找是否有当天内容
            if (diaOld != null)
            {
                diaNew.ID = diaOld.ID;
                diaNew.PubTime = diaOld.PubTime;
                return diaryDal.Update(diaNew);
            }

            int lastID = diaryDal.Insert(diaNew);
            isok = treeBll.Save(lastID, datetime);
            return isok;
        }

        //获得当天的日记
        internal DiaryModel getTodayDiary(DateTime diaryTime)
        {
            int pubTime = TimesTampHelper.ConvertDateTimeInt(diaryTime.Date);
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

        internal FlowDocument GetDoc(string DiaryTime)
        {
            FlowDocument fd = new FlowDocument();
            DateTime datetime = Convert.ToDateTime(DiaryTime);
            DiaryModel diaOld = getTodayDiary(datetime);

            if (diaOld != null)
            {
                fd = XamlHelper.FromXaml(diaOld.Contents);
            }
            else
            {
                //设置日记标题
                Paragraph p = new Paragraph();
                Run run = new Run() { Text = datetime.ToString("yyyy-MM-dd") + " " + datetime.DayOfWeek };
                p.Inlines.Add(run);
                fd.Blocks.Add(p);
                fd.Blocks.Add(new Paragraph());
            }
            return fd;
        }

        internal DateTime SelectTimeById(int? id)
        {
            int diaID = Convert.ToInt32(id);
            DiaryModel model = new DiaryDAL().GetById(diaID);
            return TimesTampHelper.GetTime(model.PubTime);
        }
    }
}
