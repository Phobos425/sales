using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.db
{
    /// <summary>
    /// класс, который 
    /// </summary>
    public struct Transaction
    {
        private static uint _newId = 0; //количество транзакций

        public uint Id { get; private set; } // id транзакции
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
        public uint ProdId { get; set; } // id товара
        public string Name { get; set; } // Наименование товара
        public uint Count { get; set; } // Количество
        public uint PricePerUnit { get; set; } // Цена за единицу
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
        /// <summary>
        /// конструктор класса
        /// </summary>
        /// <param name="date">дата транзакции</param>
        /// <param name="prodId">id товара</param>
        /// <param name="name">наименование товара</param>
        /// <param name="count">количество товаров</param>
        /// <param name="pricePerUnit">цена за шт</param>
        /// <param name="region">номер региона</param>
        public Transaction(DateTime date, uint prodId, string name, uint count, uint pricePerUnit, byte region)
        {
            Id = _newId;
            Date = date;
            ProdId = prodId;
            Name = name;
            Count = count;
            PricePerUnit = pricePerUnit;
            Region = region;
            ++_newId;
        }
        /// <summary>
        /// конструктор класса
        /// </summary>
        /// <param name="id">id транзакции</param>
        /// <param name="date">дата транзакции</param>
        /// <param name="prodId">id товара</param>
        /// <param name="name">наименование товара</param>
        /// <param name="count">количество товаров</param>
        /// <param name="pricePerUnit">цена за шт</param>
        /// <param name="region">номер региона</param>
        public Transaction(uint id, DateTime date, uint prodId, string name, uint count, uint pricePerUnit, byte region)
        {
            _newId = Math.Max(id + 1, _newId);
            Id = id;
            Date = date;
            ProdId = prodId;
            Name = name;
            Count = count;
            PricePerUnit = pricePerUnit;
            Region = region;
        }
        public override string ToString()
        {
            return $"{Id};{Date.ToString(new CultureInfo("ru-RU"))[0..10]};{ProdId};" +
                $"{Name};{Count};{PricePerUnit};{Region}";
        }
    }
}
