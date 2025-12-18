## Nauji patternai kode – kaip jie realiai veikia

Šitas failas paaiškina **3 naujus šablonus**, kuriuos pridėjome prie žaidimo:

- **Memento (k)** – kaip išsaugoma ir atstatoma `GameSession` būsena.
- **Chain of Responsibility (g)** – kaip validuojamas laivo statymas.
- **Mediator (j)** – kaip centralizuojama servisų koordinacija (žaidimas + chat + undo).

Kiekvienam patternui pateikiu:

- **Kuriose klasėse jis realizuotas.**
- **Koks duomenų srautas (kas ką kviečia).**
- **Kaip tai atrodo iš UI / „kliento“ perspektyvos.**

---

## 1. Memento (k) – `GameSession` būsenos išsaugojimas

### Pagrindinės klasės

- `GameSession` (`Domain/GameSession.cs`) – **Originator**:
  - žino visą žaidimo būseną (`P1`, `P2`, `Phase`, `Current`, `Winner`, `Draw`, `ShotsPerTurn`);
  - turi metodus:
    - `CreateMemento(): GameSessionMemento`
    - `Restore(memento: GameSessionMemento): void`
- `GameSessionMemento` (`Domain/Memento/GameSessionMemento.cs`) – **Memento**:
  - tik **duomenų konteineris**, be logikos;
  - savybės:
    - `P1`, `P2: PlayerStateDto`
    - `Phase`, `CurrentPlayerName`, `WinnerName`, `Draw`, `ShotsPerTurn`
- `PlayerStateDto`, `ShipStateDto` (`Domain/Memento/GameSessionMemento.cs`) – DTO objektai:
  - `PlayerStateDto` – vieno žaidėjo santrumpinta būsena (lentos dydis ir laivai).
  - `ShipStateDto` – vieno laivo padėtis ir būsena.
- `GameHistoryCaretaker` (`Domain/Memento/GameHistoryCaretaker.cs`) – **Caretaker**:
  - turi `Stack<GameSessionMemento> _history`;
  - pagrindiniai metodai:
    - `Save(GameSession session)` – sukuria ir išsaugo Memento.
    - `Undo(GameSession session)` – išima paskutinį Memento ir atstato būseną.

### Ką daro `GameSession.CreateMemento()`

1. **Surinka kiekvieno žaidėjo būseną**:
   - per privačią funkciją `CreatePlayerState(Player p)`:
     - paima `p.Board.Ships`,
     - kiekvienam laivui sukuria `ShipStateDto` su:
       - `Kind`, `Start`, `Orientation`, `IsSunk`.
2. Sudeda į `PlayerStateDto`:
   - `Name`, `Kind`, `BoardSize`, `Ships`.
3. Supakuoja viską į `GameSessionMemento`:
   - priskiria `P1`, `P2`, `Phase`, `CurrentPlayerName`, `WinnerName`, `Draw`, `ShotsPerTurn`.

**Rezultatas** – gaunam **pilną žaidimo „nuotrauką“** viename objekte.

### Ką daro `GameSession.Restore(memento)`

1. Kviečia `RestorePlayerState(player, dto)` žaidėjams `P1` ir `P2`:
   - išvalo lentą: `player.Board.Clear();`
   - kiekvienam `ShipStateDto`:
     - sukuria naują laivą: `Ships.Create(kind, start, orientation);`
     - padeda jį ant lentos: `player.Board.Place(ship);`
     - jei `IsSunk == true` – per visus `ship.Cells()` iškviečia `Board.FireAt(...)`, kad atkurti hit/sunk statusą.
2. Atstato bendrus laukus:
   - `Phase = memento.Phase;`
   - `Current` nustatomas pagal `CurrentPlayerName`;
   - `Winner` pagal `WinnerName`;
   - `Draw` ir `ShotsPerTurn` – tiesiogiai iš Memento.

### Ką daro `GameHistoryCaretaker`

- **Save**:
  - gauna `GameSession` objektą;
  - kviečia `session.CreateMemento()` ir `Push`’ina rezultatą į `_history`.
- **Undo**:
  - jei `_history` tuščias – nieko nedaro;
  - paima viršų iš `Stack`;
  - kviečia `session.Restore(memento)`.

### Kur Memento naudojamas

- **Mediator** (`Services/GameMediator.cs`) turi lauką:

  - `_history: GameHistoryCaretaker`

