namespace BattleShips.Domain.AttackStrategies;

/// <summary>
/// Concrete strategy for 3x3 area salvo attacks.
/// Fires shots at a 3x3 grid centered on the specified position.
/// </summary>
public class Salvo3x3Strategy : IAttackStrategy
{
    public AttackResult ExecuteAttack(GameSession session, Position centerPosition)
    {
        if (session.Phase != Phase.Playing)
            return new AttackResult(ShotResult.Invalid, centerPosition, true);

        var results = new List<ShotResult>();
        var positions = new List<Position>();
        var boardSize = session.Opponent.Board.Size;

        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                var targetRow = centerPosition.Row + dr;
                var targetCol = centerPosition.Col + dc;

                // Skip if Exout of bounds
                if (targetRow < 0 || targetRow >= boardSize || targetCol < 0 || targetCol >= boardSize)
                    continue;

                var targetPos = new Position(targetRow, targetCol);
                var result = session.Fire(targetPos);
                results.Add(result);
                positions.Add(targetPos);

                if (session.Phase == Phase.Finished) break;
            }
            if (session.Phase == Phase.Finished) break;
        }

        // 3x3 salvo always ends turn after firing
        return new AttackResult(results, positions, true);
    }
}

