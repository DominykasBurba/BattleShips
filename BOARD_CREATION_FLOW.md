# Board Creation Flow - Current Structure

## Overview
There are **TWO different game modes**, but boards are created the **SAME way** in both:

1. **Local Game** (vs AI or vs Human on same device)
2. **Online Game** (vs Human via SignalR)

---

## 🎯 How Boards Are Created

### Board Size Initialization
- **Default**: `10 × 10` (can be changed via UI)
- **Where**: Set when creating `Player` objects

### Board Creation Flow

```
User selects board size (e.g., 10)
    ↓
Player constructor called with boardSize
    ↓
Player creates Board internally: new Board(boardSize)
    ↓
Board constructor creates empty grid of Cells
```

---

## 📍 Key Locations

### 1. **Board Creation** 
**Location**: `Domain/Player.cs` line 10
```csharp
protected Player(string name, PlayerKind kind, int boardSize = 10)
{ 
    Name = name; 
    Kind = kind; 
    Board = new Board(boardSize);  // ← Board created here
}
```

**What happens**: 
- `Board` constructor creates a `size × size` grid of empty `Cell` objects
- Board is **empty** - no ships yet!

### 2. **Ship Placement** (Different for each mode)

#### A) **Local Game (vs AI)**
**Location**: `Services/PlacementService.cs` → `RandomizeFleet()`

**Flow**:
```
Game.razor OnInitialized()
    ↓
GameService.NewLocalSession(size, enemyIsAi: true)
    ↓
Creates: HumanPlayer("Player 1", size) + AiPlayer("Enemy AI", size)
    ↓
GameService.RandomizeFor(P1)  // Randomize human's board
    ↓
PlacementService.RandomizeFleet(board)
    ↓
Loops through DefaultFleet.Composition
    ↓
For each ship: randomly place it on board
```

**Ship Construction**: `PlacementService.RandomizeFleet()` lines 10-29
- Clears board
- For each ship type in `DefaultFleet.Composition`:
  - Randomly picks orientation (Horizontal/Vertical)
  - Randomly picks position
  - Creates ship via `ShipFactory.Create()`
  - Places on board via `board.Place(ship)`

#### B) **Online Game (vs Human)**
**Location**: `Services/GameLobbyService.cs` → `PlaceShips()`

**Flow**:
```
Home.razor CreateGame() or JoinOnlineGame()
    ↓
GameLobbyService.CreateGame(connectionId, boardSize, ...)
    ↓
OnlineGameSession constructor
    ↓
Creates: HumanPlayer("Player 1", boardSize) + HumanPlayer("Player 2", boardSize)
    ↓
Player manually places ships via UI
    ↓
GameLobbyService.PlaceShips(gameId, connectionId, ships)
    ↓
For each ship placement:
    - Creates ship via ShipFactory.Create()
    - Places on board via board.Place(ship)
```

**Ship Construction**: `GameLobbyService.PlaceShips()` lines 36-63
- Clears board
- For each ship in the provided list:
  - Creates ship via `ShipFactory.Create(kind, position, orientation)`
  - Places on board via `board.Place(ship)`

---

## 🔍 Important Differences

### Local vs Online: **Same Board Creation, Different Ship Placement**

| Aspect | Local Game | Online Game |
|--------|-----------|-------------|
| **Board Creation** | Same: `Player` constructor → `new Board(size)` | Same: `Player` constructor → `new Board(size)` |
| **Ship Placement** | **Random** via `PlacementService.RandomizeFleet()` | **Manual** via `GameLobbyService.PlaceShips()` |
| **When Ships Placed** | Immediately after game creation | After player manually places via UI |
| **Who Controls** | AI/Service automatically | Player via UI interactions |

---

## 📂 Code Flow Summary

### Local Game Path:
```
User clicks "Start Game" (size=10)
    ↓
Home.razor → Nav.NavigateTo("/game?size=10&mode=Single")
    ↓
Game.razor.OnInitialized()
    ↓
GameService.NewLocalSession(10, enemyIsAi: true)
    ├─→ Creates HumanPlayer("Player 1", 10) → Board(10) created
    └─→ Creates AiPlayer("Enemy AI", 10) → Board(10) created
    ↓
GameService.RandomizeFor(P1) → PlacementService.RandomizeFleet(P1.Board)
    ↓
GameService.RandomizeFor(P2) → PlacementService.RandomizeFleet(P2.Board)
    ↓
Ships placed on both boards!
```

### Online Game Path:
```
User clicks "Create Online Game" (size=10)
    ↓
Home.razor.CreateGame() → SignalR Hub
    ↓
GameLobbyService.CreateGame(connectionId, 10, ...)
    ↓
OnlineGameSession constructor
    ├─→ Creates HumanPlayer("Player 1", 10) → Board(10) created
    └─→ Creates HumanPlayer("Player 2", 10) → Board(10) created
    ↓
[Player 1 places ships manually via UI]
    ↓
GameLobbyService.PlaceShips() → Creates ships and places them
    ↓
[Player 2 places ships manually via UI]
    ↓
GameLobbyService.PlaceShips() → Creates ships and places them
    ↓
Both players ready → Game starts!
```

---

## 🎯 Where Builder Pattern Should Go

The Builder pattern should centralize **ship placement logic**:

- **Current**: Ship placement scattered across:
  - `PlacementService.RandomizeFleet()` - random placement
  - `GameLobbyService.PlaceShips()` - manual placement
  - Potentially `PlacementBoard.razor` - UI placement logic

- **With Builder**: All ship placement goes through Builder pattern:
  - Director knows: "What ships to place" (fleet composition)
  - Builder knows: "How to place them" (random vs manual)
  - Both use the same construction algorithm

---

## Key Files to Understand

1. **`Domain/Player.cs`** - Creates Board when Player is created
2. **`Domain/Board.cs`** - The Board class (empty grid initially)
3. **`Services/PlacementService.cs`** - Random ship placement
4. **`Services/GameLobbyService.cs`** - Manual ship placement for online
5. **`Services/GameService.cs`** - Orchestrates local games
6. **`Domain/Ships/ShipFactory.cs`** - Creates ship objects
7. **`Domain/Ships/ShipKind.cs`** - Defines fleet composition

