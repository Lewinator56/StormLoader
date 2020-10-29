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



namespace StormLoader.repository
{
    /// <summary>
    /// Interaction logic for RepoModListItem.xaml
    /// </summary>
    public partial class RepoModListItem : UserControl
    {
        int id;
        public RepoModListItem(string name, string author, string version, string description, byte[] image, int id)
        {
            InitializeComponent();
            modName.Content = name;
            Author.Content += author;
            Version.Content += version;
            Description.Text = description;
            this.id = id;

            var bitmap = (BitmapSource)new ImageSourceConverter().ConvertFrom(image);
            ModImage.Source = bitmap;



        }
    }
}
