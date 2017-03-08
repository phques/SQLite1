using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace SQLite1
{
    public class Language
    {
        int _id;
        string _langTitle;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string LangTitle
        {
            get { return _langTitle; }
            set { _langTitle = value; }
        }
    }

    class Program
    {
        static string connectionString = @"Data Source=langDB.sqlite; Version=3; FailIfMissing=True; Foreign Keys=True;";


        public static List<Language> GetLanguages(int langId)
        {
            List<Language> langs = new List<Language>();
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Language WHERE Id = " + langId;
                    if (langId == 0)
                    {
                        sql = "SELECT * FROM Language";
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
                    {
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Language la = new Language();
                                la.LangTitle = reader["LangTitle"].ToString();
                                la.Id = Int32.Parse(reader["Id"].ToString());
                                langs.Add(la);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (SQLiteException e)
            {

            }

            return langs;
        }


        public static int UpdateLang(int id, string newLangTitle)
        {
            int result = -1;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "UPDATE Language "
                        + "SET LangTitle = @Lang "
                        + "WHERE Id = @Id";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Lang", newLangTitle);
                    cmd.Parameters.AddWithValue("@Id", id);
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException)
                    {
                    }
                }
                conn.Close();
            }
            return result;
        }

        public static int AddLang(string langTitle)
        {
            int result = -1;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "INSERT INTO Language(LangTitle) VALUES (@Lang)";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@Lang", langTitle);
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException e)
                    {
                    }
                }
                conn.Close();
            }
            return result;
        }


        public static int DeleteLang(int id)
        {
            int result = -1;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "DELETE FROM Language WHERE Id = @I";
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@I", id);
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException e)
                    {
                    }
                }
                conn.Close();
            }
            return result;
        }

        private static int CreateLangTable()
        {
            SQLiteConnection.CreateFile("langDB.sqlite");
            int result = -1;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"CREATE TABLE Language ( Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,";
                    cmd.CommandText += @"LangTitle TEXT NOT NULL UNIQUE CHECK (LangTitle <> '') )";
                    try
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException)
                    {
                    }
                }
                conn.Close();
            }
            return result;
        }

        static void Main(string[] args)
        {
            CreateLangTable();

            AddLang("french");      // 1
            AddLang("english");     // 2
            AddLang("deutch");      // 3
            AddLang("ststh");       // 4

            UpdateLang(1, "francais");
            DeleteLang(4);

            var lls = GetLanguages(0);
        }


    }
}
