using System.Security.AccessControl;
using System.Text;
using System.Xml;
using CsvDataUtility;

namespace CSVDataProcessing;

public static class UserDataProcessing
{
    public static string[][] GetCsvDataFile()
    {
        Console.Clear();
        bool isRead = false;
        string[][] data = null;
        do
        {
            Console.WriteLine("Please enter absolute file path. File exstension should be csv:");
            string filePath = Console.ReadLine();
            try
            {
                /*if (filePath != Path.GetFullPath(Path.GetFileName(filePath)))
                {
                    Console.WriteLine(filePath);
                    Console.WriteLine(Path.GetFullPath(Path.GetFileName(filePath)));
                    isRead = false;
                    Console.WriteLine("It is not absolute path. Please try again");
                    continue;
                }*/
                CsvDataUtility.CsvProcessing.fPath = filePath;
                string[] notFormatedData = CsvDataUtility.CsvProcessing.Read();
                data = CsvDataUtility.CsvProcessing.RefactorData(notFormatedData);
                isRead = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try again:");
                isRead = false;
            }
        } while (!isRead);

        return data;
    }

    public static string UserGetValueSelectionByRow(string[][] data, string columnName)
    {
        string[] uniqueValues = CsvDataUtility.DataProcessing.GetUniqueValueInRow(data, columnName);
        Console.Clear();
        Console.WriteLine($"For this column name {columnName} there are this values to make selection by:");
        Console.WriteLine("Write the corresponding name");
        for (int i = 0; i < uniqueValues.Length; ++i)
        {
            Console.WriteLine($"{i}. {uniqueValues[i]}");
        }

        string valueSelection;
        bool isRead = false;
        do
        {
            valueSelection = Console.ReadLine();
            if (uniqueValues.Contains(valueSelection))
            {
                isRead = true;
            }
            else
            {
                Console.WriteLine("No such value in values of row. Try again:");
            }
        } while (!isRead);
        Console.WriteLine(valueSelection);
        return valueSelection;
    }

    private static string CutString(string str, int sz)
    {
        StringBuilder result = new StringBuilder();
        sz -= 3;
        for (int i = 0; i < Math.Min(sz, str.Length); ++i)
        {
            result.Append(str[i]);
        }

        result.Append("...");
        return result.ToString();
    }
    
    public static void PrintData(string[][] data)
    {
        Console.Clear();
        if (data is null)
        {
            Console.WriteLine("No data to print");
            return;
        }

        if (data.GetLength(0) <= 2)
        {
            Console.WriteLine("No data to print");
            return;
        }
        const int spaceId = 5;
        const int spaceStationStart = 22;
        const int spaceLine = 19;
        const int spaceTimeStart = 9;
        const int spaceStationEnd = 22;
        const int spaceTimeEnd = 7;
        const int spaceGlobalId = 10;
        // heading
        Console.WriteLine($"{data[0][0],spaceId} | {data[0][1],spaceStationStart} | {data[0][2],spaceLine} | " +
                          $"{data[0][3],spaceTimeStart} | {data[0][4],spaceStationEnd} | {data[0][5],spaceTimeEnd} | " +
                          $"{data[0][6],spaceGlobalId} | {data[0][7]} |");
        Console.WriteLine($"{CutString(data[1][0],spaceId),spaceId} | {CutString(data[1][1],spaceStationStart),spaceStationStart} | " +
                          $"{CutString(data[1][2],spaceLine),spaceLine} | {CutString(data[1][3],spaceTimeStart),spaceTimeStart} | " +
                          $"{CutString(data[1][4],spaceStationEnd),spaceStationEnd} | {CutString(data[1][5],spaceTimeEnd),spaceTimeEnd} | " +
                          $"{CutString(data[1][6],spaceGlobalId),spaceGlobalId} | {data[1][7]} |");
        // data
        for (int i = 2; i < data.GetLength(0); ++i)
        {
            Console.WriteLine($"{data[i][0],spaceId} | {data[i][1],spaceStationStart} | {data[i][2],spaceLine} | " +
                              $"{data[i][3],spaceTimeStart} | {data[i][4],spaceStationEnd} | {data[i][5],spaceTimeEnd} | " +
                              $"{data[i][6],spaceGlobalId} | {data[i][7]} |");

        }
    }
    
