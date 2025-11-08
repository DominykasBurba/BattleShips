# UML Class Diagram - BattleShips Complete System

## Overview
This document provides a comprehensive UML class diagram specification for the entire BattleShips project, including all design patterns, classes, interfaces, and relationships. This diagram should be used as a reference for implementing the complete system architecture in MagicDraw or similar UML modeling tools.

---

## Design Patterns Implemented

1. **Observer Pattern** - Game state changes
2. **Command Pattern** - Chat message operations
3. **Builder Pattern** - Board/Fleet construction
4. **Strategy Pattern** - Attack strategies (Single shot vs 3x3 Salvo)
5. **Abstract Factory Pattern** - Ship creation (Classic vs Modern)
6. **Factory Method Pattern** - Cell creation
7. **Decorator Pattern** - Ship skins (Camouflage)
8. **Adapter Pattern** - Fleet placement service
9. **Singleton Pattern** - GameSession instance

---

## 1. Observer Pattern

### 1.1 Subject (Abstract Class)
**Stereotype**: `<<abstract>>`  
**Location**: Top-left of diagram

**Attributes:**
- `-observers: List<IObserver>` (private)

**Methods:**
- `+Attach(observer: IObserver): void`
- `+Detach(observer: IObserver): void`
- `#Notify(): void` (protected)

**Notes:**
- Contains algorithm: `for all o in observers { o->Update(); }`

**Relationships:**
- Composition (filled diamond) with `IObserver`
- Inheritance relationship with `GameSession`

---

### 1.2 IObserver (Interface)
**Stereotype**: `<<interface>>`  
**Location**: Top-right of diagram

**Methods:**
- `+Update(): void`

**Relationships:**
- Realization relationship (dashed line with hollow triangle) with concrete observers

---

### 1.3 GameSession (ConcreteSubject)
**Stereotype**: `<<ConcreteSubject>>` / `<<Singleton>>`  
**Location**: Center-left, inheriting from Subject

**Attributes:**
- `-instance: GameSession?` (private static)
- `-lock: object` (private static)
- `-phase: Phase` (private)
- `-current: Player` (private)
- `-winner: Player?` (private)
- `-draw: DrawState` (private)
- `+P1: Player` (public readonly)
- `+P2: Player` (public readonly)
- `+ShotsPerTurn: int` (public)

**Properties:**
- `+Phase: Phase` (getter/setter - triggers Notify on change)
- `+Current: Player` (getter/setter - triggers Notify on change)
- `+Winner: Player?` (getter/setter - triggers Notify on change)
- `+Draw: DrawState` (getter/setter - triggers Notify on change)
- `+Opponent: Player` (computed property)

**Methods:**
- `+GetInstance(p1: Player, p2: Player): GameSession` (static, Singleton)
- `+Initialize(p1: Player, p2: Player): void` (private)
- `+ResetInstance(): void` (static)
- `+TryStart(): bool`
- `+ResetBoards(): void`
- `+SetShotsPerTurn(n: int): void`
- `+Fire(pos: Position): ShotResult`
- `+EndTurn(): void`
- `+Surrender(who: Player): void`
- `+ProposeDraw(who: Player): void`
- `+AcceptDraw(who: Player): void`
- `-HasCompleteFleet(p: Player): bool` (private static)

**Notes:**
- Implements Singleton pattern
- Notifies observers when Phase, Current, Winner, or Draw changes

**Relationships:**
- Inherits from `Subject`
- Association with `Player` (P1, P2, Current, Winner)
- Uses `Phase`, `ShotResult`, `DrawState` enums
- Dependency on `Position`

---

### 1.4 Concrete Observers

#### 1.4.1 GameStateObserver
**Stereotype**: `<<ConcreteObserver>>`

**Attributes:**
- `-subject: GameSession` (private readonly)
- `-observerState: Phase` (private)

**Methods:**
- `+Update(): void`
- `+GetState(): Phase`

**Relationships:**
- Implements `IObserver`
- Dependency on `GameSession`
- Uses `Phase` enum

---

#### 1.4.2 TurnChangeObserver
**Stereotype**: `<<ConcreteObserver>>`

**Attributes:**
- `-subject: GameSession` (private readonly)
- `-observerState: Player?` (private)

**Methods:**
- `+Update(): void`
- `+GetState(): Player?`

**Relationships:**
- Implements `IObserver`
- Dependency on `GameSession`
- Dependency on `Player`

---

#### 1.4.3 GameEndObserver
**Stereotype**: `<<ConcreteObserver>>`

**Attributes:**
- `-subject: GameSession` (private readonly)
- `-observerState: Player?` (private)

**Methods:**
- `+Update(): void`
- `+GetState(): Player?`

**Relationships:**
- Implements `IObserver`
- Dependency on `GameSession`
- Dependency on `Player`

---

## 2. Command Pattern & Chat

### 2.1 ICommand (Interface)
**Stereotype**: `<<interface>>`  
**Location**: Top-middle

**Methods:**
- `+Execute(): void`
- `+Undo(): void`

**Relationships:**
- Realization relationship with concrete commands

---

### 2.2 ChatInvoker (Invoker)
**Stereotype**: `<<Invoker>>`

**Attributes:**
- `-command: ICommand?` (private)

**Methods:**
- `+SetCommand(command: ICommand): void`
- `+Run(): void`
- `+RunCommand(command: ICommand): void`

**Relationships:**
- Association with `ICommand`

---

### 2.3 SendMessageCommand (ConcreteCommand)
**Stereotype**: `<<ConcreteCommand>>`

**Attributes:**
- `-chatService: ChatService` (private readonly)
- `-sender: string` (private readonly)
- `-text: string` (private readonly)
- `-_message: ChatMessage?` (private)

**Methods:**
- `+Execute(): void`
- `+Undo(): void`
- `+SendMessageCommand(chatService: ChatService, sender: string, text: string)` (constructor)

**Relationships:**
- Implements `ICommand`
- Dependency on `ChatService`
- Dependency on `ChatMessage`

