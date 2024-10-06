using System.Windows.Input;

public class ButtonCommand : ICommand
{
    private readonly Action<string> _execute;

    public ButtonCommand(Action<string> execute)
    {
        _execute = execute;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if(parameter is not null
            && parameter is string parameterString)
        {
            _execute(parameterString);
        }
    }

    public event EventHandler? CanExecuteChanged = delegate { };
}