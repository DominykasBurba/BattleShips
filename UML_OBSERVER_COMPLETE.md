# UML Class Diagram - Observer Pattern (Complete System)

## Overview
This document describes the complete UML class diagram showing the Observer pattern implementation and how it integrates with GameService, GameLobbyService, and GameSession.

---

## Core Observer Pattern (Following Lecturer's Diagram)

### 1. **Subject** (Abstract Class)
**Stereotype**: `<<abstract>>`  
**Location**: Top-left of diagram

**Attributes:**
- `-observers: List<IObserver>` (private, composition relationship with IObserver)

**Methods:**
- `+Attach(observer: IObserver): void`
- `+Detach(observer: IObserver): void`
- `+Notify(): void` (protected)

**Notes:**
- Contains a note box connected with dashed line showing:
  ```
  for all o in observers {
      o->Update();
  }
  ```

**Relationships:**
- Has composition relationship (black diamond on Subject side) with `IObserver`
- Has inheritance relationship (empty triangle pointing to Subject) with `GameSession`

---

### 2. **IObserver** (Interface)
**Stereotype**: `<<interface>>`  
**Location**: Top-right of diagram  
**Style**: Italicized name or interface notation

**Methods:**
- `+Update(): void` (abstract/public)

**Relationships:**
- Has inheritance/realization relationship (dashed line with empty triangle) with concrete observers

---

### 3. **GameSession** (ConcreteSubject)
**Stereotype**: `<<ConcreteSubject>>`  
**Location**: Bottom-left, inheriting from Subject

**Attributes:**
- `-phase: Phase` (private, the state observers are interested in)
- `-current: Player` (private)
- `-winner: Player?` (private)
- `-draw: DrawState` (private)
- `+P1: Player` (public readonly)
- `+P2: Player` (public readonly)

**Methods:**
- `+Phase: Phase` (property getter that triggers notify on set)
- `+Current: Player` (property getter that triggers notify on set)
- `+Winner: Player?` (property getter that triggers notify on set)
- `+Draw: DrawState` (property getter that triggers notify on set)
- `+TryStart(): bool`
- `+Fire(pos: Position): ShotResult`
- `+EndTurn(): void`
- `+Surrender(who: Player): void`
- `+ProposeDraw(who: Player): void`
- `+AcceptDraw(who: Player): void`

**Notes:**
- Contains a note box showing:
  ```
  When state changes:
  Setter calls Notify()
  ```

**Relationships:**
- Extends `Subject` (solid line with empty triangle arrow)
- Used by `GameService` (association)
- Used by `GameLobbyService` (via `OnlineGameSession`, association)

---

### 4. **GameStateObserver** (ConcreteObserver)
**Stereotype**: `<<ConcreteObserver>>`  
**Location**: Bottom-right, inheriting from IObserver

**Attributes:**
- `-subject: GameSession` (private, reference to the Subject it observes)
- `-observerState: Phase` (private, stores the state retrieved from Subject)

**Methods:**
- `+Update(): void` (implements IObserver)
- `+GetState(): Phase`

**Notes:**
- Contains a note box connected with dashed line showing:
  ```
  observerState = subject->Phase
  ```

**Relationships:**
- Implements `IObserver` (dashed line with empty triangle arrow)
- Has association relationship (solid line with arrow) with `GameSession`

---

### 5. **TurnChangeObserver** (ConcreteObserver)
**Stereotype**: `<<ConcreteObserver>>`  
**Location**: Bottom-right, next to GameStateObserver

**Attributes:**
- `-subject: GameSession` (private, reference to the Subject it observes)
- `-observerState: Player?` (private, stores the current player from Subject)

**Methods:**
- `+Update(): void` (implements IObserver)
- `+GetState(): Player?`

**Notes:**
- Contains a note box showing:
  ```
  observerState = subject->Current
  ```

**Relationships:**
- Implements `IObserver` (dashed line with empty triangle arrow)
- Has association relationship (solid line with arrow) with `GameSession`

---

### 6. **GameEndObserver** (ConcreteObserver)
**Stereotype**: `<<ConcreteObserver>>`  
**Location**: Bottom-right, next to other observers

**Attributes:**
- `-subject: GameSession` (private, reference to the Subject it observes)
- `-observerState: Player?` (private, stores the winner from Subject)

**Methods:**
- `+Update(): void` (implements IObserver)
- `+GetState(): Player?`

