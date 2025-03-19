﻿using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.db;
using System.Globalization;
using project.edit;
using System.Security.Cryptography.X509Certificates;
using project.DbClasses;

namespace project.ConsoleHandler
{
    /// <summary>
    /// класс для вывода в консоль
    /// </summary>
    public static class ConsoleHandler
    {
        /// <summary>
        /// вывод изначального меню
        /// </summary>
        public static void PrintInitialMenu()
        {
            Console.WriteLine("1. Загрузить данные из файла");
            Console.WriteLine("Esc. Выход");
        }
        /// <summary>
        /// вывод обычного меню
        /// </summary>
        public static void PrintMenu()
        {
            Console.WriteLine("1. Загрузить данные из файла");
            Console.WriteLine("2. Просмотр всех транзакций");
            Console.WriteLine("3. Добавление новой транзакции");
            Console.WriteLine("4. Удаление транзакции");
            Console.WriteLine("5. Редактирование транзакции");
            Console.WriteLine("6. Вывести статистику по регионам");
            Console.WriteLine("7. Вывести сумму всех транзакций");
            Console.WriteLine("8. Сохранение данных");
            Console.WriteLine("Esc. Выход");
        }
        public static void PrintRegionInfo(ref List<Transaction> transactions)
        {
            int pageSize = 5;  // Количество строк на экране
            int shift = 0;     // Смещение (прокрутка)
            // с помощью Linq запроса к transactions получаем сумму покупок по регионам
            var res = from el in transactions
                      group el by el.Region
                      into g
                      select new { Region = g.Key, Count= g.Sum(el => el.Count * el.PricePerUnit) };
            res = res.OrderByDescending(el => el.Count); // сортируем результат по сумме транзакций
            sbyte sort = 0;
            var sorted = res;
            while (true)
            {
                Console.Clear();

                // сортировка данных
                if (sort == 1)
                {
                    sorted = sorted.OrderBy(el => el.Region);
                } else if (sort == -1)
                {
                    sorted = sorted.OrderByDescending(el => el.Region);
                } else
                {
                    sorted = res;
                }

                // инициализация таблицы
                var table = new Table().AddColumn(new TableColumn("Регион")).AddColumn(new TableColumn("Сумма продаж"));
                // добавление строк
                foreach (var el in sorted.Skip(shift).Take(pageSize))
                {
                    table.AddRow(el.Region.ToString(), $"{el.Count:f2}");
                }
                AnsiConsole.Render(table);

                Console.WriteLine("\nИспользуйте ↑ ↓ для прокрутки, 2 - Сортировка во возрастанию," +
                    " 5 - убрать сортировку, 8 - сортировка по убыванию, Esc - выход");

                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow && shift + pageSize < sorted.Count()) { ++shift; }
                else if (key == ConsoleKey.UpArrow && shift > 0) { --shift; }
                else if (key == ConsoleKey.Escape) { break; }
                else if (key == ConsoleKey.D2)
                {
                    sort = 1;
                    shift = 0;
                }
                else if (key == ConsoleKey.D5)
                {
                    sort = 0;
                    shift = 0;
                }
                else if (key == ConsoleKey.D8)
                {
                    sort = -1;
                    shift = 0;
                }
            }
        }
        /// <summary>
        /// вывод информации о транзакциях
        /// </summary>
        /// <param name="transactions">список покупок</param>
        public static void PrintInfo(ref List<Transaction> transactions)
        {
            Console.WriteLine("1. Вывести информацию таблицей");
            Console.WriteLine("2. Вывести информацию гистограммой");
            Console.WriteLine("3. Вывести информацию с помощью break down chart");
            Console.WriteLine("Нажмите любую кнопку, чтобы вернуться на главный экран");
            var key = Console.ReadKey().Key;
            Console.Clear();
            switch(key)
            {
                case ConsoleKey.D1:
                    PrintTable(ref transactions);
                    break;
                case ConsoleKey.D2:
                    PrintBarChart(ref transactions);
                    break;
                case ConsoleKey.D3:
                    PrintBreakdownChart(ref transactions);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// вывод информации о покупках таблицей
        /// </summary>
        /// <param name="transactions">список покупок</param>
        private static void PrintTable(ref List<Transaction> transactions)
        {
            int pageSize = 5;  // Количество строк на экране
            int shift = 0;     // Смещение (прокрутка)
            byte filter = 0;   // filter равен региону, по которому надо фильтровать
            sbyte sort = 0;    // sort = 1 - по возрастанию sort = -1 - по убыванию
            while (true)
            {
                Console.Clear();

                IEnumerable<Transaction> filtered = transactions;
                //фильтрация
                if (filter != 0)
                {
                    filtered = from el in transactions
                               where el.Region == filter
                               select el;
                }
                //сортировка
                if (sort == 1)
                {
                    filtered = filtered.OrderBy(x => x.Date);
                } else if (sort == -1)
                {
                    filtered = filtered.OrderByDescending(x => x.Date);
                }

                var table = new Table();

                // имена столбцов
                string[] names = ["ID", "Дата", "ID продукта", "название", "количество", "цена/шт в рублях",
                    "цена/шт в валюте", "валюта", "Region"];

                // добавление столбцов
                for (int i = 0; i < names.Length; i++)
                {
                    table.AddColumn(new TableColumn(names[i]).Centered());
                }
                
                // добавление строк
                foreach (var trans in filtered.Skip(shift).Take(pageSize))
                {
                    string[] item = trans.ToString().Split(";");
                    table.AddRow(item);
                }
                AnsiConsole.Render(table);

                Console.WriteLine(filter != 0 ?$"\nФильтр: {filter}" : "Фильтр не установлен");
                Console.WriteLine("\nИспользуйте ↑ ↓ для прокрутки, F - изменить(установить) фильтр," +
                    " 2 - Сортировка во возрастанию, 5 - убрать сортировку, 8 - сортировка по убыванию, Esc - выход");

                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow && shift + pageSize < filtered.Count()) { ++shift; }
                else if (key == ConsoleKey.UpArrow && shift > 0) { --shift; }
                else if (key == ConsoleKey.Escape) { break; }
                else if (key == ConsoleKey.F)
                {
                    Console.WriteLine("\nВведите фильтр");
                    try
                    {
                        byte tmp = byte.Parse(Console.ReadLine());
                        if (tmp > 89)
                        {
                            throw new Exception();
                        }
                        filter = tmp;
                    }
                    catch {}
                    shift = 0;
                }
                else if (key == ConsoleKey.D2)
                {
                    sort = 1;
                    shift = 0;
                }
                else if (key == ConsoleKey.D5)
                {
                    sort = 0;
                    shift = 0;
                } else if (key == ConsoleKey.D8)
                {
                    sort = -1;
                    shift = 0;
                }
            }
        }
        /// <summary>
        /// ввод даты для отображения информации о транзакциях
        /// </summary>
        /// <param name="s1">начало или конец</param>
        /// <returns>дату</returns>
        private static DateTime InputDate(string s1="")
        {
            DateTime dt;
            while (true)
            {
                Console.WriteLine($"Введите дату {s1} показа информации или -1, чтобы вернуться на главный экран");
                string s = Console.ReadLine();
                Console.Clear();
                if (s == "-1") { return DateTime.MinValue; }
                try
                {
                    dt = DateTime.Parse(s);
                    if (dt.Date > DateTime.Now.Date)
                    {
                        throw new Exception();
                    }
                    return dt;
                }
                catch
                {
                    Console.WriteLine("Неверная дата");
                }
            }
        }
        /// <summary>
        /// Вывод информации гистограммой
        /// </summary>
        /// <param name="transactions">список транзакций</param>
        private static void PrintBarChart(ref List<Transaction> transactions)
        {
            DateTime dt1, dt2;
            dt1 = InputDate("начала");
            dt2 = InputDate("конца");
            if (dt1 > dt2)
            {
                Console.WriteLine("Дата конца позже даты начала");
                return;
            }
            int pageSize = 5;  // Количество строк на экране
            int shift = 0;     // Смещение (прокрутка)
            Dictionary<DateTime, int> dict = new Dictionary<DateTime, int>(); //ключ - дата, значение сумма транзакций
            while (true)
            {
                var dtStart = dt1.AddDays(shift);
                var dtEnd = dtStart.AddDays(Math.Min(pageSize - 1, (dt2 - dtStart).Days));
                // выборака транзакций по дате входящей в [dt1; dt2]
                for (int i = 0; i < Math.Min(pageSize, (dtEnd - dtStart).Days + 1); i++)
                {
                    DateTime tmp = dtStart.AddDays(i);
                    if (!dict.ContainsKey(tmp))
                    {
                        int res = (int)transactions.Where(x => x.Date.Date == tmp.Date).Sum(x => x.Count * x.PricePerUnit); // поиск суммы по дате
                        dict.Add(tmp, res);
                    }
                }
                // вывод гистограммы
                var chart = new BarChart().Width(60).Label("Сумма продаж по дням за выбранный переод").CenterLabel();
                for (int i = 0; i < Math.Min(pageSize, (dtEnd - dtStart).Days + 1); ++i)
                {
                    chart.AddItem(dtStart.AddDays(i).Date.ToString(new CultureInfo("ru-RU"))[0..10], dict[dtStart.AddDays(i)]);
                }
                AnsiConsole.Write(chart);

                Console.WriteLine("\nИспользуйте ↑ ↓ для прокрутки, Esc - выход");

                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow && dt1.AddDays(shift) < dt2) { ++shift; }
                else if (key == ConsoleKey.UpArrow && shift > 0) { --shift; }
                else if (key == ConsoleKey.Escape) { break; }
                Console.Clear();
            }
        }
        /// <summary>
        /// выводит информацию с помощью breakdown chart'а
        /// </summary>
        /// <param name="transactions">список транзакций</param>
        private static void PrintBreakdownChart(ref List<Transaction> transactions)
        {
            DateTime dt1, dt2;
            dt1 = InputDate("начала");
            dt2 = InputDate("конца");
            if (dt1 > dt2)
            {
                Console.WriteLine("Дата конца позже даты начала");
                return;
            }

            var res = from el in transactions
                      where el.Date >= dt1 && el.Date <= dt2
                      group el by el.Name
                      into g
                      select new {Name = g.Key, Count = g.Sum(el => el.Count) };

            var chart = new BreakdownChart().Width(60);
            int i = 0;
            foreach ( var el in res)
            {
                chart.AddItem(el.Name, el.Count, i);
                i = (i + 1) % 256;
            }
            AnsiConsole.Write(chart);
            
        }
        /// <summary>
        /// Выводит сумму всех транзакций в выбранной валюте
        /// </summary>
        /// <param name="transactions">список транзакций</param>
        public static void PrintAllSales(ref List<Transaction> transactions)
        {
            Console.WriteLine("Введите валюту, сумму продаж в которой хотите посмотреть");
            Console.WriteLine("Чтобы вернуться в главное меню, введите -1");
            
            string currency = Console.ReadLine();
            if (currency == "-1")
            {
                return;
            }

            DateTime dt = DateTime.MinValue;
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("1. Вывести сумму по курсу на дату транзакции");
                Console.WriteLine("2. Вывести сумму по текущему курсу");
                Console.WriteLine("Esc. Вернуться в главное меню");

                var key = Console.ReadKey().Key;
                Console.Clear();
                switch (key)
                {
                    case ConsoleKey.D1:
                        flag = false;
                        break;
                    case ConsoleKey.D2:
                        dt = DateTime.Now;
                        flag = false;
                        break;
                    case ConsoleKey.Escape:
                        return;
                    default:
                        Console.WriteLine("Неверная кнопка");
                        break;
                }
            }
            try
            {
                var res = TransactionsHandler.CalculateAllSales(ref transactions, dt, currency);
                Console.WriteLine($"Сумма продаж в валюте {currency} равна {res:f2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Вывод ошибки при добавлении транзакции
        /// </summary>
        public static void PrintAddError()
        {
            Console.WriteLine("Неверная запись транзакции");
            Console.WriteLine("Чтобы вернуться в главное меню, введите -1");
        }
        // вывод ошибки при удалении транзакции
        public static void PrintEditError()
        {
            Console.WriteLine("Неверное название. Повторите попытку");
            Console.WriteLine("Чтобы вернуться в главное меню, введите -1");
        }
        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        /// <param name="transactions"></param>
        /// <returns>смогла ли программа загрузить данные</returns>
        public static bool DownloadData(ref List<Transaction> transactions)
        {
            Console.WriteLine("Введите название файла");
            string path = @"..\..\..\db\" + Console.ReadLine();
            // проверка существования файла
            try
            {
                FileHandler.CurrentPath = path;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            // попытка выгрузить данные из файла
            try
            {
                FileHandler.DownloadData(ref transactions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// Добавление транзакции
        /// </summary>
        /// <param name="data">список транзакций</param>
        /// <exception cref="ArgumentException">выбрасывается в случае неверно введенной транзакции</exception>
        public static void Add(ref List<Transaction> data)
        {
            // дальше идут блоки инициализации траназакции, в случае ввода -1, пользователя возвращает в главное меню

            // дата
            Console.WriteLine("Введите дату создания транзакции");
            Console.WriteLine("Чтобы вернуться в главное меню, введите -1");
            DateTime dt;
            string s = Console.ReadLine();
            if (s == "-1") {return; }
            bool f = DateTime.TryParse(s, out dt);
            while (!f)
            {
                if (s == "-1") { return; }
                PrintAddError();
                s = Console.ReadLine();
                f = DateTime.TryParse(s, out dt);
            }

            // id товара
            Console.WriteLine("Введите id товара");
            uint prodId;
            s = Console.ReadLine();
            if (s == "-1") { return; }
            f = uint.TryParse(s, out prodId);
            while (!f)
            {
                if (s == "-1") { return; }
                PrintAddError();
                s = Console.ReadLine();
                f = uint.TryParse(s, out prodId);
            }

            // название
            Console.WriteLine("Введите название товара");
            string name = Console.ReadLine();
            if (s == "-1") { return; }

            // количество
            uint count;
            Console.WriteLine("Введите количество товара");
            s = Console.ReadLine();
            if (s == "-1") { return; }
            f = uint.TryParse(s, out count);
            while (!f)
            {
                if (s == "-1") { return; }
                PrintAddError();
                s = Console.ReadLine();
                f = uint.TryParse(s, out count);
            }

            // цена за единицу
            double price;
            Console.WriteLine("Введите цену за единицу");
            s = Console.ReadLine();
            if (s == "-1") { return; }
            f &= double.TryParse(s, out price);
            while (!f)
            {
                if (s == "-1") { return; }
                PrintAddError();
                s = Console.ReadLine();
                f = double.TryParse(s, out price);
            }

            // код валюты
            Console.WriteLine("Введите код валюты");
            string currency = Console.ReadLine();
            if (s == "-1") { return; }

            // регион
            byte reg;
            Console.WriteLine("Введите номер региона");
            s = Console.ReadLine();
            f &= byte.TryParse(s, out reg);
            while (!f)
            {
                if (s == "-1") { return; }
                PrintAddError();
                s = Console.ReadLine();
                f = byte.TryParse(s, out reg);
            }

            try
            {
                TransactionsHandler.Add(ref data, new Transaction(dt, prodId, name, count, price, currency, reg));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// Удаление транзакции
        /// </summary>
        /// <param name="data">список транзакций</param>
        public static void Delete(ref List<Transaction> data)
        {
            Console.WriteLine("Введите id транзакции, которую хотите удалить");
            uint id;
            bool f = uint.TryParse(Console.ReadLine(), out id);

            // проверка на то, что ввели возможное значение id
            if (!f)
            {
                Console.WriteLine("Неверный id");
                return;
            }
            // попытка удалить транзакцию
            try
            {
                TransactionsHandler.Delete(ref data, id);
                Console.WriteLine("Удаление успешно завершено");
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            } 
        }
        /// <summary>
        /// изменение транзакции
        /// </summary>
        /// <param name="data">список транзакций</param>
        public static void Edit(ref List<Transaction> data)
        {
            Console.WriteLine("Введите id транзакции, которую хотите изменить");
            Console.WriteLine("Чтобы вернуться в главное меню, введите -1");
            uint id;
            string s = Console.ReadLine();
            bool f = uint.TryParse(s, out id);
            Console.Clear();
            if (s == "-1")
            {
                return;
            }
            // цикл ввода id, пока пользователь не введет корректное значение
            while (!f)
            {
                Console.WriteLine("Неверный id");
                s = Console.ReadLine();
                f = uint.TryParse(s, out id);
                if (s == "-1")
                {
                    return;
                }
            }
            Elements el;
            Console.WriteLine("Введите название элемента, который хотите изменить");
            Console.WriteLine("Чтобы вернуться в главное меню, введите -1");
            string resp = Console.ReadLine();
            Console.Clear();
            // присваение соответствующего перечисления
            while (true)
            {
                if (resp == "дата") { el = Elements.Date; break; }
                else if (resp == "id") { el = Elements.ProdId; break; }
                else if (resp == "название") { el = Elements.Name; break; }
                else if (resp == "количество") { el = Elements.Count; break; }
                else if (resp == "цена в рублях") { el = Elements.PricePerUnit; break; }
                else if (resp == "цена в валюте") { el = Elements.PriceInCurrency; break; }
                else if (resp == "валюта") { el = Elements.Currency; break; }
                else if (resp == "регион") { el = Elements.Region; break; }
                else if (resp == "-1") { return; }
                else 
                {
                    PrintEditError();
                    resp = Console.ReadLine();
                }
            }
            Console.WriteLine("Введите новое значение");
            s = Console.ReadLine();
            Console.Clear();
            // код преобразует к нужному типу данных введенное значение и вызывает соответствующую функцию
            switch (el)
            {
                case Elements.Date:
                    DateTime dt;
                    f = DateTime.TryParse(s, out dt);
                    // цикл ввода даты, пока пользователь не введет корректную
                    while (!f)
                    {
                        PrintEditError();
                        s = Console.ReadLine();
                        if (s == "-1") { return; }
                        f = DateTime.TryParse(s, out dt);
                    }
                    try
                    {
                        TransactionsHandler.Edit(ref data, id, el, dt);
                    } catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case Elements.Name:
                    try
                    {
                        TransactionsHandler.Edit(ref data, id, el, s);
                    } catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case Elements.Currency:
                    // цикл ввода валюты, пока пользователь не введет корректную
                    while (!CurrencyConverter.IsValid(s))
                    {
                        s = Console.ReadLine();
                    }
                    try
                    {
                        TransactionsHandler.Edit(ref data, id, el, s);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case Elements.Region:
                    byte reg;
                    f = byte.TryParse(s, out reg);
                    // цикл ввода региона, пока пользователь не введет корректный
                    while (!f)
                    {
                        PrintEditError();
                        s = Console.ReadLine();
                        if (s == "-1") { return; }
                        f = byte.TryParse(s, out reg);
                    }
                    try
                    {
                        TransactionsHandler.Edit(ref data, id, el, reg);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case Elements.Id:
                    uint val;
                    f = uint.TryParse(s, out val);
                    // цикл ввода id товара, пока пользователь не введет корректное значение
                    while (!f)
                    {
                        PrintEditError();
                        s = Console.ReadLine();
                        if (s == "-1") { return; }
                        f = uint.TryParse(s, out val);
                    }
                    try
                    {
                        TransactionsHandler.Edit(ref data, id, el, val);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case Elements.Count:
                    uint val1;
                    f = uint.TryParse(s, out val1);
                    // цикл ввода количества товара, пока пользователь не введет корректное значение
                    while (!f)
                    {
                        PrintEditError();
                        s = Console.ReadLine();
                        if (s == "-1") { return; }
                        f = uint.TryParse(s, out val1);
                    }
                    try
                    {
                        TransactionsHandler.Edit(ref data, id, el, val1);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                default:
                    double val2;
                    f = double.TryParse(s, out val2);
                    // цикл ввода цену товара, пока пользователь не введет корректное значение
                    while (!f)
                    {
                        PrintEditError();
                        s = Console.ReadLine();
                        if (s == "-1") { return; }
                        f = double.TryParse(s, out val2);
                    }
                    try
                    {
                        TransactionsHandler.Edit(ref data, id, el, val2);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
            }
        }
    }
}
