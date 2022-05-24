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

namespace StormLoader
{
    /// <summary>
    /// Interaction logic for ModListItem.xaml
    /// </summary>
    public partial class ModListItem : UserControl
    {

        public string modPath = "";
        public ModListItem()
        {
            InitializeComponent();
        }
        public void SetActive(bool active)
        {
            if (active)
            {
                ModActive.Kind = PackIconKind.Check;
                ModActive.Foreground = new SolidColorBrush(Colors.Green);
            } else
            {
                ModActive.Kind = PackIconKind.Close;
                ModActive.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void ActvMod_Btn_Click(object sender, RoutedEventArgs e)
        {
            SetActive(true);
            //mod_handling.ModInstaller mi = new mod_handling.ModInstaller();
            //mi.InstallMod(ModName.Content.ToString(), modPath, GlobalVar.mw.gameLocation);
            DbgLog.WriteLine(modPath);
            //GlobalVar.mw.ModInstallList.Children.Add(new Label() { Content = "INSTALLING: " + ModName.Text.ToString() });
            //GlobalVar.mw.installQueue.Enqueue(new Mod(ModName.Text.ToString(), modPath));
            GlobalVar.mw.AddModToInstallQueue(ModName.Text.ToString(), modPath, true);
            //GlobalVar.mw.SetModActive(ModName.Text.ToString(), modPath, true);
            
        }

        private void DeActvMod_Click(object sender, RoutedEventArgs e)
        {
            SetActive(false);
            //mod_handling.ModInstaller mi = new mod_handling.ModInstaller();
            //mi.DeleteByInstallInfo(ModName.Content.ToString(), GlobalVar.mw.gameLocation);
            //GlobalVar.mw.SetModActive(ModName.Text.ToString(), modPath, false);
            GlobalVar.mw.AddModToInstallQueue(ModName.Text.ToString(), modPath, false);
        }

        private void UninsMod_Click(object sender, RoutedEventArgs e)
        {
            //mod_handling.ModInstaller mi = new mod_handling.ModInstaller();
            //mi.DeleteByInstallInfo(ModName.Content.ToString(), GlobalVar.mw.gameLocation);
            GlobalVar.mw.DeleteMod(ModName.Text.ToString(), modPath);
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GlobalVar.mw.SelectMod(ModName.Text.ToString(), modPath);
        }

        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            MaterialDesignThemes.Wpf.ShadowAssist.SetShadowDepth(ModCard, ShadowDepth.Depth2);
        }

        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            MaterialDesignThemes.Wpf.ShadowAssist.SetShadowDepth(ModCard, ShadowDepth.Depth1);
        }
    }
}

    
