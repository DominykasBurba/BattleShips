namespace BattleShips.Domain.Validation;

/// <summary>
/// Ensures that a ship does not overlap existing ships.
/// </summary>
public sealed class OverlapValidationHandler : ValidationHandler
{
    protected override bool Validate(ValidationContext context)
    {
        if (context.Ship is null) return true;

        foreach (var pos in context.Ship.Cells())
        {
            var cell = context.Board[pos];
            if (cell.Ship is not null)
            {
                context.Errors.Add("Ship overlaps another ship.");
                return false;
            }
        }

        return true;
    }
}


