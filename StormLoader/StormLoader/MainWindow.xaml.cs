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

namespace StormLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public XmlDocument settingsDoc = new XmlDocument();
        public String modExtractionDir;
        public XmlDocument currentProfile = new XmlDocument();
        public string gameLocation;
        public List<ModListItem> modListItems = new List<ModListItem>();
        public MainWindow()
        {
            
            InitializeComponent();
            GlobalVar.mw = this;

            createSetupFile();
            settingsDoc.Load("Settings.xml");
            modExtractionDir = settingsDoc.SelectSingleNode("/Settings/Mod_Location").InnerText;
            gameLocation = settingsDoc.SelectSingleNode("/Settings/Game_Location").InnerText;

            currentProfile.Load("CurrentProfile.xml");
            displayModList();
            ApplyProfile();






        }

        private void displayModList()
        {
            ModList.Children.Clear();
            string[] filesInDirectory = Directory.GetDirectories(modExtractionDir);
            foreach (string dir in filesInDirectory)
            {
                ModListItem mli = new ModListItem();
                mli.ModName.Content = new DirectoryInfo(dir).Name;
                ModList.Children.Add(mli);
            }
            checkModActive();
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


        private void AddMod_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "Compressed Mod File|*.zip";
            Nullable<bool> r = opf.ShowDialog();

            if (r == true)
            {
                ZipFile z = new ZipFile(opf.FileName);
                z.ExtractAll(modExtractionDir + "/" + System.IO.Path.GetFileNameWithoutExtension(opf.FileName), ExtractExistingFileAction.OverwriteSilently);
                XmlDocument meta = new XmlDocument();
                meta.Load(modExtractionDir + "/" + System.IO.Path.GetFileNameWithoutExtension(opf.FileName) + "/metadata.xml");
                Debug.WriteLine(meta.OuterXml);
                AddModNew(modExtractionDir + "/" + System.IO.Path.GetFileNameWithoutExtension(opf.FileName), System.IO.Path.GetFileNameWithoutExtension(opf.FileName), meta.SelectSingleNode("/Metadata/Version").InnerText, meta.SelectSingleNode("/Metadata/Author").InnerText);
            }
            displayModList();

        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

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
            FirstRunModLocation frml = new FirstRunModLocation();
            await MaterialDesignThemes.Wpf.DialogHost.Show(frml);
            settingsDoc.SelectSingleNode("/Settings/Mod_Location").InnerText = frml.ModLoc.Text == "" ? "./Extracted" : frml.ModLoc.Text;
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
                Debug.WriteLine("Running");
                RunSetup();

            }
            
        }


        private void OpenProfile_Btn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = "XML Profile file (.xml)|*.xml";
            Nullable<bool> r = opf.ShowDialog();

            if (r == true)
            {

                File.Copy(opf.FileName, "CurrentProfile.xml", true);
            }
            displayModList();
            checkModActive();
            ApplyProfile();
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
                    string[] mesh = Directory.GetFiles(modPath + "/Meshes");
                    string[] def = Directory.GetFiles(modPath + "/Definitions");
                    foreach(string fileName in mesh)
                    {

                        File.Copy(fileName, gameLocation + "/rom/meshes/" + System.IO.Path.GetFileName(fileName), true);
                    }
                    foreach (string fileName in def)
                    {

                        File.Copy(fileName, gameLocation + "/rom/data/definitions/" + System.IO.Path.GetFileName(fileName), true);
                    }
                } else if (mod.SelectSingleNode("Active").InnerText =="false")
                {
                    string modPath = mod.SelectSingleNode("Path").InnerText;
                    string[] mesh = Directory.GetFiles(modPath + "/Meshes");
                    string[] def = Directory.GetFiles(modPath + "/Definitions");
                    foreach (string fileName in mesh)
                    {

                        File.Delete(gameLocation + "/rom/meshes/" + System.IO.Path.GetFileName(fileName));
                    }
                    foreach (string fileName in def)
                    {

                        File.Delete(gameLocation + "/rom/data/definitions/" + System.IO.Path.GetFileName(fileName));
                    }
                }

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
                    modActive = n.SelectSingleNode("Active").InnerText;
                    
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

            XmlElement active = currentProfile.CreateElement("Active");
            active.InnerText = modActive;

            ModNode.AppendChild(name);
            ModNode.AppendChild(path);
            ModNode.AppendChild(version);
            ModNode.AppendChild(author);
            ModNode.AppendChild(active);

            ModRoot.AppendChild(ModNode);

            currentProfile.Save("CurrentProfile.xml");
            ApplyProfile();
        }

        

        public void SetModActive(string modName, string active)
        {
            currentProfile.Load("CurrentProfile.xml");
            XmlNode modRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            foreach (XmlNode mod in modRoot)
            {
                if (mod.SelectSingleNode("Name").InnerText == modName)
                {
                    mod.SelectSingleNode("Active").InnerText = active;
                    currentProfile.Save("CurrentProfile.xml");
                }
                
            }
            ApplyProfile();
        }

        public void DeleteMod(string modName)
        {
            SetModActive(modName, "false");
            currentProfile.Load("CurrentProfile.xml");
            XmlNode ModRoot = currentProfile.SelectSingleNode("/Profile/Mods");
            foreach (XmlNode n in ModRoot)
            {
                if (n.SelectSingleNode("Name").InnerText == modName)
                {
                    if (Directory.Exists(n.SelectSingleNode("Path").InnerText)) {
                        Directory.Delete(n.SelectSingleNode("Path").InnerText, true);
                    }
                    currentProfile.SelectSingleNode("/Profile/Mods").RemoveChild(n);
                    

                }

            }
            displayModList();
        }
    }
}
