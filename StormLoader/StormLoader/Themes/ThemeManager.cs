using MySql.Data;
using StormLoader.mod_handling.install;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StormLoader.Themes
{
    public class ThemeManager
    {
        public Theme currentTheme;
        
        public ThemeManager ()
        {
            // try to load the urrent theme, otherwise revert to the built in theme
            Directory.CreateDirectory("./themes");
            if (!Load("./themes/current.thm"))
            {
                currentTheme = new Theme ();
            }
            
        }
        public bool Load(string path)
        {
            try
            {
                // assume the format is correct
                BinaryFormatter f = new BinaryFormatter();
                using (FileStream sr = new FileStream(path, FileMode.Open))
                {

                     currentTheme = (Theme)f.Deserialize(sr);
                }
                foreach (var key in currentTheme.colors)
                {
                    Application.Current.Resources[key.Key] = key.Value.ToBrush();
                }
                
                return true;

            }
            catch (Exception e)
            {
                // empty file
            }
            return false;
        }

        public void Save(string path)
        {
            try
            {
                Stream s = File.OpenWrite(path);
                BinaryFormatter f = new BinaryFormatter();
                f.Serialize(s, currentTheme);
                s.Close();
            } catch (Exception e)
            {
                DbgLog.WriteLine("Failed to save theme");
                throw e;
            }
            
        }

        public void UpdateTheme()
        {
            
        }

    }
}