**Notes:**
- Contains a note box showing:
  ```
  observerState = subject->Winner
  ```

**Relationships:**
- Implements `IObserver` (dashed line with empty triangle arrow)
- Has association relationship (solid line with arrow) with `GameSession`

---

## Service Layer Integration

### 7. **GameService** (Service)
**Location**: Middle-left, separate from Observer pattern core

**Attributes:**
- `-Session: GameSession?` (private, association with GameSession)
- `-placement: PlacementService` (private)
- `-attackStrategy: IAttackStrategy` (private)
- `+KeepTurnOnHit: bool` (public)
- `+ShootingMode: ShootingMode` (public)

**Methods:**
- `+NewLocalSession(size: int, enemyIsAi: bool): void`
- `+Start(): bool`
- `+FireAt(pos: Position): ShotResult`
- `+ResetShips(): void`
- `+ClearSession(): void`

**Notes:**
- Contains a note box showing:
  ```
  Creates GameSession
  Attaches observers:
  - GameStateObserver
  - TurnChangeObserver
  - GameEndObserver
  ```

**Relationships:**
- Has association relationship (solid line with arrow) with `GameSession`
- Creates and manages `GameSession` instances

---

### 8. **GameLobbyService** (Service)
**Location**: Middle-left, below GameService

**Attributes:**
- `-games: ConcurrentDictionary<string, OnlineGameSession>` (private)
- `-connectionToGame: ConcurrentDictionary<string, string>` (private)
- `-placementService: PlacementService` (private)

**Methods:**
- `+CreateGame(connectionId: string, boardSize: int, shootingMode: ShootingMode): string`
- `+JoinGame(gameId: string, connectionId: string): (bool, int, ShootingMode)`
- `+PlaceShips(gameId: string, connectionId: string, ships: List<ShipPlacement>): bool`
- `+FireShot(gameId: string, connectionId: string, position: Position): ...`

**Relationships:**
- Has composition relationship (filled diamond) with `OnlineGameSession`
- Manages multiple `OnlineGameSession` instances

---

### 9. **OnlineGameSession** (Wrapper Class)
**Location**: Middle-center, between GameLobbyService and GameSession

**Attributes:**
- `+GameId: string` (public readonly)
- `+Player1ConnectionId: string` (public readonly)
- `+Player2ConnectionId: string?` (public)
- `+GameSession: GameSession` (public, association with GameSession)
- `+Player1Ready: bool` (public)
- `+Player2Ready: bool` (public)
- `+ShootingMode: ShootingMode` (public)

**Methods:**
- `+GetPlayer(connectionId: string): Player?`
- `+GetConnectionId(player: Player): string?`

**Notes:**
- Contains a note box showing:
  ```
  Creates GameSession
  Attaches observers:
  - GameStateObserver
  - TurnChangeObserver
  - GameEndObserver
  ```

**Relationships:**
- Has composition relationship (filled diamond) with `GameLobbyService`
- Has association relationship (solid line with arrow) with `GameSession`
- Creates and manages `GameSession` instance

---

## Complete Diagram Layout

```
┌─────────────────────┐                    ┌─────────────────────┐
│  Subject            │                    │  IObserver          │
│  <<abstract>>       │                    │  <<interface>>      │
│  -observers         │◄───────┐           │  +Update()          │
│  +Attach()          │       │           │                     │
│  +Detach()          │       │           │                     │
│  +Notify()          │       │           │                     │
└─────────────────────┘       │           └─────────────────────┘
         ▲                    │                    ▲
         │                    │                    │
         │ Inheritance        │                    │ Realization
         │                    │                    │
         │                    │                    │
    ┌────┴─────────────┐      │           ┌────────┴────────────┐
    │  GameSession     │      │           │ GameStateObserver   │
    │ <<ConcreteSubject│      │           │ -subject            │
    │  -phase          │──────┼──────────►│ -observerState      │
    │  -current        │      │           │ +Update()           │
    │  -winner         │      │           │ +GetState()         │
    │  -draw           │      │           └─────────────────────┘
    │  +Phase          │      │
    │  +Current        │      │           ┌─────────────────────┐
    │  +Winner         │      │           │ TurnChangeObserver  │
    │  +Draw           │      │──────────►│ -subject            │
    │  +TryStart()     │      │           │ -observerState      │
    │  +Fire()         │      │           │ +Update()           │
    │  +EndTurn()      │      │           │ +GetState()         │
    └──────────────────┘      │           └─────────────────────┘
         ▲                    │
         │                    │           ┌─────────────────────┐
         │ Uses               │──────────►│ GameEndObserver     │
         │                    │           │ -subject            │
    ┌────┴─────────────┐      │           │ -observerState      │
    │  GameService     │      │           │ +Update()           │
    │  -Session        │      │           │ +GetState()         │
    │  +NewLocalSession│      │           └─────────────────────┘
    │  +Start()        │      │
    │  +FireAt()       │      │
    └──────────────────┘      │
         │                    │
         │ Uses               │
         │                    │
    ┌────┴─────────────┐      │
    │ GameLobbyService  │      │
    │  -games           │      │
    │  +CreateGame()    │      │
    │  +JoinGame()      │      │
    └───────────────────┘      │
         │                    │
         │ Contains           │
         │                    │
    ┌────┴─────────────┐      │
    │OnlineGameSession │      │
    │  +GameSession    │──────┘
    │  +GameId         │
    └──────────────────┘
```

