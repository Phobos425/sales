using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poject.db
{
    /// <summary>
    /// класс, который 
    /// </summary>
    public class Transaction
    {
        public static int NewId { get; private set; } //количество транзакций

        public int Id { get; private set; } // id транзакции
        private DateTime _date; // дата транзакции
        public DateTime Date {
            get {
                return _date;
            }
            set {
                if (DateTime.Compare(value, DateTime.Now) <= 0) { // проверка, что такая дата существует
                    _date = value;
                } else
                {
                    throw new ArgumentException("Неверная дата");
                }
            }
        }
        public int ProdId { get; set; } // id товара
        public string Name { get; set; } // Наименование товара
        public int Count { get; set; } // Количество
        public int PricePerUnit { get; set; } // Цена за единицу
        private byte _region; // регион транзакции
        public byte Region
        {
            get
            {
                return _region;
            }
            set
            {
                if (value >= 1 && value <= 89) // проверка существования региона
                {
                    _region = value;
                } else
                {
                    throw new ArgumentException("Неверный регион");
                }
            }
        }
    }
}
