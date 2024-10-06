
using System.Data;

namespace MyCalculator.Models;

public class CalculatorProcessor
{
	public string Calculate(string input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return "0";
		}

		try
		{
			var result = new DataTable().Compute(input, null);
			if (result is null)
			{
				return "0";
			}
			result = Math.Round(Convert.ToDouble(result), 10);
			var stringResult = result.ToString();
			if (stringResult is null)
			{
				return "0";
			}
			return stringResult; 
		}
		catch (SyntaxErrorException)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax");
		}
		catch (Exception)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax");
		}
	}
}

public class IllegalExpressionSyntaxException : Exception
{
	public IllegalExpressionSyntaxException(string message) : base(message)
	{
	}
}