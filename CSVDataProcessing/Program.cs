using System;
using System.ComponentModel.Design;
using CSVDataProcessing;

class Program
{
    
    /// <summary>
    /// Method to have data from user and do all actions in the project
    /// </summary>
    static void Main()
    {
        do
        {
            try
            {
                string[][] data = UserDataProcessing.GetCsvDataFile();
                UserDataProcessing.MainMenu(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("To have another session press any key, to finish - press Backspace");
        } while (Console.ReadKey().Key != ConsoleKey.Backspace);
    }
}