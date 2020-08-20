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
            
            GlobalVar.mw.SetModActive(ModName.Content.ToString(), modPath, true);
            
        }

        private void DeActvMod_Click(object sender, RoutedEventArgs e)
        {
            SetActive(false);
            GlobalVar.mw.SetModActive(ModName.Content.ToString(), modPath, false);
        }

        private void UninsMod_Click(object sender, RoutedEventArgs e)
        {
            GlobalVar.mw.DeleteMod(ModName.Content.ToString(), modPath);
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GlobalVar.mw.SelectMod(ModName.Content.ToString(), modPath);
        }
    }
}

    
