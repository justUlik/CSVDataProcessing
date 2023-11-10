namespace CSVDataProcessing;

public static class UserDataProcessing
{
    public static string[] GetCsvDataFile()
    {
        Console.Clear();
        bool isRead = false;
        string[] data = null;
        do
        {
            Console.WriteLine("Please enter absolute file path. File exstension should be csv:");
            string filePath = Console.ReadLine();
            try
            {
                CsvDataUtility.CsvProcessing.fPath = filePath;
                data = CsvDataUtility.CsvProcessing.Read();
                isRead = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isRead = false;
            }
        } while (!isRead);

        return data;
    }
}