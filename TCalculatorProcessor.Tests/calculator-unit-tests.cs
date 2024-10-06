using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCalculator.Models;
using static MyCalculator.Models.CalculatorProcessor;

[TestClass]
public class CalculatorProcessorTests
{
	private CalculatorProcessor _calculator = null!;

	[TestInitialize]
	public void Initialize()
	{
		_calculator = new CalculatorProcessor();
	}

	[TestMethod]
	[DataRow("2 + 3", "5")]
	[DataRow("5 - 3", "2")]
	[DataRow("3 * 5", "15")]
	[DataRow("10 / 5", "2")]
	[DataRow("2 + 3 * 4", "14")]
	[DataRow("(2 + 3) * 4", "20")]
	[DataRow("2 + 3 * 4", "14")]
	[DataRow("1111111111 + 2222222222", "3333333333")]
	public void Calculate_Simple_examples_Valid_Inputs(string input, string expected)
	{
		Assert.AreEqual(expected, _calculator.Calculate(input));
	}

	[TestMethod]
	[DataRow("-2", "-2")]
	[DataRow("--2", "2")]
	[DataRow("---2", "-2")]
	[DataRow("----2", "2")]
	[DataRow("-----2", "-2")]
	[DataRow("-(2)", "-2")]
	[DataRow("(-2)", "-2")]
	[DataRow("-(-(-(-(2))))", "2")]
	[DataRow("-(2 + 3)", "-5")]
	[DataRow("-(2 + 3) * 4", "-20")]
	[DataRow("4*-2", "-8")]
	[DataRow("4 * -(2+3)*-1", "20")]
	public void Calculate_UnaryMinus_ReturnsCorrectResult(string input, string expected)
	{
		Assert.AreEqual(expected, _calculator.Calculate(input));
	}
	
	[TestMethod]
	[DataRow("+2", "2")]
	[DataRow("++2", "2")]
	[DataRow("+++2", "2")]
	[DataRow("+(2)", "2")]
	[DataRow("+(2 + 3)", "5")]
	[DataRow("+(2 + 3) * 4", "20")]
	[DataRow("4*+2", "8")]
	[DataRow("4 * +(2+3)*1", "20")]
	public void Calculate_UnaryPlus_ReturnsCorrectResult(string input, string expected)
	{
		Assert.AreEqual(expected, _calculator.Calculate(input));
	}
	
	[TestMethod]
	[DataRow("2.5", "2.5")]
	[DataRow("2.5 + 3.5", "6")]
	[DataRow("2.5 + 3.5 * 2", "9.5")]
	public void Calculate_DecimalNumbers_ReturnsCorrectResult(string input, string expected)
	{
		Assert.AreEqual(expected, _calculator.Calculate(input));
	}

	
	[TestMethod]
	[ExpectedException(typeof(IllegalExpressionSyntaxException))]
	[DataRow("2 +")]
	[DataRow("2 + (")]
	[DataRow("2 + )")]
	[DataRow("")]
	[DataRow("*2")]
	[DataRow("2*")]
	[DataRow("/2")]
	[DataRow("2/")]
	[DataRow("2 + 3 +")]
	[DataRow("(")]
	[DataRow(")")]
	[DataRow("2 + (3")]
	[DataRow("2 + ) * 3")]
	[DataRow("2 + (3 + 4")]
	[DataRow("a + b")]
	[DataRow("a")]
	[DataRow("+")]
	[DataRow("-")]
	[DataRow("2..2")]
	[DataRow("2.2.2")]
	[DataRow("2.2 + 3.3.3")]
	[DataRow("2 + 3.3.")]
	[DataRow("2 + 3.3456.4 + 2")]
	public void Calculate_Invalid_Inputs(string input)
	{
		_calculator.Calculate(input);
	}
}
