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
    /// Interaction logic for BrowsableFolderSelect.xaml
    /// </summary>
    public partial class BrowsableFolderSelect : UserControl
    {
        public string Tooltip { get; set; }
        public string Hint { get; set; }
        public bool isModified = false;

        public BrowsableFolderSelect()
        {
            InitializeComponent();

            this.DataContext = this;
            Tooltip = "";
            Hint = "";
        }

        private void Browse_Btn_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult r = fbd.ShowDialog();
            Loc.Text = fbd.SelectedPath;
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
