## UML papildymas: nauji šablonai (Iterator, Memento, Chain of Responsibility)

Šitas dokumentas paaiškina, **kokius naujus klases / interfeisus** reikia pridėti ir **kaip atnaujinti esamą UML**, kad jis atitiktų reikalavimus:

- **Iterator (b)** – lentos / laivų perėjimui
- **Memento (k)** – žaidimo būsenos išsaugojimui / atstatymui
- **Chain of Responsibility (g)** – validacijų grandinei

Toliau – kiekvienam šablonui: nauji tipai, ryšiai su esamomis klasėmis ir trumpas veikimo aprašymas.

---

## 1. Iterator (b) – Board / Ship perėjimas

### Nauji tipai UML diagramos

- **`IIterator<T>`** (interface, bendras iteratorius)
  - `+ MoveNext(): bool`
  - `+ Current: T`
  - `+ Reset(): void`

- **`IBoardEnumerable`** (interface, „aggregate“ lentelei)
  - `+ GetCellIterator(): IIterator<Cell>`
  - `+ GetShipIterator(): IIterator<IShip>`

- **`BoardCellIterator`** (class)
  - implements **`IIterator<Cell>`**
  - laukai:
    - `- _board: Board`
    - `- _row: int`
    - `- _col: int`

- **`BoardShipIterator`** (class)
  - implements **`IIterator<IShip>`**
  - laukai:
    - `- _ships: IReadOnlyList<IShip>`
    - `- _index: int`

### Ryšiai su esamomis klasėmis

- `Board`:
  - **implements** `IBoardEnumerable`
  - **pridedami metodai**:
    - `+ GetCellIterator(): IIterator<Cell>`
    - `+ GetShipIterator(): IIterator<IShip>`
  - **association** su `BoardCellIterator` ir `BoardShipIterator` (Board juos sukuria).

- `IShip`:
  - jau turi metodą `Cells()` → UML gali rodyti, kad `BoardShipIterator` naudoja `IShip` kolekciją (`Board.Ships`).

### Kaip nupiešti UML

- Pridėk naują paketą / regioną, pavadintą **«Iterator»**.
- Į jį įdėk:
  - `IIterator<T>` ir `IBoardEnumerable` (stereotipas `«interface»`).
  - `BoardCellIterator` ir `BoardShipIterator` (paprastos klasės).
- Nubrėžk:
  - **realization** rodykles (`BoardCellIterator` → `IIterator<Cell>`, `BoardShipIterator` → `IIterator<IShip>`, `Board` → `IBoardEnumerable`).
  - **association** nuo `Board` iki `BoardCellIterator` ir `BoardShipIterator` su multiplicitetais `1` (Board) – `*` (iterator instances).
- Šalia diagramos, prie „Iterator (b)“, gali parašyti, kad:
  - **Client** (pvz., `GameService` arba UI komponentas) naudoja `IBoardEnumerable` vietoje tiesioginio prieigos prie masyvų.

### Kaip tai veikia

- Klientas prašo iš `Board` iteratoriaus:
  - `var it = board.GetCellIterator(); while (it.MoveNext()) { var cell = it.Current; ... }`
- Taip paslepiama, kad `Board` saugo duomenis `Cell[,]` masyve ir leidžia ateityje pakeisti realizaciją nekeičiat kliento kodo.

---

## 2. Memento (k) – GameSession būsenos saugojimas / atstatymas

### Nauji tipai UML diagramos

- **`GameSessionMemento`** (class, `«Memento»`)
  - tik duomenys (jokių logikos metodų):
    - `+ P1BoardState: BoardStateDto`
    - `+ P2BoardState: BoardStateDto`
    - `+ CurrentPlayerId: string`
    - `+ Phase: Phase`
    - `+ DrawState: DrawState`
    - `+ ShotsPerTurn: int`
  - pagal poreikį gali turėti papildomus laukus (chat istorija ir pan.).

- **`IGameSessionOriginator`** (interface, neprivalomas, bet gražus UML’ui)
  - `+ CreateMemento(): GameSessionMemento`
  - `+ Restore(memento: GameSessionMemento): void`

- **`GameHistoryCaretaker`** (class, `«Caretaker»`)
  - laukai:
    - `- _history: Stack<GameSessionMemento>`
  - metodai:
    - `+ Save(GameSession session): void`
    - `+ Undo(GameSession session): void`

### Ryšiai su esamomis klasėmis

- `GameSession`:
  - **implements** `IGameSessionOriginator`
  - **nauji metodai**:
    - `+ CreateMemento(): GameSessionMemento`
    - `+ Restore(m: GameSessionMemento): void`
  - `Restore` viduje atstato:
    - `P1`, `P2` lentas (per kokį nors `BoardStateDto` → `Board` mapinimą),
    - `Phase`, `Current`, `Winner`, `Draw`, `ShotsPerTurn`.

