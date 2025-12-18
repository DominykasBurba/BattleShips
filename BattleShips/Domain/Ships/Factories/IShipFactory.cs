namespace BattleShips.Domain.Ships.Factories;

public interface IShipFactory
{
    IShip CreateBattleship(Position start, Orientation orientation);
    IShip CreateSubmarine(Position start, Orientation orientation);
    IShip CreateDestroyer(Position start, Orientation orientation);
    IShip CreateCruiser(Position start, Orientation orientation);
    
    IShip CreateShip(ShipKind kind, Position start, Orientation orientation);
}
