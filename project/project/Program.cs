using System;
using project.ConsoleHandler;
using project.db;
using project.edit;

public static class Program
{
    public static void Main()
    {
        bool isLoaded = false;
        List<Transaction> transactions = new List<Transaction>();
        ConsoleKeyInfo currKey = new ConsoleKeyInfo('0', ConsoleKey.D0, false, false, false);
        // цикл работы программы
        while ((isLoaded == (currKey.Key == ConsoleKey.D2)) && (!isLoaded == (currKey.Key == ConsoleKey.D7))) {
            if (!isLoaded)
            {
                ConsoleHandler.PrintInitialMenu(); // вывод изначального меню
                currKey = Console.ReadKey();
                Console.Clear();
                switch (currKey.Key)
                {
                    case ConsoleKey.D1:
                        isLoaded = ConsoleHandler.DownloadData(ref  transactions); // загрузка данных и её результат
                        break;
                    case ConsoleKey.D2: // выход из программы
                        Console.WriteLine("Работа завершена");
                        break;
                    default:
                        Console.WriteLine("Нажата неверная клавиша");
                        break;
                }
            } else
            {
                ConsoleHandler.PrintMenu();
                currKey = Console.ReadKey();
                Console.Clear();
                switch (currKey.Key)
                {
                    case ConsoleKey.D1: // загрузка новых данных, в случае неудачи сохраняется старая бд
                        ConsoleHandler.DownloadData(ref transactions); break;
                    case ConsoleKey.D2: // вывод транзакций
                        ConsoleHandler.PrintInfo(ref transactions); break;
                    case ConsoleKey.D3: // добавление транзакции
                        ConsoleHandler.Add(ref transactions); break;
                    case ConsoleKey.D4: // удаление транзакции
                        ConsoleHandler.Delete(ref transactions); break;
                    case ConsoleKey.D5: // изменение транзакции
                        ConsoleHandler.Edit(ref transactions); break;
                    case ConsoleKey.D6: // сохранение данных
                        FileHandler.WriteData(transactions);
                        Console.WriteLine("Данные успешно сохранены");
                        break;
                    case ConsoleKey.D7: // выход из программы
                        Console.WriteLine("Работа завершена");
                        break;
                    default:
                        Console.WriteLine("Неверная кнопка");
                        break;
                }
            }
        }
    }
}
