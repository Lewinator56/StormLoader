using MaterialDesignThemes.Wpf;
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
using System.Windows.Shapes;

namespace StormLoader.modder_control_panel
{
    /// <summary>
    /// Interaction logic for ModderPanelRoot.xaml
    /// </summary>
    public partial class ModderPanelRoot : Window
    {
        string username;
        public ModderPanelRoot()
        {
            InitializeComponent();
        }

        public void setUser(string username)
        {
            this.username = username;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e) // upload button, i just forgot to name it
        {
            ModderUploadPanel mup = new ModderUploadPanel();
            mup.user = username;
            modderPanelDialogHost.ShowDialog(mup);
        }
    }
}
