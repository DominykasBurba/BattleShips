namespace BattleShips.Services;

using BattleShips.Domain;
using BattleShips.Domain.Ships;

/// <summary>
/// Adapter that adapts the existing PlacementService (Builder + Director)
/// to the simple IFleetPlacer interface.
/// </summary>
public class RandomFleetPlacerAdapter(PlacementService placementService) : IFleetPlacer
{
    public void PlaceFleet(Board board)
    {
        placementService.RandomizeFleet(board);
    }

    public void SetShipType(ShipType shipType)
    {
        placementService.SetShipType(shipType);
    }

    public void SetShipSkin(ShipSkin shipSkin)
    {
        placementService.SetShipSkin(shipSkin);
    }
}


