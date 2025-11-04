# UML Class Diagram - Builder Pattern Implementation

## Overview
This document describes the UML class diagram for the Builder pattern implementation in the BattleShips project.

---

## Classes and Interfaces

### 1. **FleetDirector** (Director)
**Stereotype**: `<<Director>>`  
**Location**: Top-left of diagram

**Attributes:**
- `-builder : IBoardBuilder` (private, association with IBoardBuilder)

**Methods:**
- `+SetBuilder(builder: IBoardBuilder) : void`
- `+Construct() : Board`
- `+Construct(composition: IEnumerable<ShipKind>) : Board`

**Notes:**
- Contains a note box connected with dashed line showing:
  ```
  for all objects {
      builder->BuildPart(shipKind);
  }
  ```

---

### 2. **IBoardBuilder** (Abstract Builder)
**Stereotype**: `<<interface>>` or `<<abstract>>`  
**Location**: Top-center of diagram  
**Style**: Italicized name or interface notation

**Methods:**
- `+BuildPart(shipKind: ShipKind) : void` (abstract/public)

**Returns:**
- `+GetResult() : Board` (abstract/public)

---

### 3. **RandomFleetBuilder** (Concrete Builder)
**Stereotype**: `<<ConcreteBuilder>>`  
**Location**: Below IBoardBuilder, left side

**Attributes:**
- `-_board : Board` (private, composition/aggregation with Board)
- `-_rng : Random` (private)

**Methods:**
- `+BuildPart(shipKind: ShipKind) : void` (implements IBoardBuilder)
- `+GetResult() : Board` (implements IBoardBuilder)
- `+RandomFleetBuilder(board: Board)` (constructor)

**Relationships:**
- Implements `IBoardBuilder` (solid line with hollow triangle arrow)
- Uses `Board` (dependency/association)

---

### 4. **ManualFleetBuilder** (Concrete Builder)
**Stereotype**: `<<ConcreteBuilder>>`  
**Location**: Below IBoardBuilder, right side

**Attributes:**
- `-_board : Board` (private, composition/aggregation with Board)
- `-_pendingPosition : Position?` (private)
- `-_pendingOrientation : Orientation?` (private)

**Methods:**
- `+BuildPart(shipKind: ShipKind) : void` (implements IBoardBuilder)
- `+BuildPart(shipKind: ShipKind, position: Position, orientation: Orientation) : void` (overload)
- `+GetResult() : Board` (implements IBoardBuilder)
- `+SetNextShipPlacement(position: Position, orientation: Orientation) : void`
- `+TryBuildPart(shipKind: ShipKind, position: Position, orientation: Orientation) : bool`
- `+ManualFleetBuilder(board: Board)` (constructor)

**Relationships:**
- Implements `IBoardBuilder` (solid line with hollow triangle arrow)
- Uses `Board` (dependency/association)

---

### 5. **Board** (Product)
**Stereotype**: `<<Product>>`  
**Location**: Bottom-right of diagram

**Attributes:**
- `+Size : int` (public, readonly)
- `+Ships : IReadOnlyList<ShipBase>` (public, readonly)
- `-_cells : Cell[,]` (private)
- `-_ships : List<ShipBase>` (private)

**Methods:**
- `+Board(size: int = 10)` (constructor)
- `+Place(ship: ShipBase) : bool`
- `+Remove(ship: ShipBase) : void`
- `+Clear() : void`
- `+CanPlace(ship: ShipBase) : bool`
- `+FireAt(position: Position) : ShotResult`
- `+AllShipsSunk : bool` (property)

---

### 6. **Supporting Classes** (Optional to show)

#### **DefaultFleet** (Static Class)
**Location**: Top-right or separate area

**Attributes:**
- `+Composition : ShipKind[]` (public static readonly)

**Notes:**
- Used by FleetDirector to know what ships to build

#### **ShipKind** (Enum)
**Location**: Separate area or in note

