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
using System.Diagnostics;
using MaterialDesignThemes.Wpf;

namespace StormLoader.repository
{
    /// <summary>
    /// Interaction logic for RepoModListItem.xaml
    /// </summary>
    public partial class RepoModListItem : UserControl
    {
        int id;
        string infoPath;
        RepoBrowserRoot rbr;
        public RepoModListItem(string name, string author, string version, string description, byte[] image, int id, int verified, RepoBrowserRoot rbr)
        {
            InitializeComponent();
            modName.Content = name;
            Author.Content += author;
            Version.Content += version;
            Description.Text = description;
            this.id = id;
            if (verified != 1)
            {
                VerifiedMod.Visibility = Visibility.Hidden;
            }
            this.rbr = rbr;

            var bitmap = (BitmapSource)new ImageSourceConverter().ConvertFrom(image);
            ModImage.Source = bitmap;



        }

        public void enableInfoButton(string link)
        {
            if (link.StartsWith("http://")) {
                infoBtn.IsEnabled = true;
                infoPath = link;
                infoBtn.ToolTip = link;
            }
            
            
        }

        private void infoBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(infoPath);
        }

        private  void getModBtn_Click(object sender, RoutedEventArgs e)
        {
            downloadMod();
        }

        private async void downloadMod()
        {
            byte[] mod = await GlobalVar.sqcm.downloadMod(id);
            File.WriteAllBytes("Downloaded/" + modName.Content + ".slp", mod);
            InfoPopup ifp = new InfoPopup();
            ifp.titleText.Content = "Download Complete, Activating Mod";
            GlobalVar.mw.addModFromFile("Downloaded/" + modName.Content + ".slp", modName.Content.ToString(), ".slp");
            await rbr.RepoDialog.ShowDialog(ifp);

        }
    }

}
