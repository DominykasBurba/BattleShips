namespace BattleShips.Services;

using BattleShips.Domain;

/// <summary>
/// Adapter target interface: a simple fleet placement API.
/// </summary>
public interface IFleetPlacer
{
    void PlaceFleet(Board board);
}


