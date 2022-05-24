using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using Ionic.Zip;

namespace StormLoader.mod_handling
{
    internal class Workshop
    {
        
        
        public static string getWorkshopPath()
        {
            string steamPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "installPath", null);
            if (steamPath == null)
            {
                return null;
            }

            string path = steamPath + "\\steamapps\\workshop\\content\\573090";

            if (!Directory.Exists(path))
            {
                // look at the other steam library locations
                foreach (string l in File.ReadLines(steamPath + "\\steamapps\\libraryfolders.vdf"))
                {
                    if (l.Contains("path"))
                    {
                        DbgLog.WriteLine(l);
                        string lpath = l.Split('"')[3];
                        if (Directory.Exists(lpath + "\\steamapps\\workshop\\content\\573090"))
                        {
                            path = lpath + "\\steamapps\\workshop\\content\\573090";
                            break;
                        }
                    }
                }
            }
            return path;
        }

        /* Checks each top level directory to see if it contains a playlist folder, if so proceeds to check if each vehicle XML is a valid zip folder (i.e an slp).
         * multithreaded to be faster
         */
        public static void copyMods(string path)
        {
            
            var directoryInfo = new DirectoryInfo(path);
            int directoryCount = directoryInfo.GetDirectories().Length;
            DbgLog.WriteLine(directoryCount.ToString());
            
            

            foreach(DirectoryInfo d in directoryInfo.GetDirectories())
            {
                DbgLog.WriteLine(d.FullName);
                foreach(DirectoryInfo id in d.GetDirectories())
                {
                    //DbgLog.WriteLine(id.Name);
                    if (String.Equals(id.Name, "playlist", StringComparison.OrdinalIgnoreCase))
                    {
                        // look inside here at each vehicle xml
                        foreach (FileInfo f in id.GetFiles())
                        {
                            if (f.Extension == ".xml")
                            {
                                // check the header to see if its a zip
                                try
                                {
                                    Byte[] head = new byte[4];
                                    using (FileStream fs = new FileStream(f.FullName, FileMode.Open, FileAccess.Read))
                                    {
                                        fs.Read(head, 0, 4);
                                    }
                                    //DbgLog.WriteLine(BitConverter.ToUInt32(head, 0).ToString("X2")) ;
                                    if (BitConverter.ToUInt32(head, 0) == 0x04034b50)
                                    {
                                        // we know this is a zip, so extract the contents (the .slp file) into the stormloader downloads directory
                                        ZipFile z = new ZipFile(f.FullName);
                                        z.ExtractAll("./temp_steam", ExtractExistingFileAction.OverwriteSilently);
                                        foreach(FileInfo slpf in new DirectoryInfo("./temp_steam").GetFiles())
                                        {
                                            DbgLog.WriteLine(slpf.Name);
                                            if (File.Exists("./Downloaded/" + slpf.Name))
                                            {
                                                DbgLog.WriteLine(slpf.Name);
                                                System.Diagnostics.Debug.WriteLine(slpf.Name);
                                                // check the hash
                                                if (slpf.Length == new FileInfo("./Downloaded/" + slpf.Name).Length)
                                                {
                                                    // dont overwrite, they are the same
                                                    DbgLog.WriteLine("File sizes the same, dont copy over");
                                                    System.Diagnostics.Debug.WriteLine("not copyiing");
                                                }
                                                else
                                                {
                                                    File.Copy(slpf.FullName, "./Downloaded/" + slpf.Name, true);
                                                    
                                                    GlobalVar.mw.addModFromFile(slpf.FullName, System.IO.Path.GetFileNameWithoutExtension(slpf.FullName), ".slp");

                                                    
                                                    //GlobalVar.mw.SetModActive(System.IO.Path.GetFileNameWithoutExtension(slpf.FullName), slpf.FullName, true);
                                                }
                                            } else
                                            {
                                                File.Copy(slpf.FullName, "./Downloaded/" + slpf.Name, true);
                                                GlobalVar.mw.addModFromFile(slpf.FullName, System.IO.Path.GetFileNameWithoutExtension(slpf.FullName), ".slp");
                                                //GlobalVar.mw.SetModActive(System.IO.Path.GetFileNameWithoutExtension(slpf.FullName), slpf.FullName, true);
                                            }
                                        }
                                        Directory.Delete("./temp_steam", true);
                                    }
                                } catch (Exception e)
                                {
                                    DbgLog.WriteLine(f.Name);
                                    DbgLog.WriteLine(e.ToString());
                                    //throw e;
                                }

                            }
                        }
                    }
                }
            }
        }

    }
}
