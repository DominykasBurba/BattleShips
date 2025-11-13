using BattleShips.Domain.Ships;

namespace BattleShips.Domain.BoardBuilder;

public class FleetDirector
{
    private IBoardBuilder? _builder;
    
    public void SetBuilder(IBoardBuilder builder)
    {
        _builder = builder;
    }
    
    // Jeigu reikia eiti per visus imanomus laivu tipus, kvieciam sita.
    public Board Construct()
    {
        if (_builder == null)
            throw new InvalidOperationException("Builder must be set before constructing. Call SetBuilder() first.");

        foreach (var shipKind in DefaultFleet.Composition)
        {
            _builder.BuildPart(shipKind);
        }

        return _builder.GetResult();
    }
}

