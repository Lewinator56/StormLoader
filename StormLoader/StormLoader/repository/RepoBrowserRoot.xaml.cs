using MaterialDesignThemes.Wpf;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
        SQLManager sqcm;
        public RepoBrowserRoot()
        {
            InitializeComponent();
            this.sqcm = new SQLManager();
            sqcm.connect(GlobalVar.server, GlobalVar.database, GlobalVar.user, GlobalVar.password, GlobalVar.port);
            GlobalVar.sqcm = sqcm;
            refreshMods();
        }

        void refreshMods() {
            string searchterm = SearchTxbx.Text;
            bool verified = (bool)VerifiedOnly.IsChecked;
            ModList.Children.Clear();
            DataTable dt = sqcm.getModListWithoutData(searchterm, verified);
            foreach (DataRow rmds in dt.Rows)
            {
                int mod_id = (int)rmds["mod_id"];
                addModListItem(mod_id);
                //Thread t = new Thread(() => addModListItem(mod_id));
                //t.SetApartmentState(ApartmentState.STA);
                //t.Start();

                
                

            }
        }

        public void addModListItem(int mod_id)
        {
            DataTable dtm = (DataTable)sqcm.getModDataByID(mod_id);
            foreach (DataRow r in dtm.Rows)
            {
                RepoModListItem li = new RepoModListItem(r["mod_name"].ToString(), r["user_name"].ToString(), r["mod_version"].ToString(), r["mod_description"].ToString(), (byte[])r["mod_data_image"], Convert.ToInt32(r["mod_id"]), Convert.ToInt32(r["mod_smf_verified"]), this);
                if (r["mod_details_path"].ToString() != "")
                {
                    li.enableInfoButton(r["mod_details_path"].ToString());
                }
                ModList.Children.Add(li);

                //li.Dispatcher.BeginInvoke((Action)(() => { ModList.Children.Add(li); }));
            }
        }

        private void UploadMod_Click(object sender, RoutedEventArgs e)
        {
            RepoLoginPanel rplp = new RepoLoginPanel();
            RepoDialog.ShowDialog(rplp);
        }

        private void refreshListing_Click(object sender, RoutedEventArgs e)
        {
            refreshMods();
        }
    }
}
