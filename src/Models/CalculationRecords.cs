using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MyCalculator.Models;
using MyCalculator.Services;

public class CalculationRecords
{
    [Newtonsoft.Json.JsonProperty("Items")]
    public ObservableCollection<CalculationRecordModel> Items { get; } = new();
    public void AddRecord(string calculation, string result)
    {
        Items.Add(new CalculationRecordModel(calculation, result));
    }

    public CalculationRecords()
    {
        Items.CollectionChanged += async (sender, args) =>
        {
            await CalculationRecordsLoader.SaveAsync(this);
        };
    }
}
