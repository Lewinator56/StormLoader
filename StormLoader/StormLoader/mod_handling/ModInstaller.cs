using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

using System.Windows;


namespace StormLoader.mod_handling
{
    class ModInstaller
    {
        // installs a mod
        // first checks if there is an installinfo xml document
        public void InstallMod(string modName, string modPath, string gamePath)
        {
            bool overwrite = false;
            CreateInstallXML(gamePath);
            // check the mod isnt already installed (check the instalinfo xml), if it is ask if they want to overwrite the old version
            if (CheckModInstalled(modName, gamePath))
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
            if (!CheckModInstalled(modName, gamePath) || overwrite)
            {
                //add the mod to the installinfo file
                
                if (overwrite)
                {
                    // uninstall the mod before re-installing it
                    DeleteByInstallInfo(modName, gamePath);
                }
                XmlDocument installInfo = new XmlDocument();
                installInfo.Load(gamePath + "/StormLoader_install_info.xml");
                XmlNode installRoot = installInfo.SelectSingleNode("/InstallInfo");
                XmlElement modParent;
                modParent = installInfo.CreateElement("Mod");
                modParent.SetAttribute("Name", modName);
                installRoot.AppendChild(modParent);
                
                

                

                try
                {
                    RecursiveCopyCheckInstalled(installInfo, installRoot, modParent, new DirectoryInfo(modPath + "/Meshes/"), new DirectoryInfo(gamePath + "/rom/meshes/"));
                    //DbgLog.WriteLine("Running");
                }
                catch (Exception e) { }
                try
                {
                    RecursiveCopyCheckInstalled(installInfo, installRoot, modParent, new DirectoryInfo(modPath + "/Definitions/"), new DirectoryInfo(gamePath + "/rom/data/definitions/"));
                }
                catch (Exception e) { }
                try
                {
                    RecursiveCopyCheckInstalled(installInfo,installRoot, modParent, new DirectoryInfo(modPath + "/Audio/"), new DirectoryInfo(gamePath + "/rom/audio/"));
                }
                catch (Exception e) { }

                installInfo.Save(gamePath + "/StormLoader_install_info.xml");


                //proceed to install
            }
        }
        //
        // recursive copy and delete operations
        //
        public void RecursiveCopyCheckInstalled(XmlDocument installInfo, XmlNode installRoot, XmlElement currentModNode, DirectoryInfo source, DirectoryInfo location)
        {

            

            foreach (FileInfo f in source.GetFiles())
            {
                bool installFile = true;
                bool fileExists = false;
                bool modFileExists = false;
                string overwriteSource = "";
                string relativeFileName = location.ToString().Replace(GlobalVar.mw.gameLocation, "") + f.Name;
                // check the file doesnt exist
                foreach (XmlElement modRoot in installRoot)
                {
                    foreach (XmlElement modFile in modRoot)
                    {
                        // loop for every file
                        // in the installinfo xml to check for overwrites
                        string filePath = modFile.Attributes["Path"].Value;
                        string overwritesData = "";
                        string overwrittenData = "";

                        // find the mod the current iteration overwrites
                        overwritesData = modFile.GetAttribute("Overwrites");

                        // find the mod overwriting the current iteration
                        overwrittenData = modFile.GetAttribute("Overwritten");

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
                        if (filePath == relativeFileName && overwrittenData == "" && modRoot.GetAttribute("Name") != currentModNode.GetAttribute("Name"))
                        {
                            modFileExists = true;
                            //ask overwrite
                            if (AskOverwriteFile(relativeFileName, modRoot.GetAttribute("Name")))
                            {
                                // overwrite the file
                                overwriteSource = modRoot.GetAttribute("Name") + "::" + filePath;
                                installFile = true;
                                


                                //modFile.RemoveAttribute("Overwrites");
                                modFile.SetAttribute("Overwritten", (currentModNode.GetAttribute("Name") + "::" + relativeFileName));
                                installInfo.Save(location + "/StormLoader_install_info.xml");
                                

                                //modRoot.RemoveChild(modFile);

                            } else
                            {
                                // skip this file
                                installFile = false;
                            }
                        } else
                        {

                            installFile = true;

                            
                            
                        }
                    }
                }
                // this only runs if the file exists, but isnt in the installinfo file, this means its probably installed by the game, or manually
                if (fileExists && !modFileExists)
                {
                    if (MessageBox.Show("The file:\n\n" + f.Name + "\n\nAlready exists in your Stormworks installlation, this means it is either a BASE GAME file, or was a mod installed manually. \n\n Uninstalling this mod will delete this file, you may experience issues if it is a base game part. \n\n Do you want to overwrite this file? (this cannot be undone with stormloader)" , "Overwrite File", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
                    if (overwriteSource != "")
                    {
                        InstallModFile(source + f.Name, location + f.Name, true, overwriteSource, installInfo, currentModNode);
                    } else
                    {
                        InstallModFile(source + f.Name, location + f.Name, false, "", installInfo, currentModNode);
                    }
                }
                

            }
            foreach (DirectoryInfo d in source.GetDirectories())
            {

                DirectoryInfo next = location.CreateSubdirectory(d.Name);
                DirectoryInfo dn = new DirectoryInfo(d.FullName + "/");
                next = new DirectoryInfo(next.FullName + "/");
                RecursiveCopyCheckInstalled(installInfo, installRoot, currentModNode, dn, next);
            }
        }

        private void InstallModFile(string source, string location, bool overwrite, string overwriteSource, XmlDocument installInfo, XmlElement currentModNode)
        {
            File.Copy(source, location, true);
            XmlElement modFileElement = installInfo.CreateElement("File");
            modFileElement.SetAttribute("Path", location.ToString().Replace(GlobalVar.mw.gameLocation, ""));
            if (overwrite)
            {
                modFileElement.SetAttribute("Overwrites", overwriteSource);

            }
            modFileElement.SetAttribute("ContentPath", source);
            currentModNode.AppendChild(modFileElement);
            installInfo.Save(GlobalVar.mw.gameLocation + "/StormLoader_install_info.xml");
        }


        public void DeleteByInstallInfo(string modName, string gameLocation)
        {
            XmlDocument installInfo = new XmlDocument();
            installInfo.Load(gameLocation + "/StormLoader_install_info.xml");
            XmlNode modRoot = installInfo.SelectSingleNode("InstallInfo");
            foreach (XmlElement mod in modRoot)
            {
                if (mod.Attributes["Name"].Value == modName)
                {
                    // we are now traversing the mod being uninstalled
                    foreach (XmlElement file in mod)
                    {
                        string overwrittenBy = "";
                        string overwrites = "";
                        

                        // check the current mod file in the iteratio is overwritten
                        overwrittenBy = file.GetAttribute("Overwritten");
                        // check if the current mod file in the iteration is overwriting another
                        overwrites = file.GetAttribute("Overwrites");

                        XmlElement nodeOverwriting = null;
                        XmlElement overwrittenNode = null;

                        if (overwrittenBy != "")
                        {
                            nodeOverwriting = (XmlElement)modRoot.SelectSingleNode("Mod[contains(@Name, '" + overwrittenBy.Substring(0, overwrittenBy.IndexOf(':')) + "')]/File[contains(@Overwrites, '" + mod.GetAttribute("Name") + "::" + file.GetAttribute(("Path")) + "')]");
                        }
                        if (overwrites != "")
                        {
                            overwrittenNode = (XmlElement)modRoot.SelectSingleNode("Mod[contains(@Name, '" + overwrites.Substring(0, overwrites.IndexOf(':')) + "')]/File[contains(@Overwritten, '" + mod.GetAttribute("Name") + "::" + mod.GetAttribute(("Path")) + "')]");
                        }
                        //System.Diagnostics.Debug.WriteLine(overwrittenNode);
                        //System.Diagnostics.Debug.WriteLine(nodeOverwriting);

                        if (nodeOverwriting != null && overwrittenNode == null)
                        {
                            // mod file is only overwritten, we just tell the overwriting mod its no longer overwriting this one
                            // no need to delete the file from the game
                            DbgLog.WriteLine("Not deleting " + file.GetAttribute("Path"));
                            nodeOverwriting.RemoveAttribute("Overwrites");
                            
                        } else if (overwrittenNode != null && nodeOverwriting == null)
                        {
                            // mod file is only overwriting, so we just tell the overwritten mod its no longer overwritten by this one
                            // delete the file from the game as because its ONLY overwriting, we know its currently installed
                            // then install the file it was overwriting
                            overwrittenNode.RemoveAttribute("Overwritten");

                            DbgLog.WriteLine("Deleting file " + gameLocation + file.Attributes["Path"].Value);
                            DbgLog.WriteLine("Installing file " + overwrittenNode.GetAttribute("ContentPath") + " to " + gameLocation + file.Attributes["Path"].Value);
                            try
                            {
                                File.Delete(gameLocation + file.Attributes["Path"].Value);
                                File.Copy(overwrittenNode.GetAttribute("ContentPath"), gameLocation + file.GetAttribute("Path"));
                                
                                //mod.RemoveChild(file);
                            }
                            catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                        } else if (nodeOverwriting != null && overwrittenNode != null)
                        {
                            // mod file is both overwritten AND overwriting, so we tell the overwritten mod that its now overtten by the overwriting mod, and the overwriting mod that 
                            // its not overwriting the overwritten mod

                            // we dont need to delete the file as its not installed by this mod
                            // we also dont need to re-install any files
                            overwrittenNode.SetAttribute("Overwritten", overwrittenBy);
                            nodeOverwriting.SetAttribute("Overwrites", overwrites);
                            DbgLog.WriteLine("Not deleting " + file.GetAttribute("Path"));

                        } else if (nodeOverwriting == null && overwrittenNode == null)
                        {
                            // if there are no overwrite issues, we delete the file
                            DbgLog.WriteLine("Deleting file " + gameLocation + file.Attributes["Path"].Value);
                            try
                            {
                                File.Delete(gameLocation + file.Attributes["Path"].Value);
                                
                                //mod.RemoveChild(file);
                            }
                            catch (Exception e) { DbgLog.WriteLine(e.ToString()); }
                        }
                        DbgLog.WriteLine("Deleting file " + gameLocation + file.Attributes["Path"].Value);
                        // for whatever reason, this breaks it
                        // no clue why, but dont uncomment this line
                        //mod.RemoveChild(file);

                    }
                    modRoot.RemoveChild(mod);
                }
            }
            installInfo.Save(gameLocation + "/StormLoader_install_info.xml");
            
        }

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


        //
        // Not used
        //
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

        //
        /// <summary>
        /// Creates the StormLoader_install_info xml file, this lets stormloader know what mods are and arent installed, even after
        /// removing stormloader
        /// </summary>
        /// <param name="gamePath">The path to the game installation</param>
        private void CreateInstallXML(string gamePath)
        {
            if (!File.Exists(gamePath + "/StormLoader_install_info.xml"))
            {
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.IndentChars = "\t";
                xws.OmitXmlDeclaration = false;
                xws.Encoding = Encoding.UTF8;

                XmlWriter xw = XmlWriter.Create(gamePath + "/StormLoader_install_info.xml", xws);
                xw.WriteStartDocument();
                xw.WriteStartElement("InstallInfo");
                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }
        }

        private bool CheckModInstalled(string modName, string gamePath)
        {
            try
            {
                XmlDocument xmd = new XmlDocument();
                xmd.Load(gamePath + "/StormLoader_install_info.xml");
                XmlNode modRoot = xmd.SelectSingleNode("/InstallInfo");
                foreach (XmlNode xn in modRoot)
                {
                    if (xn.Attributes["Name"].Value == modName)
                    {
                        return true;
                    }
                }
            }
            catch { System.Diagnostics.Debug.WriteLine("error"); }
            return false;
        }
    }
}
