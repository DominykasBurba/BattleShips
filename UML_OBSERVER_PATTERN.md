# UML Class Diagram - Observer Pattern Implementation

## Overview
This document describes the UML class diagram for the Observer pattern implementation in the BattleShips project, following the lecturer's diagram structure.

---

## Classes and Interfaces

### 1. **Subject** (Abstract Class)
**Stereotype**: `<<abstract>>` or `<<Subject>>`  
**Location**: Top-left of diagram

**Attributes:**
- `-observers: List<IObserver>` (private, composition relationship with IObserver)

**Methods:**
- `+Attach(observer: IObserver) : void`
- `+Detach(observer: IObserver) : void`
- `+Notify() : void` (protected)

**Notes:**
- Contains a note box connected with dashed line showing:
  ```
  for all o in observers {
      o->Update();
  }
  ```

**Relationships:**
- Has composition/aggregation relationship (black diamond on Subject side) with `IObserver`
- Has inheritance relationship (empty triangle pointing to Subject) with `GameSession`

---

### 2. **IObserver** (Interface)
**Stereotype**: `<<interface>>` or `<<abstract>>`  
**Location**: Top-right of diagram  
**Style**: Italicized name or interface notation

**Methods:**
- `+Update() : void` (abstract/public)

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
- `+GetState() : Phase` (implicit - via Phase property)
- `+SetState(phase: Phase)` (implicit - via Phase property setter)
- `+TryStart() : bool`
- `+Fire(pos: Position) : ShotResult`
- `+EndTurn() : void`
- `+Surrender(who: Player) : void`
- `+ProposeDraw(who: Player) : void`
- `+AcceptDraw(who: Player) : void`

**Notes:**
- Contains a note box connected with dashed line showing:
  ```
  When state changes:
  Notify() is called automatically
  ```

**Relationships:**
- Extends `Subject` (solid line with empty triangle arrow)
- Has association relationship (solid line with arrow) with `GameStateObserver`
- Has association relationship (solid line with arrow) with `TurnChangeObserver`
- Has association relationship (solid line with arrow) with `GameEndObserver`

---

### 4. **GameStateObserver** (ConcreteObserver)
**Stereotype**: `<<ConcreteObserver>>`  
**Location**: Bottom-right, inheriting from IObserver

**Attributes:**
- `-subject: GameSession` (private, reference to the Subject it observes)
- `-observerState: Phase` (private, stores the state retrieved from Subject)

**Methods:**
- `+Update() : void` (implements IObserver)
- `+GetState() : Phase`

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
- `+Update() : void` (implements IObserver)
- `+GetState() : Player?`

**Notes:**
- Contains a note box connected with dashed line showing:
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
- `+Update() : void` (implements IObserver)
- `+GetState() : Player?`

**Notes:**
- Contains a note box connected with dashed line showing:
  ```
  observerState = subject->Winner
  ```

**Relationships:**
- Implements `IObserver` (dashed line with empty triangle arrow)
- Has association relationship (solid line with arrow) with `GameSession`

---

## Relationships (Connections)

### 1. **Subject → IObserver** (Composition/Aggregation)
- **Type**: Composition (solid line with filled diamond on Subject side)
- **Label**: `-observers` (on Subject side)
- **Multiplicity**: 0..* (Subject can have zero or more observers)
- **Direction**: Subject → IObserver
- **Stereotype**: `manages`

### 2. **IObserver → GameStateObserver** (Realization/Implementation)
- **Type**: Realization (dashed line with hollow triangle arrow)
- **Direction**: GameStateObserver → IObserver
- **Label**: `implements` or `<<implements>>`

### 3. **IObserver → TurnChangeObserver** (Realization/Implementation)
- **Type**: Realization (dashed line with hollow triangle arrow)
- **Direction**: TurnChangeObserver → IObserver
- **Label**: `implements` or `<<implements>>`

### 4. **IObserver → GameEndObserver** (Realization/Implementation)
- **Type**: Realization (dashed line with hollow triangle arrow)
- **Direction**: GameEndObserver → IObserver
- **Label**: `implements` or `<<implements>>`

### 5. **Subject → GameSession** (Inheritance)
- **Type**: Inheritance (solid line with hollow triangle arrow)
- **Direction**: GameSession → Subject
- **Label**: `extends` or `<<inherits>>`

### 6. **GameSession → GameStateObserver** (Association)
- **Type**: Association (solid line with arrow)
- **Direction**: GameSession → GameStateObserver
- **Label**: `notifies` or `<<notifies>>`
- **Note**: GameSession notifies observers when state changes

### 7. **GameStateObserver → GameSession** (Association)
- **Type**: Association (solid line with arrow)
- **Direction**: GameStateObserver → GameSession
- **Label**: `-subject` (on observer side)
- **Multiplicity**: 1 (observer has one subject)
- **Stereotype**: `observes`

### 8. **TurnChangeObserver → GameSession** (Association)
- **Type**: Association (solid line with arrow)
- **Direction**: TurnChangeObserver → GameSession
- **Label**: `-subject` (on observer side)
- **Multiplicity**: 1

### 9. **GameEndObserver → GameSession** (Association)
- **Type**: Association (solid line with arrow)
- **Direction**: GameEndObserver → GameSession
- **Label**: `-subject` (on observer side)
- **Multiplicity**: 1

