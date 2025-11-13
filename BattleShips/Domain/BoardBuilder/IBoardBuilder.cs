using BattleShips.Domain.Ships;

namespace BattleShips.Domain.BoardBuilder;

public interface IBoardBuilder
{
    // pastatom laiva pagal laivo tipa
    void BuildPart(ShipKind shipKind);

    // Grazina lenta sukonstruota
    Board GetResult();
}

