namespace BattleShips.Domain.Ships;

using BattleShips.Domain;
using BattleShips.Domain.Ships.Modern;

/// <summary>
/// Abstract base class for all ships. Implements IShip interface.
/// This is the Component in the Decorator pattern.
/// </summary>
public abstract class ShipBase : IShip
{
    public abstract string Name { get; }
    public abstract int Length { get; }

    public Position Start { get; private set; }
    public Orientation Orientation { get; private set; }
    public virtual ShipSkin Skin => ShipSkin.Default;

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

    public virtual bool RegisterHit(Position p)
    {
        var added = _hits.Add(p);
        return added;
    }
    public virtual bool IsSunk => _hits.Count >= Length;

    /// <summary>
    /// Gets the number of hits this ship has taken.
    /// </summary>
    public int HitCount => _hits.Count;

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