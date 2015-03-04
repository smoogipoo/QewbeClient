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
    public partial class CreateAccountForm : Form
    {
        public CreateAccountForm()
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
            //Todo: Prompt for account creation
            HttpClient.SendRequest(new NetRequest(Endpoints.CREATE_ACCOUNT, delegate(object r)
            {
                CreateAccountReply reply = Serializer.Deserialize<CreateAccountReply>(r.ToString());

                //Todo: Probably don't throw here...
                if (!reply.OK)
                    throw new Exception(reply.Response.ToString());

                Qewbe.Config.Write<string>(@"activeuser", usernameBox.Text);
                Qewbe.ActiveUser = new User(usernameBox.Text, passwordBox.Text);

                DialogResult = DialogResult.OK;
                Qewbe.RunMainThread(delegate { Close(); });
            }, usernameBox.Text, passwordBox.Text, emailBox.Text));
        }
    }
}