---

### 2.4 ClearChatCommand (ConcreteCommand)
**Stereotype**: `<<ConcreteCommand>>`

**Attributes:**
- `-chatService: ChatService` (private readonly)
- `-_backup: List<ChatMessage>?` (private)

**Methods:**
- `+Execute(): void`
- `+Undo(): void`
- `+ClearChatCommand(chatService: ChatService)` (constructor)

**Relationships:**
- Implements `ICommand`
- Dependency on `ChatService`
- Dependency on `ChatMessage`

---

### 2.5 ChatService (Receiver)
**Stereotype**: `<<Receiver>>`

**Attributes:**
- `-_messages: List<ChatMessage>` (private)
- `+Messages: IReadOnlyList<ChatMessage>` (public readonly property)
- `+Updated: event Action?` (public event)

**Methods:**
- `+SendMessage(message: ChatMessage): void`
- `+RemoveMessage(message: ChatMessage): void`
- `+ClearMessages(): void`
- `+RestoreMessages(messages: IEnumerable<ChatMessage>): void`

**Relationships:**
- Aggregation (hollow diamond) with `ChatMessage`

---

### 2.6 ChatMessage (Record)
**Stereotype**: `<<record>>`

**Attributes:**
- `+Sender: string` (public readonly)
- `+Text: string` (public readonly)
- `+Timestamp: DateTime` (public readonly)

**Methods:**
- `+ChatMessage(sender: string, text: string, timestamp: DateTime)` (constructor)

**Relationships:**
- Used by `ChatService`

---

## 3. Builder Pattern

### 3.1 FleetDirector (Director)
**Stereotype**: `<<Director>>`  
**Location**: Middle-right

**Attributes:**
- `-builder: IBoardBuilder?` (private)

**Methods:**
- `+SetBuilder(builder: IBoardBuilder): void`
- `+Construct(): Board`
- `+Construct(composition: IEnumerable<ShipKind>): Board`

**Notes:**
- Knows the construction sequence (fleet composition)
- Orchestrates the building process

**Relationships:**
- Association with `IBoardBuilder`
- Returns `Board` (Product)
- Uses `ShipKind` enum
- Uses `DefaultFleet` static class

---

### 3.2 IBoardBuilder (Abstract Builder Interface)
**Stereotype**: `<<interface>>`

**Methods:**
- `+BuildPart(shipKind: ShipKind): void`
- `+GetResult(): Board`

**Relationships:**
- Realization relationship with concrete builders

---

### 3.3 RandomFleetBuilder (Concrete Builder)
**Stereotype**: `<<ConcreteBuilder>>`

**Attributes:**
- `-_board: Board` (private readonly)
- `-_rng: Random` (private readonly)
- `-_shipFactory: IShipFactory` (private readonly)
- `-_shipSkin: ShipSkin` (private readonly)

**Methods:**
- `+BuildPart(shipKind: ShipKind): void`
- `+GetResult(): Board`
- `+RandomFleetBuilder(board: Board, shipFactory: IShipFactory?, shipSkin: ShipSkin)` (constructor)

**Relationships:**
- Implements `IBoardBuilder`
- Association with `Board`
- Dependency on `IShipFactory`
- Uses `ShipKind`, `ShipSkin` enums
- Uses `Position`, `Orientation`

---

### 3.4 ManualFleetBuilder (Concrete Builder)
**Stereotype**: `<<ConcreteBuilder>>`

**Attributes:**
- `-_board: Board` (private readonly)
- `-_shipFactory: IShipFactory` (private readonly)
- `-_pendingPosition: Position?` (private)
- `-_pendingOrientation: Orientation?` (private)

**Methods:**
- `+BuildPart(shipKind: ShipKind): void`
- `+BuildPart(shipKind: ShipKind, position: Position, orientation: Orientation): void`
- `+TryBuildPart(shipKind: ShipKind, position: Position, orientation: Orientation): bool`
- `+SetNextShipPlacement(position: Position, orientation: Orientation): void`
- `+GetResult(): Board`
- `+ManualFleetBuilder(board: Board, shipFactory: IShipFactory?)` (constructor)

**Relationships:**
- Implements `IBoardBuilder`
- Association with `Board`
- Dependency on `IShipFactory`
- Uses `ShipKind`, `Position`, `Orientation`

---

### 3.5 Board (Product)
**Stereotype**: `<<Product>>`

**Attributes:**
- `+Size: int` (public readonly)
- `+Ships: IReadOnlyList<IShip>` (public readonly property)
- `-_cells: Cell[,]` (private)
- `-_ships: List<IShip>` (private)
- `-_cellFactory: CellFactory` (private readonly)

**Methods:**
- `+Board(size: int, cellFactory: CellFactory?)` (constructor)
- `+this[r: int, c: int]: Cell` (indexer)
- `+this[p: Position]: Cell` (indexer)
- `+Clear(): void`
- `+Place(ship: IShip): bool`
- `+Remove(ship: IShip): void`
- `+CanPlace(ship: IShip): bool`
- `+FireAt(p: Position): ShotResult`
- `+AllShipsSunk: bool` (property)
- `-HasAdjacentShip(p: Position): bool` (private)

**Relationships:**
- Composition (filled diamond) with `Cell`
- Aggregation (hollow diamond) with `IShip`
- Association with `CellFactory`
- Uses `Position`, `CellStatus`, `ShotResult` enums

---

### 3.6 DefaultFleet (Static Class)
**Stereotype**: `<<utility>>`

**Attributes:**
- `+Composition: ShipKind[]` (public static readonly)

**Relationships:**
- Uses `ShipKind` enum
- Used by `FleetDirector`

---

## 4. Strategy Pattern

### 4.1 IAttackStrategy (Strategy Interface)
**Stereotype**: `<<interface>>` / `<<strategy>>`  
**Location**: Middle-left

**Methods:**
- `+ExecuteAttack(session: GameSession, position: Position): AttackResult`

**Relationships:**
- Realization relationship with concrete strategies
- Returns `AttackResult`