---

## Diagram Layout (Suggested)

```
┌─────────────────────┐         ┌─────────────────────┐
│  Subject            │         │  IObserver          │
│  <<abstract>>       │         │  <<interface>>     │
│  -observers         │◄──────┐ │  +Update()          │
│  +Attach()          │       │ │                     │
│  +Detach()          │       │ │                     │
│  +Notify()          │       │ │                     │
└─────────────────────┘       │ └─────────────────────┘
         ▲                    │         ▲
         │                    │         │
         │ Inheritance        │         │ Realization
         │                    │         │
         │                    │         │
    ┌────┴─────────────┐      │    ┌────┴──────────────┐
    │  GameSession     │      │    │ GameStateObserver │
    │ <<ConcreteSubject│      │    │ -subject          │
    │  -phase          │──────┼───►│ -observerState    │
    │  -current        │      │    │ +Update()         │
    │  -winner         │      │    │ +GetState()       │
    │  +Phase          │      │    └───────────────────┘
    │  +Current        │      │
    │  +GetState()     │      │    ┌──────────────────────┐
    │  +SetState()     │      │    │ TurnChangeObserver  │
    └──────────────────┘      │    │ -subject            │
                              │    │ -observerState      │
                              │    │ +Update()           │
                              │    │ +GetState()         │
                              │    └─────────────────────┘
                              │
                              │    ┌──────────────────────┐
                              │    │ GameEndObserver      │
                              └───►│ -subject            │
                                   │ -observerState      │
                                   │ +Update()           │
                                   │ +GetState()         │
                                   └─────────────────────┘
```

---

## Detailed Relationship Specifications

### Composition Details

**Subject → IObserver**
- **Role on Subject**: `observers`
- **Visibility**: Private (`-`)
- **Navigability**: Subject → IObserver (one-way)
- **Cardinality**: 0..* (Subject can have zero or more observers)
- **Aggregation**: Composition (Subject owns observers list)

### Association Details

**GameStateObserver → GameSession**
- **Role on Observer**: `subject`
- **Visibility**: Private (`-`)
- **Navigability**: Observer → Subject (one-way)
- **Cardinality**: 1 (observer has one subject)
- **Purpose**: Observer holds reference to subject to call `GetState()`

---

## Notes and Comments

### Note 1: Notify() Method Implementation
**Connected to**: Subject's `Notify()` method  
**Content**:
```
for all o in observers {
    o->Update();
}
```

### Note 2: Update() Method in ConcreteObserver
**Connected to**: ConcreteObserver's `Update()` method  
**Content**:
```
observerState = subject->Phase
(or subject->Current, subject->Winner, etc.)
```

### Note 3: State Change Flow
**Connected to**: GameSession  
**Content**:
```
When Phase/Current/Winner/Draw changes:
1. Setter detects change
2. Calls Notify()
3. Notify() calls Update() on all observers
4. Observers retrieve state via subject->GetState()
```

---

## Stereotypes and Constraints

### Stereotypes
- `<<abstract>>` on Subject
- `<<interface>>` on IObserver
- `<<ConcreteSubject>>` on GameSession
- `<<ConcreteObserver>>` on GameStateObserver, TurnChangeObserver, GameEndObserver

### Constraints
- `{readonly}` on Subject's observers list (managed internally)
- `{observable}` on GameSession state properties

---

## Implementation Notes for UML Tools

### For Enterprise Architect:
1. Create abstract Subject class with `<<abstract>>` stereotype
2. Create IObserver interface with `<<interface>>` stereotype
3. Use "Generalization" relationship for GameSession → Subject
4. Use "Realization" relationship for observers → IObserver
5. Use "Composition" for Subject → IObserver
6. Use "Association" for observers → GameSession
7. Add note boxes with dashed line connections

### For Visual Paradigm:
1. Interface: Use `<<interface>>` stereotype
2. Abstract class: Use `<<abstract>>` stereotype
3. Composition: Use filled diamond on Subject side
4. Realization: Use dashed line with arrow
5. Add constraints: `{readonly}`, `{observable}`

### For Draw.io / Lucidchart:
1. Use interface shape (circle with "I") or rectangle with `<<interface>>`
2. Use abstract class shape with `<<abstract>>`
3. Use filled diamond for composition
4. Use dashed arrows for realizations
5. Add note boxes as text boxes with dashed borders

---

## Validation Checklist

✅ Subject has composition with IObserver (`-observers: List<IObserver>`)  
✅ Subject has `Attach()`, `Detach()`, `Notify()` methods  
✅ IObserver has `Update()` method  
✅ GameSession extends Subject  
✅ All concrete observers implement IObserver  
✅ All observers have reference to GameSession (`-subject`)  
✅ All observers have `observerState` attribute  
✅ Notify() method calls `Update()` on all observers (shown in note)  
✅ Update() method retrieves state from subject (shown in note)  
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

### Differences (Adaptations for BattleShips):
- Multiple concrete observers (GameStateObserver, TurnChangeObserver, GameEndObserver) instead of one
- GameSession has multiple state properties (Phase, Current, Winner, Draw) instead of single `state`
- Observers retrieve different state properties (Phase, Current, Winner) based on what they observe

