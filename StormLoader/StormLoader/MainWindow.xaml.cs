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
using System.IO;
using System.Net;
using StormLoader.repository;

namespace StormLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public XmlDocument settingsDoc = new XmlDocument();
        public String modExtractionDir = "";
        public XmlDocument currentProfile = new XmlDocument();
        public string gameLocation = "";
        public List<ModListItem> modListItems = new List<ModListItem>();
        public string version = "v1.0.4";
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
            settingsDoc.Save("Settings.xml");


            currentProfile.Load("CurrentProfile.xml");
            this.Title = "StormLoader : " + currentProfile.SelectSingleNode("/Profile").Attributes["Name"].InnerText;
            displayModList();
            ApplyProfileAlt();
            
            
            





        }

        static void UnhandledExceptionHandler(Object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            DbgLog.WriteLine(e.Message);
            DbgLog.WriteLine(e.StackTrace.ToString());
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
            foreach (string dir in filesInDirectory)
            {
                ModListItem mli = new ModListItem();
                mli.ModName.Content = new DirectoryInfo(dir).Name;
                mli.modPath = dir;
                DbgLog.WriteLine(dir);
                ModList.Children.Add(mli);
            }
            CheckModActiveAlt();
        }
        public void checkModActive()
        {
            currentProfile.Load("CurrentProfile.xml");
            XmlNode modRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            foreach (XmlNode mod in modRoot)
            {
                if (mod.SelectSingleNode("Active").InnerText == "true")
                {
                    foreach (ModListItem mli in ModList.Children)
                    {
                        if (mod.SelectSingleNode("Name").InnerText == (string)mli.ModName.Content)
                        {
                            mli.SetActive(true);

                        }
                    }
                }
                else
                {
                    foreach (ModListItem mli in ModList.Children)
                    {
                        if (mod.SelectSingleNode("Name").InnerText == (string)mli.ModName.Content)
                        {
                            mli.SetActive(false);

                        }
                    }
                }
            }
        }

        public void CheckModActiveAlt()
        {
            currentProfile.Load("CurrentProfile.xml");
            foreach (ModListItem mli in ModList.Children)
            {
                if (currentProfile.SelectSingleNode("/Profile/Mods/Mod/Name[text()='" + mli.ModName.Content + "']") != null)
                {
                    DbgLog.WriteLine(mli.ModName.Content.ToString());
                    DbgLog.WriteLine(currentProfile.SelectSingleNode("/Profile/Mods/Mod/Name[text()='" + mli.ModName.Content + "']").InnerText);
                    DbgLog.WriteLine("Found");
                    mli.SetActive(true);
                } else
                {
                    mli.SetActive(false);
                    
                }

            }
        }


        private void AddMod_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "StormLoader mod package|*.slp";
            Nullable<bool> r = opf.ShowDialog();

            if (r == true)
            {
                addModFromFile(opf.FileName,System.IO.Path.GetFileNameWithoutExtension(opf.FileName));
                
                
            }
            

        }

        public void addModFromFile(string path, string nameWithoutExt)
        {
            ZipFile z = new ZipFile(path);
            
            z.ExtractAll(modExtractionDir + "/" + nameWithoutExt, ExtractExistingFileAction.OverwriteSilently);
            XmlDocument meta = new XmlDocument();
            meta.Load(modExtractionDir + "/" + nameWithoutExt + "/metadata.xml");
            DbgLog.WriteLine(meta.OuterXml);


            AddModNew(modExtractionDir + "/" + nameWithoutExt, nameWithoutExt, meta.SelectSingleNode("/Metadata/Version").InnerText, meta.SelectSingleNode("/Metadata/Author").InnerText);
            displayModList();
            ApplyProfileAlt();
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
            ApplyProfileAlt();
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
                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }

            System.IO.Directory.CreateDirectory("Profiles");

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
            CheckModActiveAlt();
            ApplyProfileAlt();
        }

        private void SaveProfile_Btn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog opf = new Microsoft.Win32.SaveFileDialog();
            opf.Filter = "XML Profile file (.xml)|*.xml";
            Nullable<bool> r = opf.ShowDialog();

            if (r == true)
            {
                currentProfile.Load("CurrentProfile.xml");
                currentProfile.SelectSingleNode("/Profile").Attributes["Name"].InnerText = System.IO.Path.GetFileNameWithoutExtension(opf.FileName);
                currentProfile.Save("CurrentProfile.xml");
                File.Copy("CurrentProfile.xml", opf.FileName, true);
            }
            this.Title = "StormLoader : " + currentProfile.SelectSingleNode("/Profile").Attributes["Name"].InnerText;
        }

        public void ApplyProfile()
        {
            currentProfile.Load("CurrentProfile.xml");
            XmlNode modRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            foreach (XmlNode mod in modRoot)
            {
                if (mod.SelectSingleNode("Active").InnerText == "true")
                {
                    string modPath = mod.SelectSingleNode("Path").InnerText;
                    //string[] mesh = Directory.GetFiles(modPath + "/Meshes");
                    //string[] def = Directory.GetFiles(modPath + "/Definitions");
                    //string[] sound = Directory.GetFiles(modPath + "/Sounds");
                    try
                    {
                        RecursiveCopy(new DirectoryInfo(modPath + "/Meshes/"), new DirectoryInfo(gameLocation + "/rom/meshes/"));
                    }
                    catch (Exception) { }
                    try
                    {
                        RecursiveCopy(new DirectoryInfo(modPath + "/Definitions/"), new DirectoryInfo(gameLocation + "/rom/data/definitions/"));
                    }
                    catch (Exception) {}
                    try
                    {
                        RecursiveCopy(new DirectoryInfo(modPath + "/Audio/"), new DirectoryInfo(gameLocation + "/rom/audio/"));
                    }
                    catch (Exception) { }

//                    foreach(string fileName in mesh)
//                    {
//
//                        File.Copy(fileName, gameLocation + "/rom/meshes/" + System.IO.Path.GetFileName(fileName), true);
//                    }
//                    foreach (string fileName in def)
//                    {
//
//                        File.Copy(fileName, gameLocation + "/rom/data/definitions/" + System.IO.Path.GetFileName(fileName), true);
//                    }
//                    foreach (string fileName in sound)
//                    {
//
//                        File.Copy(fileName, gameLocation + "/rom/audio/" + System.IO.Path.GetFileName(fileName), true);
//                    }

                    // copy any directories

                } else if (mod.SelectSingleNode("Active").InnerText =="false")
                {
                    string modPath = mod.SelectSingleNode("Path").InnerText;
                    try
                    {
                        RecursiveDelete(new DirectoryInfo(modPath + "/Meshes/"), new DirectoryInfo(gameLocation + "/rom/meshes/"));
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
                    } catch (Exception) { }
                    
                    //string[] mesh = Directory.GetFiles(modPath + "/Meshes/");
                    //string[] def = Directory.GetFiles(modPath + "/Definitions/");
                    //string[] sound = Directory.GetFiles(modPath + "/Sounds");
                    //foreach (string fileName in mesh)
                    //{

//                        File.Delete(gameLocation + "/rom/meshes/" + System.IO.Path.GetFileName(fileName));
//                    }
//                    foreach (string fileName in def)
//                    {

//                        File.Delete(gameLocation + "/rom/data/definitions/" + System.IO.Path.GetFileName(fileName));
///                    }
                    //foreach (string fileName in sound)
                    //{

                      //  File.Delete(gameLocation + "/rom/audio/" + System.IO.Path.GetFileName(fileName));
                    //}

                    // delete any directories
                }

            }
        }
        public void ApplyProfileAlt()
        {
            currentProfile.Load("CurrentProfile.xml");
            foreach (ModListItem mli in ModList.Children)
            {
                if (currentProfile.SelectSingleNode("/Profile/Mods/Mod/Name[text()='" + mli.ModName.Content + "']") != null)
                {
                    //Debug.WriteLine(mli.ModName.Content);
                    //Debug.WriteLine(currentProfile.SelectSingleNode("/Profile/Mods/Mod/Name[text()='" + mli.ModName.Content + "']"));
                    DbgLog.WriteLine("Found");
                    string modPath = mli.modPath;
                    DbgLog.WriteLine("test");
                    try
                    {
                        RecursiveCopy(new DirectoryInfo(modPath + "/Meshes/"), new DirectoryInfo(gameLocation + "/rom/meshes/"));
                        DbgLog.WriteLine("Running");
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
                        DbgLog.WriteLine("Running Delete");
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

        
        public void AddModNew(string modPath, string modName, string modVersion, string modAuthor)
        {
            XmlNode ModRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            string modActive = "false";
            foreach (XmlNode n in ModRoot)
            {
                if (n.SelectSingleNode("Name").InnerText == modName)
                {

                    currentProfile.SelectSingleNode("/Profile/Mods").RemoveChild(n);
                    
                }
                
            }
            XmlNode ModNode = currentProfile.CreateElement("Mod");
            XmlElement name = currentProfile.CreateElement("Name");
            name.InnerText = modName;
            XmlElement path = currentProfile.CreateElement("Path");
            path.InnerText = modPath;
            XmlElement version = currentProfile.CreateElement("Version");
            version.InnerText = modVersion;
            XmlElement author = currentProfile.CreateElement("Author");
            author.InnerText = modAuthor;



            ModNode.AppendChild(name);
            ModNode.AppendChild(path);
            ModNode.AppendChild(version);
            ModNode.AppendChild(author);
            //ModNode.AppendChild(active);

            ModRoot.AppendChild(ModNode);

            currentProfile.Save("CurrentProfile.xml");
            //ApplyProfileAlt();
        }

        

        public void SetModActive(string modName, string path, bool active)
        {
            currentProfile.Load("CurrentProfile.xml");
            if (active)
            {
                XmlDocument meta = new XmlDocument();
                meta.Load(path + "/metadata.xml");
                DbgLog.WriteLine(meta.OuterXml);
                AddModNew(path, modName, meta.SelectSingleNode("/Metadata/Version").InnerText, meta.SelectSingleNode("/Metadata/Author").InnerText);
            } else
            {
                try
                {
                    XmlElement modNode = (XmlElement)currentProfile.SelectSingleNode("/Profile/Mods/Mod[Name='" + modName + "']");
                    if (modNode != null)
                    {
                        modNode.ParentNode.RemoveChild(modNode);
                    }
                    
                
                }
                catch (Exception) { }
            }
            currentProfile.Save("CurrentProfile.xml");
            ApplyProfileAlt();

        }

        public void DeleteMod(string modName, string path)
        {
            //SetModActive(modName, "false");
            currentProfile.Load("CurrentProfile.xml");
            XmlNode ModRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            SetModActive(modName, path, false);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            ApplyProfileAlt();
            displayModList();
        }

        public void SelectMod(string modName, string modPath)
        {
            ModNameLabel.Content = "Name: " + modName;
            currentProfile.Load("CurrentProfile.xml");
            XmlDocument meta = new XmlDocument();
            meta.Load(modPath + "/Metadata.xml");
            AuthorLabel.Content = "Author: " + meta.SelectSingleNode("/Metadata/Author").InnerText;
            ModVersionLabel.Content = "Version: " + meta.SelectSingleNode("/Metadata/Version").InnerText;
            //string modPath = n.ParentNode.SelectSingleNode("Path").InnerText;
            string infoPath = modPath + "/info.html";
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

            foreach (Control c in controls)
            {
                ifp.infoContainer.Children.Add(c);
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
            ShowInfoPopup("Launching game", new List<Control>(), PackIconKind.Information);
            try
            {
                System.Diagnostics.Process.Start(gameLocation + "/stormworks64.exe");
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
    }
}
