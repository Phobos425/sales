using System;
using project.db;

public class Program
{
    public static void Main()
    {
        List<Transaction> transactions = new List<Transaction>();
        FileHandler.CurrentPath = @"..\..\..\db\db.csv";
        FileHandler.DownloadData(ref transactions);
        Console.WriteLine("1");
        var trans = new Transaction(DateTime.Now, 3, "ada", 5, 15, 52);
        Console.WriteLine(trans);
        transactions.Add(trans);
        FileHandler.WriteData(transactions);
    }
}
