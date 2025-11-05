namespace BattleShips.Domain.Ships;

using BattleShips.Domain;
using BattleShips.Domain.Ships.Modern;

public abstract class ShipBase
{
    public abstract string Name { get; }
    public abstract int Length { get; }

    public Position Start { get; private set; }
    public Orientation Orientation { get; private set; }

    protected readonly HashSet<Position> _hits = new();

    protected ShipBase(Position start, Orientation orientation)
    {
        Start = start; Orientation = orientation;
    }

    public IEnumerable<Position> Cells()
    {
        for (int i = 0; i < Length; i++)
            yield return Orientation == Orientation.Horizontal
                ? new Position(Start.Row, Start.Col + i)
                : new Position(Start.Row + i, Start.Col);
    }

    public virtual void Reposition(Position start, Orientation orientation)
    {
        Start = start; Orientation = orientation;
        _hits.Clear();
    }

    public virtual void RegisterHit(Position p) => _hits.Add(p);
    public virtual bool IsSunk => _hits.Count >= Length;

    public ShipKind Kind => this switch
    {
        Battleship => ShipKind.Battleship,
        ModernBattleship => ShipKind.Battleship,
        Submarine => ShipKind.Submarine,
        ModernSubmarine => ShipKind.Submarine,
        Destroyer => ShipKind.Destroyer,
        ModernDestroyer => ShipKind.Destroyer,
        Cruiser => ShipKind.Cruiser,
        ModernCruiser => ShipKind.Cruiser,
        _ => throw new InvalidOperationException("Unknown ship type")
    };
}