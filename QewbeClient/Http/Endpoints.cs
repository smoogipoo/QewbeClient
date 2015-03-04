using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.Http
{
    internal static class Endpoints
    {
        private const string DOMAIN = @"http://smgi.me/Sync/";
        private const string SERVICE = @"qewbe";

        private const string API_ENDPOINT = DOMAIN + @"api/API.php?req={0}&service=" + SERVICE;

        /// <summary>
        /// GET APIModel/CreateAccount
        /// <para>
        /// 0 - Username, 1 - PwdHash, 2 - Email
        /// </para>
        /// </summary>
        internal static readonly string CREATE_ACCOUNT = string.Format(API_ENDPOINT, @"CreateAccount") + @"&username={0}&password={1}&email={2}";

        /// <summary>
        /// GET APIModel/Login
        /// <para>Returns a token for further use of the account. Invalidated every 24hr.</para>
        /// <para>
        /// 0 - Username, 1 - PwdHash
        /// </para>
        /// </summary>
        internal static readonly string LOGIN = string.Format(API_ENDPOINT, @"Login") + @"&username={0}&password={1}";

        /// <summary>
        /// GET APIModel/Logout
        /// <para>
        /// 0 - Token
        /// </para>
        /// </summary>
        internal static readonly string LOGOUT = string.Format(API_ENDPOINT, @"Logout") + @"&token={0}";

        /// <summary>
        /// GET QewbeAPI/UploadFile
        /// <para>Returns the next unique filename.</para>
        /// <para>
        /// 0 - Token, 1 - Extension, 2 - Checksum, 3 - Mime type
        /// </para>
        /// </summary>
        internal static readonly string UPLOAD_FILE = string.Format(API_ENDPOINT, @"UploadFile") + @"&token={0}";

        /// <summary>
        /// GET QewbeAPI/RemoveFile
        /// <para>Removes a file.</para>
        /// <para>
        /// 0 - Token, 1 - Checksum
        /// </para>
        /// </summary>
        internal static readonly string REMOVE_FILE = string.Format(API_ENDPOINT, @"RemoveFile") + @"&token={0}&hash={1}";

        /// <summary>
        /// GET QewbeAPI/GetFiles
        /// <para>Returns all the files for the user.</para>
        /// <para>
        /// 0 - Token
        /// </para>
        /// </summary>
        internal static readonly string GET_FILES = string.Format(API_ENDPOINT, @"GetFiles") + @"&token={0}";
    }
}
