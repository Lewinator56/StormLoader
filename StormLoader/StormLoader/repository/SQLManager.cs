using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public DataTable getModListWithoutData()
        {
            try
            {
                conn.Open();
                string sql = "SELECT mod_id FROM mods;";
                MySqlCommand msc = new MySqlCommand(sql, conn);
                MySqlDataReader mdr = msc.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(mdr);
                conn.Close();
                return dt;
            } catch (Exception e)
            {
                return null;
            }
        }
        public DataTable getModDataByID(int id)
        {
            try
            {
                conn.Open();
                string sql = "SELECT m.mod_id, m.mod_name, m.mod_version, m.mod_description, m.mod_data_image, u.user_name FROM mods m, users u WHERE m.mod_author_id = u.user_id AND m.mod_id = " + id + ";";
                MySqlCommand msc = new MySqlCommand(sql, conn);
                MySqlDataReader mdr = msc.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(mdr);
                conn.Close();
                return dt;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> uploadMod(string username, string description, string version, string name, string imagepath, string modpath)
        {
            byte[] modfile = File.ReadAllBytes(modpath);
            byte[] modimage = File.ReadAllBytes(imagepath);
            try
            {
                conn.Open();
                string sql = "INSERT INTO mods (mod_name, mod_version, mod_description, mod_author_id, mod_local_data, mod_data_image) VALUES (@mod_name, @mod_version, @mod_description, (SELECT user_id FROM users WHERE user_name='" + username + "'), @mod_local_data, @mod_data_image);";
                MySqlCommand msc = new MySqlCommand(sql, conn);
                msc.Parameters.Add("@mod_name", MySqlDbType.VarChar, 255);
                msc.Parameters.Add("@mod_version", MySqlDbType.VarChar, 64);
                msc.Parameters.Add("@mod_description", MySqlDbType.Text);
                msc.Parameters.Add("@mod_local_data", MySqlDbType.LongBlob);
                msc.Parameters.Add("@mod_data_image", MySqlDbType.MediumBlob);

                msc.Parameters["@mod_name"].Value = name;
                msc.Parameters["@mod_version"].Value = version;
                msc.Parameters["@mod_description"].Value = description;
                msc.Parameters["@mod_local_data"].Value = modfile;
                msc.Parameters["@mod_data_image"].Value = modimage;

                int n = await msc.ExecuteNonQueryAsync();
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
