using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Weather
{
    internal static class Logger
    {
        internal static object LoggerLock = new object();
        internal enum LogLevel
        {
            Info,
            Warn,
            Error
        }
        /*internal static void WriteToLog(LogLevel logLevel, int threadId, string message)
        {
            lock (LoggerLock)
            {
                string logPath = $"{GlobalData.LogsDirectory}\\{threadId}.txt";

                if (!File.Exists(logPath))
                    File.Create(logPath);

                using (StreamWriter file = new StreamWriter(logPath, append: true))
                    file.WriteLine($"{logLevel} | {message}");
            }
        }*/
        internal static void WriteToLog(int threadId, string message)
        {
            lock (LoggerLock)
            {
                string logPath = $"{GlobalData.LogsDirectory}\\{threadId}.json";

                if (File.Exists(logPath))
                {
                    string existingJson = File.ReadAllText(logPath);

                    existingJson = existingJson.TrimEnd(']') + "," + message + "]";

                    File.WriteAllText(logPath, existingJson);
                }
                else
                {
                    string json = "[" + message + "]";
                    File.WriteAllText(logPath, json);
                }
            }
        }
        internal static string ReadLastLog(int threadId)
        {
            lock (LoggerLock)
            {
                string logPath = $"{GlobalData.LogsDirectory}\\{threadId}.json";

                if (File.Exists(logPath))
                {
                    string jsonContent = File.ReadAllText(logPath);

                    JArray jsonArray = JArray.Parse(jsonContent);

                    var lastLog = jsonArray.LastOrDefault();

                    return lastLog?.ToString() ?? "Нет логов";
                }
                else
                {
                    return "Файл логов отсутствует. Запустите в режиме логгирования.";
                }
            }
        }
    }
}
