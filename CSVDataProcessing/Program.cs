using System;
using System.ComponentModel.Design;
using CSVDataProcessing;

class Program
{
    static void Main()
    {
        do
        {
            string[][] data = UserDataProcessing.GetCsvDataFile();
            UserDataProcessing.MainMenu(data);

            Console.WriteLine("To have another session press any key, to finish - press escape");
        } while (Console.ReadKey().Key != ConsoleKey.Escape);
    }
}