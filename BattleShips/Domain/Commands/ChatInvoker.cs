using BattleShips.Services;

namespace BattleShips.Domain.Commands;

/// <summary>
/// Invoker class in Command Design Pattern.
/// Holds a command and executes it when Run() is called.
/// Separates the command execution from the client and receiver.
/// </summary>
public class ChatInvoker
{
    private ICommand? _command;

    /// <summary>
    /// Sets the command to be executed.
    /// </summary>
    /// <param name="command">The command to execute</param>
    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    /// <summary>
    /// Executes the command (if one is set).
    /// This is the Run() method that triggers command execution.
    /// </summary>
    public void Run()
    {
        if (_command != null)
        {
            _command.Execute();
        }
    }

    /// <summary>
    /// Executes a command directly without storing it.
    /// Convenience method for one-time execution.
    /// </summary>
    /// <param name="command">The command to execute</param>
    public void RunCommand(ICommand command)
    {
        command.Execute();
    }
}


