namespace DataManager.Core.Database.Data;

public class FileLocator
{
    public static (string csvFilePath, string xlsxFilePath) GetFilePaths()
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string sourceDirectory = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\..\..\"));
        string csvFilePath = Path.Combine(sourceDirectory, "Data_Manager", "DataManager.Core", "Database", "Data", "ModelOne_100k_rows.csv");
        string xlsxFilePath = Path.Combine(sourceDirectory, "Data_Manager", "DataManager.Core", "Database", "Data", "ModelTwo_100k_rows.xlsx");

        return (csvFilePath, xlsxFilePath);
    }
}