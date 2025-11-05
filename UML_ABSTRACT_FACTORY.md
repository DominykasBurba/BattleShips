# Abstract Factory Pattern - UML Diagram

## Complete UML Class Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        ABSTRACT FACTORY PATTERN                              │
│                     (Ship Creation with Two Families)                        │
└─────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│                            ABSTRACT FACTORY                                   │
└──────────────────────────────────────────────────────────────────────────────┘

                            ┌─────────────────────────┐
                            │   <<interface>>         │
                            │   IShipFactory          │
                            ├─────────────────────────┤
                            │ + CreateBattleship()    │
                            │ + CreateSubmarine()     │
                            │ + CreateDestroyer()     │
                            │ + CreateCruiser()       │
                            │ + CreateShip(kind)      │
                            └─────────────────────────┘
                                       △
                                       │
                         ┌─────────────┴─────────────┐
                         │                           │
                         │                           │
         ┌───────────────▼──────────┐   ┌───────────▼──────────────┐
         │ ClassicShipFactory       │   │ ModernShipFactory        │
         ├──────────────────────────┤   ├──────────────────────────┤
         │ + CreateBattleship()     │   │ + CreateBattleship()     │
         │   → new Battleship()     │   │   → new ModernBattleship()│
         │ + CreateSubmarine()      │   │ + CreateSubmarine()      │
         │   → new Submarine()      │   │   → new ModernSubmarine()│
         │ + CreateDestroyer()      │   │ + CreateDestroyer()      │
         │   → new Destroyer()      │   │   → new ModernDestroyer()│
         │ + CreateCruiser()        │   │ + CreateCruiser()        │
         │   → new Cruiser()        │   │   → new ModernCruiser()  │
         └──────────────────────────┘   └──────────────────────────┘
                    │                              │
                    │ creates                      │ creates
                    │                              │
         ┌──────────▼──────────┐        ┌─────────▼─────────────┐
         │  CLASSIC FAMILY     │        │   MODERN FAMILY       │
         └─────────────────────┘        └───────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│                          ABSTRACT PRODUCT                                     │
└──────────────────────────────────────────────────────────────────────────────┘

                        ┌──────────────────────────┐
                        │    ShipBase              │
                        │    <<abstract>>          │
                        ├──────────────────────────┤
                        │ + Name: string           │
                        │ + Length: int            │
                        │ + Kind: ShipKind         │
                        │ + Start: Position        │
                        │ + Orientation: Orientation│
                        ├──────────────────────────┤
                        │ + Cells(): Position[]    │
                        │ + RegisterHit(pos)       │
                        │ + IsSunk: bool           │
                        └──────────────────────────┘
                                   △
                                   │
                    ┌──────────────┴──────────────┐
                    │                             │
         ┌──────────┴──────────┐       ┌─────────┴──────────┐
         │  CLASSIC PRODUCTS   │       │  MODERN PRODUCTS   │
         └─────────────────────┘       └────────────────────┘

┌────────────────────┐              ┌─────────────────────┐
│ Battleship         │              │ ModernBattleship    │
├────────────────────┤              ├─────────────────────┤
│ + Name = "Battleship"│            │ + Name = "Modern Battleship"│
│ + Length = 4       │              │ + Length = 5        │
│ + Kind = Battleship│              │ + Kind = Battleship │
└────────────────────┘              └─────────────────────┘

┌────────────────────┐              ┌─────────────────────┐
│ Submarine          │              │ ModernSubmarine     │
├────────────────────┤              ├─────────────────────┤
│ + Name = "Submarine"│             │ + Name = "Modern Submarine"│
│ + Length = 3       │              │ + Length = 4        │
│ + Kind = Submarine │              │ + Kind = Submarine  │
└────────────────────┘              └─────────────────────┘

┌────────────────────┐              ┌─────────────────────┐
│ Destroyer          │              │ ModernDestroyer     │
├────────────────────┤              ├─────────────────────┤
│ + Name = "Destroyer"│             │ + Name = "Modern Destroyer"│
│ + Length = 2       │              │ + Length = 3        │
│ + Kind = Destroyer │              │ + Kind = Destroyer  │
└────────────────────┘              └─────────────────────┘

