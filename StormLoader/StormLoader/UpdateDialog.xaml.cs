using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.IO;

namespace StormLoader
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : UserControl
    {
        public UpdateDialog()
        {
            InitializeComponent();
        }

        private void GetUpdate_Btn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Lewinator56/StormLoader/releases/latest");
        }

        private void Check_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                checkNewVersion();
            } catch (Exception)
            {
                UpdateInfo.Content = "Could not connect";
            }
            
        }

        private void checkNewVersion()
        {
            
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("https://github.com/Lewinator56/StormLoader/releases/latest");
            wr.AllowAutoRedirect = true;
            HttpWebResponse wrs = (HttpWebResponse) wr.GetResponse();
            string onlineVer = wrs.ResponseUri.ToString().Substring(wrs.ResponseUri.ToString().LastIndexOf('/') + 1);
            
            if (onlineVer != GlobalVar.mw.version)
            {
                GetUpdate_Btn.IsEnabled = true;
                UpdateInfo.Content = "A new Version is available\nVersion: " + GlobalVar.mw.version + " -> Version: " + onlineVer;

            } else
            {
                UpdateInfo.Content = "You are already running the latest version";
            }
            
        }
    }
}
