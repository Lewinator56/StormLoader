using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using System.Windows;
using StormLoader.mod_handling.install;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace StormLoader.mod_handling
{
    class ModInstaller
    {
        public InstallList il;

        public ModInstaller()
        {
            il = new InstallList();
        }
        public void DeserializeOrCreateMods(string source)
        {
            if (!File.Exists(source + "/StormLoader_install_info.bin"))
            {
                File.Create(source + "/StormLoader_install_info.bin");
            } else
            {
                try
                {
                    // assume the format is correct
                    BinaryFormatter f = new BinaryFormatter();
                    using (FileStream sr = new FileStream(source + "/StormLoader_install_info.bin", FileMode.Open))
                    {

                        il = (InstallList)f.Deserialize(sr);
                    }
                    
                }
                catch (Exception e)
                {
                    // empty file
                }
                
            }

           
        }

        public void SerializeToInstallFile(string source)
        {
            Stream s = File.OpenWrite(source + "/StormLoader_install_info.bin");
            BinaryFormatter f = new BinaryFormatter();
            f.Serialize(s, il);
            s.Close();
        }


        public void InstallModPack(string modName, string modPath, string gamePath)
        {
            ModPack mp = new ModPack(modName, modPath);
            bool overwrite = false;

            if (il.IsModInstalled(mp))
            {
                //ask overwrite
                if (MessageBox.Show("The mod:\n\n" + modName + "\n\nis already installed, do you want to overwrite it? \n\n (Overwriting a mod will place THIS mod at the top of the overwrites list unless you choose not to overwrite individual files)", "Overwrite File", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    overwrite = false;
                }
                else
                {
                    overwrite = true;
                }
            }

            if (!il.IsModInstalled(mp) || overwrite)
            {
                if (overwrite)
                {
                    DeleteByInstallList(mp.name, gamePath);
                }

                // add the modpack to the install list, right now its blank
                il.mods.Add(mp);

                try
                {
                    RecursiveCopyCheckInstalledList(mp, new DirectoryInfo(modPath + "/Meshes/"), new DirectoryInfo(gamePath + "/rom/meshes/"));
                    //DbgLog.WriteLine("Running");
                }
                catch (Exception e)
                {
                    DbgLog.WriteLine(e.ToString());

                }
                
                try
                {
                    RecursiveCopyCheckInstalledList(mp, new DirectoryInfo(modPath + "/Definitions/"), new DirectoryInfo(gamePath + "/rom/data/definitions/"));
                }
                catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                try
                {
                    RecursiveCopyCheckInstalledList(mp, new DirectoryInfo(modPath + "/Audio/"), new DirectoryInfo(gamePath + "/rom/audio/"));
                }
                catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                try
                {
                    RecursiveCopyCheckInstalledList(mp, new DirectoryInfo(modPath + "/Graphics/"), new DirectoryInfo(gamePath + "/rom/graphics/"));
                }
                catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                try
                {
                    RecursiveCopyCheckInstalledList(mp, new DirectoryInfo(modPath + "/Data/"), new DirectoryInfo(gamePath + "/rom/data/"));
                }
                catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                
                SerializeToInstallFile(gamePath);
            }
        }

        public void DeleteByInstallList(string modName, string gameLocation)
        {
            int i = 0;
            while (il.mods.Count > i)
            {
                ModPack mp = il.mods[i];
                if (mp.name == modName)
                {
                    
                    foreach (ModFile mf in mp.modFiles)
                    {
                        // mod is overwritten and does not overwrite
                        if (mf.IsOverwritten() && !mf.Overwrites())
                        {
                            // dont delete the mod (because the fiel doesnt exist), and wipe the reference from the list
                            mf.GetOverwrittenBy().SetOverwrites(null);
                        }  
                        // mod overwrites and is not overwritten
                        else if (!mf.IsOverwritten() && mf.Overwrites())
                        {
                            // delete the file, reinstall the file it was overwriting, and remove the reference
                            try
                            {
                                File.Delete(mf.installPath);
                                File.Copy(mf.GetOverwrites().contentPath, gameLocation + mf.installPath);
                                mf.GetOverwrites().SetOverwrittenBy(null);
                            }
                            catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                        }
                        // mod is overwritten and overwrites another
                        else if (mf.IsOverwritten() && mf.Overwrites())
                        {
                            // nothing to delete, but we do need to update references
                            mf.GetOverwrittenBy().SetOverwrites(mf.GetOverwrites());
                            mf.GetOverwrites().SetOverwrittenBy(mf.GetOverwrittenBy());
                        }
                        // mod isnt iverwriting anything and isnt overwritten by anything
                        else if (!mf.IsOverwritten() && !mf.Overwrites())
                        {
                            try
                            {
                                File.Delete(mf.installPath);
                            }
                            catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                        }

                    }
                    il.mods.Remove(mp);
                    
                }
                i++;
            }
            SerializeToInstallFile(gameLocation);


            
        }


        public void RecursiveCopyCheckInstalledList(ModPack pack, DirectoryInfo source, DirectoryInfo location)
        {



            foreach (FileInfo f in source.GetFiles())
            {
                ModFile m = new ModFile(location + f.Name);
                m.contentPath = source + f.Name;
                pack.modFiles.Add(m);



                bool installFile = true;
                bool fileExists = false;
                bool modFileExists = false;
                ModFile overwriteSource = null;
                string relativeFileName = location.ToString().Replace(GlobalVar.mw.gameLocation, "") + f.Name;
                // check the file doesnt exist
                foreach (ModPack modRoot in il.mods)
                {
                    foreach (ModFile modFile in modRoot.modFiles)
                    {
                        // loop for every file
                        // in the installinfo xml to check for overwrites
                        string filePath = modFile.installPath.Replace(GlobalVar.mw.gameLocation, "");
                        ModFile overwritesData = null;
                        ModFile overwrittenData = null;
                        bool active;

                        // find the mod the current iteration overwrites
                        overwritesData = modFile.GetOverwrites();

                        // find the mod overwriting the current iteration
                        overwrittenData = modFile.GetOverwrittenBy();

                        active = modFile.active;


                        //
                        // The final mod in an insttall with many overwrites will only have an 'Overwrites' attribute, NOT an overwritten attribute,
                        // this can be checked by checking if overwrittenData, which specifys the path for the mod overwriting the file, doesnt exist
                        // if it doesnt, then its the top mod, and the one that needs overwriting
                        //
                        // if the path for the current FILE is equal to the path for the current iteration in installinfo AND if the mod isnt overwritten
                        // ask the user if they want to overwrite the current mod
                        // if yes, add the 'overwritten' attribute to the current iteration mod, and then write the new mod with the overwrites attribute

                        // If the user doesnt want to overwrite the existing file, we can just ignore this and not install the file,
                        // if there are no files to overwrite, proceed with a normal install
                        if (filePath == relativeFileName && overwrittenData == null && modRoot.name != pack.name)
                        {
                            modFileExists = true;
                            //ask overwrite
                            if (AskOverwriteFile(relativeFileName, modRoot.name))
                            {
                                // overwrite the file
                                overwriteSource = modFile;
                                installFile = true;



                                //modFile.RemoveAttribute("Overwrites");
                                //modFile.SetAttribute("Overwritten", (currentModNode.GetAttribute("Name") + "::" + relativeFileName));
                                //installInfo.Save(location + "/StormLoader_install_info.xml");

                                modFile.SetOverwrittenBy(m);


                                //modRoot.RemoveChild(modFile);

                            }
                            else
                            {
                                // skip this file
                                installFile = false;
                            }
                        }
                        else
                        {

                            installFile = true;



                        }
                    }
                }
                // this only runs if the file exists, but isnt in the installinfo file, this means its probably installed by the game, or manually
                if (fileExists && !modFileExists)
                {
                    if (MessageBox.Show("The file:\n\n" + f.Name + "\n\nAlready exists in your Stormworks installlation, this means it is either a BASE GAME file, or was a mod installed manually. \n\n Uninstalling this mod will delete this file, you may experience issues if it is a base game part. \n\n Do you want to overwrite this file? (this cannot be undone with stormloader)", "Overwrite File", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        installFile = false;
                    }
                    else
                    {
                        installFile = true;
                    }
                }

                if (installFile)
                {
                    if (overwriteSource != null)
                    {
                        InstallModFile(m, true, overwriteSource);
                    }
                    else
                    {
                        InstallModFile(m, false, null);
                    }
                }


            }
            foreach (DirectoryInfo d in source.GetDirectories())
            {

                DirectoryInfo next = location.CreateSubdirectory(d.Name);
                DirectoryInfo dn = new DirectoryInfo(d.FullName + "/");
                next = new DirectoryInfo(next.FullName + "/");
                RecursiveCopyCheckInstalledList(pack, dn, next);
            }
        }

        public void InstallModFile(ModFile mf, bool overwrite, ModFile overwriteSource)
        {
            File.Copy(mf.contentPath, mf.installPath, true);
            if (overwrite)
            {
                mf.SetOverwrites(overwriteSource);
                //overwriteSource.SetOverwrittenBy(mf);
            }
        }










        /**
         * obsolete (ish) - rewrite
         * 
         */

        

        private bool AskOverwriteFile(string file, string installedBy)
        {
            bool overwrite;
            
            if (MessageBox.Show( "The file:\n\n" + file + "\n\nis installed by:\n\n" + installedBy + "\n\nDo you want to overwrite this file?", "Overwrite File", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                overwrite = false;
            }
            else
            {
                overwrite = true;
            }
            
            return overwrite;
        }


        
        public void RecursiveDeleteCheckInstalled(DirectoryInfo extractedPath, DirectoryInfo location)
        {
            foreach (FileInfo f in extractedPath.GetFiles())
            {
                File.Delete(location + f.Name);
            }
            foreach (DirectoryInfo d in extractedPath.GetDirectories())
            {

                DirectoryInfo next = new DirectoryInfo(location.FullName + d.Name);
                DirectoryInfo dn = new DirectoryInfo(d.FullName + "/");
                next = new DirectoryInfo(next.FullName + "/");
                RecursiveDeleteCheckInstalled(dn, next);
            }
        }
        //
        //
        //

     
    }
}
