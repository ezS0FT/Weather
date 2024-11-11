using static Weather.GlobalData;

namespace Weather
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в метео-сервис.");
            try
            {
                App app = new App(new(GetApiKey(), 58.0, 56.2, 1));

                Console.WriteLine(
                    $"Выберите режим работы. Введите цифру от 1-3:\r\n" +
                    $"1- Прогноз погоды (интервал 10 сек.)\r\n" +
                    $"2- Запись в лог\r\n" +
                    $"3- Чтение из лога\r\n");
                int workMode = GetWorkMode();

                Console.WriteLine("Введите количество параллельных потоков:");
                int threads = GetThreads();

                app.Run(threads, workMode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Неизвестная ошибка работы приложения.\r\n" +
                    $"·Ex - {ex.Message}\r\n" +
                    $"·InnerEx - {ex.InnerException?.Message}\r\n" +
                    $"·TargetSite - {ex.TargetSite?.DeclaringType}\r\n" +
                    $"·StackTrace - {ex.StackTrace}");
            }
        }
        internal static string GetApiKey()
        {
            string? line;
            using (StreamReader file = new StreamReader(ApiKeyFilePath))
            {
                line = file.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    throw new Exception(
                        "В файле API_KEY.txt не найден API ключ.\r\n" +
                        "Получите ключ на сайте https://yandex.ru/pogoda/b2b/console/api-page\r\n" +
                        "И добавьте его в файл.");
                }
            }
            return line;
        }
        internal static int GetThreads()
        {
            string? line = Console.ReadLine();
            int result;
            if (!int.TryParse(line, out result) || result <= 0)
            {
                Console.WriteLine("Перезапустите приложение и введите корректное значение.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            return result;
        }
        internal static int GetWorkMode()
        {
            string? line = Console.ReadLine();
            int result;
            if (!int.TryParse(line, out result) || result <= 0)
            {
                Console.WriteLine("Перезапустите приложение и введите корректное значение.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            return result;
        }
    }
}
