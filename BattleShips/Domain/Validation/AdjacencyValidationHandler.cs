namespace BattleShips.Domain.Validation;

/// <summary>
/// Ensures that a ship does not touch other ships (including diagonals).
/// Reuses the same adjacency rule as Board.CanPlace.
/// </summary>
public sealed class AdjacencyValidationHandler : ValidationHandler
{
    protected override bool Validate(ValidationContext context)
    {
        if (context.Ship is null) return true;

        foreach (var pos in context.Ship.Cells())
        {
            if (HasAdjacentShip(context.Board, pos))
            {
                context.Errors.Add("Ship is adjacent to another ship.");
                return false;
            }
        }

        return true;
    }

    private static bool HasAdjacentShip(Board board, Position p)
    {
        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            var rr = p.Row + dr;
            var cc = p.Col + dc;
            if (rr < 0 || cc < 0 || rr >= board.Size || cc >= board.Size) continue;
            if (rr == p.Row && cc == p.Col) continue;

            var neighbor = board[rr, cc];
            if (neighbor.Ship is not null) return true;
        }

        return false;
    }
}


