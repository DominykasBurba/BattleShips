namespace BattleShips.Domain;

public readonly record struct Position(int Row, int Col)
{
    public bool InBounds(int size) => Row >= 0 && Col >= 0 && Row < size && Col < size;
    public override string ToString() => $"{(char)('A' + Row)}{Col + 1}";
}