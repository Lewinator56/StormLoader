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

namespace StormLoader.repository
{
    /// <summary>
    /// Interaction logic for RepoBrowserRoot.xaml
    /// </summary>
    public partial class RepoBrowserRoot : Window
    {
        public RepoBrowserRoot()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                RepoModListItem rpml = new RepoModListItem();
                ModList.Children.Add(rpml);
            }
        }

        private void UploadMod_Click(object sender, RoutedEventArgs e)
        {
            RepoLoginPanel rplp = new RepoLoginPanel();
            RepoDialog.ShowDialog(rplp);
        }


    }
}
