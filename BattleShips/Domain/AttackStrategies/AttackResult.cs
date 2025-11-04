namespace BattleShips.Domain.AttackStrategies;

/// <summary>
/// Represents the result of an attack action.
/// Can contain single or multiple shot results.
/// </summary>
public class AttackResult
{
    public List<ShotResult> Results { get; }
    public List<Position> Positions { get; }
    public bool ShouldEndTurn { get; }

    public AttackResult(List<ShotResult> results, List<Position> positions, bool shouldEndTurn)
    {
        Results = results;
        Positions = positions;
        ShouldEndTurn = shouldEndTurn;
    }

    public AttackResult(ShotResult result, Position position, bool shouldEndTurn)
    {
        Results = new List<ShotResult> { result };
        Positions = new List<Position> { position };
        ShouldEndTurn = shouldEndTurn;
    }

    // Convenience property for single shot results
    public ShotResult? SingleResult => Results.Count == 1 ? Results[0] : null;
}