---

## Detailed Relationships

### 1. **Subject → IObserver** (Composition)
- **Type**: Composition (solid line with filled diamond on Subject side)
- **Label**: `-observers` (on Subject side)
- **Multiplicity**: 0..* (Subject can have zero or more observers)
- **Direction**: Subject → IObserver

### 2. **Subject → GameSession** (Inheritance)
- **Type**: Inheritance (solid line with empty triangle arrow)
- **Direction**: GameSession → Subject
- **Label**: `extends`

### 3. **IObserver → Concrete Observers** (Realization)
- **Type**: Realization (dashed line with empty triangle arrow)
- **Direction**: ConcreteObserver → IObserver
- **Label**: `implements`

### 4. **Concrete Observers → GameSession** (Association)
- **Type**: Association (solid line with arrow)
- **Direction**: Observer → GameSession
- **Label**: `-subject` (on observer side)
- **Multiplicity**: 1 (observer has one subject)

### 5. **GameService → GameSession** (Association)
- **Type**: Association (solid line with arrow)
- **Direction**: GameService → GameSession
- **Label**: `-Session` (on GameService side)
- **Multiplicity**: 0..1 (optional, can be null)
- **Stereotype**: `<<creates>>`, `<<uses>>`

### 6. **GameLobbyService → OnlineGameSession** (Composition)
- **Type**: Composition (solid line with filled diamond on GameLobbyService side)
- **Direction**: GameLobbyService → OnlineGameSession
- **Label**: `-games` (on GameLobbyService side)
- **Multiplicity**: 0..* (can have many online sessions)

### 7. **OnlineGameSession → GameSession** (Association)
- **Type**: Association (solid line with arrow)
- **Direction**: OnlineGameSession → GameSession
- **Label**: `+GameSession` (on OnlineGameSession side)
- **Multiplicity**: 1 (one GameSession per OnlineGameSession)
- **Stereotype**: `<<creates>>`, `<<contains>>`

---

## Observer Attachment Flow

### Flow 1: Local Game (via GameService)
```
GameService.NewLocalSession()
    ↓
Creates: GameSession(p1, p2)
    ↓
Creates: GameStateObserver(session)
    ├─→ Observer constructor calls: session.Attach(this)
    └─→ Observer stores: _subject = session
    ↓
Creates: TurnChangeObserver(session)
    ├─→ Observer constructor calls: session.Attach(this)
    └─→ Observer stores: _subject = session
    ↓
Creates: GameEndObserver(session)
    ├─→ Observer constructor calls: session.Attach(this)
    └─→ Observer stores: _subject = session
```

### Flow 2: Online Game (via GameLobbyService)
```
GameLobbyService.CreateGame()
    ↓
Creates: OnlineGameSession(...)
    ↓
OnlineGameSession constructor:
    Creates: GameSession(p1, p2)
    ↓
    Creates: GameStateObserver(session)
    ├─→ Observer constructor calls: session.Attach(this)
    └─→ Observer stores: _subject = session
    ↓
    Creates: TurnChangeObserver(session)
    ├─→ Observer constructor calls: session.Attach(this)
    └─→ Observer stores: _subject = session
    ↓
    Creates: GameEndObserver(session)
    ├─→ Observer constructor calls: session.Attach(this)
    └─→ Observer stores: _subject = session
```

---

## State Change Notification Flow

