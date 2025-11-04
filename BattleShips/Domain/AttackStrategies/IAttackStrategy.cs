namespace BattleShips.Domain.AttackStrategies;

public interface IAttackStrategy
{
    AttackResult ExecuteAttack(GameSession session, Position position);
}

