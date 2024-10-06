
using System.Data;
using System.Text;

namespace MyCalculator.Models;

public class CalculatorProcessor
{
	public string Calculatee(string input)
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


	private class Operator
	{
		public char Symbol { get; set; }
		public int Precedence { get; set; }
	}

	private class UnaryOperator : Operator
	{
		public Func<double, double>? Operation { get; set; }
	}
	private class BinaryOperator : Operator
	{
		public Func<double, double, double>? Operation { get; set; }
	}

	private static Dictionary<char, Tuple<UnaryOperator?, BinaryOperator?>> allOperators = new()
	{
		{ '+', Tuple.Create<UnaryOperator?, BinaryOperator?>(new UnaryOperator { Symbol = '+', Precedence = 1, Operation = a => a }, 
			new BinaryOperator { Symbol = '+', Precedence = 1, Operation = (a, b) => a + b }) },
		{ '-', Tuple.Create<UnaryOperator?, BinaryOperator?>(new UnaryOperator { Symbol = '-', Precedence = 1, Operation = a => -a },
			new BinaryOperator { Symbol = '-', Precedence = 1, Operation = (a, b) => a - b }) },
		{ '*', Tuple.Create<UnaryOperator?, BinaryOperator?>(null, new BinaryOperator { Symbol = '*', Precedence = 2, Operation = (a, b) => a * b }) },
		{ '/', Tuple.Create<UnaryOperator?, BinaryOperator?>(null, new BinaryOperator { Symbol = '/', Precedence = 2, Operation = (a, b) => a / b }) },
		{ '(', Tuple.Create<UnaryOperator?, BinaryOperator?>(null, new BinaryOperator { Symbol = '(', Precedence = 0, Operation = (a, b) => 0 }) },
	};

	public string Calculate(string input)
	{
		var operators = new Stack<Operator>();
		var operands = new Stack<double>();

		var isNextOpUnary = true;

		var currentPosition = 0;
		while (currentPosition < input.Length)
		{
			char currentChar = input[currentPosition];

			if (currentChar == ' ')
			{
				currentPosition++;
				continue;
			}

			//  Number
			if (char.IsDigit(currentChar))
			{
				isNextOpUnary = false;
				operands.Push(loadNumber(input, ref currentPosition));
				continue;
			}

			if (currentChar == '(')
			{
				operators.Push(allOperators[currentChar].Item2!);
				currentPosition++;
				isNextOpUnary = true;
				continue;
			}

			if (currentChar == ')')
			{
				while (operators.Count > 0 && operators.Peek().Symbol != '(')
				{
					performTopOperation(operators, operands);
				}
				if(operators.Count == 0)
				{
					throw new IllegalExpressionSyntaxException("Illegal expression syntax, missing opening bracket");
				}
				operators.Pop();
				currentPosition++;
				isNextOpUnary = false;
				continue;
			}

			// Operator
			if (allOperators.ContainsKey(currentChar))
            {
                handleNewOperator(operators, operands, ref isNextOpUnary, ref currentPosition, currentChar);
                continue;
            }

            throw new IllegalExpressionSyntaxException("Illegal expression syntax, unknown character");
		}

		while (operators.Count > 0)
		{
			performTopOperation(operators, operands);
		}

		if (operands.Count != 1)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, not enough operands");
		}

		return operands.Pop().ToString();
	}

    private void handleNewOperator(Stack<Operator> operators, Stack<double> operands, ref bool isNextOpUnary, ref int currentPosition, char currentChar)
    {
        if (isNextOpUnary)
        {
            var unaryOp = allOperators[currentChar].Item1;
            if (unaryOp == null)
            {
                throw new IllegalExpressionSyntaxException("Illegal expression syntax, operator cannot be unary");
            }
            operators.Push(unaryOp);
            currentPosition++;
        }
        else
        {
            var binaryOp = allOperators[currentChar].Item2;
            if (binaryOp == null)
            {
                throw new IllegalExpressionSyntaxException("Illegal expression syntax, operator cannot be binary");
            }

            while (operators.Count > 0
                && (binaryOp.Precedence <= operators.Peek().Precedence
                    || operators.Peek() is UnaryOperator))
            {
                performTopOperation(operators, operands);
            }
            operators.Push(binaryOp);
            currentPosition++;
        }
        isNextOpUnary = true;
    }

    private void performTopOperation(Stack<Operator> operators, Stack<double> operands)
    {
        if (operators.Peek() is UnaryOperator)
        {
            performTopUnaryOperation(operators, operands);
        }
        else
        {
            performTopBinaryOperation(operators, operands);
        }
    }

    private void performTopUnaryOperation(Stack<Operator> operators, Stack<double> operands)
	{
		if (operators.Peek().Symbol == '(')
		{
			return;
		}

		if (operators.Count < 1)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, not enough operators");
		}

		var op = operators.Pop() as UnaryOperator;

		if (op is null)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, operator not unary");
		}

		if (operands.Count < 1)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, not enough operands");
		}

		if (op.Operation == null)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, operator not implemented");
		}

		var v1 = operands.Pop();
		operands.Push(op.Operation(v1));
	}

	private static void performTopBinaryOperation(Stack<Operator> operators, Stack<double> operands)
	{
		if (operators.Count == 0)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, not enough operators");
		}

		var op = operators.Pop() as BinaryOperator;

		if (op is null)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, operator not binary");
		}

		if (operands.Count < 2)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, not enough operands");
		}

		if (operands.Count >= 2 && op.Operation != null)
		{
			var v2 = operands.Pop();
			var v1 = operands.Pop();

			if (op.Operation != null)
			{
				operands.Push(op.Operation(v1, v2));
			}
			else
			{
				throw new IllegalExpressionSyntaxException("Illegal expression syntax, operator not implemented");
			}
		}
		else
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, not enough operands, or operator not implemented");
		}

	}

	private double loadNumber(string input, ref int currentPosition)
	{
		var numberInString = new StringBuilder();
		numberInString.Append(input[currentPosition]);
		currentPosition++;
		while (currentPosition < input.Length && (char.IsDigit(input[currentPosition]) || input[currentPosition] == '.'))
		{
			if (input[currentPosition] == '.')
			{
				if (numberInString.ToString().Contains('.'))
				{
					throw new IllegalExpressionSyntaxException("Illegal expression syntax, cannot have multiple decimal points in a number");
				}
			}

			numberInString.Append(input[currentPosition]);
			currentPosition++;
		}

		try
		{
			return Convert.ToDouble(numberInString.ToString());
		}
		catch (Exception)
		{
			throw new IllegalExpressionSyntaxException("Illegal expression syntax, cannot convert to double");
		}
	}

	public class IllegalExpressionSyntaxException : Exception
	{
		public IllegalExpressionSyntaxException(string message) : base(message)
		{
		}
	}
}
