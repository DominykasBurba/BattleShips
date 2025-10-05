namespace BattleShips.Domain;

public abstract class Player
{
    public string Name { get; }
    public Board Board { get; }
    public PlayerKind Kind { get; }

    protected Player(string name, PlayerKind kind, int boardSize = 10)
    { Name = name; Kind = kind; Board = new Board(boardSize); }

    public virtual Position ChooseTarget(Board enemyBoard, HashSet<Position> tried) =>
        throw new NotSupportedException("Human chooses via UI");
}