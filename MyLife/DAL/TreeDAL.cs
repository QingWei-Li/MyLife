using MyLife.Helper;
using MyLife.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace MyLife.DAL
{
    class TreeDAL
    {

        public TreeModel ToModel(DataRow row)
        {
            TreeModel model = new TreeModel();
            model.ID = Convert.ToInt32(row["ID"]);
            model.PID = Convert.ToInt32(row["PID"]);
            model.DiaID = Convert.ToInt32(SQLiteHelper.FromDBValue(row["DiaID"]));
            model.Name = Convert.ToString(row["Name"]);
            return model;
        }

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <param name="model">Tree类的对象</param>
        /// <returns>插入是否成功</returns>
        public int Insert(TreeModel model)
        {
            SQLiteParameter[] parameters ={
                                               new SQLiteParameter("@PID")
                                              , new SQLiteParameter("@DiaID")
                                              , new SQLiteParameter("@Name")
                                          };
            parameters[0].Value = model.PID;
            parameters[1].Value = model.DiaID;
            parameters[2].Value = model.Name;

            object count = SQLiteHelper.ExecuteNonQuery(@"INSERT INTO Tree(PID, DiaID, Name) VALUES(@PID, @DiaID, @Name)", parameters);
            if (count != null)
            {
                return Convert.ToInt32(count);
            }
            return -1;
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>删除是否成功</returns>
        public bool DeleteById(System.Int32 ID)
        {
            SQLiteParameter[] parameters ={
                                              new SQLiteParameter("@ID")
                                          };
            parameters[0].Value = ID;

            int rows = SQLiteHelper.ExecuteNonQuery("DELETE FROM Tree WHERE ID = @ID", parameters);
            return rows > 0;
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="model">T_GuideTypes类的对象</param>
        /// <returns>更新是否成功</returns>
        public bool Update(TreeModel model)
        {
            SQLiteParameter[] parameters ={
                                              new SQLiteParameter("@ID")
                                              , new SQLiteParameter("@PID")
                                              , new SQLiteParameter("@DiaID")
                                              , new SQLiteParameter("@Name")
                                          };
            parameters[0].Value = model.ID;
            parameters[1].Value = model.PID;
            parameters[2].Value = model.DiaID;
            parameters[2].Value = model.Name;

            int count = SQLiteHelper.ExecuteNonQuery("UPDATE Tree SET PID=@PID, DiaID=@DiaID, Name=@Name WHERE ID=@ID", parameters);
            return count > 0;
        }

        /// <summary>
        /// 获得一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>T_GuideTypes类的对象</returns>
        public TreeModel GetById(System.Int32 ID)
        {
            SQLiteParameter[] parameters ={
                                              new SQLiteParameter("@ID")
                                          };
            parameters[0].Value = ID;

            DataTable dt = SQLiteHelper.ExecuteDataTable("SELECT ID,PID,DiaID,Name FROM Tree WHERE ID=@ID", parameters);
            if (dt.Rows.Count > 1)
            {
                throw new Exception("more than 1 row was found");
            }
            else if (dt.Rows.Count <= 0)
            {
                return null;
            }
            DataRow row = dt.Rows[0];
            TreeModel model = ToModel(row);
            return model;
        }

        /// <summary>
        /// 获得所有记录
        /// </summary>
        /// <returns>T_GuideTypes类的对象的枚举</returns>
        public IEnumerable<TreeModel> ListAll()
        {
            List<TreeModel> list = new List<TreeModel>();
            DataTable dt = SQLiteHelper.ExecuteDataTable("SELECT ID,PID,DiaID,Name FROM Tree");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(ToModel(row));
            }
            return list;
        }

    }
}
