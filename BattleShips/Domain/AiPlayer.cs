namespace BattleShips.Domain;

public class AiPlayer : Player
{
    private readonly Random _rng = new();
    private readonly HashSet<Position> _tried = new();

    public AiPlayer(string name, int boardSize = 10) : base(name, PlayerKind.AI, boardSize) {}

    public override Position ChooseTarget(Board enemyBoard, HashSet<Position> _)
    {
        while (true)
        {
            var p = new Position(_rng.Next(enemyBoard.Size), _rng.Next(enemyBoard.Size));
            if (_tried.Add(p)) return p;
        }
    }
}