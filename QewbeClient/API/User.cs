using QewbeClient.Config;
using QewbeClient.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using QewbeClient.API.Reply;

namespace QewbeClient.API
{
    internal class User
    {
        internal ConfigManager Config { get; private set; }

        internal bool IsLoggedIn { get; private set; }

        internal string Username { get; private set; }
        internal static string Token { get; private set; }

        internal User(string username, string newPassword = "")
        {
            Config = new ConfigManager(string.Format(@"qewbe.{0}.cfg", username));
            Username = username;
            Token = Config.Read<string>(@"password", newPassword);

            Login();
        }

        internal void Login()
        {
            if (IsLoggedIn)
                return;

            HttpClient.SendRequest(new NetRequest(Endpoints.LOGIN, delegate(object r)
            {
                LoginReply reply = Serializer.Deserialize<LoginReply>(r.ToString());
                if (!reply.OK)
                    throw new Exception(reply.Response.ToString()); //Todo: Change this
                Token = reply.Token;
                IsLoggedIn = true;
                Qewbe.RunMainThread(delegate { Qewbe.OverlayForm.Show(); });
            }, Username, Token));
        }

        internal void Logout()
        {
            if (!IsLoggedIn)
                return;

            HttpClient.SendRequest(new NetRequest(Endpoints.LOGOUT, delegate(object r)
            {
                IsLoggedIn = false;
                Qewbe.RunMainThread(delegate { Qewbe.OverlayForm.Hide(); });
            }, Token));
        }

        internal void RemoveFile(string file)
        {
            if (!IsLoggedIn)
                return;

            HttpClient.SendRequest(new NetRequest(Endpoints.REMOVE_FILE, delegate(object r)
            {
                ClientReply reply = Serializer.Deserialize<ClientReply>(r.ToString());
                if (!reply.OK)
                    return;
                //Todo: Remove from list here
            }, Token, file));
        }
    }
}
