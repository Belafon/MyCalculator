using System.Windows.Controls;
using MyCalculator.ViewModels;

namespace MyCalculator.Views;
public partial class HistoryListView : UserControl
{
    public HistoryListView()
    {
        InitializeComponent();

        DataContextChanged += (s, e) =>
        {
            var viewModel = DataContext as HistoryListViewModel;
            if (viewModel != null)
            {
                viewModel.ScrollToBottom = () =>
                {
                    if (HistoryListBox.Items.Count > 0)
                    {
                        var lastItem = HistoryListBox.Items[^1];
                        HistoryListBox.ScrollIntoView(lastItem);
                    }
                };
            }
        };
    }
}