using System.Windows.Input;

namespace Silence.Infrastructure.Utils;

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<Task>? _executeAsync;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute
            ?? throw new ArgumentNullException(nameof(execute));

        _canExecute = canExecute;
    }

    public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
    {
        _executeAsync = executeAsync
            ?? throw new ArgumentNullException(nameof(executeAsync));

        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter)
    {
        // not best solution in terms of exceptions handling
        // TODO: make this safe with exceptions handling

        if (_executeAsync is not null)
        {
            _executeAsync();
        }
        else if (_execute is not null)
        {
            _execute();
        }
    }
}