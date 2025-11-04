namespace BattleShips.Domain.AttackStrategies;


public class SingleShotStrategy : IAttackStrategy
{
    public AttackResult ExecuteAttack(GameSession session, Position position)
    {
        if (session.Phase != Phase.Playing)
            return new AttackResult(ShotResult.Invalid, position, true);

        // Fire single shot at the position
        var result = session.Fire(position);
        
        // End turn on miss/invalid/already tried, keep turn on hit/sunk
        bool endTurn = result is ShotResult.Miss or ShotResult.Invalid or ShotResult.AlreadyTried;

        return new AttackResult(result, position, endTurn);
    }
}

