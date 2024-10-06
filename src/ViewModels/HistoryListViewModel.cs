using System.Collections.ObjectModel;
using MyCalculator.Models;

namespace MyCalculator.ViewModels;

public class HistoryListViewModel : ReactiveObject
{
	private CalculationRecordModel? _selectedHistoryItem;

	public CalculationRecordModel? SelectedHistoryItem
	{
		get => _selectedHistoryItem;
		set
		{
			_selectedHistoryItem = value;
			OnPropertyChanged();
		}
	}

	private ObservableCollection<CalculationRecordModel> _historyItems;
	public Action? ScrollToBottom { get; set; }

	public ObservableCollection<CalculationRecordModel> HistoryItems
	{
		get => _historyItems;
		init
		{
			_historyItems = value;
			OnPropertyChanged(nameof(HistoryItems));
		}
	}


	public HistoryListViewModel(CalculationRecords records)
	{
		_historyItems = records.Items;
		HistoryItems.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(HistoryItems));
		_selectedHistoryItem = null;

		if (HistoryItems.Count > 0)
		{
			SelectedHistoryItem = HistoryItems[0];
		}
	}
}