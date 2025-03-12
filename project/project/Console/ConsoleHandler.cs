using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
        public static void PrintInfo(List<Transaction> transactions)
        {
            foreach (var el in  transactions)
            {
                Console.WriteLine(el.ToString());
            }
        }
    }
}
