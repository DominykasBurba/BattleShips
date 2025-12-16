using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Visitor;

/// <summary>
/// Concrete Visitor #1: Collects statistical data about the board.
/// Counts different cell types, ships, and calculates coverage metrics.
/// </summary>
public class StatisticsVisitor : IBoardVisitor
{
    public int EmptyCells { get; private set; }
    public int HitCells { get; private set; }
    public int MissCells { get; private set; }
    public int SunkCells { get; private set; }
    public int ShieldedCells { get; private set; }
    public int ShipCells { get; private set; }

    public int TotalShips { get; private set; }
    public int SunkShips { get; private set; }
    public int ActiveShips { get; private set; }

    public int TotalCells { get; private set; }

    public void VisitCell(Cell cell)
    {
        TotalCells++;

        switch (cell.Status)
        {
            case CellStatus.Empty:
                EmptyCells++;
                break;
            case CellStatus.Hit:
                HitCells++;
                break;
            case CellStatus.Miss:
                MissCells++;
                break;
            case CellStatus.Sunk:
                SunkCells++;
                break;
            case CellStatus.Shielded:
                ShieldedCells++;
                break;
            case CellStatus.Ship:
                ShipCells++;
                break;
        }
    }

    public void VisitShip(IShip ship)
    {
        TotalShips++;
        if (ship.IsSunk)
            SunkShips++;
        else
            ActiveShips++;
    }

    public void VisitComplete()
    {
        // All data collected, ready for reporting
    }

    /// <summary>
    /// Calculates the percentage of board covered by ships.
    /// </summary>
    public double CoveragePercentage => TotalCells > 0
        ? (double)(ShipCells + HitCells + SunkCells) / TotalCells * 100
        : 0;

    /// <summary>
    /// Calculates the percentage of shots fired.
    /// </summary>
    public double ShotsFiredPercentage => TotalCells > 0
        ? (double)(HitCells + MissCells + SunkCells + ShieldedCells) / TotalCells * 100
        : 0;

    /// <summary>
    /// Generates a text summary of board statistics.
    /// </summary>
    public string GetSummary()
    {
        return $"""
            === Board Statistics ===
            Total Cells: {TotalCells}
            Empty: {EmptyCells}, Hits: {HitCells}, Misses: {MissCells}
            Sunk: {SunkCells}, Shielded: {ShieldedCells}, Ships: {ShipCells}

            Ships: {TotalShips} total ({ActiveShips} active, {SunkShips} sunk)
            Coverage: {CoveragePercentage:F1}%
            Shots Fired: {ShotsFiredPercentage:F1}%
            """;
    }
}
