using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace StormLoader.Packager
{
    /// <summary>
    /// Interaction logic for PackagerControlPanel.xaml
    /// </summary>
    public partial class PackagerControlPanel : UserControl
    {
        public PackagerControlPanel()
        {
            InitializeComponent();
        }

        

        private void ModName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = Regex.IsMatch(e.Text, "[^a-zA-Z- _0-9+]+");
            //DbgLog.WriteLine(Regex.IsMatch(e.Text, "[^a-zA-Z- _0-9+]+").ToString());
        }

        private void PackageBtn_Click(object sender, RoutedEventArgs e)
        {
            // check required data
            bool steam = (bool)SteamPackage.IsChecked;
            if (Author.Text == "" || Author.Text == null || Version.Text == "" || Author.Text == null || ModName.Text == "" || ModName.Text == null)
            {
                MessageBox.Show("Plese ensure you have set a mod name, version and author!");
                return;
            }
            Dictionary<string, DirectoryInfo> d = new Dictionary<string, DirectoryInfo>();
            d.Add("meshes", MeshLoc.GetDirectory());
            d.Add("definitions", DefLoc.GetDirectory());
            d.Add("audio", AudioLoc.GetDirectory());
            d.Add("graphics", GraphicsLoc.GetDirectory());
            d.Add("data", DataLoc.GetDirectory());
            d.Add("info", InfoLoc.GetDirectory());
            ModPackager.Package(ModName.Text, Author.Text, Version.Text, steam, d);



        }
    }
}