---

### 4.2 SingleShotStrategy (Concrete Strategy)
**Stereotype**: `<<ConcreteStrategy>>`

**Methods:**
- `+ExecuteAttack(session: GameSession, position: Position): AttackResult`

**Relationships:**
- Implements `IAttackStrategy`
- Dependency on `GameSession`
- Uses `Position`, `Phase`, `ShotResult` enums
- Returns `AttackResult`

---

### 4.3 Salvo3x3Strategy (Concrete Strategy)
**Stereotype**: `<<ConcreteStrategy>>`

**Methods:**
- `+ExecuteAttack(session: GameSession, centerPosition: Position): AttackResult`

**Relationships:**
- Implements `IAttackStrategy`
- Dependency on `GameSession`
- Uses `Position`, `Phase`, `ShotResult` enums
- Returns `AttackResult`

---

### 4.4 AttackResult
**Stereotype**: `<<DataClass>>`

**Attributes:**
- `+Results: List<ShotResult>` (public)
- `+Positions: List<Position>` (public)
- `+ShouldEndTurn: bool` (public)
- `+SingleResult: ShotResult?` (public property)

**Methods:**
- `+AttackResult(results: List<ShotResult>, positions: List<Position>, shouldEndTurn: bool)` (constructor)
- `+AttackResult(result: ShotResult, position: Position, shouldEndTurn: bool)` (constructor)

**Relationships:**
- Uses `ShotResult` enum
- Uses `Position`

---

## 5. Abstract Factory Pattern & Factory Method

### 5.1 IShipFactory (Abstract Factory Interface)
**Stereotype**: `<<interface>>` / `<<AbstractFactory>>`  
**Location**: Bottom-right

**Methods:**
- `+CreateBattleship(start: Position, orientation: Orientation): IShip`
- `+CreateSubmarine(start: Position, orientation: Orientation): IShip`
- `+CreateDestroyer(start: Position, orientation: Orientation): IShip`
- `+CreateCruiser(start: Position, orientation: Orientation): IShip`
- `+CreateShip(kind: ShipKind, start: Position, orientation: Orientation): IShip`

**Relationships:**
- Realization relationship with concrete factories
- Returns `IShip`
- Uses `Position`, `Orientation`, `ShipKind`

---

### 5.2 ClassicShipFactory (Concrete Factory)
**Stereotype**: `<<ConcreteFactory>>`

**Methods:**
- `+CreateBattleship(start: Position, orientation: Orientation): IShip`
- `+CreateSubmarine(start: Position, orientation: Orientation): IShip`
- `+CreateDestroyer(start: Position, orientation: Orientation): IShip`
- `+CreateCruiser(start: Position, orientation: Orientation): IShip`
- `+CreateShip(kind: ShipKind, start: Position, orientation: Orientation): IShip`

**Relationships:**
- Implements `IShipFactory`
- Creates: `Battleship`, `Submarine`, `Destroyer`, `Cruiser`
- Uses `Position`, `Orientation`, `ShipKind`

---

### 5.3 ModernShipFactory (Concrete Factory)
**Stereotype**: `<<ConcreteFactory>>`

**Methods:**
- `+CreateBattleship(start: Position, orientation: Orientation): IShip`
- `+CreateSubmarine(start: Position, orientation: Orientation): IShip`
- `+CreateDestroyer(start: Position, orientation: Orientation): IShip`
- `+CreateCruiser(start: Position, orientation: Orientation): IShip`
- `+CreateShip(kind: ShipKind, start: Position, orientation: Orientation): IShip`

**Relationships:**
- Implements `IShipFactory`
- Creates: `ModernBattleship`, `ModernSubmarine`, `ModernDestroyer`, `ModernCruiser`
- Uses `Position`, `Orientation`, `ShipKind`

---

### 5.4 CellFactory (Abstract Creator - Factory Method)
**Stereotype**: `<<abstract>>` / `<<FactoryMethod>>`

**Methods:**
- `#CreateCell(row: int, col: int): Cell` (abstract)
- `+CreateCellWithPosition(position: Position): Cell` (virtual)

**Relationships:**
- Inheritance relationship with `StandardCellFactory`
- Returns `Cell`
- Uses `Position`

---

### 5.5 StandardCellFactory (Concrete Creator)
**Stereotype**: `<<ConcreteCreator>>`

**Methods:**
- `+CreateCell(row: int, col: int): Cell` (override)

**Relationships:**
- Inherits from `CellFactory`
- Creates `Cell` instances

---

## 6. Decorator Pattern

### 6.1 IShip (Component Interface)
**Stereotype**: `<<interface>>`  
**Location**: Bottom-right

**Attributes:**
- `+Name: string` (readonly)
- `+Length: int` (readonly)
- `+Start: Position` (readonly)
- `+Orientation: Orientation` (readonly)
- `+IsSunk: bool` (readonly)
- `+Kind: ShipKind` (readonly)
- `+Skin: ShipSkin` (readonly)
- `+HitCount: int` (readonly)

**Methods:**
- `+Cells(): IEnumerable<Position>`
- `+Reposition(start: Position, orientation: Orientation): void`
- `+RegisterHit(p: Position): bool`

**Relationships:**
- Realization relationship with `ShipBase`
- Used by `ShipDecorator`
- Uses `Position`, `Orientation`, `ShipKind`, `ShipSkin`

---

### 6.2 ShipBase (Concrete Component)
**Stereotype**: `<<ConcreteComponent>>`

**Attributes:**
- `+Name: string` (abstract)
- `+Length: int` (abstract)
- `+Start: Position` (private set)
- `+Orientation: Orientation` (private set)
- `+Skin: ShipSkin` (virtual, default: Default)
- `-hits: HashSet<Position>` (private)
- `+HitCount: int` (readonly property)

**Methods:**
- `+Cells(): IEnumerable<Position>`
- `+Reposition(start: Position, orientation: Orientation): void` (virtual)
- `+RegisterHit(p: Position): bool` (virtual)
- `+IsSunk: bool` (virtual property)
- `+Kind: ShipKind` (computed property)

