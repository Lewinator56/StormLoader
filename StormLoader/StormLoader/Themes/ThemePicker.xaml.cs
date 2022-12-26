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

namespace StormLoader.Themes
{
    /// <summary>
    /// Interaction logic for ThemePicker.xaml
    /// </summary>
    public partial class ThemePicker : UserControl
    {
        ThemeManager themeManager;
        string[] colors = { "background", "foreground", "primary", "secondary", "border-color", "text-dark", "text-light", "text-hint", "button-icon", "button-icon-dark" };
        
        Window w;
        public ThemePicker(ThemeManager tm, Window w)
        {
            InitializeComponent();
            this.themeManager = tm;
            foreach (var key in colors)
            {
                ThemeColorPicker tcp = new ThemeColorPicker(key, (SolidColorBrush)Application.Current.Resources[key], themeManager);
                ColorList.Children.Add(tcp);
            }

            this.w = w;
        }

        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Stormloader thene (*thm)|*.thm";
            ofd.InitialDirectory = Directory.GetCurrentDirectory().ToString() + "\\themes";

            Nullable<bool> r = ofd.ShowDialog();

            if (r == true)
            {
                themeManager.Load(ofd.FileName);
            }
            foreach (ThemeColorPicker tcp in ColorList.Children)
            {
                tcp.UpdateColor();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.SaveFileDialog opf = new Microsoft.Win32.SaveFileDialog();
            opf.Filter = "Stormloader theme (.thm)|*.thm";
            opf.InitialDirectory = Directory.GetCurrentDirectory().ToString() + "\\themes";
            Nullable<bool> r = opf.ShowDialog();
            

            if (r == true)
            {
                themeManager.Save(opf.FileName);
                themeManager.Save("./themes/current.thm");
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            themeManager.Save("./themes/current.thm");
            w.Close();
            
        }
    }
}
