using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QewbeClient.Config
{
    internal class ConfigManager
    {
        private string file;

        private Dictionary<string, object> properties = new Dictionary<string, object>();

        internal ConfigManager(string filename)
        {
            file = filename;

            using (StreamReader sr = new StreamReader(new FileStream(file, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)))
            {
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (line.Contains(':'))
                    {
                        string key = line.Substring(0, line.IndexOf(':'));
                        string val = line.Substring(line.IndexOf(':') + 1);
                        properties.Add(key, val);
                    }
                    else
                        properties.Add(line, null);
                }
            }
        }

        internal T Read<T>(string key, object defaultValue)
        {
            object value;
            if (properties.TryGetValue(key, out value))
                return (T)value;
            properties.Add(key, defaultValue);
            return (T)defaultValue;
        }

        internal void Write<T>(string key, T value)
        {
            properties[key] = value;
        }

        internal void Save()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, object> kvp in properties)
                sb.AppendLine(string.Format("{0}:{1}", kvp.Key, kvp.Value.ToString()));
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.AutoFlush = true;
                sw.Write(sb.ToString());
            }
        }
    }
}
