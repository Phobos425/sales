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
        /// <param name="data">список транзакций</param>
        /// <param name="id">id</param>
        /// <returns>индекс транзакции</returns>
        private static int GetById(List<Transaction> data, uint id)
		{
		    var res1 = from el in data
					  where el.Id == id
					  select data.IndexOf(el);
            int res = -1;
            foreach (var el in res1)
            {
                res = el;
            }
            return res;
		}
        /// <summary>
        /// Добавление транзакции
        /// </summary>
        /// <param name="data">список транзакций</param>
        /// <param name="trans">транзакция</param>
		public static void Add(ref List<Transaction> data, Transaction trans)
		{
            data.Add(trans);
        }
        /// <summary>
        /// метод, отвечающий за удаление транзакций
        /// </summary>
        /// <param name="id">id транзакции</param>
        /// <param name="data">список транзакций</param>
        /// <returns>список транзакций</returns>
        public static void Delete(ref List<Transaction> data, uint id)
        {
            if (GetById(data, id) == -1)
            {
                throw new Exception("Транзакция не найдена");
            }
            data.RemoveAll(x => x.Id == id);
        }
        /// <summary>
        /// редактирование uint элементов транзакции
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="el">элемент транзакции</param>
        /// <param name="val">новое значение</param>
        /// <returns>список транзакций</returns>
        /// <exception cref="ArgumentException">выбрасывается в случае ввода некорректного id</exception>
        public static void Edit(ref List<Transaction> data, uint id, Elements el, uint val)
        {
            int ind = GetById(data, id);
            if (ind == -1)
            {
                throw new ArgumentException("Неверный id");
            }
            var trans = data[ind];
            switch (el)
            {
                case Elements.ProdId:
                    trans.ProdId = val; break;
                case Elements.Count:
                    trans.Count = val; break;
                case Elements.PricePerUnit:
                    trans.PricePerUnit = val; break;
            }
            data[ind] = trans;
        }
        /// <summary>
        /// редактирование времени транзакции
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="el">элемент транзакции</param>
        /// <param name="val">новое значение</param>
        /// <returns>список транзакций</returns>
        /// <exception cref="ArgumentException">выбрасывается в случае ввода некорректного id</exception>
        public static void Edit(ref List<Transaction> data, uint id, Elements el, DateTime val)
        {
            int ind = GetById(data, id);
            if (ind == -1)
            {
                throw new ArgumentException("Неверное id");
            }
            var trans = data[ind];
            trans.Date = val;
            data[ind] = trans;
        }
        /// <summary>
        /// редактирование названия товара
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="el">элемент транзакции</param>
        /// <param name="val">новое значение</param>
        /// <returns>список транзакций</returns>
        /// <exception cref="ArgumentException">выбрасывается в случае ввода некорректного id</exception>
        public static void Edit(ref List<Transaction> data, uint id, Elements el, string val)
        {
            int ind = GetById(data, id);
            if (ind == -1)
            {
                throw new ArgumentException("Неверное id");
            }
            var trans = data[ind];
            trans.Name = val;
            data[ind] = trans;
        }
        /// <summary>
        /// редактирование региона транзакции
        /// </summary>
        /// <param name="data">список транзакции</param>
        /// <param name="id">id транзакции</param>
        /// <param name="el">элемент транзакции</param>
        /// <param name="val">новое значение</param>
        /// <returns>список транзакций</returns>
        /// <exception cref="ArgumentException">выбрасывается в случае ввода некорректного id</exception>
        public static void Edit(ref List<Transaction> data, uint id, Elements el, byte val)
        {
            int ind = GetById(data, id);
            if (ind == -1)
            {
                throw new ArgumentException("Неверное id");
            }
            var trans = data[ind];
            trans.Region = val;
            data[ind] = trans;
        }
    }
}
