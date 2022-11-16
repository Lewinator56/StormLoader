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
using System.Threading;
using StormLoader.mod_handling.install;

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
            string name = modName.Content.ToString();
            Thread t = new Thread(() => downloadMod(name));

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private async void downloadMod(string name)
        {
            bool install = true;
            this.Dispatcher.Invoke(() =>
            {
                rbr.ModDownloadList.Children.Add(new Label() { Content = "DOWNLOADING: " + name });
            });
            try
            {
                byte[] mod = await GlobalVar.sqcm.downloadMod(id);
                File.WriteAllBytes("Downloaded/" + name + ".slp", mod);
            } catch
            {
                install = false;
                MessageBox.Show("Something went wrong while downloading the mod, you can try again.\r\nIf the problem persists, contact the developer", "Something went wrong");
            }
            
            this.Dispatcher.Invoke(() =>
            {
                rbr.ModDownloadList.Children.RemoveAt(0);
            });
            //InfoPopup ifp = new InfoPopup();
            //ifp.titleText.Content = "Download Complete, Activating Mod";
            Profiles.ModPack p = new Profiles.ModPack();
            p.Active = true;
            if (install)
            {
                GlobalVar.mw.addModFromFile("Downloaded/" + name + ".slp", name, ".slp", p);
            }
            
            
            //await rbr.RepoDialog.ShowDialog(ifp);

        }
    }

}
