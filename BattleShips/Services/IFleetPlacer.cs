namespace BattleShips.Services;

using BattleShips.Domain;
using BattleShips.Domain.Ships;

/// <summary>
/// Adapter target interface: a simple fleet placement API.
/// </summary>
public interface IFleetPlacer
{
    void PlaceFleet(Board board);
    void SetShipType(ShipType shipType);
    void SetShipSkin(ShipSkin shipSkin);
}


