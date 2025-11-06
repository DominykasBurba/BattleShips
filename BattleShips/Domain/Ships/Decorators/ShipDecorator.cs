namespace BattleShips.Domain.Ships.Decorators;

/// <summary>
/// Abstract Decorator base class in the Decorator pattern.
/// Wraps an IShip and delegates all calls to it while allowing subclasses
/// to add or modify behavior (special abilities, power-ups, etc.).
/// </summary>
public abstract class ShipDecorator(IShip ship) : IShip
{
    protected readonly IShip WrappedShip = ship;

    // Delegate all IShip members to the wrapped ship
    public virtual string Name => WrappedShip.Name;
    public virtual int Length => WrappedShip.Length;
    public virtual Position Start => WrappedShip.Start;
    public virtual Orientation Orientation => WrappedShip.Orientation;
    public virtual bool IsSunk => WrappedShip.IsSunk;
    public virtual ShipKind Kind => WrappedShip.Kind;
    public virtual ShipSkin Skin => WrappedShip.Skin;

    public virtual IEnumerable<Position> Cells() => WrappedShip.Cells();
    public virtual void Reposition(Position start, Orientation orientation)
        => WrappedShip.Reposition(start, orientation);
    public virtual bool RegisterHit(Position p) => WrappedShip.RegisterHit(p);
    public virtual int HitCount => WrappedShip.HitCount;
}
