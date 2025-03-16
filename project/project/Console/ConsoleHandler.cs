using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.db;
using System.Globalization;

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
            Console.WriteLine("2. Выйти");
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
            Console.WriteLine("6. Сохранение данных");
            Console.WriteLine("Esc. Выход");
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
                    filtered = filtered.OrderBy(x => x.Region);
                } else if (sort == -1)
                {
                    filtered = filtered.OrderByDescending(x => x.Region);
                }

                var table = new Table();

                string[] names = ["ID", "Date", "Product ID", "Product name", "Count", "Price/Unit", "Region"]; // имена столбцов

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
                        if (tmp > 85)
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
            //дальше идут блоки инициализации траназакции, в случае ввода -1, пользователя возвращает в главное меню

            Console.WriteLine("Введите дату создания транзакции");
            Console.WriteLine("Чтобы вернуться в главное меню, введите -1");
            DateTime dt;
            string s = Console.ReadLine();
            bool f = DateTime.TryParse(s, out dt);
            while (!f)
            {
                PrintAddError();
                s = Console.ReadLine();
                if (s == "-1")
                {
                    return;
                }
                f = DateTime.TryParse(s, out dt);
            }

            Console.WriteLine("Введите id товара");
            uint prodId;
            s = Console.ReadLine();
            f = uint.TryParse(s, out prodId);
            while (!f)
            {
                PrintAddError();
                s = Console.ReadLine();
                if (s == "-1")
                {
                    return;
                }
                f = uint.TryParse(s, out prodId);
            }

            Console.WriteLine("Введите название товара");
            string name = Console.ReadLine();
            uint count;

            Console.WriteLine("Введите количество товара");
            s = Console.ReadLine();
            f = uint.TryParse(s, out count);
            while (!f)
            {
                PrintAddError();
                s = Console.ReadLine();
                if (s == "-1")
                {
                    return;
                }
                f = uint.TryParse(s, out count);
            }

            uint price;
            Console.WriteLine("Введите цену за еденицу");
            s = Console.ReadLine();
            f &= uint.TryParse(s, out price);
            while (!f)
            {
                PrintAddError();
                s = Console.ReadLine();
                if (s == "-1")
                {
                    return;
                }
                f = uint.TryParse(s, out price);
            }

            byte reg;
            Console.WriteLine("Введите номер региона");
            s = Console.ReadLine();
            f &= byte.TryParse(s, out reg);
            while (!f)
            {
                PrintAddError();
                s = Console.ReadLine();
                if (s == "-1")
                {
                    return;
                }
                f = uint.TryParse(s, out price);
            }

            TransactionsHandler.Add(ref data, new Transaction(dt, prodId, name, count, price, reg));
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
            if (!f)
            {
                Console.WriteLine("Неверный id");
                return;
            }
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
                else if (resp == "цена") { el = Elements.PricePerUnit; break; }
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
                default:
                    uint val;
                    f = uint.TryParse(s, out val);
                    // цикл ввода id товара, количества или цены, пока пользователь не введет корректное значение
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
            }
        }
    }
}
