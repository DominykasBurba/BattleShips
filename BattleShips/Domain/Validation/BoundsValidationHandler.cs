namespace BattleShips.Domain.Validation;

/// <summary>
/// Ensures that all affected positions are within board bounds.
/// Works for both ship placement and single-position actions.
/// </summary>
public sealed class BoundsValidationHandler : ValidationHandler
{
    protected override bool Validate(ValidationContext context)
    {
        var size = context.Board.Size;

        IEnumerable<Position> positions;
        if (context.Ship is not null)
        {
            positions = context.Ship.Cells();
        }
        else if (context.Position is { } p)
        {
            positions = new[] { p };
        }
        else
        {
            return true;
        }

        foreach (var pos in positions)
        {
            if (!pos.InBounds(size))
            {
                context.Errors.Add("Position out of board bounds.");
                return false;
            }
        }

        return true;
    }
}