- `GameHistoryCaretaker`:
  - turi **association** su `GameSession` (metodų parametruose).
  - gali būti naudojamas iš `GameService` ar UI, pvz. „Undo Move“ funkcijai.

### Kaip nupiešti UML

- Pridėk naują paketą / regioną **«Memento»**.
- Į jį įdėk:
  - `GameSessionMemento` (pažymėk `«Memento»`).
  - `IGameSessionOriginator` (interface).
  - `GameHistoryCaretaker` (pažymėk `«Caretaker»`).
- Nubrėžk:
  - **realization**: `GameSession` → `IGameSessionOriginator`.
  - **association**: `GameHistoryCaretaker` → `GameSession` (su rodykle nuo Caretaker į Originator).
  - **association**: `GameSessionMemento` – tik duomenų objektas, rodyklė iš `GameSession` ir iš `GameHistoryCaretaker`.

### Kaip tai veikia

- Prieš svarbų veiksmą (`Fire`, `Surrender`, t.t.), kodas kviečia:
  - `caretaker.Save(GameSession.GetInstance(...));`
- `Save` iškviečia `session.CreateMemento()` ir pasideda į `Stack`.
- Kai naudotojas paspaudžia „Undo“:
  - `caretaker.Undo(session)` pasiima paskutinį `GameSessionMemento` ir iškviečia `session.Restore(memento)`.
- Taip atstatoma žaidimo būsena (lentų išsidėstymas, einamas žaidėjas, fazė ir pan.).

---

## 3. Chain of Responsibility (g) – validacijų grandinė

Čia grandinė gali būti naudojama, pvz., **laivo statymo** ar **šūvio** validacijai.

### Nauji tipai UML diagramos

- **`ValidationContext`** (class)
  - bendri duomenys validacijai:
    - `+ Player: Player`
    - `+ Board: Board`
    - `+ Ship?: IShip` *(priklausomai nuo scenarijaus)*
    - `+ Position?: Position`
    - `+ Errors: List<string>`

- **`ValidationHandler`** (abstract class, `«Handler»`)
  - laukai:
    - `# _next: ValidationHandler?`
  - metodai:
    - `+ SetNext(next: ValidationHandler): ValidationHandler`
    - `+ Handle(context: ValidationContext): void` *(template method)*
    - `+ Validate(context: ValidationContext): bool` *(virtual/abstract, konkreti logika)*

- Konkrečios validacijos (concrete handlers):
  - **`BoundsValidationHandler`**
  - **`OverlapValidationHandler`**
  - **`AdjacencyValidationHandler`**
  - **`FleetCompositionValidationHandler`** (gali naudoti dabartinę `HasCompleteFleet` logiką iš `GameSession`).

### Ryšiai su esamomis klasėmis

- `ValidationHandler`:
  - **self-association** `next` laukas grandinei.
- Visi konkretūs `*ValidationHandler`:
  - **inherit** iš `ValidationHandler`.
- `Board` arba `PlacementService`:
  - turi **association** su pirma grandinės dalimi, pvz. lauką:
    - `- _placementValidator: ValidationHandler`
  - kviečia:
    - `var ctx = new ValidationContext { Board = board, Ship = ship }; _placementValidator.Handle(ctx);`

### Kaip nupiešti UML

- Pridėk paketą / regioną **«Chain of Responsibility»**.
- Įdėk:
  - `ValidationContext`
  - `ValidationHandler` (abstract class, `«Handler»`)
  - `BoundsValidationHandler`, `OverlapValidationHandler`, `AdjacencyValidationHandler`, `FleetCompositionValidationHandler`.
- Nubrėžk:
  - **inheritance**: visi konkretūs handleriai paveldi iš `ValidationHandler`.
  - **association** `ValidationHandler` → `ValidationHandler` su role name `next`.
  - **association** nuo `PlacementService` arba `Board` į pirmą handler’į (pvz. `BoundsValidationHandler`), pažymėk, kad likę handleriai sujungti per `next`.

### Kaip tai veikia

- Kodo lygmenyje (aukšto lygio idėja):
  - pradiniame nustatyme sukonstruojama grandinė:
    - `bounds → overlap → adjacency → fleetComposition`
  - kai reikia validuoti, sukuriamas `ValidationContext` ir kviečiamas `Handle` pirmajam nariui.
- Kiekvienas konkretus handleris:
  - patikrina savo taisykles,
  - jei viskas gerai – perduoda kontrolę `next`,
  - jei klaida – prideda pranešimą į `Errors` ir gali **nutraukti** grandinę (nebėra `next.Handle` kvietimo).

---

## Trumpas UML atnaujinimo „checklist“

