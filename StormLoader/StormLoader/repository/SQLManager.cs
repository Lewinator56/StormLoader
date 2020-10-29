using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace StormLoader.repository
{
    class SQLManager
    {
        MySqlConnection conn;

        public void connect(string server, string database, string user, string password, string port)
        {
            string constr = "server=" + server + ";user=" + user + ";database=" + database + ";port=" + port + ";password=" + password;
            conn = new MySqlConnection(constr);

        }

        public bool checkUser(string username, string password)
        {
            string hash = SHAHasher.SHA256Hash(password);
            try
            {
                conn.Open();
                string sql = "SELECT * FROM users WHERE user_name='" + username + "' AND user_password='" + hash + "';";
                MySqlCommand msc = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = msc.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(rdr);
                conn.Close();
                if (dt.Rows.Count > 0)
                {
                    
                    return true;
                } else
                {
                    return false;
                }

            } catch (Exception e)
            {
                Console.WriteLine( e.StackTrace.ToString());
                return false;
            }
        }
        public bool addUser(string username, string password)
        {
            string hash = SHAHasher.SHA256Hash(password);
            try
            {
                conn.Open();
                string sql = "INSERT INTO users (user_name, user_password) VALUES ('" + username + "', '" + hash + "');";
                MySqlCommand msc = new MySqlCommand(sql, conn);
                int n = msc.ExecuteNonQuery();

                conn.Close();
                if (n > 0)
                {

                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace.ToString());
                return false;
            }
        }
    }

    
}
