using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Flyweight;

/// <summary>
/// Flyweight Factory pattern implementation.
/// Manages and shares CellAppearance flyweights to minimize memory usage.
/// Instead of creating appearance objects for each cell, shares a pool of ~10 objects.
/// </summary>
public class CellAppearanceFactory
{
    private readonly Dictionary<string, CellAppearance> _flyweights = new();

    private static CellAppearanceFactory? _instance;
    private static readonly Lock Lock = new Lock();

    private CellAppearanceFactory()
    {
        InitializeCommonFlyweights();
    }

    public static CellAppearanceFactory Instance
    {
        get
        {
            if (_instance != null) return _instance;
            
            lock (Lock)
            {
                _instance ??= new CellAppearanceFactory();
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// Gets or creates a flyweight for the given cell state.
    /// Returns shared instance if already exists, creates new one otherwise.
    /// </summary>
    public CellAppearance GetAppearance(CellStatus status, bool ownBoard, ShipSkin? skin)
    {
        // Create unique key for this combination of intrinsic state
        string key = $"{status}_{ownBoard}_{skin}";

        if (!_flyweights.ContainsKey(key))
        {
            // Create new flyweight if it doesn't exist
            var cssClass = ComputeCssClass(status, ownBoard, skin);
            var displayText = ComputeDisplayText(status, ownBoard);

            _flyweights[key] = new CellAppearance(cssClass, displayText);
        }

        return _flyweights[key];
    }

    /// <summary>
    /// Pre-creates common cell appearances to populate the flyweight pool.
    /// </summary>
    private void InitializeCommonFlyweights()
    {
        // Create flyweights for all standard cell states
        GetAppearance(CellStatus.Empty, false, null);
        GetAppearance(CellStatus.Hit, false, null);
        GetAppearance(CellStatus.Miss, false, null);
        GetAppearance(CellStatus.Sunk, false, null);
        GetAppearance(CellStatus.Shielded, false, null);
        GetAppearance(CellStatus.Ship, true, null);
        GetAppearance(CellStatus.Ship, true, ShipSkin.Camouflage);
    }
    
    /// <summary>
    /// Computes CSS class based on cell state (same logic as BattleBoard).
    /// </summary>
    private string ComputeCssClass(CellStatus status, bool own, ShipSkin? skin)
    {
        var baseCls = "grid-cell";
        var statusCss = status switch
        {
            CellStatus.Hit => $"{baseCls} hit",
            CellStatus.Miss => $"{baseCls} miss",
            CellStatus.Sunk => $"{baseCls} sunk",
            CellStatus.Shielded => $"{baseCls} shielded",
            CellStatus.Ship when own => $"{baseCls} ship",
            _ => baseCls
        };

        // Add skin-specific styling for own ships
        if (own && status == CellStatus.Ship && skin == ShipSkin.Camouflage)
        {
            statusCss += " camo";
        }

        return statusCss;
    }

    /// <summary>
    /// Computes display text based on cell state (same logic as BattleBoard).
    /// </summary>
    private string ComputeDisplayText(CellStatus status, bool own)
    {
        return status switch
        {
            CellStatus.Hit => "✖",
            CellStatus.Miss => "•",
            CellStatus.Sunk => "☒",
            CellStatus.Shielded => "🛡",
            CellStatus.Ship when own => "■",
            _ => ""
        };
    }

    /// <summary>
    /// Returns the count of flyweight instances currently in the pool.
    /// Useful for debugging/monitoring memory optimization.
    /// </summary>
    public int GetFlyweightCount() => _flyweights.Count;
}