**Values:**
- `Carrier`
- `Battleship`
- `Cruiser`
- `Submarine`
- `Destroyer`

---

## Relationships (Connections)

### 1. **FleetDirector → IBoardBuilder** (Association)
- **Type**: Association (solid line with arrow)
- **Label**: `-builder` (on FleetDirector side)
- **Multiplicity**: 1 (Director has one builder)
- **Direction**: FleetDirector → IBoardBuilder
- **Stereotype**: `uses`

### 2. **IBoardBuilder → RandomFleetBuilder** (Realization/Implementation)
- **Type**: Realization (dashed line with hollow triangle arrow)
- **Direction**: RandomFleetBuilder → IBoardBuilder
- **Label**: `implements` or `<<implements>>`
- **Note**: In C#, interfaces use realization, not inheritance

### 3. **IBoardBuilder → ManualFleetBuilder** (Realization/Implementation)
- **Type**: Realization (dashed line with hollow triangle arrow)
- **Direction**: ManualFleetBuilder → IBoardBuilder
- **Label**: `implements` or `<<implements>>`

### 4. **RandomFleetBuilder → Board** (Association/Composition)
- **Type**: Association (solid line with arrow)
- **Label**: `-_board` (on RandomFleetBuilder side)
- **Direction**: RandomFleetBuilder → Board
- **Multiplicity**: 1 (one builder works with one board)
- **Stereotype**: `uses` or `constructs`

### 5. **ManualFleetBuilder → Board** (Association/Composition)
- **Type**: Association (solid line with arrow)
- **Label**: `-_board` (on ManualFleetBuilder side)
- **Direction**: ManualFleetBuilder → Board
- **Multiplicity**: 1 (one builder works with one board)
- **Stereotype**: `uses` or `constructs`

### 6. **IBoardBuilder → Board** (Dependency)
- **Type**: Dependency (dashed line with arrow)
- **Direction**: IBoardBuilder → Board
- **Label**: `returns` or `<<returns>>`
- **Note**: GetResult() returns Board

---

## Diagram Layout (Suggested)

```
┌─────────────────────┐
│  FleetDirector      │
│  -builder           │───────┐
│  +SetBuilder()      │       │
│  +Construct()       │       │
│  +Construct(comp)   │       │
└─────────────────────┘       │
         │                     │
         │ (uses)              │ Association
         │                     │
         ▼                     │
┌─────────────────────┐       │
│  IBoardBuilder      │◄──────┘
│  <<interface>>       │
│  +BuildPart()       │
│  +GetResult()        │
└─────────────────────┘
         ▲        ▲
         │        │
         │        │ Realization
         │        │
    ┌────┴────┐  ┌────┴──────┐
    │Random   │  │Manual    │
    │Fleet    │  │Fleet      │
    │Builder  │  │Builder    │
    │-_board  │  │-_board    │
    │-_rng    │  │-_pending  │
    └────┬────┘  └────┬──────┘
         │           │
         │           │ Association
         │           │
         └─────┬─────┘
               │
               ▼
         ┌──────────┐
         │  Board   │
         │ <<Product>>│
         │  +Size   │
         │  +Ships  │
         │  +Place()│
         └──────────┘
```

---

## Detailed Relationship Specifications

### Association Details

**FleetDirector → IBoardBuilder**
- **Role on FleetDirector**: `builder`
- **Visibility**: Private (`-`)
- **Navigability**: FleetDirector → IBoardBuilder (one-way)
- **Cardinality**: 0..1 (optional, can be null before SetBuilder is called)

**RandomFleetBuilder → Board**
- **Role on RandomFleetBuilder**: `board`
- **Visibility**: Private (`-`)
- **Navigability**: RandomFleetBuilder → Board (one-way)
- **Cardinality**: 1 (always has one board)
- **Aggregation**: Composition (builder owns/creates board state)

