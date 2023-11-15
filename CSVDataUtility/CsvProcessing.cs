namespace CsvDataUtility;

using System;
using System.IO;

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
            if (data[0] != firstReference || data[1] != secondReference)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

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