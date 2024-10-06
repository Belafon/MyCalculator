using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyCalculator.ViewModels;

public class ReactiveObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}