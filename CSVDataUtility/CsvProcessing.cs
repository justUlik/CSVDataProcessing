namespace CsvDataUtility;

using System;
using System.IO;


/// <summary>
/// Class for reading and writing csv files.
/// </summary>
public class CsvProcessing
{
    private static string _fPath;
    
    public static string fPath 
    { 
        get => _fPath; 
        set 
        { 
            if (string.IsNullOrEmpty(value)) 
            { 
                throw new ArgumentNullException(nameof(value));
            }
 
            _fPath = value; 
        } 
    } 

    /// <summary>
    /// The method checks the format of the string and sets an exception in case of an error.
    /// It accepts a string and a reference to the string to set the exception.
    /// If the string is empty or contains no data, the exception is set to “File is null or empty".
    /// If the number of fields in a row is not equal to 8, the exception is set to “The file is not in required format".
    /// If the field cannot be converted to an integer, the exception is set to “There is an error in the file format".
    /// </summary>
    /// <param name="line"></param>
    /// <param name="forException"></param>
    /// <returns></returns>
    private static bool CheckLineFormat(string line, ref string forException)
    {
        string[] fields = line.Split(";");
        try
        {
            // CHECK!
            if (!(fields?.Length > 0))
            {
                forException = "File null or empty";
            }
            if (fields.Length != 8)
            {
                forException = "File not require format";
                return false;
            }
            if (!int.TryParse(fields[0], out _))
            {
                forException = "Format error inside file";
                return false;
            }
            if (!DateTime.TryParse(fields[3], out _))
            {
                forException = "Format error inside file";
                return false;
            }
            if (!DateTime.TryParse(fields[5], out _))
            {
                forException = "Format error inside file";
                return false;
            }
            if (!long.TryParse(fields[6], out _))
            {
                forException = "Format error inside file";
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            switch (ex.GetType().Name)
            {
                case "ArgumentNullException":
                    forException = "Null element inside file";
                    break;
                case "IndexOutOfRangeException":
                    forException = "Not correct line format inside file";
                    break;
                default:
                    forException = "Unsupported data format";
                    break;
            }

            return false;
        }
    }

    /// <summary>
    /// The method checks whether the string matches the specified format. If the string is empty or does not match
    /// the format, the method returns false. If the string length is less than 2, the method also returns false.
    /// The method iterates through all the rows in the array and checks them for compliance with the format.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool CheckHeading(string[][] data)
    {
        if (data is null || (data is not null && data.GetLength(0) == 0))
        {
            return false;
        }

        try
        {
            if (data.GetLength(0) < 2)
            {
                return false;
            }

            foreach (var line in data)
            {
                if (line is null || (line is not null && line.GetLength(0) == 0))
                {
                    return false;
                }

                if (line.Length != 8)
                {
                    return false;
                }
            }

            string[] firstReference = new[]
                { "ID", "StationStart", "Line", "TimeStart", "StationEnd", "TimeEnd", "global_id", "" };
            string[] secondReference = new[]
            {
                "Локальный идентификатор", "Станция отправления", "Направление Аэроэкспресс",
                "Время отправления со станции", "Конечная станция направления Аэроэкспресс",
                "Время прибытия на конечную станцию направления Аэроэкспресс", "global_id", ""
            };

            for (int i = 0; i < 8; ++i)
            {
                if (data[0][i] != firstReference[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < 8; ++i)
            {
                if (data[1][i] != secondReference[i])
                {
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

    
    /// <summary>
    /// The Read() method is used to read data from a file. If the path to the file is not specified or the file
    /// does not exist, an exception is thrown. If the specified path is not absolute, another exception is thrown.
    /// After checking the path to the file, all lines from the file are read and an array of lines is returned.
    /// If the file is empty or does not exist, an empty array is returned.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    /// <exception cref="IOException"></exception>
    public static string[] Read()
    {
        if (string.IsNullOrEmpty(fPath))
        {
            throw new ArgumentNullException(nameof(fPath));
        }

        if (!File.Exists(fPath))
        {
            throw new ArgumentNullException("File does not exist");
        }

        if (Path.GetFullPath(fPath) != fPath)
        {
            throw new ArgumentException("Not absolute path given");
        }

        try
        {
            string[] lines = File.ReadAllLines(fPath);
            if (lines is null || (lines is not null && lines.Length == 0))
            {
                throw new ArgumentNullException("Empty file given");
            }

            if (lines.Length < 3)
            {
                throw new Exception("The file format is not observed");
            }
            
            for (int i = 0; i < 2; ++i)
            {
                lines[i] = lines[i].Replace("\"", "");
            }
        
            for (int i = 2; i < lines.Length; ++i)
            {
                string forException = "";
                lines[i] = lines[i].Replace("\"", "");
                if (!CheckLineFormat(lines[i], ref forException))
                {
                    throw new ArgumentException(forException);
                }
            }
        
            return lines;
        }
        catch (Exception ex)
        {
            switch (ex.GetType().Name)
            {
                case "ArgumentException":
                    throw new ArgumentException("Such file is invalid.");
                    break;
                case "IOException":
                    throw new IOException("Error while openning a file or reading data from.");
                    break;
                default:
                    throw new ("An unexpected error occurred.");
                    break;
            }
        }
    }

    
    /// <summary>
    /// The RefactorData method accepts an array of data strings as input and returns a formatted two-dimensional array
    /// of strings. If the input array is empty or null, null is returned.
    /// A new string[] array is created for each element of the input array, which is then filled with values after
    /// splitting the input string by the separator ‘;’. If the input array does not contain null values and all rows
    /// have been successfully converted to a two-dimensional array, the result is returned.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string[][]? RefactorData(string[] data)
    {
        if (data is null)
        {
            return null;
        }

        try
        {
            string[][] result = new string[data.Length][];
            for (int i = 0; i < data.Length; ++i)
            {
                string[] line = data[i].Split(';');
                if (line is null)
                {
                    break;
                }

                result[i] = line;
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    /// <summary>
    /// The Write method is intended for writing data to a file. If the data is empty or null, the method does nothing.
    /// If the path to the new file is not specified, an ArgumentNullException is thrown.
    /// If the file does not exist, it is created and the method continues to work.
    /// Then the data is written to the file and closed.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="nPath"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public static void Write(string data, string nPath)
    {
        if (data is null || (data is not null && data.Length == 0))
        {
            return;
        }
        if (string.IsNullOrEmpty(nPath))
        {
            throw new ArgumentNullException(nameof(nPath));    
        }

        if (!File.Exists(nPath))
        {
            var file = File.Create(nPath);
            file.Close();
        }

        try
        {
            if (File.Exists(nPath))
            {
                using (StreamWriter file = File.AppendText(nPath))
                {
                    file.WriteLine(data);
                }
            }
            else
            {
                throw new FileNotFoundException("File not found", nPath);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// The Write method is used to write data to a file. It accepts a two-dimensional data array and a file path as
    /// input. If the data is empty or null, an ArgumentNullException is thrown with the message “Given data is empty or
    /// null". If the path to the file is empty, an exception is thrown with the message “File path is null or empty".
    /// Then it checks if the file exists. If not, it is created. After that, the data is written to the file.
    /// </summary>
    /// <param name="data"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="Exception"></exception>
    public static void Write(string[] data)
    {
        if (data is null || (data is not null && data.GetLength(0) == 0))
        {
            throw new ArgumentNullException("Given data is empty or null");
        }
        if (string.IsNullOrEmpty(fPath))
        {
            throw new ArgumentNullException(nameof(fPath));    
        }
        
        if (!File.Exists(fPath))
        {
            var file = File.Create(fPath);
            file.Close();
        }

        try
        {
            if (File.Exists(fPath))
            {
                File.WriteAllLines(fPath, data);
            }
            else
            {
                throw new FileNotFoundException("File not found", fPath);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        
    }
    
}