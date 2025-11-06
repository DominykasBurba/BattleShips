namespace BattleShips.Services;

using BattleShips.Domain;

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
}


