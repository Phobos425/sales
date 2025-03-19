using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.db;

namespace project.Analysis
{
    /// <summary>
    /// класс, анализирующий по списку транзакций
    /// </summary>
    public static class Analisator
    {
        /// <summary>
        /// метод, делающий ABC анализ
        /// </summary>
        /// <param name="transactions">список транзакций</param>
        /// <returns>ABC анализ</returns>
        public static Dictionary<string, List<string>> ABCAnalysis(ref List<Transaction> transactions)
        {
            // создаем список с результатом
            var res = new Dictionary<string, List<string>>() 
            {
                { "A", new List<string>() },
                { "B", new List<string>() },
                { "C", new List<string>() }
            };

            var tr = transactions.Sum(x => x.Count * x.PricePerUnit); // общая сумма продаж
            double curRev = 0; // нынешний вклад в стоимость
            // выбираем товары по их названию
            var tmp = from el in transactions
                      group el by el.Name
                      into g
                      select new { Name = g.Key, Count = g.Sum(el => el.Count * el.PricePerUnit) };

            // определение категории товара
            foreach ( var el in tmp )
            {
                curRev += (el.Count / tr) * 100;
                if (curRev <= 80)
                {
                    res["A"].Add(el.Name);
                }
                else if (curRev <= 95)
                {
                    res["B"].Add(el.Name);
                }
                else
                {
                    res["C"].Add(el.Name);
                }
            }
            return res;
        }
        public static Dictionary<string, List<string>> XYZAnalysis(ref List<Transaction> transactions)
        {
            // создаем список с результатом
            var res = new Dictionary<string, List<string>>()
            {
                {"X", new List<string>() },
                {"Y", new List<string>() },
                {"Z", new List<string>() }
            };

            // выборка всех транзакций по названию товара
            var tmp = from el in transactions
                      group el by el.Name
                      into g
                      select new { Name = g.Key, Count = g.Select(el => (double)el.Count).ToList() };

            foreach ( var el in tmp )
            {
                double avg = el.Count.Average(); // Среднее арифметическое продаж
                double disp = el.Count.Sum(x => (x - avg) * (x - avg)) / el.Count.Count; // дисперсия
                double variation = avg != 0 ? Math.Sqrt(disp) * 100 / avg : 0; // коэффициент вариации в процентах
                if (variation <= 10)
                {
                    res["X"].Add(el.Name);
                }
                else if (variation <= 25)
                {
                    res["Y"].Add(el.Name);
                }
                else
                {
                    res["Z"].Add(el.Name);
                }
            }

            return res;
        }
    }
}
