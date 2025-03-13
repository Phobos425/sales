using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using project.db;

namespace project.edit {
	/// <summary>
	/// статический класс, отвечающий за изменение транзакций
	/// </summary>
	public static class TransactionsHandler
	{
		/// <summary>
		/// Метод для нахождения траназакции по её id
		/// </summary>
		/// <param name="id">id</param>
		/// <param name="data">список транзакций</param>
		/// <returns>индекс транзакции</returns>
		private static int GetById(uint id, ref List<Transaction> data)
		{
			var res = from el in data
					  where el.Id == id
					  select data.IndexOf(el);
			return res[0];
		}
        /// <summary>
        /// Добавление транзакции
        /// </summary>
        /// <param name="data">список транзакций</param>
        /// <exception cref="ArgumentException">выбрасывается в случае неверно введенной транзакции</exception>
		public static void Add(ref List<Transaction> data)
		{
			Console.WriteLine("Введите дату создания транзакции");
            DateTime dt;
            bool f = DateTime.TryParse(Console.ReadLine(), out dt);
            Console.WriteLine("Введите id товара");
            uint prodId;
            f &= uint.TryParse(Console.ReadLine(), out prodId);
            Console.WriteLine("Введите название товара");
            string name = Console.ReadLine();
            uint count;
            Console.WriteLine("Введите количество товара");
            f &= uint.TryParse(Console.ReadLine(), out count);
            uint price;
            Console.WriteLine("Введите цену за еденицу");
            f &= uint.TryParse(Console.ReadLine(), out price);
            byte reg;
            Console.WriteLine("Введите номер региона");
            f &= byte.TryParse(Console.ReadLine(), out reg);
            if (!f)
            {
                throw new ArgumentException("Неверная запись транзакции");
            }
            data.Add(new Transaction(dt, prodId, name, count, price, reg));
        }
        /// <summary>
        /// метод, отвечающий за удаление транзакций
        /// </summary>
        /// <param name="id">id транзакции</param>
        /// <param name="data">список транзакций</param>
        public static void Delete(uint id, ref List<Transaction> data)
        {
            data.RemoveAll(x => x.Id == id);
        }
        /// <summary>
        /// редактирование uint элементов транзакции
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="el">элемент транзакции</param>
        /// <param name="val">новое значение</param>
        public static void Edit(ref List<Transaction> data, uint id, Elements el, uint val)
        {
            int trans = GetById(id, ref data);
            switch (el)
            {
                case el.ProdId:
                    data[trans].ProdId = val; break;
                case el.Count:
                    data[trans].Count = val; break;
                case el.PricePerUnit:
                    data[trans].PricePerUnit = val; break;
            }
        }
        /// <summary>
        /// редактирование времени транзакции
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="val">новое значение</param>
        public static void Edit(ref List<Transaction> data, uint id, DateTime val)
        {
            int trans = GetById(id, ref data);
            data[trans].Date = val;
        }
        /// <summary>
        /// редактирование названия товара
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="val">новое значение</param>
        public static void Edit(ref List<Transaction> data, uint id, string val)
        {
            int trans = GetById(id, ref data);
            data[trans].Name = val;
        }
        /// <summary>
        /// редактирование региона транзакции
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="val">новое значение</param>
        public static void Edit(ref List<Transaction> data, uint id, byte val)
        {
            int trans = GetById(id, ref data);
            data[trans].Region = val;
        }
    }
}
