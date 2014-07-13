using MyLife.Helper;
using MyLife.Model;
using System;
using System.Data;
using System.Data.SQLite;

namespace MyLife.DAL
{
    class SettingDAL
    {
        public SettingModel ToModel(DataRow row)
        {
            SettingModel model = new SettingModel();
            model.Key = Convert.ToString(row["Key"]);
            model.Value = Convert.ToString(row["Value"]);
            return model;
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="model">T_GuideTypes类的对象</param>
        /// <returns>更新是否成功</returns>
        public bool Update(SettingModel model)
        {
            SQLiteParameter[] parameters ={
                                              new SQLiteParameter("@Key")
                                              , new SQLiteParameter("@Value")
                                          };
            parameters[0].Value = model.Key;
            parameters[1].Value = model.Value;

            int count = SQLiteHelper.ExecuteNonQuery("UPDATE Settings SET Value=@Value WHERE Key=@Key", parameters);
            return count > 0;
        }

        /// <summary>
        /// 获得一条记录
        /// </summary>
        /// <param name="Id">主键</param>
        /// <returns>T_GuideTypes类的对象</returns>
        public SettingModel GetByKey(string Key)
        {
            SQLiteParameter[] parameters ={
                                              new SQLiteParameter("@Key")
                                          };
            parameters[0].Value = Key;

            DataTable dt = SQLiteHelper.ExecuteDataTable("SELECT Key, Value FROM Settings WHERE Key=@Key", parameters);
            if (dt.Rows.Count > 1)
            {
                throw new Exception("more than 1 row was found");
            }
            else if (dt.Rows.Count <= 0)
            {
                return null;
            }
            DataRow row = dt.Rows[0];
            SettingModel model = ToModel(row);
            return model;
        }

    }

}