┌────────────────────┐              ┌─────────────────────┐
│ Cruiser            │              │ ModernCruiser       │
├────────────────────┤              ├─────────────────────┤
│ + Name = "Cruiser" │              │ + Name = "Modern Cruiser"│
│ + Length = 1       │              │ + Length = 2        │
│ + Kind = Cruiser   │              │ + Kind = Cruiser    │
└────────────────────┘              └─────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│                    INTEGRATION WITH BUILDER PATTERN                           │
└──────────────────────────────────────────────────────────────────────────────┘

                ┌────────────────────────────────┐
                │   <<interface>>                │
                │   IBoardBuilder                │
                ├────────────────────────────────┤
                │ + BuildPart(shipKind)          │
                │ + GetResult(): Board           │
                └────────────────────────────────┘
                            △
                            │
                ┌───────────┴───────────┐
                │                       │
    ┌───────────▼────────────┐  ┌──────▼──────────────────┐
    │ RandomFleetBuilder     │  │ ManualFleetBuilder      │
    ├────────────────────────┤  ├─────────────────────────┤
    │ - _board: Board        │  │ - _board: Board         │
    │ - _factory: IShipFactory│ │ - _factory: IShipFactory│
    │ - _rng: Random         │  │                         │
    ├────────────────────────┤  ├─────────────────────────┤
    │ + BuildPart(kind)      │  │ + BuildPart(kind, pos,  │
    │   1. Choose random pos │  │              orientation)│
    │   2. _factory.CreateShip│ │   1. _factory.CreateShip│
    │   3. board.Place(ship) │  │   2. board.Place(ship)  │
    │ + GetResult()          │  │ + TryBuildPart()        │
    └────────────────────────┘  └─────────────────────────┘
                │                          │
                │ uses                     │ uses
                │                          │
                └──────────┬───────────────┘
                           │
                           ▼
                ┌────────────────────┐
                │   IShipFactory     │
                │   (injected)       │
                └────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│                      CLIENT: PlacementService                                 │
└──────────────────────────────────────────────────────────────────────────────┘

            ┌────────────────────────────────────────┐
            │  PlacementService                      │
            ├────────────────────────────────────────┤
            │  - _director: FleetDirector            │
            │  - _shipType: ShipType                 │
            ├────────────────────────────────────────┤
            │  + SetShipType(shipType)               │
            │  + RandomizeFleet(board)               │
            │    1. Create factory based on _shipType│
            │       • Classic → ClassicShipFactory   │
            │       • Modern → ModernShipFactory     │
            │    2. Create RandomFleetBuilder(factory)│
            │    3. _director.SetBuilder(builder)    │
            │    4. _director.Construct()            │
            └────────────────────────────────────────┘
                            │
                            │ selects factory
                            ▼
            ┌─────────────────────────────┐
            │   ShipType (enum)           │
            ├─────────────────────────────┤
            │   • Classic                 │
            │   • Modern                  │
            └─────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│                         USAGE FLOW DIAGRAM                                    │
└──────────────────────────────────────────────────────────────────────────────┘

User selects ShipType.Modern in UI
         │
         ▼
PlacementService.SetShipType(ShipType.Modern)
         │
         ▼
PlacementService.RandomizeFleet(board)
         │
         ├──> Create ModernShipFactory (based on ShipType)
         │
         ├──> Create RandomFleetBuilder(board, factory)
         │
         ├──> FleetDirector.SetBuilder(builder)
         │
         └──> FleetDirector.Construct()
                    │
                    ├──> builder.BuildPart(ShipKind.Battleship)
                    │         │
                    │         ├──> factory.CreateShip(Battleship, pos, orient)
                    │         │         │
                    │         │         └──> new ModernBattleship(pos, orient)
                    │         │
                    │         └──> board.Place(ship)
                    │
                    ├──> builder.BuildPart(ShipKind.Submarine)
                    │         └──> new ModernSubmarine(pos, orient)
                    │
                    ├──> builder.BuildPart(ShipKind.Destroyer)
                    │         └──> new ModernDestroyer(pos, orient)
                    │
                    └──> builder.BuildPart(ShipKind.Cruiser)
                              └──> new ModernCruiser(pos, orient)

┌──────────────────────────────────────────────────────────────────────────────┐
│                         KEY DESIGN PATTERNS                                   │
└──────────────────────────────────────────────────────────────────────────────┘

1. ABSTRACT FACTORY PATTERN
   • IShipFactory defines interface for creating ship families
   • ClassicShipFactory creates classic ships (length 4,3,2,1)
   • ModernShipFactory creates modern ships (length 5,4,3,2)
   • Ensures all ships from same family are consistent

2. BUILDER PATTERN (used with Abstract Factory)
   • RandomFleetBuilder builds fleet with random placement
   • ManualFleetBuilder builds fleet with manual placement
   • Both use IShipFactory to create ships

3. STRATEGY PATTERN (implicit)
   • Different ship families have different characteristics
   • ShipType enum selects which factory strategy to use

4. DEPENDENCY INJECTION
   • Builders accept IShipFactory in constructor
   • PlacementService creates appropriate factory
   • Components receive factory through parameters

┌──────────────────────────────────────────────────────────────────────────────┐
│                         BENEFITS OF THIS DESIGN                               │
└──────────────────────────────────────────────────────────────────────────────┘

