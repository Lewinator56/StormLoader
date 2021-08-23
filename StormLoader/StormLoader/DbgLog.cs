using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace StormLoader
{
    class DbgLog
    {

        public static void WriteLine(string msg)
        {
            
            DateTime dt = DateTime.Now;
            var culture = new CultureInfo("en-GB");
            string output = dt.ToString(culture) + ": " + msg;
            Console.WriteLine(output);
            string p = "./log.txt";
            try
            {
                if (!File.Exists(p))
                {

                    using (StreamWriter sw = File.CreateText(p))
                    {
                        sw.WriteLine(output);

                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(p))
                    {
                        sw.WriteLine(output);

                    }
                }
            } catch
            {
                // log in the colsole we couldnt access the file
                Console.WriteLine("Couldn't access log file");
            }
                
        } 
            

            
     
    }
}
