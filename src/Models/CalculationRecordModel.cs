namespace MyCalculator.Models;

public class CalculationRecordModel
{
    [Newtonsoft.Json.JsonProperty("Calculation")]
    public string Calculation { get; init; }
    [Newtonsoft.Json.JsonProperty("Result")]
    public string Result { get; init; }

    public CalculationRecordModel(string calculation, string result)
    {
        Calculation = calculation;
        Result = result;
    }
	public override string ToString() => $"{Calculation} = {Result}";
}