# Singleton Design Pattern - GameSession

## Apžvalga

Singleton Design Pattern yra implementuotas `GameSession` klasėje. `GameSession` naudoja Singleton pattern, kad užtikrintų, jog aplikacijoje egzistuoja tik viena `GameSession` instancija.

## UML Diagramas

```
┌─────────────────────────────────────────────────────────┐
│                    GameSession                          │
│  (Singleton)                                            │
├─────────────────────────────────────────────────────────┤
│  - _instance: GameSession? (static)                    │
│  - _lock: object (static)                              │
│  - _phase: Phase                                       │
│  - _current: Player                                    │
│  - _winner: Player?                                    │
│  - _draw: DrawState                                    │
│  + P1: Player                                          │
│  + P2: Player                                          │
│  + Phase: Phase                                        │
│  + Current: Player                                     │
│  + Winner: Player?                                     │
│  + Draw: DrawState                                     │
│  + ShotsPerTurn: int                                   │
├─────────────────────────────────────────────────────────┤
│  - GameSession(p1: Player, p2: Player)                 │
│  + GetInstance(p1: Player, p2: Player): GameSession    │
│  - Initialize(p1: Player, p2: Player): void            │
│  + ResetInstance(): void (static)                      │
│  + TryStart(): bool                                    │
│  + ResetBoards(): void                                 │
│  + Fire(pos: Position): ShotResult                     │
│  + EndTurn(): void                                     │
│  + Surrender(who: Player): void                        │
│  + ProposeDraw(who: Player): void                      │
│  + AcceptDraw(who: Player): void                       │
└─────────────────────────────────────────────────────────┘
                        ▲
                        │
                        │ extends
                        │
┌─────────────────────────────────────────────────────────┐
│                      Subject                            │
│  (Observer Pattern)                                     │
├─────────────────────────────────────────────────────────┤
│  - _observers: List<IObserver>                         │
│  + Attach(observer: IObserver): void                   │
│  + Detach(observer: IObserver): void                   │
│  + Notify(): void                                      │
└─────────────────────────────────────────────────────────┘
```

## Singleton Pattern Komponentai

### 1. **Private Static Instance**
- **Laukas**: `_instance: GameSession?`
- **Aprašymas**: Saugo vienintelę `GameSession` instanciją
- **Vieta**: `Domain/GameSession.cs` (line 13)

### 2. **Private Constructor**
- **Metodas**: `GameSession(Player p1, Player p2)`
- **Aprašymas**: Private konstruktorius, kuris neleidžia kurti naujų instancijų iš išorės
- **Vieta**: `Domain/GameSession.cs` (line 84)
- **Funkcionalumas**: Inicializuoja P1, P2 ir nustato _current = P1

### 3. **Public Static GetInstance Method**
- **Metodas**: `GetInstance(Player p1, Player p2): GameSession`
- **Aprašymas**: Grąžina vienintelę `GameSession` instanciją. Jei instancija neegzistuoja, ją sukuria. Jei egzistuoja, ją reinicializuoja su naujais žaidėjais.
- **Vieta**: `Domain/GameSession.cs` (line 98)
- **Thread-Safety**: Naudoja double-checked locking pattern su `_lock` objektu

### 4. **Initialize Method**
- **Metodas**: `Initialize(Player p1, Player p2): void` (private)
- **Aprašymas**: Reinicializuoja Singleton instanciją su naujais žaidėjais
- **Vieta**: `Domain/GameSession.cs` (line 121)
- **Funkcionalumas**: Nustato P1, P2, _current, _phase, _winner, _draw, ShotsPerTurn

### 5. **ResetInstance Method**
- **Metodas**: `ResetInstance(): void` (static)
- **Aprašymas**: Išvalo Singleton instanciją (naudojama testavimui arba valymui)
- **Vieta**: `Domain/GameSession.cs` (line 135)

## Singleton Pattern Srautas

### GetInstance() Srautas:

```
Client calls GetInstance(p1, p2)
    ↓
Check if _instance == null
    ↓
[Yes] → Lock (_lock)
    ↓
    Check again if _instance == null (double-check)
    ↓
    [Yes] → Create new GameSession(p1, p2)
    ↓
    [No] → Initialize existing instance with (p1, p2)
    ↓
    Return _instance
    ↓
[No] → Initialize existing instance with (p1, p2)
    ↓
Return _instance
```

## Naudojimo Pavyzdžiai

