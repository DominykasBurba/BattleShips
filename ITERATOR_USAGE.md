# Iterator Pattern - How It Works

## Step-by-Step Invocation

### Basic Usage Pattern

The Iterator pattern follows this standard flow:

```csharp
// 1. Get iterator from aggregate
IIterator<T> iterator = aggregate.CreateIterator();

// 2. Reset to first element
iterator.First();

// 3. Iterate through elements
while (!iterator.IsDone())
{
    // 4. Get current element
    T item = iterator.CurrentItem();
    
    // 5. Process the item
    // ... do something with item ...
    
    // 6. Move to next element
    iterator.Next();
}
```

## Real Examples

### Example 1: Iterating Board Cells

```csharp
Board board = new Board(10);

// Step 1: Board implements IAggregate<Cell>, so we can call CreateIterator()
IIterator<Cell> iterator = board.CreateIterator();

// Step 2: Start from the beginning
iterator.First();

// Step 3: Loop through all cells
while (!iterator.IsDone())
{
    // Step 4: Get current cell
    Cell cell = iterator.CurrentItem();
    
    // Step 5: Use the cell (e.g., check status, update, etc.)
    if (cell.Status == CellStatus.Ship)
    {
        Console.WriteLine($"Ship found at {cell.Pos}");
    }
    
    // Step 6: Move to next cell
    iterator.Next();
}
```

**What happens internally:**
- `CreateIterator()` creates a `BoardCellIterator` that knows how to traverse the 2D array
- `First()` sets `_currentRow = 0` and `_currentCol = 0`
- `CurrentItem()` returns `_cells[_currentRow, _currentCol]`
- `Next()` increments column, and when it reaches the end, moves to next row
- `IsDone()` returns `true` when `_currentRow >= Size`

### Example 2: Iterating Ships

```csharp
Board board = new Board(10);
// ... place some ships ...

// Step 1: Create aggregate for ships
ShipCollection shipCollection = new ShipCollection(board.Ships);

// Step 2: Get iterator
IIterator<IShip> iterator = shipCollection.CreateIterator();

// Step 3: Iterate
iterator.First();
while (!iterator.IsDone())
{
    IShip ship = iterator.CurrentItem();
    Console.WriteLine($"{ship.Name}: {ship.Length} cells, Sunk: {ship.IsSunk}");
    iterator.Next();
}
```

**What happens internally:**
- `ShipCollection` wraps the `List<IShip>`
- `CreateIterator()` creates a `ShipListIterator`
- `First()` sets `_currentIndex = 0`
- `CurrentItem()` returns `_ships[_currentIndex]`
- `Next()` increments `_currentIndex++`
- `IsDone()` returns `true` when `_currentIndex >= _ships.Count`

### Example 3: Iterating Positions (HashSet)

```csharp
HashSet<Position> hitPositions = new HashSet<Position>
{
    new Position(0, 0),
    new Position(1, 1),
    new Position(2, 2)
};

// Step 1: Create aggregate
PositionSet positionSet = new PositionSet(hitPositions);

// Step 2: Get iterator
IIterator<Position> iterator = positionSet.CreateIterator();

// Step 3: Iterate
iterator.First();
while (!iterator.IsDone())
{
    Position pos = iterator.CurrentItem();
    Console.WriteLine($"Hit at: {pos}");
    iterator.Next();
}
```

**What happens internally:**
- `PositionSet` wraps the `HashSet<Position>`
- `CreateIterator()` creates a `PositionSetIterator`
- `First()` converts HashSet to array: `_positionArray = _positions.ToArray()`
- `CurrentItem()` returns `_positionArray[_currentIndex]`
- `Next()` increments `_currentIndex++`
- `IsDone()` returns `true` when `_currentIndex >= _positionArray.Length`

## Where It's Used in the Codebase

### 1. Board.CountRevealedCells()

```csharp
public int CountRevealedCells()
{
    IIterator<Cell> iterator = CreateIterator();  // Get iterator
    int count = 0;

    iterator.First();  // Start from beginning
    while (!iterator.IsDone())  // Check if done
    {
        Cell cell = iterator.CurrentItem();  // Get current
        if (cell.IsRevealed)
        {
            count++;
        }
        iterator.Next();  // Move to next
    }

    return count;
}
```

### 2. Potential Usage in AI Player

```csharp
// AI could use iterator to scan board systematically
public Position ChooseTarget(Board enemyBoard, HashSet<Position> tried)
{
    IIterator<Cell> iterator = enemyBoard.CreateIterator();
    iterator.First();
    
    while (!iterator.IsDone())
    {
        Cell cell = iterator.CurrentItem();
        Position pos = cell.Pos;
        
        // Skip if already tried
        if (tried.Contains(pos))
        {
            iterator.Next();
            continue;
        }
        
        // Skip if already revealed
        if (cell.IsRevealed)
        {
            iterator.Next();
            continue;
        }
        
        // Found a valid target
        return pos;
    }
    
    // Fallback to random if iterator exhausted
    return RandomPosition(enemyBoard, tried);
}
```

## Key Points

1. **Aggregate creates iterator**: `aggregate.CreateIterator()` returns a new iterator instance
2. **Iterator manages traversal**: The iterator knows how to traverse its specific data structure
3. **Uniform interface**: All iterators have the same interface (`First()`, `Next()`, `IsDone()`, `CurrentItem()`)
4. **Different implementations**: Each iterator handles a different data structure internally:
   - `BoardCellIterator` - 2D array traversal
   - `ShipListIterator` - List traversal
   - `PositionSetIterator` - HashSet traversal (converted to array)

## Benefits

- **Encapsulation**: Client code doesn't need to know about internal data structure
- **Polymorphism**: Same code works with different data structures
- **Flexibility**: Can add new iterators without changing client code
- **Separation**: Aggregate manages data, Iterator manages traversal

