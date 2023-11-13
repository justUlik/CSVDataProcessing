using System;
using System.ComponentModel.Design;
using CSVDataProcessing;

class Program
{
    static void Main()
    {
        string[][] data = UserDataProcessing.GetCsvDataFile();
        UserDataProcessing.MainMenu(data);
    }
}