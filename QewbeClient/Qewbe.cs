using QewbeClient.API;
using QewbeClient.API.Reply;
using QewbeClient.Config;
using QewbeClient.Helpers;
using QewbeClient.Http;
using QewbeClient.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QewbeClient
{
    internal static class Qewbe
    {
        private static SynchronizationContext mainContext;

        internal static ConfigManager Config;
        private static Thread workThread;

        internal static User ActiveUser;
        internal static UploadQueue UploadQueue = new UploadQueue();

        internal static OverlayForm OverlayForm;

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //.NET creates sync context after declaring out first form
            OverlayForm = new OverlayForm();
            mainContext = SynchronizationContext.Current;

            Application.Run(new StartupContext());
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
            mainContext.Post(delegate(object s) { action(); }, null);
        }

        internal static void Cleanup()
        {
            Config.Save();
            ActiveUser.Config.Save();
        }

        private class StartupContext : ApplicationContext
        {
            internal StartupContext()
            {
                mainContext = SynchronizationContext.Current;

                workThread = new Thread(update) { IsBackground = true };
                workThread.Start();

                Config = new ConfigManager(@"qewbe.cfg");
                string activeUser = Config.Read<string>(@"activeuser", string.Empty);
                string password = string.Empty;

                if (string.IsNullOrEmpty(activeUser))
                {
                    if (new LoginCreateAccontForm().ShowDialog() != DialogResult.OK)
                        Thread.CurrentThread.Abort();
                }
                else
                    ActiveUser = new User(activeUser);
                Cleanup();

                UploadQueue.UploadSucceeded += delegate
                {
                    using (System.Media.SoundPlayer player = new System.Media.SoundPlayer(Resources.Ding))
                        player.Play();
                };
            }
        }
    }
}
