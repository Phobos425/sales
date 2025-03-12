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
            string[] strings = File.ReadAllLines(CurrentPath);
            int n = strings.Length;
            for (int i = 0; i < n; ++i)
            {
                string[] s = strings[i].Split(";");
                if ( s.Length != 7)
                {
                    throw new ArgumentException("Неверная запись транзакции");
                }
                uint id;
                bool f = uint.TryParse(s[0], out id);
                DateTime dt;
                f &= DateTime.TryParse(s[1], out dt);
                uint prodId;
                f &= uint.TryParse(s[2], out prodId);
                string name = s[3];
                uint count;
                f &= uint.TryParse(s[4], out count);
                uint price;
                f &= uint.TryParse(s[5], out price);
                byte reg;
                f &= byte.TryParse(s[6], out reg);
                if (!f)
                {
                    throw new ArgumentException("Неверная запись транзакции");
                }
                data.Add(new Transaction(id, dt, prodId, name, count, price, reg));
            }
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
