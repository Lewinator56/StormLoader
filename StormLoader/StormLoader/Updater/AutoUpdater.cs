using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ionic.Zip;
using System.Net;
using System.Text.Json;
using System.Numerics;

namespace StormLoader.Updater
{
    class AutoUpdater
    {
        /** Connects to github
         * and downloads the latest github version. Renames the currently running executable, then copies the new
         * executable into place **/

        public static bool initiateUpdate()
        {
            bool updateSuccess = false;

            try
            {
                using (WebClient w = new WebClient())
                {
                    w.Headers.Add("user-agent", "request");
                    string json = w.DownloadString("https://api.github.com/repos/Lewinator56/StormLoader/releases/latest");
                    var jsd = JsonDocument.Parse(json);
                    Console.WriteLine(jsd.RootElement.GetProperty("assets")[0].GetProperty("browser_download_url"));


                }
                


            } catch (Exception e)
            {
                throw e;
            }

            return updateSuccess;
        }
    }
}
