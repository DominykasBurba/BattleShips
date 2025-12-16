namespace BattleShips.Domain.Flyweight;

/// <summary>
/// Flyweight pattern implementation.
/// Represents the intrinsic state (shared data) for cell visual appearance.
/// Multiple cells can share the same appearance object instead of storing duplicate CSS/text.
/// </summary>
public class CellAppearance
{
    /// <summary>
    /// CSS class for rendering the cell (intrinsic state - shared).
    /// </summary>
    public string CssClass { get; }

    /// <summary>
    /// Display text/symbol for the cell (intrinsic state - shared).
    /// </summary>
    public string DisplayText { get; }

    /// <summary>
    /// Creates a flyweight with immutable intrinsic state.
    /// </summary>
    public CellAppearance(string cssClass, string displayText)
    {
        CssClass = cssClass;
        DisplayText = displayText;
    }

    /// <summary>
    /// Renders the cell appearance with extrinsic state (position).
    /// The position is NOT stored in the flyweight - it's passed as context.
    /// </summary>
    public string RenderAt(int row, int col)
    {
        // Extrinsic state (position) is used here but not stored
        // In a real scenario, this might generate position-specific HTML
        return $"Cell at ({row},{col}): {DisplayText} with class {CssClass}";
    }
}