1. **Iterator (b)**:
   - Pridėk `IIterator<T>`, `IBoardEnumerable`, `BoardCellIterator`, `BoardShipIterator`.
   - Pažymėk, kad `Board` implements `IBoardEnumerable`.
   - Parodyk realizacijos / asociacijų rodykles.

2. **Memento (k)**:
   - Pridėk `GameSessionMemento`, `IGameSessionOriginator`, `GameHistoryCaretaker`.
   - Pažymėk, kad `GameSession` implements `IGameSessionOriginator`.
   - Parodyk, kad `GameHistoryCaretaker` ir `GameSession` naudoja `GameSessionMemento`.

3. **Chain of Responsibility (g)**:
   - Pridėk `ValidationContext`, `ValidationHandler` ir 3–4 konkrečius handlerius.
   - Nubrėžk paveldėjimą ir `next` asociaciją.
   - Parodyk, kad `PlacementService` / `Board` naudoja šitą grandinę prieš laivo statymą ar kitą veiksmą.

Šito dokumento užtenka, kad UML diagramoje aiškiai būtų parodyta, **kur atsiranda nauji trys šablonai** ir **kaip jie susiję su esamomis domeno klasėmis**.

---

## ASCII UML diagramos (tokiu pačiu stiliumi kaip `ABSTRACT_FACTORY_UML.md`)

### Iterator (b) – Board / Ship traversal

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                               ITERATOR PATTERN                              │
└─────────────────────────────────────────────────────────────────────────────┘

                         ┌──────────────────────────┐
                         │   <<interface>>          │
                         │   IIterator<T>           │
                         ├──────────────────────────┤
                         │ + MoveNext(): bool       │
                         │ + Current: T             │
                         │ + Reset(): void          │
                         └──────────────────────────┘


                         ┌──────────────────────────┐
                         │   <<interface>>          │
                         │   IBoardEnumerable       │
                         ├──────────────────────────┤
                         │ + GetCellIterator():     │
                         │       IIterator<Cell>    │
                         │ + GetShipIterator():     │
                         │       IIterator<IShip>   │
                         └──────────────────────────┘
                                      ▲
                                      │ implements
                                      │
                         ┌────────────┴────────────┐
                         │                         │
                 ┌───────▼────────┐                │
                 │    Board        │                │
                 ├─────────────────┤                │
                 │ + Size: int     │                │
                 │ + Ships:        │                │
                 │   IReadOnlyList │                │
                 │   <IShip>       │                │
                 │ + this[r,c]:Cell│                │
                 │ + GetCellIter.. │                │
                 │ + GetShipIter.. │                │
                 └───────┬─────────┘
                         │ creates
       ┌─────────────────┴───────────────┐
       │                                 │
       │                                 │
┌──────▼────────────────┐      ┌────────▼────────────────┐
│  BoardCellIterator    │      │   BoardShipIterator     │
├───────────────────────┤      ├─────────────────────────┤
│ - _board: Board       │      │ - _ships:               │
│ - _row: int           │      │   IReadOnlyList<IShip>  │
│ - _col: int           │      │ - _index: int           │
├───────────────────────┤      ├─────────────────────────┤
│ + MoveNext(): bool    │      │ + MoveNext(): bool      │
│ + Current: Cell       │      │ + Current: IShip        │
│ + Reset(): void       │      │ + Reset(): void         │
└───────────▲───────────┘      └──────────▲──────────────┘
            │ implements                           │ implements
            │                                      │
            └───────────────┬──────────────────────┘
                            │
                  ┌─────────▼─────────┐
                  │  IIterator<Cell>  │
                  └───────────────────┘

                  (BoardShipIterator taip pat realizuoja IIterator<IShip>)
