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
    /// Interaction logic for ModderUpdateModPanel.xaml
    /// </summary>
    public partial class ModderUpdateModPanel : UserControl
    {
        int mod_id;
        ModderPanelRoot mpr;
        public ModderUpdateModPanel(int mod_id, ModderPanelRoot mpr)
        {
            InitializeComponent();
            this.mod_id = mod_id;
            this.mpr = mpr;
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            InfoPopup ifp = new InfoPopup();
            if (await GlobalVar.sqcm.updateMod(mod_id, Name.Text, Version.Text, Description.Text, ModImagePath.GetLocation(), ModFilePath.GetLocation(), ExtraDetailsLink.Text))
            {

                ifp.titleText.Content = "Update Succeeded";
                await UploadDialogHost.ShowDialog(ifp);
                DialogHost.CloseDialogCommand.Execute(null, null);
                mpr.refreshMods();
            }
            else
            {
                ifp.titleText.Content = "Upload Failed";
                await UploadDialogHost.ShowDialog(ifp);
                DialogHost.CloseDialogCommand.Execute(null, null);
                
            }
        }
    }
}
