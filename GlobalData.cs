using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    internal static class GlobalData
    {
        internal static object GlobalLock = new object();
        internal static string AppPath = Directory.GetCurrentDirectory();
        internal static string ApiKeyFilePath
        {
            get
            {
                string path = $"{AppPath}\\API_KEY.txt";

                if (!File.Exists(path))
                    File.Create(path);

                return path;
            }
        }
        internal static string LogsDirectory
        {
            get
            {
                lock (GlobalLock)
                {
                    string dir = $"{AppPath}\\Логи";

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    return dir;
                }
            }
        }
    }
}