**ManualFleetBuilder → Board**
- **Role on ManualFleetBuilder**: `board`
- **Visibility**: Private (`-`)
- **Navigability**: ManualFleetBuilder → Board (one-way)
- **Cardinality**: 1 (always has one board)
- **Aggregation**: Composition (builder owns/creates board state)

---

## Stereotypes and Constraints

### Stereotypes
- `<<Director>>` on FleetDirector
- `<<ConcreteBuilder>>` on RandomFleetBuilder and ManualFleetBuilder
- `<<Product>>` on Board
- `<<interface>>` on IBoardBuilder

### Constraints
- `{ordered}` on fleet composition (ships are placed in sequence)
- `{readonly}` on Board.Size and Board.Ships

---

## Notes and Comments

### Note 1: Director's Construction Algorithm
**Connected to**: FleetDirector  
**Content**:
```
Construction Algorithm:
for all shipKind in DefaultFleet.Composition {
    builder->BuildPart(shipKind);
}
```

### Note 2: Builder Pattern Purpose
**Location**: Top of diagram or in corner  
**Content**:
```
Builder Pattern separates:
- WHAT to build (Director knows sequence)
- HOW to build (Builder implements strategy)
- RESULT (Board with ships placed)
```

---

## Color Coding (Optional)
- **FleetDirector**: Light blue
- **IBoardBuilder**: Yellow (interface)
- **RandomFleetBuilder**: Light green
- **ManualFleetBuilder**: Light green
- **Board**: Light orange

---

## Alternative Views

### Simplified View (Core Pattern Only)
Show only:
- FleetDirector
- IBoardBuilder
- One ConcreteBuilder (RandomFleetBuilder)
- Board

### Complete View (Include Dependencies)
Show all classes plus:
- ShipFactory (used by builders)
- DefaultFleet (used by Director)
- ShipKind (enum used as parameter)
- Position, Orientation (used by builders)

---

## Implementation Notes for UML Tools

### For Enterprise Architect:
1. Create classes with correct stereotypes
2. Use "Realization" relationship for interface implementation
3. Use "Association" for builder-board relationship
4. Add note boxes with dashed line connections
5. Set visibility: `-` for private, `+` for public

### For Visual Paradigm:
1. Interface: Use `<<interface>>` stereotype
2. Abstract methods: Use italics or `{abstract}` constraint
3. Realization: Use dashed line with arrow
4. Composition: Use filled diamond on builder side

### For Draw.io / Lucidchart:
1. Use interface shape (circle with "I") or rectangle with `<<interface>>`
2. Use dashed arrows for realizations
3. Use solid arrows for associations
4. Add note boxes as text boxes with dashed borders

---

## Validation Checklist

✅ FleetDirector has association with IBoardBuilder  
✅ Both concrete builders implement IBoardBuilder  
✅ Both concrete builders have association with Board  
✅ IBoardBuilder has dependency on Board (GetResult returns Board)  
✅ Director's construct() method is shown calling BuildPart()  
✅ Note box shows the construction algorithm  
✅ All method signatures match actual code  
✅ Visibility modifiers are correct (- for private, + for public)  

---

## Example Usage Sequence (Separate Diagram)

For a complete picture, you might also want to show a sequence diagram:

```
PlacementService → FleetDirector → RandomFleetBuilder → Board
     |                |                   |              |
     | SetBuilder()   |                   |              |
     |───────────────>|                   |              |
     |                |                   |              |
     | Construct()    |                   |              |
     |───────────────>|                   |              |
     |                | BuildPart(ship1)  |              |
     |                |──────────────────>|              |
     |                |                   | Place(ship)  |
     |                |                   |─────────────>|
     |                | BuildPart(ship2)  |              |
     |                |──────────────────>|              |
     |                | GetResult()       |              |
     |                |<──────────────────|              |
     |                |                   |              |
     |<───────────────|                   |              |
```

