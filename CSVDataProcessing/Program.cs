using System;
using CSVDataProcessing;

class Program
{
    static void Main()
    {
        string[] data = UserDataProcessing.GetCsvDataFile();
        foreach (var line in data)
        {
            Console.WriteLine(line);
        }
    }
}