using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Visitor;

/// <summary>
/// Concrete Visitor #2: Analyzes damage across the board.
/// Calculates total damage, accuracy, and fleet health percentages.
/// </summary>
public class DamageReportVisitor : IBoardVisitor
{
    private int _totalHits;
    private int _totalMisses;
    private int _totalShots;

    private int _totalShipHealth;
    private int _currentShipHealth;

    private readonly List<ShipDamageInfo> _shipDamage = new();

    public void VisitCell(Cell cell)
    {
        if (cell.Status == CellStatus.Hit || cell.Status == CellStatus.Sunk)
            _totalHits++;

        if (cell.Status == CellStatus.Miss)
            _totalMisses++;
    }

    public void VisitShip(IShip ship)
    {
        _totalShipHealth += ship.Length;

        int hits = ship.HitCount;
        int health = ship.Length - hits;

        _currentShipHealth += health;

        _shipDamage.Add(new ShipDamageInfo
        {
            ShipName = ship.Name,
            TotalHealth = ship.Length,
            RemainingHealth = health,
            Hits = hits,
            IsSunk = ship.IsSunk,
            HealthPercentage = (double)health / ship.Length * 100
        });
    }

    public void VisitComplete()
    {
        _totalShots = _totalHits + _totalMisses;
    }

    /// <summary>
    /// Calculates overall shooting accuracy (hits / total shots).
    /// </summary>
    public double Accuracy => _totalShots > 0
        ? (double)_totalHits / _totalShots * 100
        : 0;

    /// <summary>
    /// Calculates overall fleet health percentage.
    /// </summary>
    public double FleetHealthPercentage => _totalShipHealth > 0
        ? (double)_currentShipHealth / _totalShipHealth * 100
        : 0;

    /// <summary>
    /// Gets detailed damage information for all ships.
    /// </summary>
    public IReadOnlyList<ShipDamageInfo> ShipDamageDetails => _shipDamage;

    /// <summary>
    /// Generates a detailed damage report.
    /// </summary>
    public string GetReport()
    {
        var report = $"""
            === Damage Report ===
            Shots Fired: {_totalShots} (Hits: {_totalHits}, Misses: {_totalMisses})
            Accuracy: {Accuracy:F1}%
            Fleet Health: {FleetHealthPercentage:F1}%

            Ship Details:
            """;

        foreach (var ship in _shipDamage)
        {
            var status = ship.IsSunk ? "SUNK" : "ACTIVE";
            report += $"\n  {ship.ShipName} [{status}]: {ship.RemainingHealth}/{ship.TotalHealth} HP ({ship.HealthPercentage:F0}%)";
        }

        return report;
    }
}

/// <summary>
/// Data class holding damage information for a single ship.
/// </summary>
public class ShipDamageInfo
{
    public string ShipName { get; init; } = "";
    public int TotalHealth { get; init; }
    public int RemainingHealth { get; init; }
    public int Hits { get; init; }
    public bool IsSunk { get; init; }
    public double HealthPercentage { get; init; }
}
