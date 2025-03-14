using project.db;
using project.edit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach (var el in  transactions)
            {
                Console.WriteLine(el.ToString());
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
