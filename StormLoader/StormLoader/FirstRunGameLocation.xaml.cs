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
    /// Interaction logic for FirstRunGameLocation.xaml
    /// </summary>
    public partial class FirstRunGameLocation : UserControl
    {
        public FirstRunGameLocation()
        {
            InitializeComponent();
        }

        private void InsLoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            Next_Btn.IsEnabled = true;
        }

    }
}
