using BattleShips.Services;

namespace BattleShips.Domain.Commands;


public class ChatInvoker
{
    private ICommand? _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    public void Run()
    {
        if (_command != null)
        {
            _command.Execute();
        }
    }
    public void RunCommand(ICommand command)
    {
        command.Execute();
    }
}