- Prieš **svarbius veiksmus** (naujas žaidimas, randomize, šūvis, surrender, draw pasiūlymas ir pan.) Mediator kviečia:
  - `_history.SaveIfPossible(Session);`
- Kai vartotojas aktyvuoja **Undo**:
  - `GameMediator.UndoLastMove()` kviečia `_history.Undo(Session)`.

---

## 2. Chain of Responsibility (g) – laivo statymo validacija

### Pagrindinės klasės

- `ValidationContext` (`Domain/Validation/ValidationContext.cs`):
  - laukai:
    - `Board: Board` – lenta, su kuria dirbame;
    - `Ship?: IShip` – laivas, kurį statome;
    - `Position?: Position` – gali būti naudojama šūvio validacijai;
    - `Errors: List<string>` – surinktos klaidos;
  - `IsValid` – graži property (`Errors.Count == 0`).

- `ValidationHandler` (`Domain/Validation/ValidationHandler.cs`) – **abstraktus handleris**:
  - turi privatų lauką `_next: ValidationHandler?`;
  - metodas `SetNext(next)` nustato grandinės sekantį elementą ir grąžina `next`, kad būtų galima „chaining“.
  - metodas `Handle(context)`:
    1. kviečia `Validate(context)`;
    2. jei `Validate` grąžina `false` – grandinė **sustabdyta**;
    3. jei `true` – kviečiamas `_next?.Handle(context)`.
  - `Validate(context)` – abstraktus, realizuojamas konkrečiuose handleriuose.

- Konkrečios validacijos:
  - `BoundsValidationHandler` (`Domain/Validation/BoundsValidationHandler.cs`)
    - tikrina, ar **visos pozicijos yra lentoje**:
      - jei turime `Ship`, pereina per `Ship.Cells()`;
      - jei tik `Position`, validuoja vieną tašką.
  - `OverlapValidationHandler` (`Domain/Validation/OverlapValidationHandler.cs`)
    - tikrina, ar laivo pozicijose **nėra kitų laivų** (`cell.Ship != null`).
  - `AdjacencyValidationHandler` (`Domain/Validation/AdjacencyValidationHandler.cs`)
    - tikrina, ar laivas **nesiliečia** su kitais (įskaitant įstrižai),
    - naudoja helper’į `HasAdjacentShip(Board, Position)`.

### Kaip grandinė sukonstruojama

`PlacementService` konstruktoriuje (`Services/PlacementService.cs`):

- sukuriamas laukas:
  - `_placementValidator: ValidationHandler`;
- grandinė sudėliojama taip:
  - `BoundsValidationHandler` → `OverlapValidationHandler` → `AdjacencyValidationHandler`.

Tai reiškia:

1. Pirmiausia tikrinamos **ribos**;
2. tada – **persidengimas** su kitais laivais;
3. pabaigoje – **gretimumo taisyklė**.

### Kaip grandinė naudojama statant laivą

Metodas `PlacementService.TryPlace(Board board, IShip ship)`:

1. Sukuria `ValidationContext`:
   - `Board = board;`
   - `Ship = ship;`
2. Paleidžia grandinę:
   - `_placementValidator.Handle(ctx);`
3. Jei `ctx.IsValid == false` – grąžina `false`, **Board.Place** net nebandomas.
4. Jei `ctx.IsValid == true` – kviečia `board.Place(ship)` ir grąžina rezultatą.

**Svarbu**: pati lenta (`Board`) **nebemato** viso validacijos kodo – jis iškeltas į atskirą CoR struktūrą.

---

## 3. Mediator (j) – `GameMediator` tarp UI ir servisų

### Pagrindinės klasės

- `IGameMediator` (`Services/IGameMediator.cs`):
  - **sąsaja, per kurią turi kalbėtis UI / komponentai**:
    - `Session { get; }` – duoda dabartinę `GameSession`.
    - `ChatMessages { get; }` – grąžina `ChatService.Messages`.
    - Žaidimo veiksmai:
      - `StartLocalGame(...)`
      - `RandomizeForCurrentPlayer()`
      - `FireAt(Position)`
      - `Surrender()`
      - `ProposeDraw()`
      - `AcceptDraw()`
    - Chat:
      - `SendChatMessage(sender, text)`
    - Undo:
      - `CanUndo { get; }`
      - `UndoLastMove()`

