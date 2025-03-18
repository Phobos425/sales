using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.db
{
    public static class FileHandler
    {
        private static string _currentPath = "";
        /// <summary>
        /// Присваивание пути файла
        /// </summary>
        public static string CurrentPath
        {
            get { return _currentPath; }
            set
            {
                if (File.Exists(value))
                {
                    _currentPath = value;
                } else
                {
                    throw new FileNotFoundException("Некорректный путь");
                }
            }
        }
        /// <summary>
        /// загрузка данных из файла
        /// </summary>
        /// <param name="data">список со всеми транзакциями</param>
        /// <exception cref="FileNotFoundException">выбрасываем, если пользователь не ввел путь до файла</exception>
        public static void DownloadData (ref List<Transaction> data)
        {
            if (_currentPath == "")
            {
                throw new FileNotFoundException("Файл не загружен");
            }
            string[] strings = File.ReadAllLines(CurrentPath); // загрузка строк файла в массив
            int n = strings.Length;
            List<Transaction> tmp = new List<Transaction>(); // создание временного списка с транзакциями
            for (int i = 0; i < n; ++i)
            {
                string[] s = strings[i].Split(";"); // сплит строки по разделителю бд
                if (s.Length == 1 && s[0] == "") { continue; }
                if ( s.Length != 7) // проверка количества элементов транзакции
                {
                    throw new ArgumentException("Неверная запись транзакции");
                }
                // получение id
                uint id;
                bool f = uint.TryParse(s[0], out id);
                // получение даты
                DateTime dt;
                f &= DateTime.TryParse(s[1], out dt);
                // получение id товара
                uint prodId;
                f &= uint.TryParse(s[2], out prodId);
                // получение названия товара
                string name = s[3];
                // получение количества товаров
                uint count;
                f &= uint.TryParse(s[4], out count);
                // получение цены 1шт в рублях
                double price;
                f &= double.TryParse(s[5], out price);
                // получение цены 1шт в валюте
                double priceCur;
                f &= double.TryParse(s[5], out priceCur);
                // получение региона
                byte reg;
                f &= byte.TryParse(s[6], out reg);
                // проверка, что все данные верны
                if (!f)
                {
                    throw new ArgumentException("Неверная запись транзакции");
                }
                tmp.Add(new Transaction(id, dt, prodId, name, count, price, reg));
            }
            if (tmp.Count == 0) {
                throw new Exception("Пустой файл");
            }
            data = tmp;
        }
        /// <summary>
        /// Сохранение данных в файл
        /// </summary>
        /// <param name="data">транзакции</param>
        public static void WriteData(List<Transaction> data) { 
            int n = data.Count;
            string[] output = new string[n];
            for (int i = 0; i < n; ++i)
            {
                output[i] = data[i].ToString();
            }
            File.WriteAllLines(_currentPath, output);
        }
    }
}
