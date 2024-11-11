using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Weather.GlobalData;

namespace Weather
{
    internal class App
    {
        internal WeatherService WeatherService;
        List<Task> Tasks = new List<Task>();

        internal App(WeatherService ws) => WeatherService = ws;

        internal void Run(int threadsAmount, int workMode)
        {
            using (CancellationTokenSource cancellationTokenSource = new())
            {
                CancellationToken token = cancellationTokenSource.Token;

                for (int i = 0; i < threadsAmount; i++)
                {
                    int localI = i;
                    switch (workMode)
                    {
                        case 1: 
                            Tasks.Add(Task.Run(() => Forecast(localI, token)));
                            break;
                        case 2:
                            Tasks.Add(Task.Run(() => WriteLog(localI)));
                            break;
                        case 3:
                            Tasks.Add(Task.Run(() => ReadLog(localI)));
                            break;
                        default:
                            Console.WriteLine("Такого режима работы нет.");
                            break;
                    }
                }

                Task.Run(() =>
                {
                    Console.WriteLine("Нажмите любую клавишу для отмены...");
                    Console.ReadKey();
                    cancellationTokenSource.Cancel();
                    Console.WriteLine("Завершамем задачи..");
                });

                var task = Task.WhenAll(Tasks);
                task.Wait();
                Console.WriteLine("Все задачи завершены или отменены.");
            }
        }
        private async Task Forecast(int taskId, CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"Задача {taskId} завершена.");
                    return;
                }

                string message;
                string cache = SimpleMemoryCache.GetFromCache($"query:{taskId}");
                if(cache == null)
                {
                    message = await WeatherService.GetFormattedWeatherForecast();
                    if (message == null)
                    {
                        await Task.Delay(10000);
                        continue;
                    }
                    SimpleMemoryCache.AddToCache($"query:{taskId}", message);
                    message += " ADDED TO CACHE (60 sec refresh)";

                }
                else
                {
                    message = SimpleMemoryCache.GetFromCache($"query:{taskId}");
                    message += " FROM CACHE";
                }

                Console.WriteLine(message);
                await Task.Delay(10000);
            }
        }
        private async Task WriteLog(int taskId)
        {
            dynamic raw = await WeatherService.GetRawWeatherAsync();
            Console.WriteLine(raw.fact.ToString());
            Logger.WriteToLog(taskId, raw.fact.ToString());
            Console.WriteLine($"Задача {taskId} записала в лог.");
        }
        private async Task ReadLog(int taskId)
        {
            string log = Logger.ReadLastLog(taskId);
            Console.WriteLine(log);
        }
    }
}