**Relationships:**
- Implements `IShip`
- Inheritance relationship with concrete ships
- Uses `Position`, `Orientation`, `ShipKind`, `ShipSkin`

---

### 6.3 Concrete Ship Classes

#### 6.3.1 Battleship
**Attributes:**
- `+Name: string = "Battleship"`
- `+Length: int = 4`

**Relationships:**
- Inherits from `ShipBase`

---

#### 6.3.2 Submarine
**Attributes:**
- `+Name: string = "Submarine"`
- `+Length: int = 3`

**Relationships:**
- Inherits from `ShipBase`

---

#### 6.3.3 Destroyer
**Attributes:**
- `+Name: string = "Destroyer"`
- `+Length: int = 2`

**Relationships:**
- Inherits from `ShipBase`

---

#### 6.3.4 Cruiser
**Attributes:**
- `+Name: string = "Cruiser"`
- `+Length: int = 1`

**Relationships:**
- Inherits from `ShipBase`

---

#### 6.3.5 ModernBattleship
**Attributes:**
- `+Name: string = "Modern Battleship"`
- `+Length: int = 5`

**Relationships:**
- Inherits from `ShipBase`

---

#### 6.3.6 ModernSubmarine
**Attributes:**
- `+Name: string = "Modern Submarine"`
- `+Length: int = 4`

**Relationships:**
- Inherits from `ShipBase`

---

#### 6.3.7 ModernDestroyer
**Attributes:**
- `+Name: string = "Modern Destroyer"`
- `+Length: int = 3`

**Relationships:**
- Inherits from `ShipBase`

---

#### 6.3.8 ModernCruiser
**Attributes:**
- `+Name: string = "Modern Cruiser"`
- `+Length: int = 2`

**Relationships:**
- Inherits from `ShipBase`

---

### 6.4 ShipDecorator (Abstract Decorator)
**Stereotype**: `<<abstract>>` / `<<Decorator>>`

**Attributes:**
- `#WrappedShip: IShip` (protected readonly)

**Methods:**
- `+Name: string` (virtual, delegates to WrappedShip)
- `+Length: int` (virtual, delegates to WrappedShip)
- `+Start: Position` (virtual, delegates to WrappedShip)
- `+Orientation: Orientation` (virtual, delegates to WrappedShip)
- `+IsSunk: bool` (virtual, delegates to WrappedShip)
- `+Kind: ShipKind` (virtual, delegates to WrappedShip)
- `+Skin: ShipSkin` (virtual, delegates to WrappedShip)
- `+HitCount: int` (virtual, delegates to WrappedShip)
- `+Cells(): IEnumerable<Position>` (virtual, delegates to WrappedShip)
- `+Reposition(start: Position, orientation: Orientation): void` (virtual, delegates to WrappedShip)
- `+RegisterHit(p: Position): bool` (virtual, delegates to WrappedShip)

**Relationships:**
- Implements `IShip`
- Aggregation (hollow diamond) with `IShip`
- Inheritance relationship with `SkinnedShipDecorator`

---

### 6.5 SkinnedShipDecorator (Concrete Decorator)
**Stereotype**: `<<ConcreteDecorator>>`

**Attributes:**
- `-_shieldConsumed: bool` (private)
- `+Skin: ShipSkin` (override)

**Methods:**
- `+RegisterHit(p: Position): bool` (override - implements camouflage shield)
- `+IsSunk: bool` (override)
- `+SkinnedShipDecorator(ship: IShip, skin: ShipSkin)` (constructor)

**Notes:**
- Camouflage skin: first hit is absorbed (no damage)
- Subsequent hits deal normal damage

**Relationships:**
- Inherits from `ShipDecorator`
- Uses `ShipSkin` enum

---

## 7. Adapter Pattern

### 7.1 IFleetPlacer (Target Interface)
**Stereotype**: `<<interface>>` / `<<Target>>`

**Methods:**
- `+PlaceFleet(board: Board): void`
- `+SetShipType(shipType: ShipType): void`
- `+SetShipSkin(shipSkin: ShipSkin): void`

**Relationships:**
- Realization relationship with `RandomFleetPlacerAdapter`
- Uses `Board`, `ShipType`, `ShipSkin`

---

### 7.2 PlacementService (Adaptee)
**Stereotype**: `<<Adaptee>>`

**Attributes:**
- `-_director: FleetDirector` (private readonly)
- `-_shipType: ShipType` (private)
- `-_shipSkin: ShipSkin` (private)

**Methods:**
- `+RandomizeFleet(board: Board): void`
- `+SetShipType(shipType: ShipType): void`
- `+SetShipSkin(shipSkin: ShipSkin): void`
- `+TryPlace(board: Board, ship: IShip): bool`
- `+TryRotate(board: Board, ship: IShip): bool`

**Relationships:**
- Association with `FleetDirector`
- Uses `Board`, `IShip`, `ShipType`, `ShipSkin`

---

### 7.3 RandomFleetPlacerAdapter (Adapter)
**Stereotype**: `<<Adapter>>`

**Attributes:**
- `-placementService: PlacementService` (private readonly)

**Methods:**
- `+PlaceFleet(board: Board): void`
- `+SetShipType(shipType: ShipType): void`
- `+SetShipSkin(shipSkin: ShipSkin): void`
- `+RandomFleetPlacerAdapter(placementService: PlacementService)` (constructor)

**Relationships:**
- Implements `IFleetPlacer`
- Composition (filled diamond) with `PlacementService`
- Uses `Board`, `ShipType`, `ShipSkin`

---

## 8. Game Services

### 8.1 GameService
**Stereotype**: `<<Service>>`

**Attributes:**
- `+Session: GameSession?` (public property)
- `-fleetPlacer: IFleetPlacer` (private readonly)
- `-attackStrategy: IAttackStrategy` (private)
- `-shotsUsedThisTurn: int` (private)
- `+KeepTurnOnHit: bool` (public property)
- `+ShootingMode: ShootingMode` (public property)

