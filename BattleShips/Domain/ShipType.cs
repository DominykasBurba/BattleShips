namespace BattleShips.Domain;

/// <summary>
/// Represents the type of ship family to use in the game.
/// Part of the Abstract Factory pattern for ship creation.
/// </summary>
public enum ShipType
{
    /// <summary>
    /// Classic battleship game ships (smaller, traditional sizes)
    /// </summary>
    Classic,

    /// <summary>
    /// Modern naval ships (larger, more challenging)
    /// </summary>
    Modern
}
