using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace MyCalculator.Services;

public class CalculationRecordsLoader
{
    private const string FileName = "calculationRecords.json";

    private static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MyCalculator",
        FileName);

    public static async Task SaveAsync(CalculationRecords records)
    {
        var directory = Path.GetDirectoryName(FilePath);
        try{
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

        var json = JsonConvert.SerializeObject(records);
        await File.WriteAllTextAsync(FilePath, contents: json);
        } catch (Exception){
            return;
        }
    }

    public static async Task<CalculationRecords> LoadAsync()
    {
        if (!File.Exists(FilePath))
        {
            return new CalculationRecords();
        }

        try{
        var json = await File.ReadAllTextAsync(FilePath);
            return JsonConvert.DeserializeObject<CalculationRecords>(json) ?? new CalculationRecords();
        } catch (Exception){
            return new CalculationRecords();
        }
    }
}