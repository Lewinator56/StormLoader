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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StormLoader.modder_control_panel
{
    /// <summary>
    /// Interaction logic for modderPanelModListItem.xaml
    /// </summary>
    public partial class modderPanelModListItem : UserControl
    {
        private int mod_id;
        ModderPanelRoot mpr;

        public modderPanelModListItem(ModderPanelRoot mpr, string mod_name, string mod_version, int mod_id)
        {
            InitializeComponent();
            ModName.Content = mod_name;
            ModVersion.Content += mod_version;
            this.mod_id = mod_id;
            this.mpr = mpr;
        }

        private void UpdateMod_Click(object sender, RoutedEventArgs e)
        {
            mpr.modderPanelDialogHost.ShowDialog(new ModderUpdateModPanel(mod_id, mpr));
            mpr.refreshMods();
        }

        private void DeleteMod_Click(object sender, RoutedEventArgs e)
        {
            GlobalVar.sqcm.DeleteModFromTable(mod_id);
            mpr.refreshMods();
        }
    }
}
