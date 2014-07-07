using MyLife.Helper;
using MyLife.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace MyLife.DAL
{
    class DiaryDAL
    {
        public DiaryModel ToModel(DataRow row)
        {
            DiaryModel model = new DiaryModel();
            model.ID = Convert.ToInt32(row["ID"]);
            model.PubTime = Convert.ToInt32(row["PubTime"]);
            model.IsUpload = Convert.ToInt32(row["IsUpload"]);
            model.Title = Convert.ToString(row["Title"]);
            model.Contents = Convert.ToString(row["Contents"]);
            return model;
        }

        /// <summary>
        /// 插入一条记录
        /// </summary>
        /// <param name="model">Diaries类的对象</param>
        /// <returns>插入是否成功</returns>
        public int Insert(DiaryModel model)
        {
            SQLiteParameter[] parameters ={ 
                                              new SQLiteParameter("@PubTime")
                                              , new SQLiteParameter("@IsUpload")
                                              , new SQLiteParameter("@Title")
                                              , new SQLiteParameter("@Contents")
                                          };
            parameters[0].Value = model.PubTime;
            parameters[1].Value = model.IsUpload;
            parameters[2].Value = model.Title;
            parameters[3].Value = model.Contents;

            object count = SQLiteHelper.ExecuteScalar(@"INSERT INTO Diaries(PubTime, IsUpload, Title, Contents) VALUES(@PubTime, @IsUpload, @Title, @Contents)", parameters);
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

            int rows = SQLiteHelper.ExecuteNonQuery("DELETE FROM Diaries WHERE ID = @ID", parameters);
            return rows > 0;
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="model">T_GuideTypes类的对象</param>
        /// <returns>更新是否成功</returns>
        public bool Update(DiaryModel model)
        {
            SQLiteParameter[] parameters ={
                                              new SQLiteParameter("@ID")
                                              , new SQLiteParameter("@PubTime")
                                              , new SQLiteParameter("@IsUpload")
                                              , new SQLiteParameter("@Title")
                                              , new SQLiteParameter("@Contents")
                                          };
            parameters[0].Value = model.ID;
            parameters[1].Value = model.PubTime;
            parameters[2].Value = model.IsUpload;
            parameters[3].Value = model.Title;
            parameters[4].Value = model.Contents;

            int count = SQLiteHelper.ExecuteNonQuery("UPDATE Diaries SET PubTime=@PubTime, IsUpload=@IsUpload, Title=@Title, Contents=@Contents WHERE ID=@ID", parameters);
            return count > 0;
        }

        /// <summary>
        /// 获得一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>T_GuideTypes类的对象</returns>
        public DiaryModel GetById(System.Int32 ID)
        {
            SQLiteParameter[] parameters ={
                                              new SQLiteParameter("@ID")
                                          };
            parameters[0].Value = ID;

            DataTable dt = SQLiteHelper.ExecuteDataTable("SELECT ID, PubTime, IsUpload, Title, Contents FROM Diaries WHERE ID=@ID", parameters);
            if (dt.Rows.Count > 1)
            {
                throw new Exception("more than 1 row was found");
            }
            else if (dt.Rows.Count <= 0)
            {
                return null;
            }
            DataRow row = dt.Rows[0];
            DiaryModel model = ToModel(row);
            return model;
        }

        /// <summary>
        /// 获得所有记录
        /// </summary>
        /// <returns>T_GuideTypes类的对象的枚举</returns>
        public IEnumerable<DiaryModel> ListAll()
        {
            List<DiaryModel> list = new List<DiaryModel>();
            DataTable dt = SQLiteHelper.ExecuteDataTable("SELECT ID, PubTime, IsUpload, Title, Contents FROM Diaries");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(ToModel(row));
            }
            return list;
        }

    }
}
