using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Ionic.Zip;
using System.Security.RightsManagement;
using System.Threading;

using System.Net;
using StormLoader.repository;
using StormLoader.Profiles;
using StormLoader.Themes;
using Microsoft.Win32;
using StormLoader.mod_handling.install;

namespace StormLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// 
    /// I'm really sorry, this code is an abomination of legacy c**p and years of me becoming a better programmer.
    /// A lot is written badly, and following function calls is spaghetti. I've made some changes, but these are for the most part relying on 
    /// the legacy stuff that will be a pain in the backside to refactor. 
    /// 
    /// All future updates and changes should  aim to reduce the relience on the legacy code and will be added in self contained modules where
    /// possible, take for example the mod packager.
    /// 
    /// Oh, and an automatic version counter would be nice, I just don't know how to do that (probably a build configuration somewhere).
    /// 
    /// Anyway...
    /// 
    /// This is the main window for stormloader, this file handles most of the interaction logic for this, but also a lot of the backend logic (which it shouldnt)
    /// Mod installs, for the most part, are handled by the modInstaller in mod_handling, but some stuff is still in here, waiting to break and scare me when I
    /// least expect it.
    /// 
    /// Theres some multithreaded shenanigans going on too for mod installation to isolate the UI and mod threads, but the queue situation is a mess, and updates
    /// to the UI to show whats going on sometimes happen in the wrong order, breaking things. Oh, and you cant uninstall and install at the same time - i.e you cant queue up
    /// uninstalls... don't know why, thats just how i wrote it and probably need to change it.
    ///
    /// </summary>
    public partial class MainWindow : Window
    {
        public Queue<Mod> installQueue = new Queue<Mod>();
        public XmlDocument settingsDoc = new XmlDocument();
        public String modExtractionDir = "";
        public XmlDocument currentProfile = new XmlDocument();
        public string gameLocation = "";
        public List<ModListItem> modListItems = new List<ModListItem>();
        public string version = "v1.1.1";
        public bool x64 { get; set; }
        public bool notx64 { get { return !x64; } set { x64=!value; } }
        mod_handling.ModInstaller mi = new mod_handling.ModInstaller();

        Profile liveProfile = new Profile();

        ThemeManager themeManager = new ThemeManager();
        public MainWindow()
        {
            AppDomain cd = AppDomain.CurrentDomain;
            cd.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            


            InitializeComponent();
            GlobalVar.mw = this;

            createSetupFile();
            settingsDoc.Load("Settings.xml");
            modExtractionDir = settingsDoc.SelectSingleNode("/Settings/Mod_Location").InnerText;
            CreateDir(modExtractionDir);
            CreateDir("Downloaded");
            gameLocation = settingsDoc.SelectSingleNode("/Settings/Game_Location").InnerText;
            settingsDoc.SelectSingleNode("/Settings/Version").InnerText = version;
            x64 = settingsDoc.SelectSingleNode("/Settings/x64").InnerText == "true";
            settingsDoc.Save("Settings.xml");


            liveProfile = Profile.Load("CurrentProfile.spf");
            this.Title = "StormLoader : " + liveProfile.Name;

            //currentProfile.Load("CurrentProfile.xml");
            //this.Title = "StormLoader : " + currentProfile.SelectSingleNode("/Profile").Attributes["Name"].InnerText;
            mi.DeserializeOrCreateMods(gameLocation);

            x64Box.DataContext = this;
            x86Box.DataContext = this;
            displayModList();

            // install mods on a separate thread
            Thread t = new Thread(ModInstallListenerThread);
            t.SetApartmentState(ApartmentState.STA);
            t.IsBackground = true;
            t.Start();

            //ApplyProfileAlt();

            //check the stuff from steam
            
            







        }

        static void UnhandledExceptionHandler(Object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            DbgLog.WriteLine(e.Message);
            DbgLog.WriteLine(e.StackTrace.ToString());
            MessageBox.Show(e.Message + "\r\n\r\n" + "Kindly report this to the developer, your log is located in the install directory.\r\nThe application will now quit", "An error occurred");
            Application.Current.Shutdown();
            
        }
        public void AddModToInstallQueue(Profiles.ModPack pack, bool active)
        {
            
            this.Dispatcher.Invoke(() =>
            {
                Label l = new Label() { Content = (active? "INSTALLING: " : "UNINSTALLING :") + pack.Name };
                ModInstallList.Children.Add(l);
                
            });
            
            installQueue.Enqueue(new Mod(pack));
        }
        private string checkNewVersion(bool ShowDialog)
        {
            
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("https://github.com/Lewinator56/StormLoader/releases/latest");
            wr.AllowAutoRedirect = true;
            HttpWebResponse wrs = (HttpWebResponse)wr.GetResponse();
            string onlineVer = wrs.ResponseUri.ToString().Substring(wrs.ResponseUri.ToString().LastIndexOf('/') + 1);
            DbgLog.WriteLine(onlineVer);
            if (onlineVer != version)
            {
                DbgLog.WriteLine("New version released");
                if (ShowDialog)
                {
                    List<Control> c = new List<Control>();
                    Label l = new Label();
                    l.Content = "Version: " + version + " -> Version: " + onlineVer;
                    DockPanel.SetDock(l, Dock.Top);
                    c.Add(l);
                    Label ls = new Label();
                    ls.Content = "Head over to github to get it";
                    DockPanel.SetDock(ls, Dock.Top);
                    c.Add(ls);
                    ShowInfoPopup("An update is available", c, PackIconKind.Update);
                }
                
            }
            return onlineVer;
        }

        public void ModInstallListenerThread()
        {
            while (true)
            {
                // check if mod installs are queued;
                
                if (installQueue.Count == 0)
                {
                    Thread.Sleep(50);
                }
                else
                {
                    Mod m = installQueue.Dequeue();
                    DbgLog.WriteLine(m.pack.ContentPath);
                    SetModActive(m.pack, m.pack.Active);
                    this.Dispatcher.Invoke(() =>
                    {
                        ModInstallList.Children.RemoveAt(0);
                    });
                    
                }
            } 
            
        }

        

        public void CreateDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void displayModList()
        {
            ModList.Children.Clear();
            string[] filesInDirectory = Directory.GetDirectories(modExtractionDir);
            int i = 0;
            while (liveProfile.ModPacks.Count > i)
            {
                
                ModListItem mli = new ModListItem(liveProfile.ModPacks[liveProfile.ModPacks.Count - 1 -i]);
                ModList.Children.Add(mli);
                i++;
            }
            CheckModActive();
        }
        public void CheckModActive()
        {
            //currentProfile.Load("CurrentProfile.xml");
            //XmlNode modRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            foreach (Profiles.ModPack mod in liveProfile.ModPacks)
            {
                if (mod.Active == true)
                {
                    foreach (ModListItem mli in ModList.Children)
                    {
                        if (mod.Name == (string)mli.ModName.Content)
                        {
                            mli.SetActive(true);

                        }
                    }
                }
                else
                {
                    foreach (ModListItem mli in ModList.Children)
                    {
                        if (mod.Name == (string)mli.ModName.Content)
                        {
                            mli.SetActive(false);

                        }
                    }
                }
            }
        }


        private void AddMod_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "StormLoader mod package|*.slp|Zip File|*.zip";
            Nullable<bool> r = opf.ShowDialog();

            if (r == true)
            {
                Profiles.ModPack p = new Profiles.ModPack();
                p.Active = true;
                p.Name = System.IO.Path.GetFileNameWithoutExtension(opf.FileName);
                addModFromFile(opf.FileName, System.IO.Path.GetFileNameWithoutExtension(opf.FileName), System.IO.Path.GetExtension(opf.FileName), p);
                
                
            }
            

        }

        public void addModFromFile(string path, string nameWithoutExt, string ext, Profiles.ModPack pack)
        {
            this.Dispatcher.Invoke(() =>
            {
                ModInstallList.Children.Add(new Label() { Content = "COPYING: " + nameWithoutExt });
            });
            
            if (ext == ".slp")
            {
                AddModFromSLP(path, nameWithoutExt);
            } else if (ext == ".zip")
            {
                AddModFromZip(path, nameWithoutExt);
            }
            XmlDocument meta = new XmlDocument();
            meta.Load(modExtractionDir + "/" + nameWithoutExt + "/metadata.xml");

            

            pack.Name = nameWithoutExt;
            pack.ContentPath = modExtractionDir + "/" + nameWithoutExt;
            pack.Author = meta.SelectSingleNode("/Metadata/Author").InnerText;
            pack.Version = meta.SelectSingleNode("/Metadata/Version").InnerText;
            pack.InstalledOn = DateTime.Now;
            DbgLog.WriteLine(meta.OuterXml);
            this.Dispatcher.Invoke(() =>
            {
                ModInstallList.Children.RemoveAt(0);
            });
            AddModToInstallQueue(pack, true);
            //SetModActive(modExtractionDir + "/" + nameWithoutExt, nameWithoutExt, true);

            //AddModNew(modExtractionDir + "/" + nameWithoutExt, nameWithoutExt, meta.SelectSingleNode("/Metadata/Version").InnerText, meta.SelectSingleNode("/Metadata/Author").InnerText);
            this.Dispatcher.Invoke(() =>
            {
                displayModList();
                //ApplyProfileAlt();
            });
            
        }

        public void AddModFromSLP(string path, string nameWithoutExt)
        {
            ZipFile z = new ZipFile(path);
            // extract to a temporary directory
            Directory.CreateDirectory("temp");
            z.ExtractAll("temp", ExtractExistingFileAction.OverwriteSilently);
            Directory.CreateDirectory(modExtractionDir + "/" + nameWithoutExt);
            string workingDirectory = "./temp";
            while (!File.Exists(workingDirectory + "/" + "metadata.xml"))
            {
                workingDirectory = Directory.GetDirectories(workingDirectory)[0];
                //DbgLog.WriteLine(workingDirectory);
            }
            RecursiveCopy(new DirectoryInfo(workingDirectory + "/" ), new DirectoryInfo(modExtractionDir + "/" + nameWithoutExt + "/"));
            Directory.Delete("temp", true);
            //z.ExtractAll(modExtractionDir + "/" + nameWithoutExt, ExtractExistingFileAction.OverwriteSilently);



        }
        private void AddModFromZip(string path, string nameWithoutExt)
        {
            Directory.CreateDirectory("temp");
            Directory.CreateDirectory(modExtractionDir + "/" + nameWithoutExt);
            ZipFile z = new ZipFile(path);
            z.ExtractAll("temp", ExtractExistingFileAction.OverwriteSilently);


            CheckFilesInDirectory("temp", modExtractionDir + "/" + nameWithoutExt, "", false);
            Directory.Delete("temp", true);
            // now check if metadata exists
            
            if (!File.Exists(modExtractionDir + "/" + nameWithoutExt + "/" + "Metadata.xml"))
            {
                
                // ok its not here, time to generate one
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.IndentChars = "\t";
                xws.OmitXmlDeclaration = false;
                xws.Encoding = Encoding.UTF8;

                XmlWriter xw = XmlWriter.Create(modExtractionDir + "/" + nameWithoutExt + "/Metadata.xml", xws);
                xw.WriteStartDocument();
                xw.WriteStartElement("Metadata");
                xw.WriteElementString("Author", "Unknown");
                xw.WriteElementString("Version", "Unknown");
                xw.WriteEndElement();
                xw.WriteEndDocument();

                xw.Close();

            }
            
        }

        private void CheckFilesInDirectory(string path, string modExtractionDir, string currentIterationPath, bool isInSubfolder)
        {
            
            // first lets check if there are any files to copy
            if (Directory.GetFiles(path).Length > 0)
            {
                // ok now lets check what the files are, .mesh goes into modExtractionDir/Meshes and .ogg goes into /Audio and .xml goes into /Definitions
                foreach(string f in Directory.GetFiles(path))
                {
                    string filenameWithoutPath = f.Substring(f.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                    // first, lets check for a metadata file
                    if (filenameWithoutPath == "Metadata.xml" || filenameWithoutPath == "info.html")
                    {
                        File.Copy(f, modExtractionDir + filenameWithoutPath, true);
                    }
                    // ok, now to actually copy the files
                    if (f.Substring(f.LastIndexOf('.') + 1) == "mesh")
                    {
                        Directory.CreateDirectory(modExtractionDir + "/Meshes/" + currentIterationPath);
                        // copy the file to the meshes folder
                        File.Copy(f, modExtractionDir + "/Meshes/" + currentIterationPath + "/" + filenameWithoutPath, true);
                    }
                    else if (f.Substring(f.LastIndexOf('.') + 1) == "ogg")
                    {
                        Directory.CreateDirectory(modExtractionDir + "/Audio/" + currentIterationPath);
                        // copy the file to the meshes folder
                        File.Copy(f, modExtractionDir + "/Audio/"+ currentIterationPath + "/" + filenameWithoutPath, true);
                    }
                    else if (f.Substring(f.LastIndexOf('.') + 1) == "xml")
                    {
                        Directory.CreateDirectory(modExtractionDir + "/Definitions");
                        // copy the file to the meshes folder
                        File.Copy(f, modExtractionDir + "/Definitions/" + filenameWithoutPath, true);
                    }
                }
            }
            // ok weve done that, now to check for subdirectories
            if (Directory.GetDirectories(path).Length > 0)
            {
                // call itself, but change a few things
                foreach(String dir in Directory.GetDirectories(path))
                {
                    string directoryWithoutPath = dir.Substring(dir.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1);
                    if (directoryWithoutPath.Equals("meshes", StringComparison.OrdinalIgnoreCase) || directoryWithoutPath.Equals("audio", StringComparison.OrdinalIgnoreCase))
                    {
                        // update the current iteration path to take this into account
                        //Directory.CreateDirectory(modExtractionDir + "/" + currentIterationPath + "/" + directoryWithoutPath);
                        CheckFilesInDirectory(path + "/" + directoryWithoutPath, modExtractionDir, currentIterationPath, true);
                    }
                    // got to do another check now, the previous one only works for the first iteration
                    if (isInSubfolder == true)
                    {
                        //Directory.CreateDirectory(modExtractionDir + "/" + currentIterationPath + "/" + directoryWithoutPath);
                        CheckFilesInDirectory(path + "/" + directoryWithoutPath, modExtractionDir, currentIterationPath + "/" + directoryWithoutPath, true);
                    }
                    CheckFilesInDirectory(path + "/" + directoryWithoutPath, modExtractionDir, currentIterationPath, false);
                }
            }
            // otherwise, we are done
            return;
        }

        private async void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsPopup sp = new SettingsPopup();
            await MaterialDesignThemes.Wpf.DialogHost.Show(sp);

            settingsDoc.Load("Settings.xml");
            settingsDoc.SelectSingleNode("/Settings/Game_Location").InnerText = sp.InsLoc.GetLocation();
            settingsDoc.SelectSingleNode("/Settings/Mod_Location").InnerText = sp.ModLoc.GetLocation() == "" ? "./Extracted" : sp.ModLoc.GetLocation();
            modExtractionDir = settingsDoc.SelectSingleNode("/Settings/Mod_Location").InnerText;
            gameLocation = sp.InsLoc.GetLocation();
            settingsDoc.Save("Settings.xml");
            CreateDir(modExtractionDir);
            //ApplyProfileAlt();
        }

        private void createSetupFile()
        {
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.IndentChars = "\t";
            xws.OmitXmlDeclaration = false;
            xws.Encoding = Encoding.UTF8;
            if (!File.Exists("Settings.xml"))
            {
                XmlWriter xw = XmlWriter.Create("Settings.xml", xws);
                xw.WriteStartDocument();
                xw.WriteStartElement("Settings");
                xw.WriteElementString("Setup_Complete", "false");
                xw.WriteElementString("Mod_Location", "./Extracted");
                xw.WriteElementString("Game_Location", "C:/Program Files (x86)/Steam/steamapps/common/Stormworks");
                xw.WriteElementString("Version", version);
                xw.WriteElementString("x64", "true");
                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }

            System.IO.Directory.CreateDirectory("Profiles");
            
            /**
            if (!File.Exists("CurrentProfile.xml"))
            {
                XmlWriter xwm = XmlWriter.Create("CurrentProfile.xml", xws);
                xwm.WriteStartDocument();
                xwm.WriteStartElement("Profile");
                xwm.WriteAttributeString("Name", "Default");
                xwm.WriteStartElement("Mods");
                xwm.WriteEndElement();
                xwm.WriteEndElement();
                xwm.WriteEndDocument();
                xwm.Close();
            }
            **/
            
            

        }

        private async void RunSetup()
        {
            settingsDoc.Load("Settings.xml");
            FirstRunStart frs = new FirstRunStart();
            await MaterialDesignThemes.Wpf.DialogHost.Show(new FirstRunStart());
            FirstRunGameLocation frgl = new FirstRunGameLocation();
            await MaterialDesignThemes.Wpf.DialogHost.Show(frgl);
            settingsDoc.SelectSingleNode("/Settings/Game_Location").InnerText = frgl.InsLoc.Text;
            gameLocation = frgl.InsLoc.Text;
            FirstRunModLocation frml = new FirstRunModLocation();
            await MaterialDesignThemes.Wpf.DialogHost.Show(frml);
            settingsDoc.SelectSingleNode("/Settings/Mod_Location").InnerText = frml.ModLoc.Text == "" ? "./Extracted" : frml.ModLoc.Text;
            modExtractionDir = frml.ModLoc.Text == "" ? "./Extracted" : frml.ModLoc.Text;
            settingsDoc.SelectSingleNode("/Settings/Setup_Complete").InnerText = "true";
            settingsDoc.Save("Settings.xml");

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void DialogHost_Loaded(object sender, RoutedEventArgs e)
        {
            if (settingsDoc.SelectSingleNode("/Settings/Setup_Complete").InnerText == "false")
            {
                DbgLog.WriteLine("Running");
                RunSetup();

            }
            try
            {
                checkNewVersion(true);
            }
            catch (Exception) { }
            

        }


        private void OpenProfile_Btn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "XML Profile file (.xml)|*.xml";
            Nullable<bool> r = opf.ShowDialog();

            if (r == true)
            {

                File.Copy(opf.FileName, "CurrentProfile.xml", true);

                currentProfile.Load("CurrentProfile.xml");

                XmlNode modRoot = currentProfile.SelectSingleNode("/Profile/Mods");
                List<Label> unavailableMods = new List<Label>();
                foreach (XmlNode m in modRoot)
                {
                    bool modFound = false;
                    foreach (ModListItem mli in ModList.Children)
                    {
                        if (mli.ModName.Content.ToString() == m.SelectSingleNode("Name").InnerText)
                        {
                            DbgLog.WriteLine("Found mod");
                            modFound = true;
                        }
                    }
                    if (!modFound)
                    {
                        Label l = new Label();
                        l.Content = m.SelectSingleNode("Name").InnerText;
                        unavailableMods.Add(l);
                    }
                }
                if (unavailableMods.Count > 0)
                {
                    ListBox lbx = new ListBox();
                    List<Control> ls = new List<Control>();
                    ls.Add(lbx);
                    lbx.ItemsSource = unavailableMods;
                    ShowInfoPopup("Mods in this profile are not known about", ls, PackIconKind.Exclamation);
                }
            }
            
            
            this.Title = "StormLoader : " + currentProfile.SelectSingleNode("/Profile").Attributes["Name"].InnerText;
            displayModList();
            }
            //CheckModActiveAlt();
            //ApplyProfileAlt();
        }

        private void SaveProfile_Btn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog opf = new Microsoft.Win32.SaveFileDialog();
            opf.Filter = "Stormloader Profile File (.spf)|*.spf";
            Nullable<bool> r = opf.ShowDialog();

            if (r == true)
            {
                //currentProfile.Load("CurrentProfile.xml");
                //currentProfile.SelectSingleNode("/Profile").Attributes["Name"].InnerText = System.IO.Path.GetFileNameWithoutExtension(opf.FileName);
                //currentProfile.Save("CurrentProfile.xml");
                //File.Copy("CurrentProfile.xml", opf.FileName, true);

                liveProfile.Name = System.IO.Path.GetFileNameWithoutExtension(opf.FileName);
                liveProfile.Save(opf.FileName);
            }
            try
            {
                this.Title = "StormLoader : " + currentProfile.SelectSingleNode("/Profile").Attributes["Name"].InnerText;
            }
            catch { }
            
        }

        
        public void ApplyProfileAlt()
        {
            currentProfile.Load("CurrentProfile.xml");
            foreach (ModListItem mli in ModList.Children)
            {
                if (currentProfile.SelectSingleNode("/Profile/Mods/Mod/Name[text()=\"" + mli.ModName.Content + "\"]") != null)
                {
                    //Debug.WriteLine(mli.ModName.Text);
                    //Debug.WriteLine(currentProfile.SelectSingleNode("/Profile/Mods/Mod/Name[text()='" + mli.ModName.Text + "']"));
                    //DbgLog.WriteLine("Found");
                    string modPath = mli.modPath;
                    //DbgLog.WriteLine("test");
                    try
                    {
                        RecursiveCopy(new DirectoryInfo(modPath + "/Meshes/"), new DirectoryInfo(gameLocation + "/rom/meshes/"));
                        //DbgLog.WriteLine("Running");
                    }
                    catch (Exception e) {}
                    try
                    {
                        RecursiveCopy(new DirectoryInfo(modPath + "/Definitions/"), new DirectoryInfo(gameLocation + "/rom/data/definitions/"));
                    }
                    catch (Exception e) {}
                    try
                    {
                        RecursiveCopy(new DirectoryInfo(modPath + "/Audio/"), new DirectoryInfo(gameLocation + "/rom/audio/"));
                    }
                    catch (Exception e) {}

                }
                else
                {
                    string modPath = mli.modPath;
                    try
                    {
                        RecursiveDelete(new DirectoryInfo(modPath + "/Meshes/"), new DirectoryInfo(gameLocation + "/rom/meshes/"));
                        //DbgLog.WriteLine("Running Delete");
                    }
                    catch (Exception) { }
                    try
                    {
                        RecursiveDelete(new DirectoryInfo(modPath + "/Definitions/"), new DirectoryInfo(gameLocation + "/rom/data/definitions/"));
                    }
                    catch (Exception) { }
                    try
                    {
                        RecursiveDelete(new DirectoryInfo(modPath + "/Audio/"), new DirectoryInfo(gameLocation + "/rom/audio/"));
                    }
                    catch (Exception) { }
                }

            }
        }

        public void RecursiveCopy(DirectoryInfo source, DirectoryInfo location)
        {
            foreach(FileInfo f in source.GetFiles())
            {
                File.Copy(source + f.Name, location + f.Name, true);
            }
            foreach (DirectoryInfo d in source.GetDirectories())
            {
                
                DirectoryInfo next = location.CreateSubdirectory(d.Name);
                DirectoryInfo dn = new DirectoryInfo(d.FullName + "/");
                next = new DirectoryInfo(next.FullName + "/");
                RecursiveCopy(dn, next);
            }
        }

        public void RecursiveDelete(DirectoryInfo extractedPath, DirectoryInfo location)
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
                RecursiveDelete(dn, next);
            }
        }

        
        public void AddModNew(Profiles.ModPack pack)
        {
            
            int i = 0;
            while (liveProfile.ModPacks.Count > i)
            {
                if (liveProfile.ModPacks[i].Name == pack.Name)
                {
                    liveProfile.RemoveMod(liveProfile.ModPacks[i]);
                }
                i++;
            }
            liveProfile.AddMod(pack);
            liveProfile.Save("CurrentProfile.spf");

            // install the mod
            
            mi.InstallModPack(pack.Name, pack.ContentPath, gameLocation);
            //ApplyProfileAlt();
        }

        

        public void SetModActive(Profiles.ModPack pack, bool active)
        {
            //currentProfile.Load("CurrentProfile.xml");
            if (active)
            {
                //XmlDocument meta = new XmlDocument();
                //meta.Load(path + "/metadata.xml");
                //DbgLog.WriteLine(meta.OuterXml);
                AddModNew(pack);
                pack.Active = true;
            } else
            {
                
                mi.DeleteByInstallList(pack.Name, gameLocation);
                pack.Active = false;
            }
            //currentProfile.Save("CurrentProfile.xml");
            //ApplyProfileAlt();
            liveProfile.Save("CurrentProfile.spf");
            this.Dispatcher.Invoke(() =>
            {
                displayModList();
            });
            

        }

        public void DeleteMod(Profiles.ModPack pack)
        {
            if (installQueue.Count > 0)
            {
                MessageBox.Show("Please wait untill all installs/uninstalls are complete before deleting a mod");
                return;
            }
            //SetModActive(modName, "false");
            //currentProfile.Load("CurrentProfile.xml");
            //XmlNode ModRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            
            SetModActive(pack, false);
            if (Directory.Exists(pack.ContentPath))
            {
                Directory.Delete(pack.ContentPath, true);
                try
                {
                    File.Delete("./Downloaded/" + pack.Name + ".slp");
                }
                catch { }
            }
            liveProfile.RemoveMod(pack);
            //ApplyProfileAlt();
            //mi.DeleteByInstallList(pack.Name, gameLocation);
            displayModList();
        }

        public void SelectMod(Profiles.ModPack pack)
        {
            ModNameLabel.Content = "Name: " + pack.Name;

            //ModNameLabel.Content = "Name: " + modName;
            //currentProfile.Load("CurrentProfile.xml");
            //XmlDocument meta = new XmlDocument();
            //meta.Load(pack.ContentPath + "/Metadata.xml");
            AuthorLabel.Content = "Author: " + pack.Author;
            ModVersionLabel.Content = "Version: " + pack.Version;
            //string modPath = n.ParentNode.SelectSingleNode("Path").InnerText;
            string infoPath = pack.ContentPath + "/info.html";
            if (File.Exists(infoPath))
            {
                infoDisp.Navigate(new Uri("file://" + System.IO.Path.GetFullPath(infoPath)));
            }
        }

        public void ShowInfoPopup(string title, List<Control> controls, PackIconKind iconKind)
        {
            InfoPopup ifp = new InfoPopup();
            ifp.titleText.Content = title;
            ifp.icon.Kind = iconKind;
            
            if (controls != null)
            {
                foreach (Control c in controls)
                {
                    ifp.infoContainer.Children.Add(c);
                }
            }
                
            
            
            MaterialDesignThemes.Wpf.DialogHost.Show(ifp);

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            
            List<Control> c = new List<Control>();
            ListBox s = new ListBox();
            Label l = new Label();
            l.Content = "Version: " + this.version;
            s.Items.Add(l);
            Label l2 = new Label();
            l2.Content = "Developer: Lewinator56";
            s.Items.Add(l2);
            c.Add(s);
            ShowInfoPopup("About", c, PackIconKind.About);
            
        }

        private void LaunchGame_Click(object sender, RoutedEventArgs e)
        {
            ShowInfoPopup("Launching game" + (x64 ? " 64-bit" : " 32-bit") , new List<Control>(), PackIconKind.Information);
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = gameLocation + "/stormworks" + (x64 ? "64" : "") + ".exe";
                p.StartInfo.Arguments = "";
                p.StartInfo.WorkingDirectory = gameLocation;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
                p.Start();
            } catch
            {
                Label tb = new Label();
                tb.Content = "Check your install location is set to the path for Stormworks' directory BEFORE you ask on discord!";
                List<Control> tbl = new List<Control>();
                tbl.Add(tb);
                ShowInfoPopup("Unable to find game", tbl ,PackIconKind.Warning);
            }
            
        }

        private void Updates_Click(object sender, RoutedEventArgs e)
        {
            UpdateDialog ud = new UpdateDialog();
            DialogHost.Show(ud);
        }

        private void BrowseRepo_Click(object sender, RoutedEventArgs e)
        {
            RepoBrowserRoot rpbr = new RepoBrowserRoot();
            rpbr.Show();
        }

        private void BrowseNexus_Click(object sender, RoutedEventArgs e)
        {
            List<Control> cons = new List<Control>();
            cons.Add(new Label
            {
                Content = "I know how much you all want this, but its taking a while :("
            });
            ShowInfoPopup("Not Implemented Yet", cons, PackIconKind.SmileySadOutline);
        }

        private void BrowseOnline_Click(object sender, RoutedEventArgs e)
        {
            BrowseOnline.ContextMenu.IsOpen = true;
        }

        private void BrowseSteam_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://steamcommunity.com/workshop/browse/?appid=573090&searchtext=%5BMOD%5D&childpublishedfileid=0&browsesort=textsearch&section=readytouseitems&requiredtags%5B%5D=Mission");
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            settingsDoc.Load("Settings.xml");
            settingsDoc.SelectSingleNode("/Settings/x64").InnerText = x64 ? "true" : "false";
            settingsDoc.Save("Settings.xml");
        }

        private async void CheckSteam_Click(object sender, RoutedEventArgs e)
        {
            Label l = new Label() { Content = "Synchronising workshop mods, please wait"};

            //ModInstallList.Children.Add(l);
            await Task.Run(() => mod_handling.Workshop.copyMods(mod_handling.Workshop.getWorkshopPath()));
            displayModList();
            //ModInstallList.Children.Remove(l);

            
        }

        private void Packager_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.Content = new Packager.PackagerControlPanel();
            window.Show();
        }

        private void Sync_Files_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://steamcommunity.com/workshop/browse/?appid=573090&searchtext=%5BMOD%5D");
        }

        private void Browse_Files_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", new DirectoryInfo(modExtractionDir).FullName);
        }

        private void Browse_Game_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", new DirectoryInfo(gameLocation).FullName);
        }

        private void Theme_Editor_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window();
            window.Content = new Themes.ThemePicker(themeManager, window);
            
            window.SizeToContent = SizeToContent.Height;
            window.MaxHeight = 600;
            window.Width = 600;
            window.Show();
        }

        private void Backup_Files_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "zip file (.zip)|*.zip";
            Nullable<bool> r = sfd.ShowDialog();

            if (r == true)
            {
                DialogHost.Show(new UI.Common.LoadingSpinner());
                Task t = new Task(delegate
                {
                ZipFile z = new ZipFile();
                z.AddDirectory(gameLocation + "/rom", "rom");
                z.Save(sfd.FileName);
                this.Dispatcher.Invoke(() => MainHost.IsOpen = false);
                    

                });
                t.Start();
                
                
                
            
                
            }
        }

        private void Restore_Files_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "zip file (.zip)|*.zip";
            Nullable<bool> r = ofd.ShowDialog();

            if (r == true)
            {
                DialogHost.Show(new UI.Common.LoadingSpinner());
                Task t = new Task(delegate
                {
                    ZipFile z = new ZipFile(ofd.FileName);
                    z.ExtractAll(gameLocation, ExtractExistingFileAction.OverwriteSilently);
                    this.Dispatcher.Invoke(() => MainHost.IsOpen = false);
                });
                t.Start();
               
            }
        }

        public void HideDialog()
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        private void Help_btn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://portal.stormloader.uk/stormloader/help.html");
        }
    }
}