**Methods:**
- `+NewLocalSession(size: int, enemyIsAi: bool, shipType: ShipType, shipSkin: ShipSkin): void`
- `+ResetShips(): void`
- `+ClearSession(): void`
- `+RandomizeFor(who: Player): void`
- `+SetShotsPerTurn(n: int): void`
- `+SetShootingMode(mode: ShootingMode): void`
- `+Start(): bool`
- `+FireAt(pos: Position): ShotResult`
- `+Fire3x3Salvo(centerPos: Position): List<ShotResult>`
- `+HandleAiTurn(): void`
- `+Surrender(who: Player): void`
- `+ProposeDraw(who: Player): void`
- `+AcceptDraw(who: Player): void`
- `-GetStrategyForMode(mode: ShootingMode): IAttackStrategy` (private)

**Relationships:**
- Association with `GameSession`
- Dependency on `IFleetPlacer`
- Dependency on `IAttackStrategy`
- Dependency on `Player`
- Uses `ShootingMode`, `ShipType`, `ShipSkin`, `Position`, `ShotResult` enums

---

### 8.2 GameLobbyService
**Stereotype**: `<<Service>>`

**Attributes:**
- `-games: ConcurrentDictionary<string, OnlineGameSession>` (private readonly)
- `-connectionToGame: ConcurrentDictionary<string, string>` (private readonly)
- `-placementService: PlacementService` (private readonly)

**Methods:**
- `+CreateGame(connectionId: string, boardSize: int, shootingMode: ShootingMode, shipType: ShipType): string`
- `+JoinGame(gameId: string, connectionId: string): (bool, int, ShootingMode, ShipType)`
- `+PlaceShips(gameId: string, connectionId: string, ships: List<ShipPlacement>): bool`
- `+AreBothPlayersReady(gameId: string): bool`
- `+StartGame(gameId: string): bool`
- `+GetCurrentPlayer(gameId: string): string?`
- `+FireShot(gameId: string, connectionId: string, position: Position): ...?`
- `+Fire3x3Salvo(gameId: string, connectionId: string, centerPosition: Position): ...?`
- `+Surrender(gameId: string, connectionId: string): void`
- `+ProposeDraw(gameId: string, connectionId: string): void`
- `+AcceptDraw(gameId: string, connectionId: string): bool`
- `+GetGameIdForConnection(connectionId: string): string?`
- `+GetPlayerName(gameId: string, connectionId: string): string?`
- `+RemovePlayer(connectionId: string): void`
- `-GenerateGameId(): string` (private)
- `-GetStrategyForMode(mode: ShootingMode): IAttackStrategy` (private)

**Relationships:**
- Composition (filled diamond) with `OnlineGameSession`
- Dependency on `PlacementService`
- Dependency on `IAttackStrategy`
- Uses `ShootingMode`, `ShipType`, `Position`, `ShipPlacement`

---

### 8.3 OnlineGameSession
**Stereotype**: `<<DataClass>>`

**Attributes:**
- `+GameId: string` (public readonly)
- `+Player1ConnectionId: string` (public readonly)
- `+Player2ConnectionId: string?` (public)
- `+GameSession: GameSession` (public)
- `+Player1Ready: bool` (public)
- `+Player2Ready: bool` (public)
- `+ShootingMode: ShootingMode` (public)
- `+ShipType: ShipType` (public)
- `+ShotsUsedThisTurn: int` (public)

**Methods:**
- `+GetPlayer(connectionId: string): Player?`
- `+GetConnectionId(player: Player): string?`
- `+OnlineGameSession(gameId: string, player1ConnectionId: string, boardSize: int, shootingMode: ShootingMode, shipType: ShipType)` (constructor)

**Notes:**
- Creates `GameSession` instance
- Attaches observers to `GameSession`

**Relationships:**
- Association with `GameSession`
- Dependency on `Player`
- Uses `ShootingMode`, `ShipType`

---

## 9. Player Classes

### 9.1 Player (Abstract Class)
**Stereotype**: `<<abstract>>`

**Attributes:**
- `+Name: string` (public readonly)
- `+Board: Board` (public readonly)
- `+Kind: PlayerKind` (public readonly)

**Methods:**
- `+ChooseTarget(enemyBoard: Board, tried: HashSet<Position>): Position` (virtual)

**Relationships:**
- Composition (filled diamond) with `Board`
- Inheritance relationship with `HumanPlayer`, `AiPlayer`
- Uses `PlayerKind` enum

---

### 9.2 HumanPlayer
**Stereotype**: `<<ConcretePlayer>>`

**Attributes:**
- (inherits from Player)

**Methods:**
- `+HumanPlayer(name: string, boardSize: int)` (constructor)

**Relationships:**
- Inherits from `Player`

---

### 9.3 AiPlayer
**Stereotype**: `<<ConcretePlayer>>`

**Attributes:**
- `-rng: Random` (private readonly)
- `-tried: HashSet<Position>` (private readonly)

**Methods:**
- `+ChooseTarget(enemyBoard: Board, tried: HashSet<Position>): Position` (override)
- `+AiPlayer(name: string, boardSize: int)` (constructor)

**Relationships:**
- Inherits from `Player`
- Uses `Position`, `Board`

---

## 10. Supporting Classes

### 10.1 Cell
**Stereotype**: `<<DataClass>>`

**Attributes:**
- `+Pos: Position` (public readonly)
- `+Status: CellStatus` (public)
- `+Ship: IShip?` (public)
- `+IsRevealed: bool` (computed property)

**Methods:**
- `+Cell(r: int, c: int)` (constructor)

**Relationships:**
- Uses `Position`
- Uses `CellStatus` enum
- Association with `IShip`

---

### 10.2 Position
**Stereotype**: `<<record>>`

**Attributes:**
- `+Row: int` (public readonly)
- `+Col: int` (public readonly)

**Methods:**
- `+InBounds(size: int): bool`
- `+ToString(): string`

