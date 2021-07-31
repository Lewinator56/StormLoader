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
                if (MessageBox.Show("The mod " + modName + " is already installed, do you want to overwrite it?", "Overwrite File", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
                XmlDocument installInfo = new XmlDocument();
                installInfo.Load(gamePath + "/StormLoader_install_info.xml");
                XmlNode installRoot = installInfo.SelectSingleNode("/InstallInfo");
                XmlElement modParent;
                if (overwrite)
                {
                    foreach (XmlNode xn in installRoot)
                    {
                        if (xn.Attributes["Name"].Value == modName)
                        {
                            installRoot.RemoveChild(xn);
                        }
                    }
                } 
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
                // check the file doesnt exist
                foreach (XmlElement modRoot in installRoot)
                {
                    foreach (XmlElement modFile in modRoot)
                    {
                        string filePath = modFile.Attributes["Path"].Value;
                        if (filePath == location.ToString().Replace(GlobalVar.mw.gameLocation, "") + f.Name)
                        {
                            //ask overwrite
                            if (AskOverwriteFile(location.ToString().Replace(GlobalVar.mw.gameLocation, "") + f.Name, modRoot.Attributes["Name"].Value))
                            {
                                // overwrite the file
                                installFile = true;
                                modRoot.RemoveChild(modFile);
                                
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
                if (installFile)
                {
                    File.Copy(source + f.Name, location + f.Name, true);
                    XmlElement modFileElement = installInfo.CreateElement("File");
                    modFileElement.SetAttribute("Path", location.ToString().Replace(GlobalVar.mw.gameLocation, "") + f.Name);
                    currentModNode.AppendChild(modFileElement);
                    installInfo.Save(location + "StormLoader_install_info.xml");
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

        public void DeleteByInstallInfo(string modName, string gameLocation)
        {
            XmlDocument installInfo = new XmlDocument();
            installInfo.Load(gameLocation + "/StormLoader_install_info.xml");
            XmlNode modRoot = installInfo.SelectSingleNode("InstallInfo");
            foreach (XmlElement mod in modRoot)
            {
                if (mod.Attributes["Name"].Value == modName)
                {
                    foreach (XmlElement file in mod)
                    {
                        try
                        {
                            File.Delete(gameLocation + file.Attributes["Path"].Value);
                            mod.RemoveChild(file);
                        } catch { }
                    }
                    modRoot.RemoveChild(mod);
                }
            }
            installInfo.Save(gameLocation + "/StormLoader_install_info.xml");
            
        }

        private bool AskOverwriteFile(string file, string installedBy)
        {
            bool overwrite;
            
            if (MessageBox.Show( "The file " + file + ", is installed by " + installedBy + ". Do you want to overwrite this file?", "Overwrite File", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
