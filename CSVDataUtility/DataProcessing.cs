namespace CsvDataUtility;

public class DataProcessing
{
    private static int GetIdxByColoumnName(string colomnName)
    {
        int getIdx = colomnName switch
        {
            "ID" => 0,
            "StationStart" => 1,
            "Line" => 2,
            "TimeStart" => 3,
            "StationEnd" => 4,
            "TimeEnd" => 5,
            "global_id" => 6,
            _ => -1
        };
        return getIdx;
    }

    public static string[][] GetSelectionByValueRow(string[][] data, string columnName1, string valueSelection1,
        string columnName2, string valueSelection2)
    {
        int getIdx1 = GetIdxByColoumnName(columnName1);
        int getIdx2 = GetIdxByColoumnName(columnName2);
        if (getIdx1 == -1 || getIdx2 == -2)
        {
            throw new ArgumentException("No such rowName in data format");
        }

        string[][] result = new string[data.GetLength(0)][];
        
        int cnt = 0;
        for (int i = 0; i < 2; ++i)
        {
            result[cnt++] = data[i];
        }
        for (int i = 0; i < data.GetLength(0); ++i)
        {
            if (data[i][getIdx1] == valueSelection1 && data[i][getIdx2] == valueSelection2)
            {
                result[cnt++] = data[i];
            }
        }
        Array.Resize(ref result, cnt);
        return result;
    }
    public static string[] GetUniqueValueInRow(string[][] data, string colomnName)
    {
        int getIdx = GetIdxByColoumnName(colomnName);
        if (getIdx == -1)
        {
            throw new ArgumentException("No such rowName in data format");
        }

        // здесь надо будет обработать -2
        string[] allValuesRow = new string[data.GetLength(0) - 2];
        for (int i = 2; i < data.GetLength(0); ++i)
        {
            allValuesRow[i - 2] = data[i][getIdx];
        }

        return allValuesRow.Distinct().ToArray();
    }

    public static string[][] GetSelectionByValueRow(string[][] data, string coloumnName, string valueName)
    {
        string[][] result = new string[data.GetLength(0)][];
        int getIdx = GetIdxByColoumnName(coloumnName);
        if (getIdx == -1)
        {
            throw new ArgumentException("No such rowName in data format");
        }

        int cnt = 0;
        for (int i = 0; i < 2; ++i)
        {
            result[cnt++] = data[i];
        }
        for (int i = 0; i < data.GetLength(0); ++i)
        {
            if (data[i][getIdx] == valueName)
            {
                result[cnt++] = data[i];
            }
        }
        Array.Resize(ref result, cnt);
        return result;
    }
}