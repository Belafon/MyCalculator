using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MyCalculator.Models;
using MyCalculator.Services;
using MyCalculator.ViewModels;

namespace MyCalculator.ViewModels;

public class CalculatorViewModel : ReactiveObject
{
	private CalculatorProcessor _calculatorProcessor = new();
	private string _calculatorDisplayText = string.Empty;
	public string CalculatorDisplayText
	{
		get => _calculatorDisplayText;
		set
		{
			_calculatorDisplayText = value;
			OnPropertyChanged();
		}
	}

	private string _calculationResultText = string.Empty;
	private const string _resultPrefix = "= ";
	public string CalculationResultText
	{
		get => _calculationResultText;
		set
		{
			_calculationResultText = _resultPrefix + value;
			OnPropertyChanged();
		}
	}

	public ICommand AddTextToDisplayCommand => new ButtonCommand(OnAddTextToDisplay);
	public ICommand ExecuteCalculationCommand => new RelayCommand(
		OnExecuteCalculation,
		parameter => true
	);
	public ICommand RemoveOneCharacterCommand => new RelayCommand(
		OnRemoveOneCharacter,
		parameter => true
	);

	public ICommand ClearDisplayCommand => new RelayCommand(
		OnClearDisplay,
		parameter => true
	);

	public HistoryListViewModel HistoryListViewModel { get; }
	private CalculationRecords _historyItems;
	public CalculatorViewModel(CalculationRecords historyItems)
	{
		_historyItems = historyItems;
		
		HistoryListViewModel = new HistoryListViewModel(_historyItems);
		HistoryListViewModel.PropertyChanged += (sender, args) =>
		{
			if (args.PropertyName == nameof(ViewModels.HistoryListViewModel.SelectedHistoryItem))
			{
				CalculatorDisplayText = HistoryListViewModel.SelectedHistoryItem?.Calculation ?? string.Empty;
				CalculationResultText = HistoryListViewModel.SelectedHistoryItem?.Result ?? string.Empty;
			}
		};

	}


	private void OnExecuteCalculation(object? obj)
	{
		string? result;
		try
		{
			result = _calculatorProcessor.Calculate(CalculatorDisplayText);
		}
		catch (IllegalExpressionSyntaxException ex)
		{
			CalculationResultText = ex.Message;
			return;
		}
		CalculationResultText = result;

		_historyItems.AddRecord(CalculatorDisplayText, result);

		HistoryListViewModel.ScrollToBottom?.Invoke();
	}

	private void OnAddTextToDisplay(string textToInsert)
	{
		CalculatorDisplayText += textToInsert;
	}

	private void OnClearDisplay(object? obj)
	{
		CalculatorDisplayText = string.Empty;
	}

	private void OnRemoveOneCharacter(object? obj)
	{
		if (CalculatorDisplayText.Length > 0)
		{
			CalculatorDisplayText = CalculatorDisplayText.Remove(CalculatorDisplayText.Length - 1);
		}
	}
}