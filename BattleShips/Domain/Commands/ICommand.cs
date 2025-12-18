namespace BattleShips.Domain.Commands;

public interface ICommand
{

    void Execute();

    void Undo();
}