**Relationships:**
- Used throughout the system

---

### 10.3 GameHub (SignalR Hub)
**Stereotype**: `<<Hub>>`

**Attributes:**
- `-lobby: GameLobbyService` (private readonly, injected)

**Methods:**
- `+OnConnectedAsync(): Task`
- `+OnDisconnectedAsync(exception: Exception?): Task`
- `+CreateGame(boardSize: int, shootingMode: ShootingMode, shipType: ShipType): Task<string>`
- `+JoinGame(gameId: string): Task<JoinGameResult?>`
- `+PlaceShips(gameId: string, ships: List<ShipPlacement>): Task`
- `+StartGame(gameId: string): Task`
- `+FireShot(gameId: string, position: Position): Task`
- `+Fire3x3Salvo(gameId: string, centerPosition: Position): Task`
- `+Surrender(gameId: string): Task`
- `+ProposeDraw(gameId: string): Task`
- `+AcceptDraw(gameId: string): Task`
- `+SendMessage(gameId: string, message: string): Task`

**Relationships:**
- Dependency on `GameLobbyService`
- Inherits from SignalR `Hub`
- Uses `ShootingMode`, `ShipType`, `Position`, `ShipPlacement`, `JoinGameResult`

---

### 10.4 ShipPlacement (Record)
**Stereotype**: `<<record>>`

**Attributes:**
- `+Kind: ShipKind` (public readonly)
- `+Start: Position` (public readonly)
- `+IsHorizontal: bool` (public readonly)

**Relationships:**
- Uses `ShipKind`, `Position`

---

### 10.5 JoinGameResult (Record)
**Stereotype**: `<<record>>`

**Attributes:**
- `+Success: bool` (public readonly)
- `+BoardSize: int` (public readonly)
- `+ShootingMode: ShootingMode` (public readonly)
- `+ShipType: ShipType` (public readonly)

**Relationships:**
- Uses `ShootingMode`, `ShipType`

---

## 11. Enumerations

### 11.1 CellStatus
**Values:**
- `Empty`
- `Ship`
- `Hit`
- `Miss`
- `Sunk`
- `Shielded`

---

### 11.2 Orientation
**Values:**
- `Horizontal`
- `Vertical`

---

### 11.3 ShotResult
**Values:**
- `Invalid`
- `AlreadyTried`
- `Miss`
- `Hit`
- `Sunk`

---

### 11.4 Phase
**Values:**
- `Preparation`
- `Playing`
- `Finished`

---

### 11.5 PlayerKind
**Values:**
- `Human`
- `AI`

---

### 11.6 DrawState
**Values:**
- `None`
- `ProposedByP1`
- `ProposedByP2`
- `Accepted`

---

### 11.7 ShootingMode
**Values:**
- `Single`
- `Salvo3x3`

---

### 11.8 ShipKind
**Values:**
- `Carrier`
- `Battleship`
- `Cruiser`
- `Submarine`
- `Destroyer`

---

### 11.9 ShipSkin
**Values:**
- `Default`
- `Camouflage`

---

### 11.10 ShipType
**Values:**
- `Classic`
- `Modern`

---

## 12. Relationships Summary

### 12.1 Inheritance Relationships
- `Subject` ← `GameSession`
- `IObserver` ← `GameStateObserver`, `TurnChangeObserver`, `GameEndObserver`
- `ICommand` ← `SendMessageCommand`, `ClearChatCommand`
- `IBoardBuilder` ← `RandomFleetBuilder`, `ManualFleetBuilder`
- `IAttackStrategy` ← `SingleShotStrategy`, `Salvo3x3Strategy`
- `IShipFactory` ← `ClassicShipFactory`, `ModernShipFactory`
- `CellFactory` ← `StandardCellFactory`
- `IShip` ← `ShipBase` ← `Battleship`, `Submarine`, `Destroyer`, `Cruiser`, `ModernBattleship`, `ModernSubmarine`, `ModernDestroyer`, `ModernCruiser`
- `ShipDecorator` ← `SkinnedShipDecorator`
- `Player` ← `HumanPlayer`, `AiPlayer`
- `IFleetPlacer` ← `RandomFleetPlacerAdapter`

### 12.2 Composition Relationships (Filled Diamond)
- `Subject` ◆─→ `IObserver`
- `GameSession` ◆─→ `Player` (P1, P2)
- `Board` ◆─→ `Cell`
- `RandomFleetPlacerAdapter` ◆─→ `PlacementService`
- `GameLobbyService` ◆─→ `OnlineGameSession`

### 12.3 Aggregation Relationships (Hollow Diamond)
- `ChatService` ◇─→ `ChatMessage`
- `Board` ◇─→ `IShip`
- `ShipDecorator` ◇─→ `IShip`

### 12.4 Association Relationships
- `FleetDirector` ─→ `IBoardBuilder`
- `RandomFleetBuilder` ─→ `Board`
- `ManualFleetBuilder` ─→ `Board`
- `Board` ─→ `CellFactory`
- `GameService` ─→ `GameSession`
- `GameService` ─→ `IFleetPlacer`
- `GameService` ─→ `IAttackStrategy`
- `GameLobbyService` ─→ `PlacementService`
- `GameLobbyService` ─→ `IAttackStrategy`
- `OnlineGameSession` ─→ `GameSession`
- `PlacementService` ─→ `FleetDirector`
- `ChatInvoker` ─→ `ICommand`
- `SendMessageCommand` ─→ `ChatService`
- `ClearChatCommand` ─→ `ChatService`
- `Cell` ─→ `IShip`
- `GameHub` ─→ `GameLobbyService`

