namespace BattleShips.Domain.Commands;

/// <summary>
/// Command interface in Command Design Pattern.
/// Defines the interface for executing commands.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Executes the command.
    /// </summary>
    void Execute();

    /// <summary>
    /// Undoes the command (if supported).
    /// </summary>
    void Undo();
}








