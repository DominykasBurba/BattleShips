using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;
using System.Text;

namespace BattleShips.Domain.Visitor;

/// <summary>
/// Concrete Visitor #3: Exports board state in different formats.
/// Generates ASCII art representation and structured text output.
/// </summary>
public class BoardExportVisitor : IBoardVisitor
{
    private readonly int _boardSize;
    private readonly Cell[,] _cellGrid;
    private readonly List<string> _shipList = new();

    public BoardExportVisitor(int boardSize)
    {
        _boardSize = boardSize;
        _cellGrid = new Cell[boardSize, boardSize];
    }

    public void VisitCell(Cell cell)
    {
        _cellGrid[cell.Pos.Row, cell.Pos.Col] = cell;
    }

    public void VisitShip(IShip ship)
    {
        var status = ship.IsSunk ? "SUNK" : "ACTIVE";
        var skin = ship.Skin != ShipSkin.Default ? $" ({ship.Skin})" : "";
        _shipList.Add($"{ship.Name} [{status}] - {ship.HitCount}/{ship.Length} hits{skin}");
    }

    public void VisitComplete()
    {
        // All data collected
    }

    /// <summary>
    /// Generates ASCII art representation of the board.
    /// </summary>
    public string ExportAsAscii()
    {
        var sb = new StringBuilder();
        sb.AppendLine("╔═══════════════════════════╗");
        sb.AppendLine("║   BATTLESHIPS BOARD       ║");
        sb.AppendLine("╠═══════════════════════════╣");

        // Column headers
        sb.Append("║   ");
        for (int c = 0; c < _boardSize; c++)
        {
            sb.Append((char)('A' + c));
            sb.Append(' ');
        }
        sb.AppendLine("║");
        sb.AppendLine("╟───────────────────────────╢");

        // Rows
        for (int r = 0; r < _boardSize; r++)
        {
            sb.Append($"║ {r + 1,2} ");
            for (int c = 0; c < _boardSize; c++)
            {
                var cell = _cellGrid[r, c];
                sb.Append(GetCellSymbol(cell));
                sb.Append(' ');
            }
            sb.AppendLine("║");
        }

        sb.AppendLine("╚═══════════════════════════╝");

        // Legend
        sb.AppendLine("\nLegend:");
        sb.AppendLine("  · = Empty    ✖ = Hit");
        sb.AppendLine("  • = Miss     ☒ = Sunk");
        sb.AppendLine("  ■ = Ship     S = Shielded");

        return sb.ToString();
    }

    /// <summary>
    /// Exports board state as structured text.
    /// </summary>
    public string ExportAsText()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== BOARD STATE EXPORT ===");
        sb.AppendLine($"Board Size: {_boardSize}x{_boardSize}");
        sb.AppendLine($"\nShips ({_shipList.Count}):");

        foreach (var ship in _shipList)
        {
            sb.AppendLine($"  - {ship}");
        }

        sb.AppendLine("\nCell Map:");
        sb.AppendLine(ExportAsAscii());

        return sb.ToString();
    }

    /// <summary>
    /// Exports ship list only.
    /// </summary>
    public string ExportShipList()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== SHIP ROSTER ===");
        foreach (var ship in _shipList)
        {
            sb.AppendLine(ship);
        }
        return sb.ToString();
    }

    private static string GetCellSymbol(Cell cell)
    {
        return cell.Status switch
        {
            CellStatus.Empty => "·",
            CellStatus.Hit => "✖",
            CellStatus.Miss => "•",
            CellStatus.Sunk => "☒",
            CellStatus.Ship => "■",
            CellStatus.Shielded => "S", // Shield - using 'S' for ASCII compatibility
            _ => "?"
        };
    }
}
