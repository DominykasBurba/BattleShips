using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Validation;

/// <summary>
/// Context object passed through the validation chain.
/// </summary>
public sealed class ValidationContext
{
    public required Board Board { get; init; }
    public IShip? Ship { get; init; }
    public Position? Position { get; init; }

    public List<string> Errors { get; } = new();

    public bool IsValid => Errors.Count == 0;
}


