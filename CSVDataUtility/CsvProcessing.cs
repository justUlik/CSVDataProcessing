namespace CsvDataUtility;

using System;
using System.IO;

public class CsvProcessing
{
    public static string fPath;

    private static bool CheckLineFormat(string line, ref string forException)
    {
        string[] fields = line.Split(";");
        if (fields is null || (fields is not null && fields.Length != 8))
        {
            forException = "File null or empty";
            return false;
        }

        try
        {
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

        string[] lines = File.ReadAllLines(fPath);

        if (lines is null || (lines is not null && lines.Length == 0))
        {
            throw new ArgumentNullException("Empty file given");
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
                throw new ArgumentNullException(forException);
            }
        }
        
        return lines;
    }

    public static string[][] RefactorData(string[] data)
    {
        if (data is null)
        {
            return null;
        }

        string[][] result = new string[data.Length][];

        try
        {
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
            Console.WriteLine(ex);
            throw;
        }
    }
    
    public static void Write(string data, string nPath)
    {
        if (string.IsNullOrEmpty(nPath))
        {
            throw new ArgumentNullException(nameof(nPath));    
        }

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

    public static void Write(string[] data)
    {
        if (string.IsNullOrEmpty(fPath))
        {
            throw new ArgumentNullException(nameof(fPath));    
        }

        if (File.Exists(fPath))
        {
            File.WriteAllLines(fPath, data);
        }
        else
        {
            throw new FileNotFoundException("File not found", fPath);
        }
    }
}