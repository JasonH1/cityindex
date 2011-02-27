using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CIMarketPriceFinder2007
{
    public partial class frmLogin : Form
    {
        private String _Username;
        private String _Password;
        private Boolean _GetLogin = false;
        public frmLogin()
        {
            InitializeComponent();
            textBoxUsername.Select();
        }

        public Boolean GetLogin
        {
            get { return _GetLogin; }
        }

        public String Username {
            get { return _Username; }
        }

        public String Password {
            get { return _Password; }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            _GetLogin = true;
            _Username = textBoxUsername.Text;
            _Password = textBoxPassword.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _GetLogin = false;
            this.Close();
        }        
    }
}
