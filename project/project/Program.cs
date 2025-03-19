using System;
using System.Text;
using project.ConsoleHandler;
using project.db;
using project.DbClasses;
using project.edit;

public static class Program
{
    public static void Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // устанавливаем нужную кодировку
        CurrencyConverter.GetAllCurrencyCodes(); // загружаем все возможные валюты
        bool isLoaded = false;
        List<Transaction> transactions = new List<Transaction>();
        ConsoleKeyInfo currKey = new ConsoleKeyInfo('0', ConsoleKey.D0, false, false, false);
        // цикл работы программы
        while (currKey.Key != ConsoleKey.Backspace) {
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
                    case ConsoleKey.Backspace: // выход из программы
                        Console.WriteLine("Работа завершена");
                        break;
                    default:
                        Console.WriteLine("Нажата неверная клавиша");
                        break;
                }
            } else
            {
                ConsoleHandler.PrintMenu();
                currKey = Console.ReadKey(true);
                Console.Clear();
                switch (currKey.Key)
                {
                    case ConsoleKey.D: // загрузка новых данных, в случае неудачи сохраняется старая бд
                        ConsoleHandler.DownloadData(ref transactions);
                        break;
                    case ConsoleKey.D1: // вывод транзакций
                        ConsoleHandler.PrintInfo(ref transactions); break;
                    case ConsoleKey.D2: // добавление транзакции
                        try {
                            ConsoleHandler.Add(ref transactions);
                        } catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case ConsoleKey.D3: // удаление транзакции
                        ConsoleHandler.Delete(ref transactions); break;
                    case ConsoleKey.D4: // изменение транзакции
                        ConsoleHandler.Edit(ref transactions); break;
                    case ConsoleKey.D5: // вывод информации по регионам
                        ConsoleHandler.PrintRegionInfo(ref transactions); break;
                    case ConsoleKey.D6: // вывод суммы всех транзакций
                        ConsoleHandler.PrintAllSales(ref transactions); break;
                    case ConsoleKey.D7: // вывод ABC анализа
                        ConsoleHandler.PrintABCAnalysis(ref transactions); break;
                    case ConsoleKey.D8: // вывод XYZ анализа
                        ConsoleHandler.PrintXYZAnalysis(ref transactions); break;
                    case ConsoleKey.D9: // вывод прогноза
                        ConsoleHandler.PrintForecast(ref transactions); break;
                    case ConsoleKey.S: // сохранение данных
                        FileHandler.WriteData(transactions);
                        Console.WriteLine("Данные успешно сохранены");
                        break;
                    case ConsoleKey.Backspace: // выход из программы
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
