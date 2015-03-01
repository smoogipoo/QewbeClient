using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.Config
{
    internal class ConfigManager
    {
        private string file;

        internal ConfigManager(string filename)
        {
            file = filename;
        }

        internal T Read<T>(string key)
        {
            return default(T);
        }

        internal void Write<T>(string key, T value)
        {

        }
    }
}
