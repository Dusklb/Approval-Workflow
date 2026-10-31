using DevExpress.XtraEditors;
using System;
using System.Configuration;
using System.Linq;

namespace Trinhky.Win.Editors
{
    public partial class ServerForm : DevExpress.XtraEditors.XtraForm
    {
        //private string eKey = "TrungMinh@2021";
        public ServerForm()
        {
            InitializeComponent();
            string ModeStr = tmLib.CommonLib.GetSetting("UseNet");
            int idx = tmLib.CommonLib.CInt(ModeStr);
            cboMode.SelectedIndex = idx;

        }

        private void SimpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SimpleButton1_Click(object sender, EventArgs e)
        {
            string encryptcn;
            string datasource = textEdit1.Text;
            string catalogue = textEdit3.Text;
            string tk = textEdit4.Text;
            string pass = textEdit5.Text;
            int idx = cboMode.SelectedIndex;
            if (idx == 2)
            {
                encryptcn = catalogue;
            }
            else
            {
                string cn = "Data Source=xxx;Initial Catalog=yyy;User ID=zzz;Password=ppp";
                cn = cn.Replace("xxx", datasource);
                cn = cn.Replace("yyy", catalogue);
                cn = cn.Replace("zzz", tk);
                cn = cn.Replace("ppp", pass);
                if (idx < 2)
                    encryptcn = tmLib.CommonLib.Encrypt(cn);
                else
                    encryptcn = cn;
            }

            tmLib.CommonLib.SetSetting("UseNet", idx.ToString());

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            if (idx == 0)
                connectionStringsSection.ConnectionStrings["ConnectionString"].ConnectionString = encryptcn;
            else if (idx == 1)
                connectionStringsSection.ConnectionStrings["LocalNet"].ConnectionString = encryptcn;
            else if (idx == 2)
                connectionStringsSection.ConnectionStrings["LocalDB"].ConnectionString = encryptcn;
            else
                connectionStringsSection.ConnectionStrings["DevMode"].ConnectionString = encryptcn;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");

            XtraMessageBox.Show("Vui lòng thoát khỏi đăng nhập hệ thống và chạy lại ứng dụng");
            this.Close();
        }

        private void ParseConnection(string conn, int index)
        {
            if (index == 2)
            {
                textEdit1.Text = "";
                textEdit3.Text = conn;
                textEdit4.Text = "";
                textEdit5.Text = "";
            }
            else
            {
                string datasource = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(conn).GetPartByName("Data Source");
                string catalogue = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(conn).GetPartByName("Initial Catalog");
                string tk = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(conn).GetPartByName("User ID");
                string pass = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(conn).GetPartByName("Password");

                textEdit1.Text = datasource;
                textEdit3.Text = catalogue;
                textEdit4.Text = tk;
                textEdit5.Text = pass;
            }
        }

        private void CboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBoxEdit edit)
            {
                int idx = edit.SelectedIndex;
                string cnnStr = tmLib.CommonLib.GetConnectionByMode(idx.ToString());
                ParseConnection(cnnStr, idx);
            }
        }
    }
}