### GameService.cs
```csharp
public void NewLocalSession(int size = 10, bool enemyIsAi = true, ShipType shipType = ShipType.Classic)
{
    var p1 = new HumanPlayer("Player 1", size);
    Player p2 = enemyIsAi ? new AiPlayer("Enemy AI", size) : new HumanPlayer("Player 2", size);
    Session = GameSession.GetInstance(p1, p2); // Use Singleton pattern
    // ...
}
```

### GameLobbyService.cs
```csharp
public OnlineGameSession(string gameId, string player1ConnectionId, int boardSize, ...)
{
    // ...
    var p1 = new HumanPlayer("Player 1", boardSize);
    var p2 = new HumanPlayer("Player 2", boardSize);
    GameSession = Domain.GameSession.GetInstance(p1, p2); // Use Singleton pattern
    // ...
}
```

## Singleton Pattern Privalumai

1. **Vienintelė Instancija**: Užtikrina, kad aplikacijoje egzistuoja tik viena `GameSession` instancija
2. **Globalus Prieigos Taškas**: Pateikia globalų prieigos tašką prie `GameSession` instancijos
3. **Lazy Initialization**: Instancija sukuriama tik tada, kai ji pirmą kartą reikalinga
4. **Thread-Safety**: Double-checked locking pattern užtikrina thread-safe operacijas
5. **Reinitialization**: Galima reinicializuoti instanciją su naujais duomenimis

## Singleton Pattern Trūkumai

1. **Global State**: Singleton naudoja globalų būseną, kas gali sukelti problemas testuojant
2. **Tight Coupling**: Kodas tampa labiau susietas su Singleton klase
3. **Multiple Games Limitation**: Jei reikia kelių vienu metu veikiančių žaidimų, Singleton gali sukelti problemas

## Failo Vieta

- **Klase**: `GameSession`
- **Failas**: `BattleShips/Domain/GameSession.cs`
- **Naudojama**: 
  - `BattleShips/Services/GameService.cs`
  - `BattleShips/Services/GameLobbyService.cs`

## Singleton Pattern Struktūra

```
┌─────────────────────────────────────────────────────────────┐
│                      Singleton Pattern                      │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Client (GameService, GameLobbyService)            │    │
│  │  - Calls GetInstance(p1, p2)                       │    │
│  └────────────────────────────────────────────────────┘    │
│                         │                                    │
│                         │ GetInstance()                      │
│                         ▼                                    │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Singleton (GameSession)                           │    │
│  │  - _instance: GameSession? (static)                │    │
│  │  - GetInstance(): GameSession (static)             │    │
│  │  - Private constructor                             │    │
│  └────────────────────────────────────────────────────┘    │
│                                                              │
└─────────────────────────────────────────────────────────────┘
```

## Thread-Safety Implementation

Singleton pattern naudoja **double-checked locking** pattern, kad užtikrintų thread-safety:

```csharp
public static GameSession GetInstance(Player p1, Player p2)
{
    if (_instance == null)  // First check (no lock)
    {
        lock (_lock)  // Lock before second check
        {
            if (_instance == null)  // Second check (with lock)
            {
                _instance = new GameSession(p1, p2);
            }
        }
    }
    else
    {
        _instance.Initialize(p1, p2);
    }
    return _instance;
}
```

**Kodėl double-checked locking?**
1. Pirmas patikrinimas (be lock): Jei instancija jau egzistuoja, praleidžiame lock operaciją (geresnis performance)
2. Lock: Užtikrina, kad tik vienas thread gali kurti instanciją
3. Antras patikrinimas (su lock): Užtikrina, kad instancija nebuvo sukurta kitame thread'e tarp pirmo patikrinimo ir lock gavimo

## Išvados

Singleton Design Pattern yra sėkmingai implementuotas `GameSession` klasėje su šiomis savybėmis:

✅ **Private Constructor**: Neleidžia kurti instancijų iš išorės  
✅ **Static Instance**: Saugo vienintelę instanciją  
✅ **GetInstance Method**: Pateikia prieigą prie instancijos  
✅ **Thread-Safety**: Double-checked locking pattern  
✅ **Reinitialization**: Galima reinicializuoti instanciją  
✅ **Reset Method**: Galima išvalyti instanciją testavimui  

Pattern užtikrina, kad aplikacijoje egzistuoja tik viena `GameSession` instancija, o visi klientai naudoja tą pačią instanciją.

