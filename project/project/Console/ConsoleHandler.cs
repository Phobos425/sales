using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.db;

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
            Console.WriteLine("7. Выход");
        }
        /// <summary>
        /// вывод информации о покупках
        /// </summary>
        /// <param name="transactions">список покупок</param>
        public static void PrintInfo(ref List<Transaction> transactions)
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
    }
}
