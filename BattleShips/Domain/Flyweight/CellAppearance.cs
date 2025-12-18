namespace BattleShips.Domain.Flyweight;

/// <summary>
/// Flyweight pattern implementation.
/// Represents the intrinsic state (shared data) for cell visual appearance.
/// Multiple cells can share the same appearance object instead of storing duplicate CSS/text.
/// </summary>
public class CellAppearance(string cssClass, string displayText)
{
    public string CssClass { get; } = cssClass;

    public string DisplayText { get; } = displayText;
    
    public string RenderAt(int row, int col)
    {
        return $"Cell at ({row},{col}): {DisplayText} with class {CssClass}";
    }
}