- `GameMediator` (`Services/GameMediator.cs`) – **konkretus Mediator**:
  - laukai:
    - `_gameService: GameService`
    - `_chatService: ChatService`
    - `_history: GameHistoryCaretaker` (Memento Caretaker)
  - visus svarbius veiksmus atlieka **per servisus**, bet išorėje rodosi kaip viena API.

### Duomenų ir kvietimų srautas

#### Naujų žaidimų sukūrimas

Metodas `StartLocalGame(size, enemyIsAi, shipType, shipSkin)`:

1. Kviečia:
   - `_gameService.NewLocalSession(size, enemyIsAi, shipType, shipSkin);`
2. Po to, jei `Session` sukurtas, išsaugo būseną Memento stirtelėje:
   - `_history.SaveIfPossible(Session);`

UI sluoksniui nereikia žinoti apie `GameService` – jis tiesiog kviečia `mediator.StartLocalGame(...)`.

#### Laivų randomizacija

Metodas `RandomizeForCurrentPlayer()`:

1. Patikrina, ar `Session` egzistuoja.
2. Kviečia:
   - `_gameService.RandomizeFor(Session.Current);`
3. Išsaugo naują būseną:
   - `_history.SaveIfPossible(Session);`

Čia Mediator **koordinuoja**:

- `GameService` (logika, kuri naudoja `IFleetPlacer / PlacementService` viduje),
- `GameHistoryCaretaker` (Undo palaikymas).

#### Šūvis į langelį

Metodas `FireAt(Position pos)`:

1. Jei `Session == null` → `ShotResult.Invalid`.
2. Prieš šūvį saugo būseną:
   - `_history.SaveIfPossible(Session);`
3. Tada deleguoja šūvį:
   - `return _gameService.FireAt(pos);`

Vėl – UI žino tik apie `mediator.FireAt(pos)`, bet po kapotu:

- vyksta `GameService` strategijos logika,
- automatiškai kuriamas Memento Undo istorijai.

#### Žaidimo pabaigos veiksmai (surrender, draw)

Metodai `Surrender()`, `ProposeDraw()`, `AcceptDraw()`:

1. Jei `Session` neegzistuoja – daro „return“.
2. Prieš veiksmą saugo Memento:
   - `_history.SaveIfPossible(Session);`
3. Tada kviečia atitinkamą `GameService` metodą su `Session.Current` žaidėju.

#### Chat žinutės

Metodas `SendChatMessage(sender, text)`:

1. Sukuria `ChatMessage(sender, text, DateTime.UtcNow)`.
2. Kvietimas:
   - `_chatService.SendMessage(message);`
3. `ChatService` pats iškviečia `Updated` eventą, kad UI galėtų persirenderinti.

Mediator čia **atskiria** UI nuo konkrečios `ChatService` API – iš esmės UI mato tik vieną tašką.

#### Undo funkcionalumas

Savybė `CanUndo`:

- grąžina `Session != null && _history.CanUndo`.

Metodas `UndoLastMove()`:

1. Jei `Session` neegzistuoja – nieko.
2. Kviečia:
   - `_history.Undo(Session);`

Šiuo atveju **Mediator** jungia:

- UI / vartotojo veiksmą „Undo“,
- `GameSession` (per `Restore`),
- `GameHistoryCaretaker` (Memento steką).

### DI registracija (kaip Mediator patenka į UI)

`Program.cs` faile:

- servisas užregistruotas taip:

```csharp
builder.Services.AddScoped<BattleShips.Services.IGameMediator, BattleShips.Services.GameMediator>();
```

Tai reiškia:

- Blazor komponentai gali injektuoti:
  - `@inject IGameMediator Mediator`
- Tada naudoti visus metodus:
  - `Mediator.StartLocalGame(...)`
  - `Mediator.FireAt(position)`
  - `Mediator.SendChatMessage("Player 1", "Hi!")`
  - `if (Mediator.CanUndo) Mediator.UndoLastMove();`

---

## Santrauka

- **Memento** – `GameSession` → `GameSessionMemento` → `GameHistoryCaretaker`:
  - suteikia galimybę **atsukti žaidimą atgal**.
- **Chain of Responsibility** – `ValidationHandler` grandinė `PlacementService` viduje:
  - atsakinga už **laivų statymo taisyklių tikrinimą** prieš `Board.Place`.
- **Mediator** – `GameMediator`:
  - sujungia `GameService`, `ChatService` ir `GameHistoryCaretaker` į **vieną API**, kurią naudoja UI;
  - užtikrina, kad Undo / Chat / Game logika koordinuojama vienoje vietoje.



