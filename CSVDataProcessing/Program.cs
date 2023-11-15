using System;
using System.ComponentModel.Design;
using CSVDataProcessing;

class Program
{
    // /Users/ulyanaeskova/Downloads/aeroexpress.csv
    static void Main()
    {
        do
        {
            // написать try catch
            string[][] data = UserDataProcessing.GetCsvDataFile();
            UserDataProcessing.MainMenu(data);

            Console.WriteLine("To have another session press any key, to finish - press Backspace");
        } while (Console.ReadKey().Key != ConsoleKey.Backspace);
    }
}