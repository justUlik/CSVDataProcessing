namespace CsvDataUtility;

/// <summary>
/// Class that organizes ways to make a selection from data or sort it.
/// </summary>
public class DataProcessing
{
    
    /// <summary>
    /// private method to get index by column name
    /// </summary>
    /// <param name="colomnName"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// This is a method named "GetSelectionByValueRow" that takes in a 2-dimensional array of strings named "data",
    /// two column names, and two value selections.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="colomnName"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static string[] GetUniqueValueInRow(string[][] data, string colomnName)
    {
        if (data is null || (data is not null && data.GetLength(0) == 0))
        {
            throw new ArgumentNullException("Data is empty or null");
        }
        int getIdx = GetIdxByColoumnName(colomnName);
        if (getIdx == -1)
        {
            throw new ArgumentException("No such rowName in data format");
        }

        try
        {
            string[] allValuesRow = new string[data.GetLength(0) - 2];
            for (int i = 2; i < data.GetLength(0); ++i)
            {
                allValuesRow[i - 2] = data[i][getIdx];
            }

            return allValuesRow.Distinct().ToArray();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    
    public static string[][] GetSelectionByValueRow(string[][] data, string columnName1, string valueSelection1,
        string columnName2, string valueSelection2)
    {
        if (data is null || (data is not null && data.GetLength(0) == 0))
        {
            throw new ArgumentNullException("Data is null of empty");
        }
        int getIdx1 = GetIdxByColoumnName(columnName1);
        int getIdx2 = GetIdxByColoumnName(columnName2);
        if (getIdx1 == -1 || getIdx2 == -2)
        {
            throw new ArgumentException("No such rowName in data format");
        }

        try
        {
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
        catch (ArgumentException ex)
        {
            throw new ArgumentException(ex.Message);
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new IndexOutOfRangeException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static string[][] GetSelectionByValueRow(string[][] data, string coloumnName, string valueName)
    {
        if (data is null || (data is not null && data.GetLength(0) == 0))
        {
            throw new ArgumentNullException("Data is empty");
        }

        try
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
        catch (ArgumentException ex)
        {
            throw new ArgumentException(ex.Message);
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new IndexOutOfRangeException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public static string[][] Sort(string[][] data, string coloumn, string typeSort)
    {
        if (data is null || (data is not null && data.GetLength(0) == 0))
        {
            throw new ArgumentNullException("Empty or null data");
        }

        try
        {
            string[][] dataNoHeading = new string[data.GetLength(0) - 2][];
            for (int i = 2; i < data.GetLength(0); ++i)
            {
                dataNoHeading[i - 2] = (string[])data[i].Clone();
            }

            string[][] result = new string[data.GetLength(0)][];
            string[][] resultSort;
            if (typeSort == "MergeSort")
            {
                resultSort = MergeSortByDateTime(dataNoHeading, coloumn);

            }
            else if (typeSort == "QuickSort")
            {
                QuickSort(dataNoHeading, 0, dataNoHeading.GetLength(0) - 1, GetIdxByColoumnName(coloumn));
                resultSort = dataNoHeading;
            }
            else
            {
                throw new ArgumentException("No such option to sort");
            }

            for (int i = 0; i < 2; ++i)
            {
                result[i] = (string[])data[i].Clone();
            }

            for (int i = 0; i < resultSort.GetLength(0); ++i)
            {
                result[i + 2] = resultSort[i];
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    private static string[][] MergeSortByDateTime(string[][] data, string colomnName)
    {
        int columnIndex = GetIdxByColoumnName(colomnName);
        if (columnIndex == -1)
        {
            throw new ArgumentException("No such column");
        }

        try
        {
            if (data.Length <= 1)
            {
                return data;
            }

            int middle = data.Length / 2;
            string[][] left = new string[middle][];
            string[][] right = new string[data.Length - middle][];

            for (int i = 0; i < middle; i++)
            {
                left[i] = data[i];
            }

            for (int i = middle; i < data.Length; i++)
            {
                right[i - middle] = data[i];
            }

            left = MergeSortByDateTime(left, colomnName);
            right = MergeSortByDateTime(right, colomnName);

            return MergeByDateTime(left, right, columnIndex);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private static string[][] MergeByDateTime(string[][] left, string[][] right, int columnIndex)
    {
        if (left is null || (left is not null && left.GetLength(0) == 0))
        {
            throw new ArgumentException("Empty or null data");
        }
        if (right is null || (right is not null && right.GetLength(0) == 0))
        {
            throw new ArgumentException("Empty or null data");
        }

        try
        {
            int leftIndex = 0, rightIndex = 0, mergeIndex = 0;

            int mergedLength = left.Length + right.Length;
            string[][] mergedArray = new string[mergedLength][];

            while (leftIndex < left.Length && rightIndex < right.Length)
            {
                DateTime leftDateTime = DateTime.Parse(left[leftIndex][columnIndex]);
                DateTime rightDateTime = DateTime.Parse(right[rightIndex][columnIndex]);

                if (leftDateTime <= rightDateTime)
                {
                    mergedArray[mergeIndex] = left[leftIndex];
                    leftIndex++;
                }
                else
                {
                    mergedArray[mergeIndex] = right[rightIndex];
                    rightIndex++;
                }

                mergeIndex++;
            }

            while (leftIndex < left.Length)
            {
                mergedArray[mergeIndex] = left[leftIndex];
                leftIndex++;
                mergeIndex++;
            }

            while (rightIndex < right.Length)
            {
                mergedArray[mergeIndex] = right[rightIndex];
                rightIndex++;
                mergeIndex++;
            }

            return mergedArray;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    private static void QuickSort(string[][] data, int low, int high, int columnToSortBy)
    {
        if (data is null || (data is not null && data.GetLength(0) == 0))
        {
            throw new ArgumentException("Data is null or empty");
        }

        try
        {
            if (low < high)
            {
                int partitionIndex = Partition(data, low, high, columnToSortBy);

                // Recursively sort the two partitions
                QuickSort(data, low, partitionIndex - 1, columnToSortBy);
                QuickSort(data, partitionIndex + 1, high, columnToSortBy);
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private static int Partition(string[][] data, int low, int high, int columnToSortBy)
    {
        try
        {
            string pivot = data[high][columnToSortBy];
            DateTime pivotDate = DateTime.Parse(pivot);

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                string currentDateValue = data[j][columnToSortBy];
                DateTime currentDate = DateTime.Parse(currentDateValue);

                if (currentDate <= pivotDate)
                {
                    i++;

                    // Swap elements
                    string[] temp = data[i];
                    data[i] = data[j];
                    data[j] = temp;
                }
            }

            // Swap pivot with element at i+1
            string[] pivotTemp = data[i + 1];
            data[i + 1] = data[high];
            data[high] = pivotTemp;

            return i + 1;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}