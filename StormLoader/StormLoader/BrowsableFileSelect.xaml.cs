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
    /// Interaction logic for BrowsableFileSelect.xaml
    /// </summary>
    public partial class BrowsableFileSelect : UserControl
    {

        public string Tooltip { get; set; }
        public string Hint { get; set; }
        public bool isModified = false;
        public string fileFilter { get; set; }

        public BrowsableFileSelect()
        {
            InitializeComponent();
            this.DataContext = this;
            Tooltip = "";
            Hint = "";
            fileFilter = "";
        }

        


        private void Browse_Btn_Click(object sender, EventArgs e)
        {
            Microsoft.Win32.OpenFileDialog opf = new Microsoft.Win32.OpenFileDialog();
            opf.Filter = fileFilter;
            Nullable<bool> r = opf.ShowDialog();

            if (r== true)
            {
                Loc.Text = opf.FileName;
            }
        }

        public string GetLocation()
        {
            return Loc.Text;
        }

        private void Loc_TextChanged(object sender, TextChangedEventArgs e)
        {
            isModified = true;
        }

        public void setText(string text)
        {
            Loc.Text = text;
        }
    }
}
