namespace BattleShips.Domain.Visitor;

/// <summary>
/// VISITOR PATTERN USAGE EXAMPLES
///
/// This file demonstrates how to use the Visitor pattern with the Board class.
/// The Visitor pattern allows adding new operations to the board without modifying
/// the Board class itself.
///
/// PATTERN STRUCTURE:
/// - IBoardVisitor: Visitor interface
/// - Board.Accept(visitor): Element that accepts visitors
/// - StatisticsVisitor, DamageReportVisitor, BoardExportVisitor: Concrete visitors
///
/// USAGE EXAMPLES:
/// </summary>
public static class VisitorUsageExample
{
    /// <summary>
    /// Example 1: Get board statistics
    /// </summary>
    public static void Example1_GetStatistics(Board board)
    {
        var statsVisitor = new StatisticsVisitor();
        board.Accept(statsVisitor);

        // Access collected statistics
        Console.WriteLine($"Total cells: {statsVisitor.TotalCells}");
        Console.WriteLine($"Hits: {statsVisitor.HitCells}");
        Console.WriteLine($"Misses: {statsVisitor.MissCells}");
        Console.WriteLine($"Ships: {statsVisitor.TotalShips} ({statsVisitor.ActiveShips} active)");
        Console.WriteLine($"Coverage: {statsVisitor.CoveragePercentage:F1}%");

        // Or get full summary
        string summary = statsVisitor.GetSummary();
        Console.WriteLine(summary);
    }

    /// <summary>
    /// Example 2: Generate damage report
    /// </summary>
    public static void Example2_DamageReport(Board board)
    {
        var damageVisitor = new DamageReportVisitor();
        board.Accept(damageVisitor);

        // Access damage metrics
        Console.WriteLine($"Accuracy: {damageVisitor.Accuracy:F1}%");
        Console.WriteLine($"Fleet Health: {damageVisitor.FleetHealthPercentage:F1}%");

        // Get detailed ship damage
        foreach (var shipInfo in damageVisitor.ShipDamageDetails)
        {
            Console.WriteLine($"{shipInfo.ShipName}: {shipInfo.RemainingHealth}/{shipInfo.TotalHealth} HP");
        }

        // Or get full report
        string report = damageVisitor.GetReport();
        Console.WriteLine(report);
    }

    /// <summary>
    /// Example 3: Export board state
    /// </summary>
    public static void Example3_ExportBoard(Board board)
    {
        var exportVisitor = new BoardExportVisitor(board.Size);
        board.Accept(exportVisitor);

        // Export as ASCII art
        string asciiBoard = exportVisitor.ExportAsAscii();
        Console.WriteLine(asciiBoard);

        // Export as structured text
        string textExport = exportVisitor.ExportAsText();
        Console.WriteLine(textExport);

        // Export ship list only
        string shipList = exportVisitor.ExportShipList();
        Console.WriteLine(shipList);
    }

    /// <summary>
    /// Example 4: Use multiple visitors on the same board
    /// </summary>
    public static void Example4_MultipleVisitors(Board board)
    {
        // Statistics
        var stats = new StatisticsVisitor();
        board.Accept(stats);
        Console.WriteLine($"Active ships: {stats.ActiveShips}");

        // Damage report
        var damage = new DamageReportVisitor();
        board.Accept(damage);
        Console.WriteLine($"Fleet health: {damage.FleetHealthPercentage:F1}%");

        // Export
        var export = new BoardExportVisitor(board.Size);
        board.Accept(export);
        Console.WriteLine(export.ExportAsAscii());
    }

    /// <summary>
    /// Example 5: Game-end statistics
    /// </summary>
    public static void Example5_GameEndStats(Board playerBoard, Board opponentBoard)
    {
        Console.WriteLine("=== GAME OVER ===\n");

        Console.WriteLine("YOUR BOARD:");
        var yourStats = new StatisticsVisitor();
        playerBoard.Accept(yourStats);
        Console.WriteLine(yourStats.GetSummary());

        Console.WriteLine("\nOPPONENT BOARD:");
        var enemyDamage = new DamageReportVisitor();
        opponentBoard.Accept(enemyDamage);
        Console.WriteLine(enemyDamage.GetReport());
        Console.WriteLine($"\nYour accuracy: {enemyDamage.Accuracy:F1}%");
    }

    /// <summary>
    /// POTENTIAL INTEGRATION POINTS:
    ///
    /// 1. End of game (FinishedState):
    ///    - Display statistics summary
    ///    - Show damage report
    ///    - Export final board state
    ///
    /// 2. During gameplay (GameService):
    ///    - Track live statistics
    ///    - Calculate AI targeting (using statistics)
    ///
    /// 3. Game UI (Game.razor):
    ///    - Display fleet health percentage
    ///    - Show accuracy stats
    ///
    /// 4. Debugging/Testing:
    ///    - Export board states for testing
    ///    - Verify game state integrity
    /// </summary>
}
