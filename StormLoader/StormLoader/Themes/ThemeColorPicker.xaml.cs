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

namespace StormLoader.Themes
{
    /// <summary>
    /// Interaction logic for ThemeColorPicker.xaml
    /// </summary>
    public partial class ThemeColorPicker : UserControl
    {
        public SolidColorBrush pickedColor;
        public string reference { get; set; }
        ThemeManager manager;
        public ThemeColorPicker(string reference, SolidColorBrush c, ThemeManager manager)
        {
            InitializeComponent();
            this.DataContext = this;
            this.reference = reference;
            ColorLabel.Content = reference;
            ColorBtn.Background = (SolidColorBrush)Application.Current.Resources[reference];
            this.manager = manager;
            
        }

        private void ColorBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pickedColor = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                Application.Current.Resources[reference] = pickedColor;
                ColorBtn.Background = pickedColor;
                manager.currentTheme.SetColor(reference, pickedColor);
            }
        }

        public void UpdateColor()
        {
            ColorBtn.Background = (SolidColorBrush)(Application.Current.Resources[reference]);
        }
    }
}
