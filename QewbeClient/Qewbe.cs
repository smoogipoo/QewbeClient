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
    public partial class Qewbe : Form
    {
        private ConfigManager config;
        private Thread workThread;

        internal User ActiveUser;
        internal static UploadQueue UploadQueue = new UploadQueue();

        public Qewbe()
        {
            InitializeComponent();
        }

        private void Qewbe_Load(object sender, EventArgs e)
        {
            workThread = new Thread(update) { IsBackground = true };
            workThread.Start();

            config = new ConfigManager(@"qewbe.cfg");
            string activeUser = config.Read<string>(@"activeuser", string.Empty);
            string password = "test2";
            if (string.IsNullOrEmpty(activeUser))
            {
                //Todo: Prompt for account creation
                HttpClient.SendRequest(new NetRequest(Endpoints.CREATE_ACCOUNT, delegate(object r)
                {
                    CreateAccountReply reply = Serializer.Deserialize<CreateAccountReply>(r.ToString());
                    if (!reply.OK)
                        throw new Exception(reply.Response.ToString());
                    //Todo: Write from login form
                    activeUser = reply.Username;


                    config.Write(@"activeuser", activeUser);

                    //Todo: Remember to change the following 2 lines
                    ActiveUser = new User(activeUser, password);
                }, "test10", password, "test@test.com"));
            }
            else
                ActiveUser = new User(activeUser);
        }

        private void switchUser()
        {
            ActiveUser.Logout();

            //Todo: Implement
        }

        private void update()
        {
            while (true)
            {
                if (ActiveUser != null && ActiveUser.IsLoggedIn)
                    UploadQueue.Update();

                HttpClient.Update();

                Thread.Sleep(20);
            }
        }

        internal void Cleanup()
        {
            config.Save();
            ActiveUser.Config.Save();
        }

        private void Qewbe_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cleanup();
        }
    }
}
