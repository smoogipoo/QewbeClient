using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QewbeClient.Http;
using QewbeClient.Config;
using QewbeClient.API;
using System.Threading;
using QewbeClient.API.Reply;

namespace QewbeClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Qewbe.ActiveUser = new User(usernameBox.Text, passwordBox.Text);
            Close();
        }
    }
}
