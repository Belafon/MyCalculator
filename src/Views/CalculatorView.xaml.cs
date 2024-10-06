using System.Windows;
using System.Windows.Controls;
using MyCalculator.Services;
using MyCalculator.ViewModels;

namespace MyCalculator.Views;

public partial class CalculatorView : UserControl
{
	public CalculatorView()
	{
		InitializeComponent();
		LoadHistory();
	}

	private async void LoadHistory()
	{
		var historyItems = await CalculationRecordsLoader.LoadAsync();
		DataContext = new CalculatorViewModel(historyItems);
		
	}
}