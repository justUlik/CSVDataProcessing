using System.Runtime.Intrinsics.Arm;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CsvDataUtility;

namespace CSVDataProcessing;

public static class UserDataProcessing
{
    private static string GetFilePath()
    {
        bool isRead = false;
        string fileName;
        Console.WriteLine("Please enter file path. File extension should be csv:");
        do
        {
            fileName = Console.ReadLine();
            try
            {
                Regex containsABadCharacter = new Regex("["
                                                        + Regex.Escape(new string(System.IO.Path.GetInvalidPathChars())) + "]");
                if (containsABadCharacter.IsMatch(fileName))
                {
                    isRead = false;
                }
                else
                {
                    if (Path.GetExtension(fileName) == ".csv")
                    {
                        isRead = true;    
                    }
                    else 
                    { 
                        isRead = false;
                    }
                }
            }
            catch (Exception ex)
            {
                isRead = false;
                Console.WriteLine(ex.Message);
            }

            if (!isRead)
            {
                Console.WriteLine("Please try again:");
            }
        } while (!isRead);

        return fileName;
    }
    public static string[][]? GetCsvDataFile()
    {
        Console.Clear();
        bool isRead = false;
        string[][] data = null;
        do
        {
            string filePath = GetFilePath();
            try
            {
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
        Console.WriteLine("Data read finished successfully!");
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
        Console.WriteLine($"№{0,5} | {data[0][0],spaceId} | {data[0][1],spaceStationStart} | {data[0][2],spaceLine} | " +
                          $"{data[0][3],spaceTimeStart} | {data[0][4],spaceStationEnd} | {data[0][5],spaceTimeEnd} | " +
                          $"{data[0][6],spaceGlobalId} | {data[0][7]} |");
        Console.WriteLine($"№{1,5} | {CutString(data[1][0],spaceId),spaceId} | {CutString(data[1][1],spaceStationStart),spaceStationStart} | " +
                          $"{CutString(data[1][2],spaceLine),spaceLine} | {CutString(data[1][3],spaceTimeStart),spaceTimeStart} | " +
                          $"{CutString(data[1][4],spaceStationEnd),spaceStationEnd} | {CutString(data[1][5],spaceTimeEnd),spaceTimeEnd} | " +
                          $"{CutString(data[1][6],spaceGlobalId),spaceGlobalId} | {data[1][7]} |");
        // data
        for (int i = 2; i < data.GetLength(0); ++i)
        {
            Console.WriteLine($"№{i,5} | {data[i][0],spaceId} | {data[i][1],spaceStationStart} | {data[i][2],spaceLine} | " +
                              $"{data[i][3],spaceTimeStart} | {data[i][4],spaceStationEnd} | {data[i][5],spaceTimeEnd} | " +
                              $"{data[i][6],spaceGlobalId} | {data[i][7]} |");

        }
        
        Console.WriteLine("\nPress Q to close data view:");
        bool isRead = false;
        do
        {
            var cmd = Console.ReadKey().Key;
            if (cmd == ConsoleKey.Q)
            {
                isRead = true;
            }
        } while (!isRead);
    }

    private static void SaveAllData(string[][] data)
    {
        string filePath = GetFilePath();
        CsvDataUtility.CsvProcessing.fPath = filePath;
        string[] reformatData = data
            .Select(row => string.Join<string>(";", row))
            .ToArray();

        CsvDataUtility.CsvProcessing.Write(reformatData);
    }

    private static void SaveOneStringData(string[][] data)
    {
        Console.Clear();
        Console.WriteLine("Choose index of this line(in table after №");
        PrintData(data);
        Console.WriteLine("Please enter index of this line(in table after №):");
        bool isRead = false;
        int ind;
        do
        {
            if (int.TryParse(Console.ReadLine(), out ind))
            {
                if (0 < ind && ind < data.GetLength(0))
                {
                    isRead = true;
                }
            }

            if (!isRead)
            {
                Console.WriteLine("Try again:");
            }
        } while (!isRead);

        bool success = false;
        do
        {
            string nPath = GetFilePath();
            string line = string.Join<string>(";", data[ind]);
            try
            {
                CsvDataUtility.CsvProcessing.Write(line, nPath);
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            
            }
        } while (!success);
    }
    
    private static void SaveDataMenu(string[][] data)
    {
        Console.Clear();
        Console.WriteLine("Choose one option by pressing corresponding number");
        Console.WriteLine("1. Save all data to file");
        Console.WriteLine("2. Write only one line from data to file");
        Console.WriteLine("3. Stop this session");

        bool isRead = false;
        do
        {
            var cmd = Console.ReadKey().Key;
            if (cmd == ConsoleKey.D1)
            {
                SaveAllData(data);
                isRead = true;
            } 
            else if (cmd == ConsoleKey.D2)
            {
                SaveOneStringData(data);
                isRead = true;
            }
            else if (cmd == ConsoleKey.D3)
            {
                isRead = true;
                return;
            }
        } while (!isRead);
        Console.WriteLine("Data successfully was written to file!");
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
                SaveDataMenu(result);
            } else if (cmd == ConsoleKey.D2) 
            { 
                isRecievedTrainsition = true; 
                string valueSelection = UserGetValueSelectionByRow(data, "StationEnd");
                string[][] result = CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationEnd", valueSelection);
                PrintData(result);
                SaveDataMenu(result);
            } else if (cmd == ConsoleKey.D3) 
            { 
                isRecievedTrainsition = true; 
                string valueSelectionStart = UserGetValueSelectionByRow(data, "StationStart"); 
                string valueSelectionEnd = UserGetValueSelectionByRow(data, "StationEnd");
                string[][] result = CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationStart",
                    valueSelectionStart, "StationEnd", valueSelectionEnd);
                PrintData(result);
                SaveDataMenu(result);
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
                SaveDataMenu(result);
            } else if (cmd == ConsoleKey.D2) 
            { 
                isRecievedTrainsition = true; 
                string[][] result = CsvDataUtility.DataProcessing.Sort(data, "TimeEnd", typeSort);
                PrintData(result);
                SaveDataMenu(result);
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