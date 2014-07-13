using MyLife.DAL;
using MyLife.Helper;
using MyLife.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyLife.BLL
{
    class TreeBLL
    {
        public List<TreeModel> CreateTree()
        {
            TreeDAL treeDal = new TreeDAL();
            List<TreeModel> nodes = treeDal.ListAll().ToList();
            List<TreeModel> outputList = new List<TreeModel>();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].PID == 0)
                {
                    outputList.Add(nodes[i]);
                }
                else
                {
                    FindDownward(nodes, nodes[i].PID).Nodes.Add(nodes[i]);
                }
            }
            return outputList;
        }

        private TreeModel FindDownward(List<TreeModel> nodes, int pid)
        {
            if (nodes == null) return null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ID == pid)
                {
                    return nodes[i];
                }
            }
            return null;
        }

        internal bool Save(int id,DateTime datetime)
        {
            bool isok = false;
            //保存树节点
            TreeDAL treeDal = new TreeDAL();
            TreeModel yearMonth = new TreeModel();
            TreeModel day = new TreeModel();
            string NodeName = datetime.ToString("yyyy-MM");
            DataTable dt = SQLiteHelper.ExecuteDataTable("select * from Tree where Name='" + NodeName + "'");

            if (dt.Rows.Count == 1)
            {
                yearMonth = treeDal.ToModel(dt.Rows[0]);
                day.PID = yearMonth.ID;
                day.DiaID = id;
                day.Name = datetime.Day + "日 " + new CommonHelper().Week(datetime);
                treeDal.Insert(day);

            }
            else if (dt.Rows.Count > 1)
            {
                throw new Exception("more than 1 row was found");
            }
            else
            {
                yearMonth.PID = 0;
                yearMonth.Name = NodeName;
                treeDal.Insert(yearMonth);
                isok = Save(id, datetime);

            }
            return isok;
        }

        internal bool Delete(int id)
        {
            TreeDAL treeDal = new TreeDAL();
            DataTable dt = SQLiteHelper.ExecuteDataTable("select * from Tree where PID =(select PID from Tree where DiaID=" + id + ")");

            if (dt.Rows.Count <= 1)
            {
                SQLiteHelper.ExecuteNonQuery("delete from Tree where  ID =(select PID from Tree where DiaID=" + id + ") ");
                SQLiteHelper.ExecuteNonQuery("delete from Tree where DiaID=" + id);
            }

            return true;
        }
    }
}
