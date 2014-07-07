using System.Data.SQLite;
using System.Data;
using MyLife.Properties;
using System;

namespace MyLife.Helper
{
    public static class SQLiteHelper
    {
        private static SQLiteConnectionStringBuilder conStr = new SQLiteConnectionStringBuilder();

        public static SQLiteConnectionStringBuilder ConStr
        {
            get { return SQLiteHelper.conStr; }
            set { SQLiteHelper.conStr = value; }
        }

        public static void CreateFile(string path)
        {
            SQLiteConnection.CreateFile(path);
        }

        public static void InitDB()
        {
            ExecuteNonQuery(new Settings().InitSql);
        }

        public static bool ChangePassword(string newPassword, string oldPassword = "")
        {
            if (oldPassword == conStr.Password)
            {
                using (SQLiteConnection conn = new SQLiteConnection(conStr.ConnectionString))
                {
                    conn.Open();
                    conn.ChangePassword(newPassword);
                    conn.Close();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, string cmdText, params object[] p)
        {

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Parameters.Clear();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            cmd.Parameters.AddRange(p);
        }

        public static DataTable ExecuteDataTable(string cmdText, params object[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection(conStr.ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    DataTable dt = new DataTable();
                    PrepareCommand(command, conn, cmdText, p);
                    SQLiteDataAdapter da = new SQLiteDataAdapter(command);
                    da.Fill(dt);
                    ExecuteZip();
                    return dt;
                }
            }
        }

        public static void ExecuteZip()
        {
            using (SQLiteConnection connection = new SQLiteConnection(conStr.ConnectionString))
            {
                using (SQLiteCommand cmd = new SQLiteCommand("VACUUM", connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (System.Data.SQLite.SQLiteException E)
                    {
                        connection.Close();
                        throw new Exception(E.Message);
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string cmdText, params object[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection(conStr.ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    PrepareCommand(command, conn, cmdText, p);
                    int i = command.ExecuteNonQuery();
                    ExecuteZip();
                    return i;
                }
            }
        }

        public static object ExecuteScalar(string cmdText, params object[] p)
        {
            using (SQLiteConnection conn = new SQLiteConnection(conStr.ConnectionString))
            {
                using (SQLiteCommand command = new SQLiteCommand())
                {
                    PrepareCommand(command, conn, cmdText, p);
                    if (command.ExecuteNonQuery() > 0)
                    {
                        command.CommandText = "select last_insert_rowid()";
                        return command.ExecuteScalar();
                    }
                    return null;
                }
            }
        }
    
        public static object ToDBValue(object value)
        {
            return value == null ? DBNull.Value : value;
        }

        public static object FromDBValue(object dbValue)
        {
            return dbValue == DBNull.Value ? null : dbValue;
        }
    }
}
