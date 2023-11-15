using System.Runtime.Intrinsics.Arm;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CsvDataUtility;

namespace CSVDataProcessing;

/// <summary>
/// Сlass that performs all user interaction
/// </summary>
public static class UserDataProcessing
{
    private static bool IsNullOrEmpty(string[] array)
    {
        return (array is null || (array is not null && array.Length == 0));
    }

    private static bool IsNullOrEmpty(string[][] array)
    {
        if (array is null || (array is not null && array.Length == 0))
        {
            return true;
        }

        try
        {
            foreach (var line in array)
            {
                if (IsNullOrEmpty(line))
                {
                    return true;
                }
            }

            return false;
        }
        catch
        {
            return true;
        }
}
    /// <summary>
    /// Get from user filepath and check if valid it is
    /// </summary>
    /// <returns>string valid filePath</returns>
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
    
    /// <summary>
    /// Retrieves CSV data from a file.
    /// </summary>
    /// <returns>An array of string arrays representing the CSV data, or null if the data could not be read.</returns>
    public static string[][]? GetCsvDataFile()
    {
        Console.Clear();
        bool isRead = false;
        string[][] data = new string[0][];
        do
        {
            string filePath = GetFilePath();
            try
            {
                CsvDataUtility.CsvProcessing.fPath = filePath;
                string[] notFormatData = CsvDataUtility.CsvProcessing.Read();
                if (notFormatData is null)
                {
                    isRead = false;
                    continue;
                }
                data = CsvDataUtility.CsvProcessing.RefactorData(notFormatData);
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
    
    /// <summary>
    /// The method organises all user actions.
    /// </summary>
    /// <param name="data"></param>
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
    
    /// <summary>
    /// The method provides Menu to choose one option to make a selection by one column.
    /// </summary>
    /// <param name="data">Data to make selection</param>
    private static void SelectionMenu(string[][] data)
    {

        if (IsNullOrEmpty(data))
        {
            return;
        }
        
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
                try
                {
                    string[][] result =
                        CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationStart", valueSelection); 
                    PrintData(result);
                    SaveDataMenu(result);
                }
                catch (Exception ex)
                {
                    isRecievedTrainsition = false; 
                    Console.WriteLine(ex.Message);
                }
            } else if (cmd == ConsoleKey.D2) 
            {
                isRecievedTrainsition = true; 
                string valueSelection = UserGetValueSelectionByRow(data, "StationEnd");
                try
                {
                    string[][] result =
                        CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationEnd", valueSelection);
                    PrintData(result);
                    SaveDataMenu(result);
                }
                catch (Exception ex)
                {
                    isRecievedTrainsition = false;
                    Console.WriteLine(ex.Message);
                }
            } else if (cmd == ConsoleKey.D3) 
            {
                isRecievedTrainsition = true; 
                string valueSelectionStart = UserGetValueSelectionByRow(data, "StationStart"); 
                string valueSelectionEnd = UserGetValueSelectionByRow(data, "StationEnd");
                try
                {
                    string[][] result = CsvDataUtility.DataProcessing.GetSelectionByValueRow(data, "StationStart",
                        valueSelectionStart, "StationEnd", valueSelectionEnd);
                    PrintData(result);
                    SaveDataMenu(result);
                }
                catch (Exception ex)
                {
                    isRecievedTrainsition = false;
                    Console.WriteLine(ex.Message);
                }
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

    /// <summary>
    /// This method allows the user to input a value selection based on a given column name.
    /// It prompts the user to select from a list of unique values found in the specified column.
    /// The method validates the user input and ensures it is a valid value from the list.
    /// Once a valid value is selected, it is returned as a string.
    /// </summary>
    /// <param name="data">The data array containing the rows and columns of data</param>
    /// <param name="columnName">The name of the column to retrieve unique values from</param>
    /// <returns>The user-selected value as a string</returns>
    private static string UserGetValueSelectionByRow(string[][] data, string columnName)
    {
        if (IsNullOrEmpty(data))
        {
            return "";
        }

        string[] uniqueValues;
        try
        {
            uniqueValues = CsvDataUtility.DataProcessing.GetUniqueValueInRow(data, columnName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Something went wrong. {ex.Message}");
            return "";
        }

        if (IsNullOrEmpty(uniqueValues))
        {
            return "";
        }
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

    /// <summary>
    /// This method prints the given data in a formatted table. It checks if the data is null or if the length of the
    /// first dimension of the data is less than or equal to 2. If either of these conditions is met, it displays
    /// "No data to print" and returns. 
    /// 
    /// The method defines constants for column spacing in the table. 
    /// 
    /// It then prints the heading of the table using the values in the first row of the data array,
    /// aligning each column based on the respective spacing constant.
    /// 
    /// After printing the heading, the method iterates through the data array starting from the second row and prints
    /// each row of data in a formatted manner.
    /// 
    /// Finally, it waits for the user to press the 'Q' key to close the data view.
    /// </summary>
    /// <param name="data">The data to be printed</param>
    private static void PrintData(string[][] data)
    {
        Console.Clear();
        if (IsNullOrEmpty(data))
        {
            Console.WriteLine("No data to print");
            return;
        }

        if (data.GetLength(0) <= 2)
        {
            Console.WriteLine("No data to print");
            return;
        }
        const int spaceId = 4;
        const int spaceStationStart = 20;
        const int spaceLine = 15;
        const int spaceTimeStart = 9;
        const int spaceStationEnd = 20;
        const int spaceTimeEnd = 7;
        const int spaceGlobalId = 10;
        
        foreach (var line in data)
        {
            if (IsNullOrEmpty(line))
            {
                Console.WriteLine("Data doesn`t require format");
                return;
            }

            if (line.Length != 8)
            {
                Console.WriteLine("Data doesn`t require format");
                return;
            }
        }
        
        // heading
        Console.WriteLine($"№{0,4} | {data[0][0],spaceId} | {data[0][1],spaceStationStart} | {data[0][2],spaceLine} | " +
                          $"{data[0][3],spaceTimeStart} | {data[0][4],spaceStationEnd} | {data[0][5],spaceTimeEnd} | " +
                          $"{data[0][6],spaceGlobalId} | {data[0][7]} |");
        Console.WriteLine($"№{1,4} | {CutString(data[1][0],spaceId),spaceId} | {CutString(data[1][1],spaceStationStart),spaceStationStart} | " +
                          $"{CutString(data[1][2],spaceLine),spaceLine} | {CutString(data[1][3],spaceTimeStart),spaceTimeStart} | " +
                          $"{CutString(data[1][4],spaceStationEnd),spaceStationEnd} | {CutString(data[1][5],spaceTimeEnd),spaceTimeEnd} | " +
                          $"{CutString(data[1][6],spaceGlobalId),spaceGlobalId} | {data[1][7]} |");
        // data
        for (int i = 2; i < data.GetLength(0); ++i)
        {
            Console.WriteLine($"№{i,4} | {data[i][0],spaceId} | {data[i][1],spaceStationStart} | {data[i][2],spaceLine} | " +
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
    
    /// <summary>
    /// This method takes in a string and an integer representing the desired size of the resulting string, and returns a truncated version of the input string with an ellipsis appended.
    /// </summary>
    /// <param name="str">The input string to be truncated</param>
    /// <param name="sz">The desired size of the resulting string</param>
    /// <returns>A truncated version of the input string</returns>
    private static string CutString(string str, int sz)
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        if (sz <= 0)
        {
            return "";
        }

        if (sz > str.Length)
        {
            return str;
        }
        StringBuilder result = new StringBuilder();
        sz -= 3;
        for (int i = 0; i < Math.Min(sz, str.Length); ++i)
        {
            result.Append(str[i]);
        }

        result.Append("...");
        return result.ToString();
    }
    
    /// <summary>
    /// The method provides Menu for user to choose one option how to save a file.
    /// </summary>
    /// <param name="data">Which data to write</param>
    private static void SaveDataMenu(string[][] data)
    {
        Console.Clear();
        if (IsNullOrEmpty(data))
        {
            Console.WriteLine("Data is empty or null");
            return;
        }
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
                try
                {
                    SaveAllData(data);
                    isRead = true;
                }
                catch (Exception ex)
                {
                    isRead = false;
                    Console.WriteLine(ex.Message);
                }
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
    
    /// <summary>
    /// This method saves all the data to a CSV file after reformatting it and setting the file path.
    /// </summary>
    /// <param name="data">The data to be written</param>
    private static void SaveAllData(string[][] data)
    {
        if (IsNullOrEmpty(data))
        {
            return;
        }

        try
        {
            string filePath = GetFilePath();
            CsvDataUtility.CsvProcessing.fPath = filePath;
            string[] reformatData = data
                .Select(row => string.Join<string>(";", row))
                .ToArray();

            CsvDataUtility.CsvProcessing.Write(reformatData);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// This method allows the user to save a specific line of data to a CSV file.
    /// It prompts the user to choose the index of the line they want to save, then prompts for a file path.
    /// It then reformats the data and writes it to the CSV file. If there is an error during the process,
    /// it will display an error message.
    /// </summary>
    /// <param name="data">The data from which the string is taken for writing</param>
    private static void SaveOneStringData(string[][] data)
    {
        if (IsNullOrEmpty(data))
        {
            return;
        }
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
                success = false;
            }
        } while (!success);
    }
    
    /// <summary>
    /// The method to provide a menu for Sorting data by coloumn name.
    /// </summary>
    /// <param name="data"></param>
    private static void SortMenu(string[][] data)
    {
        if (IsNullOrEmpty(data))
        {
            Console.WriteLine("Empty data");
            return;
        }
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
                try
                {
                    string[][] result = CsvDataUtility.DataProcessing.Sort(data, "TimeStart", typeSort);
                    PrintData(result);
                    SaveDataMenu(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    isRecievedTrainsition = false;
                }
            } else if (cmd == ConsoleKey.D2) 
            { 
                isRecievedTrainsition = true;
                try
                {
                    string[][] result = CsvDataUtility.DataProcessing.Sort(data, "TimeEnd", typeSort);
                    PrintData(result);
                    SaveDataMenu(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    isRecievedTrainsition = false;
                }
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
}