### 12.5 Dependency Relationships (Dashed Arrow)
- `GameSession` ─ ─ → `Position`
- `GameSession` ─ ─ → `Phase`, `ShotResult`, `DrawState`
- `RandomFleetBuilder` ─ ─ → `IShipFactory`
- `ManualFleetBuilder` ─ ─ → `IShipFactory`
- `GameService` ─ ─ → `Player`
- `GameService` ─ ─ → `ShootingMode`, `ShipType`, `ShipSkin`
- `GameLobbyService` ─ ─ → `IAttackStrategy`
- `GameLobbyService` ─ ─ → `Position`, `ShipPlacement`
- `OnlineGameSession` ─ ─ → `Player`
- `AiPlayer` ─ ─ → `Position`, `Board`
- `GameHub` ─ ─ → `ShootingMode`, `ShipType`, `Position`

---

## 13. Diagram Layout Recommendations

### 13.1 Suggested Organization

1. **Top Section - Observer Pattern:**
   - `Subject` (left)
   - `IObserver` (right)
   - `GameSession` (center, below Subject)
   - Three concrete observers (right side)

2. **Top-Middle Section - Command Pattern:**
   - `ICommand` (center)
   - `ChatInvoker` (below ICommand)
   - `SendMessageCommand`, `ClearChatCommand` (left and right)
   - `ChatService` (below commands)
   - `ChatMessage` (below ChatService)

3. **Middle-Left Section - Strategy Pattern:**
   - `IAttackStrategy` (center)
   - `SingleShotStrategy`, `Salvo3x3Strategy` (below)
   - `AttackResult` (right side)

4. **Middle-Right Section - Builder Pattern:**
   - `FleetDirector` (top)
   - `IBoardBuilder` (below Director)
   - `RandomFleetBuilder`, `ManualFleetBuilder` (below interface)
   - `Board` (bottom)
   - `DefaultFleet` (top-right)

5. **Bottom-Right Section - Factory & Decorator:**
   - `IShipFactory` (top)
   - `ClassicShipFactory`, `ModernShipFactory` (below)
   - `IShip` (center)
   - `ShipBase` (below IShip)
   - Concrete ships (below ShipBase, in two columns: Classic and Modern)
   - `ShipDecorator` (right side)
   - `SkinnedShipDecorator` (below decorator)

6. **Bottom-Left Section - Adapter & Services:**
   - `IFleetPlacer` (top)
   - `RandomFleetPlacerAdapter` (below)
   - `PlacementService` (below adapter)
   - `GameService` (left)
   - `GameLobbyService` (center)
   - `OnlineGameSession` (below GameLobbyService)

7. **Left Side - Player & Board:**
   - `Player` (abstract, top)
   - `HumanPlayer`, `AiPlayer` (below Player)
   - `CellFactory` (center)
   - `StandardCellFactory` (below)
   - `Cell` (bottom)

8. **Supporting Classes:**
   - `Position` (record, center)
   - `GameHub` (SignalR, bottom)
   - `ShipPlacement`, `JoinGameResult` (records, near GameHub)

9. **Enumerations:**
   - Group all enums in a separate area (right side or bottom)
   - `CellStatus`, `Orientation`, `ShotResult`, `Phase`, `PlayerKind`, `DrawState`, `ShootingMode`, `ShipKind`, `ShipSkin`, `ShipType`

---

## 14. Notes and Annotations

### 14.1 Pattern Annotations
- Label Observer pattern relationships with `<<observes>>`
- Label Command pattern with `<<command>>`, `<<invoker>>`, `<<receiver>>`
- Label Builder pattern with `<<director>>`, `<<builder>>`, `<<product>>`
- Label Strategy pattern with `<<strategy>>`
- Label Factory patterns with `<<AbstractFactory>>`, `<<FactoryMethod>>`
- Label Decorator pattern with `<<decorator>>`
- Label Adapter pattern with `<<adapter>>`, `<<target>>`, `<<adaptee>>`
- Label Singleton pattern with `<<singleton>>`

### 14.2 Key Notes
- **GameSession**: "Implements Singleton pattern. Notifies observers on state changes (Phase, Current, Winner, Draw)."
- **FleetDirector**: "Orchestrates fleet construction. Knows the sequence of ships to build."
- **RandomFleetBuilder**: "Uses Abstract Factory (IShipFactory) to create ships. Applies Decorator pattern for skins."
- **SkinnedShipDecorator**: "Camouflage skin: first hit is absorbed (no damage)."
- **GameService**: "Uses Strategy pattern for attack modes. Uses Adapter pattern for fleet placement."
- **GameLobbyService**: "Manages online multiplayer sessions. Creates GameSession instances and attaches observers."

---

## 15. Implementation Notes

### 15.1 Key Design Decisions
1. **Singleton GameSession**: Ensures only one game session exists at a time
2. **Observer Pattern**: Decouples game state changes from logging/UI updates
3. **Builder Pattern**: Separates ship placement logic from board construction
4. **Strategy Pattern**: Allows switching between attack modes without changing game logic
5. **Abstract Factory**: Supports Classic and Modern ship families
6. **Decorator Pattern**: Allows adding ship skins without modifying ship classes
7. **Adapter Pattern**: Bridges PlacementService (Builder) with GameService (IFleetPlacer interface)

### 15.2 Extension Points
- New ship types: Add to `ShipKind` enum and implement in factories
- New attack strategies: Implement `IAttackStrategy`
- New ship skins: Create new decorators extending `ShipDecorator`
- New observers: Implement `IObserver` and attach to `GameSession`
- New commands: Implement `ICommand` and use with `ChatInvoker`

---

## 16. Complete Relationship Matrix