```
GameSession.Phase = Phase.Finished
    ↓
Phase setter executes
    ↓
Checks: _phase != value? YES
    ↓
Sets: _phase = Phase.Finished
    ↓
Calls: Notify()
    ↓
Notify() loops through _observers:
    ├─→ GameStateObserver.Update()
    │   ├─→ Gets: _observerState = _subject.Phase
    │   └─→ Logs: "Game state changed to: Finished"
    │
    ├─→ TurnChangeObserver.Update()
    │   ├─→ Gets: _observerState = _subject.Current
    │   └─→ Logs: "Turn changed to: ..."
    │
    └─→ GameEndObserver.Update()
        ├─→ Gets: _observerState = _subject.Winner
        └─→ Logs: "Game finished! Winner: ..."
```

---

## Color Coding (Optional)
- **Subject, IObserver**: Yellow (abstract/interface)
- **GameSession**: Light blue (ConcreteSubject)
- **Concrete Observers**: Light green
- **GameService, GameLobbyService**: Light orange (services)
- **OnlineGameSession**: Light purple (wrapper)

---

## Stereotypes and Constraints

### Stereotypes
- `<<abstract>>` on Subject
- `<<interface>>` on IObserver
- `<<ConcreteSubject>>` on GameSession
- `<<ConcreteObserver>>` on all observers
- `<<Service>>` on GameService and GameLobbyService

### Constraints
- `{readonly}` on Subject's observers list (managed internally)
- `{observable}` on GameSession state properties
- `{singleton}` on GameService Session (only one active session)

---

## Implementation Notes for UML Tools

### For Enterprise Architect:
1. Create abstract Subject class with `<<abstract>>` stereotype
2. Create IObserver interface with `<<interface>>` stereotype
3. Use "Generalization" for GameSession → Subject
4. Use "Realization" for observers → IObserver
5. Use "Composition" for Subject → IObserver (with filled diamond)
6. Use "Association" for observers → GameSession
7. Use "Association" for GameService → GameSession
8. Use "Composition" for GameLobbyService → OnlineGameSession
9. Use "Association" for OnlineGameSession → GameSession
10. Add note boxes with dashed line connections

### For Visual Paradigm:
1. Interface: Use `<<interface>>` stereotype
2. Abstract class: Use `<<abstract>>` stereotype
3. Composition: Use filled diamond on Subject and GameLobbyService sides
4. Realization: Use dashed line with arrow
5. Add constraints: `{readonly}`, `{observable}`, `{singleton}`

### For Draw.io / Lucidchart:
1. Use interface shape (circle with "I") or rectangle with `<<interface>>`
2. Use abstract class shape with `<<abstract>>`
3. Use filled diamond for composition
4. Use dashed arrows for realizations
5. Group related classes (Observer pattern core, Services, Wrapper)
6. Add note boxes as text boxes with dashed borders

---

## Validation Checklist

✅ Subject has composition with IObserver (`-observers: List<IObserver>`)  
✅ Subject has `Attach()`, `Detach()`, `Notify()` methods  
✅ IObserver has `Update()` method  
✅ GameSession extends Subject  
✅ All concrete observers implement IObserver  
✅ All observers have reference to GameSession (`-subject`)  
✅ All observers have `observerState` attribute  
✅ GameService has association with GameSession  
✅ GameLobbyService has composition with OnlineGameSession  
✅ OnlineGameSession has association with GameSession  
✅ Notify() method calls `Update()` on all observers (shown in note)  
✅ Update() method retrieves state from subject (shown in note)  
✅ Observer attachment shown in GameService and OnlineGameSession notes  
✅ All method signatures match actual code  
✅ Visibility modifiers are correct (- for private, + for public)  

---

## Pattern Alignment with Lecturer's Diagram

### Matches Lecturer's Structure:
- ✅ **Subject** (abstract) with `-observers` list and `+notify()` method
- ✅ **Observer** (interface) with `+update()` method
- ✅ **ConcreteSubject** (GameSession) extends Subject, has state (`-phase`, `-current`, etc.)
- ✅ **ConcreteObserver** (multiple) implements Observer, has `-subject` reference
- ✅ **Note on notify()**: Shows `for all o in observers { o->update(); }`
- ✅ **Note on update()**: Shows `observerState = subject->getState()`

### Additional Integration:
- ✅ Shows how GameService creates and uses GameSession
- ✅ Shows how GameLobbyService creates and uses GameSession via OnlineGameSession
- ✅ Shows observer attachment points in both services
- ✅ Shows complete system architecture

