using System;
using System.Collections.Generic;
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

namespace StormLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            createSetupFile();

            InitializeComponent();
            for (int i = 0; i < 20; i++)
            {
                ModListItem mli = new ModListItem();

                ModList.Children.Add(mli);
            }
            
        }

        private void AddMod_Click(object sender, RoutedEventArgs e)
        {

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
                xw.WriteElementString("Mod_Location", "");
                xw.WriteElementString("Game_Location", "");
                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }
            
            
            if (!File.Exists("Mods.xml"))
            {
                XmlWriter xwm = XmlWriter.Create("Mods.xml", xws);
                xwm.WriteStartDocument();
                xwm.WriteStartElement("Mods");
                xwm.WriteEndElement();
                xwm.WriteEndDocument();
                xwm.Close();
            }
            
            

        }
    }
}