| From Class | To Class/Interface | Relationship Type | Multiplicity | Notes |
|------------|-------------------|-------------------|--------------|-------|
| Subject | IObserver | Composition | 0..* | Maintains list of observers |
| GameSession | Subject | Inheritance | 1 | Extends Subject |
| GameSession | Player | Association | 2 | P1, P2, Current, Winner |
| GameStateObserver | IObserver | Realization | 1 | Implements interface |
| GameStateObserver | GameSession | Dependency | 1 | Observes GameSession |
| TurnChangeObserver | IObserver | Realization | 1 | Implements interface |
| TurnChangeObserver | GameSession | Dependency | 1 | Observes GameSession |
| GameEndObserver | IObserver | Realization | 1 | Implements interface |
| GameEndObserver | GameSession | Dependency | 1 | Observes GameSession |
| ChatInvoker | ICommand | Association | 0..1 | Holds current command |
| SendMessageCommand | ICommand | Realization | 1 | Implements interface |
| SendMessageCommand | ChatService | Dependency | 1 | Calls receiver methods |
| ClearChatCommand | ICommand | Realization | 1 | Implements interface |
| ClearChatCommand | ChatService | Dependency | 1 | Calls receiver methods |
| ChatService | ChatMessage | Aggregation | 0..* | Contains messages |
| FleetDirector | IBoardBuilder | Association | 0..1 | Uses builder |
| RandomFleetBuilder | IBoardBuilder | Realization | 1 | Implements interface |
| RandomFleetBuilder | Board | Association | 1 | Works with board |
| RandomFleetBuilder | IShipFactory | Dependency | 1 | Creates ships |
| ManualFleetBuilder | IBoardBuilder | Realization | 1 | Implements interface |
| ManualFleetBuilder | Board | Association | 1 | Works with board |
| ManualFleetBuilder | IShipFactory | Dependency | 1 | Creates ships |
| Board | Cell | Composition | Size×Size | Contains cells |
| Board | IShip | Aggregation | 0..* | Contains ships |
| Board | CellFactory | Association | 1 | Creates cells |
| StandardCellFactory | CellFactory | Inheritance | 1 | Extends factory |
| GameService | GameSession | Association | 0..1 | Manages session |
| GameService | IFleetPlacer | Dependency | 1 | Uses adapter |
| GameService | IAttackStrategy | Dependency | 1 | Uses strategy |
| GameLobbyService | OnlineGameSession | Composition | 0..* | Manages sessions |
| GameLobbyService | PlacementService | Dependency | 1 | Uses service |
| GameLobbyService | IAttackStrategy | Dependency | 1 | Uses strategy |
| OnlineGameSession | GameSession | Association | 1 | Contains session |
| Player | Board | Composition | 1 | Owns board |
| HumanPlayer | Player | Inheritance | 1 | Extends Player |
| AiPlayer | Player | Inheritance | 1 | Extends Player |
| IAttackStrategy | AttackResult | Dependency | 1 | Returns result |
| SingleShotStrategy | IAttackStrategy | Realization | 1 | Implements interface |
| Salvo3x3Strategy | IAttackStrategy | Realization | 1 | Implements interface |
| ClassicShipFactory | IShipFactory | Realization | 1 | Implements interface |
| ModernShipFactory | IShipFactory | Realization | 1 | Implements interface |
| ShipBase | IShip | Realization | 1 | Implements interface |
| Battleship | ShipBase | Inheritance | 1 | Extends ShipBase |
| Submarine | ShipBase | Inheritance | 1 | Extends ShipBase |
| Destroyer | ShipBase | Inheritance | 1 | Extends ShipBase |
| Cruiser | ShipBase | Inheritance | 1 | Extends ShipBase |
| ModernBattleship | ShipBase | Inheritance | 1 | Extends ShipBase |
| ModernSubmarine | ShipBase | Inheritance | 1 | Extends ShipBase |
| ModernDestroyer | ShipBase | Inheritance | 1 | Extends ShipBase |
| ModernCruiser | ShipBase | Inheritance | 1 | Extends ShipBase |
| ShipDecorator | IShip | Realization | 1 | Implements interface |
| ShipDecorator | IShip | Aggregation | 1 | Wraps ship |
| SkinnedShipDecorator | ShipDecorator | Inheritance | 1 | Extends decorator |
| RandomFleetPlacerAdapter | IFleetPlacer | Realization | 1 | Implements interface |
| RandomFleetPlacerAdapter | PlacementService | Composition | 1 | Contains service |
| PlacementService | FleetDirector | Association | 1 | Uses director |
| Cell | IShip | Association | 0..1 | May contain ship |
| GameHub | GameLobbyService | Dependency | 1 | Uses service |

---

## 17. Usage Scenarios

### 17.1 Creating a Local Game
1. `GameService.NewLocalSession()` creates `HumanPlayer` and `AiPlayer`
2. `GameSession.GetInstance()` creates/returns Singleton instance
3. Observers are attached to `GameSession`
4. `GameService.RandomizeFor()` uses `IFleetPlacer` (Adapter)
5. Adapter calls `PlacementService.RandomizeFleet()`
6. `PlacementService` uses `FleetDirector` with `RandomFleetBuilder`
7. `RandomFleetBuilder` uses `IShipFactory` to create ships
8. Ships are placed on `Board`

### 17.2 Sending a Chat Message
1. Client creates `SendMessageCommand` with `ChatService`, sender, text
2. Client passes command to `ChatInvoker`
3. `ChatInvoker.RunCommand()` calls `command.Execute()`
4. `SendMessageCommand.Execute()` creates `ChatMessage` and calls `ChatService.SendMessage()`
5. `ChatService` adds message and raises `Updated` event
6. UI components subscribed to event update display

### 17.3 Firing a Shot
1. `GameService.FireAt()` called with position
2. `GameService` uses current `IAttackStrategy` (Strategy pattern)
3. Strategy calls `GameSession.Fire()`
4. `GameSession` calls `Opponent.Board.FireAt()`
5. `Board` updates `Cell` status and ship hit count
6. If ship sunk or game won, `GameSession` updates state
7. State changes trigger `Notify()` in `GameSession`
8. All observers receive `Update()` calls

### 17.4 Applying Ship Skin
1. `RandomFleetBuilder` creates ship using `IShipFactory`
2. If skin is not Default, wraps ship with `SkinnedShipDecorator`
3. Decorator adds camouflage shield behavior
4. When ship is hit, decorator's `RegisterHit()` is called first
5. First hit on camouflaged ship is absorbed (returns false)
6. Subsequent hits are passed to wrapped ship

---

This comprehensive UML class diagram specification should serve as a complete reference for implementing the BattleShips system architecture in any UML modeling tool.