✓ Open/Closed Principle: Can add new ship families without modifying existing code
✓ Single Responsibility: Each factory only creates one family
✓ Consistency: Ensures all ships in a game use same family
✓ Flexibility: Easy to switch families at runtime
✓ Testability: Can inject mock factories for testing
✓ Extensibility: Easy to add SciFi, Fantasy, or Historical ship families

┌──────────────────────────────────────────────────────────────────────────────┐
│                    SHIP KIND vs CONCRETE TYPE                                 │
└──────────────────────────────────────────────────────────────────────────────┘

ShipKind Enum (Game Logic)          Concrete Types (Implementation)
┌─────────────────────┐             ┌──────────────────────────────┐
│ • Battleship        │────────────>│ Battleship | ModernBattleship│
│ • Submarine         │────────────>│ Submarine  | ModernSubmarine │
│ • Destroyer         │────────────>│ Destroyer  | ModernDestroyer │
│ • Cruiser           │────────────>│ Cruiser    | ModernCruiser   │
└─────────────────────┘             └──────────────────────────────┘
   (4 kinds)                          (8 concrete classes)

Note: ship.Kind property maps concrete types back to ShipKind enum
      This allows validation and UI to work with both families
```

## Sequence Diagram: Creating a Modern Fleet

```
User          UI          PlacementService    ModernShipFactory    ModernBattleship
 │             │                 │                    │                   │
 │ Select      │                 │                    │                   │
 │ "Modern"    │                 │                    │                   │
 ├────────────>│                 │                    │                   │
 │             │ SetShipType     │                    │                   │
 │             │ (Modern)        │                    │                   │
 │             ├────────────────>│                    │                   │
 │             │                 │                    │                   │
 │ Click       │                 │                    │                   │
 │ "Randomize" │                 │                    │                   │
 ├────────────>│                 │                    │                   │
 │             │ RandomizeFleet()│                    │                   │
 │             ├────────────────>│                    │                   │
 │             │                 │ new                │                   │
 │             │                 │ ModernShipFactory()│                   │
 │             │                 ├───────────────────>│                   │
 │             │                 │                    │                   │
 │             │                 │ CreateBattleship() │                   │
 │             │                 ├───────────────────>│                   │
 │             │                 │                    │ new               │
 │             │                 │                    │ ModernBattleship()│
 │             │                 │                    ├──────────────────>│
 │             │                 │                    │                   │
 │             │                 │                    │ ModernBattleship  │
 │             │                 │                    │<──────────────────┤
 │             │                 │ ModernBattleship   │                   │
 │             │                 │<───────────────────┤                   │
 │             │                 │                    │                   │
 │             │                 │ (repeat for other ships...)            │
 │             │                 │                    │                   │
 │             │ Board with      │                    │                   │
 │             │ Modern Ships    │                    │                   │
 │             │<────────────────┤                    │                   │
 │ Display     │                 │                    │                   │
 │ Modern Ships│                 │                    │                   │
 │<────────────┤                 │                    │                   │
```

## File Structure

```
BattleShips/
├── Domain/
│   ├── ShipType.cs                    (Enum: Classic, Modern)
│   │
│   └── Ships/
│       ├── ShipBase.cs                (Abstract Product)
│       │   └── Kind property          (Maps concrete type to ShipKind)
│       │
│       ├── Battleship.cs              (Classic Product - Length 4)
│       ├── Submarine.cs               (Classic Product - Length 3)
│       ├── Destroyer.cs               (Classic Product - Length 2)
│       ├── Cruiser.cs                 (Classic Product - Length 1)
│       │
│       ├── Modern/
│       │   ├── ModernBattleship.cs   (Modern Product - Length 5)
│       │   ├── ModernSubmarine.cs    (Modern Product - Length 4)
│       │   ├── ModernDestroyer.cs    (Modern Product - Length 3)
│       │   └── ModernCruiser.cs      (Modern Product - Length 2)
│       │
│       └── Factories/
│           ├── IShipFactory.cs        (Abstract Factory Interface)
│           ├── ClassicShipFactory.cs  (Concrete Factory 1)
│           ├── ModernShipFactory.cs   (Concrete Factory 2)
│           └── README.md              (Pattern Documentation)
│
├── Services/
│   ├── PlacementService.cs            (Client - selects factory)
│   └── GameLobbyService.cs            (Client - uses factory)
│
└── Components/Pages/
    ├── Home.razor                      (UI - selects ShipType)
    └── Game.razor                      (UI - uses selected type)
```

---
*This UML diagram represents the Abstract Factory pattern implementation*
*for creating families of ships (Classic vs Modern) in the Battleships game.*
