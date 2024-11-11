using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    internal class WeatherService
    {
        internal WeatherService(string apiKey, double latitude, double longitude, int days)
        {
            Query = $"https://api.weather.yandex.ru/v2/forecast?lat={latitude.ToString(CultureInfo.InvariantCulture)}&lon={longitude.ToString(CultureInfo.InvariantCulture)}&lang=ru_RU&limit={days}&hours=false&extra=false";

            WeatherHttpClient.DefaultRequestHeaders.Clear();
            WeatherHttpClient.DefaultRequestHeaders.Add("X-Yandex-Weather-Key", apiKey);
        }

        private static readonly HttpClient WeatherHttpClient = new HttpClient();
        private string Query;

        private string? FormatRawWetherData(dynamic data)
        {
            if (data == null)
                return null;

            var fact = data.fact;

            return
                $"------------ |{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")} | Поток {Thread.CurrentThread.ManagedThreadId} | ------------ \r\n" +
                $"Текущая погода:\r\n" +
                $"Температура: {fact.temp}°C,\r\n" +
                $"Ощущается как: {fact.feels_like}°C,\r\n" +
                $"Облачность: {fact.cloudness * 100}%,\r\n" +
                $"Состояние: {fact.condition},\r\n" +
                $"Ветер: {fact.wind_speed} м/с (порывы до {fact.wind_gust} м/с) - {fact.wind_dir},\r\n" +
                $"Давление: {fact.pressure_mm} мм рт. ст.,\r\n" +
                $"Влажность: {fact.humidity}%,\r\n" +
                $"Видимость: {fact.visibility / 1000} км,\r\n" +
                $"Вероятность осадков: {fact.prec_prob}%\r\n";
        }
        internal async Task<string?> GetFormattedWeatherForecast()
        {
            return FormatRawWetherData(await GetRawWeatherAsync());
        }
        internal async Task<dynamic> GetRawWeatherAsync()
        {
            try
            {
                HttpResponseMessage response = await WeatherHttpClient.GetAsync(Query);
                if (response.IsSuccessStatusCode)
                {
                    string raw = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject(raw);
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Ошибка обращения к яндекс-погоде.\r\n" +
                    $"{ex.Message}\r\n" +
                    $"{ex.InnerException?.Message}");
                return null;
            }
        }
    }
}
