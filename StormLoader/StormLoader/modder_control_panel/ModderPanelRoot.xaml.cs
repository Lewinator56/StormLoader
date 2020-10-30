using MaterialDesignThemes.Wpf;
using MySql.Data.MySqlClient;
using StormLoader.repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        SQLManager sqcm;
        public ModderPanelRoot(string username)
        {
            InitializeComponent();
            this.sqcm = GlobalVar.sqcm;
            this.username = username;
            titleText.Content += username;
            refreshMods();
        }


        private void Logout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e) // upload button, i just forgot to name it
        {
            ModderUploadPanel mup = new ModderUploadPanel(this);
            mup.user = username;
            modderPanelDialogHost.ShowDialog(mup);
        }
        public void refreshMods()
        {
            ModList.Children.Clear();
            DataTable dt = sqcm.getModListByUser(username);
            
            foreach (DataRow r in dt.Rows)
            {
                Console.Write("Reading");
                modderPanelModListItem mpmli = new modderPanelModListItem(this, r["mod_name"].ToString(), r["mod_version"].ToString(), (int)r["mod_id"]);
                ModList.Children.Add(mpmli);
            }
        }
    }
}