    public static void SelectionMenu(string[][] data)
    {
        bool isRecievedTrainsition = false;
        
        do
        {
            Console.Clear();
            Console.WriteLine("Make a selection by:");
            Console.WriteLine("1. StationStart");
            Console.WriteLine("2. StationEnd");
            Console.WriteLine("3. StationStart and StationEnd");
            Console.WriteLine("4. Stop the program");
            
            var cmd = Console.ReadKey().Key;
            if (cmd == ConsoleKey.D1) 
            { 
                isRecievedTrainsition = true; 
                string valueSelection = UserGetValueSelectionByRow(data, "StationStart");
                string[][] result = CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationStart", valueSelection);
                PrintData(result);
            } else if (cmd == ConsoleKey.D2) 
            { 
                isRecievedTrainsition = true; 
                string valueSelection = UserGetValueSelectionByRow(data, "StationEnd");
                string[][] result = CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationEnd", valueSelection);
                PrintData(result);
            } else if (cmd == ConsoleKey.D3) 
            { 
                isRecievedTrainsition = true; 
                string valueSelectionStart = UserGetValueSelectionByRow(data, "StationStart"); 
                string valueSelectionEnd = UserGetValueSelectionByRow(data, "StationEnd");
                string[][] result = CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationStart",
                    valueSelectionStart, "StationEnd", valueSelectionEnd);
                PrintData(result);
            }
            else
            {
                isRecievedTrainsition = false;
            }

            if (!isRecievedTrainsition)
            {
                Console.WriteLine("Try again");
            }
        } while (!isRecievedTrainsition);
    }

    public static void SortMenu(string[][] data)
    {
        Console.Clear();
        Console.WriteLine("Choose by which sort method you want to sort data. Press the corresponding number");
        Console.WriteLine("1. MergeSort");
        Console.WriteLine("2. QuickSort");
        string typeSort = "";
        bool isRecievedTrainsition = false;
        do
        {
            var cmd = Console.ReadKey().Key;
            if (cmd == ConsoleKey.D1) 
            { 
                isRecievedTrainsition = true;
                typeSort = "MergeSort";
            } else if (cmd == ConsoleKey.D2) 
            { 
                isRecievedTrainsition = true;
                typeSort = "QuickSort";
            }
            else
            {
                isRecievedTrainsition = false;
            }

            if (!isRecievedTrainsition)
            {
                Console.WriteLine("Try again");
            }
        } while (!isRecievedTrainsition);

        Console.Clear();
        Console.WriteLine("Choose by which row you want to sort data. Press the corresponding number");
        Console.WriteLine("1. TimeStart - in order of increasing time");
        Console.WriteLine("2. TimeEnd - in order of increasing time");
        
        isRecievedTrainsition = false;
        do
        {
            var cmd = Console.ReadKey().Key;
            if (cmd == ConsoleKey.D1) 
            { 
                isRecievedTrainsition = true;
                string[][] result = CsvDataUtility.DataProcessing.Sort(data, "TimeStart", typeSort);
                PrintData(result);
            } else if (cmd == ConsoleKey.D2) 
            { 
                isRecievedTrainsition = true; 
                string[][] result = CsvDataUtility.DataProcessing.Sort(data, "TimeEnd", typeSort);
                PrintData(result);
            }
            else
            {
                isRecievedTrainsition = false;
            }

            if (!isRecievedTrainsition)
            {
                Console.WriteLine("Try again");
            }
        } while (!isRecievedTrainsition);
    }
    
    public static void MainMenu(string[][] data)
    {
        bool isRecievedTrainsition = false;

        do
        {
            Console.Clear();
            Console.WriteLine("For this file choose one option and press the corresponding number:");
            Console.WriteLine("1. Make a selection by column");
            Console.WriteLine("2. To sort by column");
            Console.WriteLine("3. Stop the program");

            var cmd = Console.ReadKey().Key;
            switch (cmd)
            {
                case ConsoleKey.D1:
                    isRecievedTrainsition = true;
                    SelectionMenu(data);
                    break;
                case ConsoleKey.D2:
                    isRecievedTrainsition = true;
                    SortMenu(data);
                    break;
                case ConsoleKey.D3:
                    isRecievedTrainsition = true;
                    return;
                default:
                    isRecievedTrainsition = false;
                    break;
            }
        } while (!isRecievedTrainsition);
    }
}