```

**Ryšiai (santrauka):**

- `BoardCellIterator` ir `BoardShipIterator` realizuoja `IIterator<T>`.
- `Board` realizuoja `IBoardEnumerable` ir kuria iteratorius metoduose `GetCellIterator()` ir `GetShipIterator()`.

---

### Memento (k) – GameSession state save / restore

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                                 MEMENTO PATTERN                             │
└─────────────────────────────────────────────────────────────────────────────┘

                  ┌──────────────────────────┐
                  │     GameSession          │   «Originator»
                  ├──────────────────────────┤
                  │ - P1: Player             │
                  │ - P2: Player             │
                  │ - Phase: Phase           │
                  │ - Current: Player        │
                  │ - Winner: Player?        │
                  │ - Draw: DrawState        │
                  │ - ShotsPerTurn: int      │
                  ├──────────────────────────┤
                  │ + CreateMemento():       │
                  │     GameSessionMemento   │
                  │ + Restore(m:             │
                  │     GameSessionMemento)  │
                  └───────────┬──────────────┘
                              │ creates / restores
                              │
                              ▼
                  ┌──────────────────────────┐
                  │  GameSessionMemento      │  «Memento»
                  ├──────────────────────────┤
                  │ + P1: PlayerStateDto     │
                  │ + P2: PlayerStateDto     │
                  │ + Phase: Phase           │
                  │ + CurrentPlayerName:     │
                  │       string             │
                  │ + WinnerName: string?    │
                  │ + Draw: DrawState        │
                  │ + ShotsPerTurn: int      │
                  └───────────┬──────────────┘
                              │ has
                ┌─────────────┴──────────────┐
                │                            │
        ┌───────▼────────────┐       (P2 toks pats)
        │  PlayerStateDto    │
        ├────────────────────┤
        │ + Name: string     │
        │ + Kind: PlayerKind │
        │ + BoardSize: int   │
        │ + Ships:           │
        │   List<ShipStateDto│
        └───────────┬────────┘
                    │ has 0..*
                    ▼
           ┌─────────────────────┐
           │   ShipStateDto      │
           ├─────────────────────┤
           │ + Kind: ShipKind    │
           │ + Start: Position   │
           │ + Orientation:      │
           │     Orientation     │
           │ + IsSunk: bool      │
           └─────────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                                CARETAKER                                    │
└─────────────────────────────────────────────────────────────────────────────┘

                  ┌──────────────────────────┐
                  │  GameHistoryCaretaker    │  «Caretaker»
                  ├──────────────────────────┤
                  │ - _history:              │
                  │    Stack<GameSessionM..> │
                  ├──────────────────────────┤
                  │ + Save(session:          │
                  │     GameSession)         │
                  │ + Undo(session:          │
                  │     GameSession)         │
                  └───────────┬──────────────┘
                              │ uses
               ┌──────────────┴──────────────┐
               │                             │
        ┌──────▼───────────┐        ┌───────▼────────────┐
        │  GameSession     │        │ GameSessionMemento │
        └──────────────────┘        └────────────────────┘
```

**Ryšiai (santrauka):**

- `GameSession` kuria ir atstato `GameSessionMemento` (Originator ↔ Memento).
- `GameHistoryCaretaker` laiko `Stack<GameSessionMemento>` ir kviečia `Save` / `Undo`.

---

### Chain of Responsibility (g) – Placement validation

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         CHAIN OF RESPONSIBILITY PATTERN                     │
└─────────────────────────────────────────────────────────────────────────────┘

                    ┌──────────────────────────┐
                    │  ValidationContext       │
                    ├──────────────────────────┤
                    │ + Board: Board           │
                    │ + Ship?: IShip           │
                    │ + Position?: Position    │
                    │ + Errors: List<string>   │
                    │ + IsValid: bool          │
                    └──────────────────────────┘


                    ┌──────────────────────────┐
                    │ <<abstract>>            │
                    │ ValidationHandler        │  «Handler»
                    ├──────────────────────────┤
                    │ - _next:                │
                    │    ValidationHandler?    │
                    ├──────────────────────────┤
                    │ + SetNext(next):        │
                    │     ValidationHandler    │
                    │ + Handle(ctx:           │
                    │     ValidationContext)   │
                    │ # Validate(ctx): bool    │
                    └───────────┬──────────────┘
                                │
             ┌──────────────────┼───────────────────────┐
             │                  │                       │
   ┌─────────▼────────┐ ┌───────▼────────────┐ ┌────────▼────────────┐
   │BoundsValidation.. │ │OverlapValidation.. │ │AdjacencyValidation..│
   ├───────────────────┤ ├───────────────────┤ ├─────────────────────┤
   │ tikrina ribas     │ │ tikrina persideng.│ │ tikrina gretimumą   │
   └───────────────────┘ └───────────────────┘ └─────────────────────┘


            grandinės ryšys (_next)

   BoundsValidationHandler   ──next──>   OverlapValidationHandler   ──next──>   AdjacencyValidationHandler


┌─────────────────────────────────────────────────────────────────────────────┐
│                        INTEGRATION SU PlacementService                      │
└─────────────────────────────────────────────────────────────────────────────┘

                    ┌──────────────────────────┐
                    │   PlacementService       │
                    ├──────────────────────────┤
                    │ - _director: FleetDir..  │
                    │ - _placementValidator:   │
                    │       ValidationHandler  │
                    ├──────────────────────────┤
                    │ + RandomizeFleet(board)  │
                    │ + TryPlace(board, ship): │
                    │       bool               │
                    └───────────┬──────────────┘
                                │ uses
                                ▼
                    ┌──────────────────────────┐
                    │  ValidationHandler       │
                    └──────────────────────────┘

TryPlace(board, ship):

1. Sukuria `ValidationContext` su `Board` ir `Ship`.
2. Kvies ` _placementValidator.Handle(context)` → leidžia grandinei patikrinti klaidas.
3. Jei `context.IsValid == true`, kviečia `board.Place(ship)`, kitaip grąžina `false`.
```

