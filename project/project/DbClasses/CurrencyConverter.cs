using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace project.DbClasses
{
    /// <summary>
    /// Класс для конвертации валют
    /// </summary>
    public static class CurrencyConverter
    {
        private static List<string> _currencies;
        /// <summary>
        /// метод, который использует API центробанка возвращает курс валюты
        /// </summary>
        /// <param name="dt">дата</param>
        /// <param name="currencyCode">код валюты</param>
        /// <returns>курс для конвертации валюты в рубли</returns>
        /// <exception cref="Exception">выбрасывается в случае ошибки конвертации</exception>
        private static async Task<double> GetExchangeRate(DateTime dt, string currencyCode)
        {
            if (currencyCode == "RUB")
            {
                return 1;
            }
            string day = dt.Day <= 9 ? $"0{dt.Day}" : dt.Day.ToString();
            string month = dt.Month <= 9 ? $"0{dt.Month}" : dt.Month.ToString();
            string date = $"{day}/{month}/{dt.Year}";
            // url для GET запроса
            string url = $"https://www.cbr.ru/scripts/XML_daily.asp?date_req={date}";

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url); // GET запрос

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Ошибка получения курса валюты");
            }

            // приводим ответ сервера в тип string
            string xml = await response.Content.ReadAsStringAsync();

            // парсим ответ
            XDocument doc = XDocument.Parse(xml);

            // находим нужную валюту по CharCode
            var currency = doc.Descendants("Valute")
                .FirstOrDefault(v => (string)v.Element("CharCode") == currencyCode);

            if (currency == null)
            {
                throw new Exception($"Валюты с кодом {currencyCode} не найдено.");
            }

            // Извлекаем курс из элемента <Value>
            string rateString = currency.Element("Value")?.Value; // Меняем запятую на точку
            double rate;
            if (double.TryParse(rateString, out rate))
            {
                return rate;
            }
            else
            {
                throw new Exception($"Не удалось распарсить курс для {currencyCode}.");
            }
        }
        /// <summary>
        /// перевод рубля в валюту
        /// </summary>
        /// <param name="count">сумма в рублях</param>
        /// <param name="currencyCode">код валюты</param>
        /// <param name="date">дата транзакции</param>
        /// <returns>сумма в валюте</returns>
        public static double RubToCur(double count, string currencyCode, DateTime date)
        {
            var rate = GetExchangeRate(date, currencyCode);
            return count / rate.Result;
        }
        /// <summary>
        /// переводz валюты в рубль
        /// </summary>
        /// <param name="count">сумма в валюте</param>
        /// <param name="currencyCode">код валюты</param>
        /// <param name="date">дата транзакции</param>
        /// <returns>сумма в рублях</returns>
        public static double CurToRub(double count, string currencyCode, DateTime date)
        {
            var rate = GetExchangeRate(date, currencyCode);
            return count * rate.Result;
        }
        /// <summary>
        /// проверка, что есть курс конвекртации с данной валюты
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public static bool IsValid(string currencyCode)
        {
            return _currencies.Contains(currencyCode);
        }
        /// <summary>
        /// получает список всех доступных кодов валют
        /// вызывается только при запуске программы
        /// </summary>
        public static async void GetAllCurrencyCodes()
        {
            string url = "https://www.cbr.ru/scripts/XML_valFull.asp";

            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            string xml = await response.Content.ReadAsStringAsync();

            XDocument doc = XDocument.Parse(xml);

            // Список всех кодов валют (CharCode)
            var currencyCodes = new List<string>();

            foreach (var currency in doc.Descendants("Item"))
            {
                var charCode = currency.Element("ISO_Char_Code")?.Value;
                if (!string.IsNullOrEmpty(charCode))
                {
                    currencyCodes.Add(charCode);
                }
            }
            currencyCodes.Add("RUB");
            _currencies = currencyCodes;
        }
    }
}