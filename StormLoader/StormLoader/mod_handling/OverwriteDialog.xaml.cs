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

namespace StormLoader.mod_handling
{
    /// <summary>
    /// Interaction logic for OverwriteDialog.xaml
    /// </summary>
    public partial class OverwriteDialog : UserControl
    {
        public string message { get; set; }
        private  int overwrite;
        public OverwriteDialog(ref int overwrite, string message)
        {
            
            InitializeComponent();
            this.message = message;
            this.overwrite = overwrite;
            this.DataContext = this;
        }

        private void Option_Btn_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            DbgLog.WriteLine(b.Name);
            switch (b.Name)
            {
                case "No":
                    overwrite = 0;
                    break;
                case "NoToAll":
                    overwrite = 1;
                    break;
                case "Yes":
                    overwrite = 2;
                    break;
                case "YesToAll":
                    overwrite = 3;
                    break;
                case null:
                    overwrite = 0;
                    break;
            }
            Window parent = Window.GetWindow(this);
            parent.Close();
            
        }

        public int GetOverwrite()
        {
            return overwrite;
        }
    }
}
