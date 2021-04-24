using System;
using System.Collections.Generic;
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

namespace StormLoader
{
    /// <summary>
    /// Interaction logic for SettingsPopup.xaml
    /// </summary>
    public partial class SettingsPopup : UserControl
    {
        public SettingsPopup()
        {
            InitializeComponent();
            InsLoc.setText(GlobalVar.mw.gameLocation);
            ModLoc.setText(GlobalVar.mw.modExtractionDir);
            NexAPIKey.Text = GlobalVar.NMAPIKey;
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
