using QewbeClient.API;
using QewbeClient.API.Reply;
using QewbeClient.Config;
using QewbeClient.Helpers;
using QewbeClient.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QewbeClient
{
    internal static class Qewbe
    {
        private static SynchronizationContext mainContext;

        private static ConfigManager config;
        private static Thread workThread;

        internal static User ActiveUser;
        internal static UploadQueue UploadQueue = new UploadQueue();

        internal static OverlayForm OverlayForm;

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            OverlayForm = new OverlayForm();

            startup();

            mainContext = SynchronizationContext.Current;

            Application.Run();
        }

        private static void startup()
        {
            workThread = new Thread(update) { IsBackground = true };
            workThread.Start();

            config = new ConfigManager(@"qewbe.cfg");
            string activeUser = config.Read<string>(@"activeuser", string.Empty);
            string password = string.Empty;

            if (string.IsNullOrEmpty(activeUser))
            {
                if (new LoginCreateAccount().ShowDialog() != DialogResult.OK)
                    Application.Exit();

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

        internal static void SwitchUser()
        {
            ActiveUser.Logout();

            //Todo: Implement
        }

        private static void update()
        {
            while (true)
            {
                if (ActiveUser != null && ActiveUser.IsLoggedIn)
                    UploadQueue.Update();

                HttpClient.Update();

                Thread.Sleep(20);
            }
        }

        internal static void RunMainThread(Action action)
        {
            SynchronizationContext.SetSynchronizationContext(mainContext);
            mainContext.Send(new SendOrPostCallback(delegate(object o) { action.Invoke(); }), null);
        }

        internal static void Cleanup()
        {
            config.Save();
            ActiveUser.Config.Save();
        }
    }
}
