# Factory Method Pattern - Class Diagram

## Apžvalga (Overview)

Factory Method pattern yra sukurtas tarp `Board` ir `Cell` klasių. `Board` klasė naudoja `CellFactory` (Factory Method pattern) kurdama `Cell` objektus, o ne tiesiogiai naudodama `new Cell()`.

---

## UML Klasės Diagrama (UML Class Diagram)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        FACTORY METHOD PATTERN                                 │
│                         Board → Cell Creation                                │
└─────────────────────────────────────────────────────────────────────────────┘


                            ┌──────────────────────────┐
                            │    <<abstract>>          │
                            │    CellFactory           │
                            ├──────────────────────────┤
                            │                          │
                            ├──────────────────────────┤
                            │ + CreateCell(row, col):  │
                            │     Cell                 │
                            │ + CreateCellWithPosition │
                            │     (pos): Cell          │
                            └──────────────────────────┘
                                       △
                                       │inherits
                                       │
                                       │
                         ┌────────────┴─────────────┐
                         │                          │
         ┌───────────────┴──────────┐               │
         │                          │               │
         │ StandardCellFactory      │               │
         ├──────────────────────────┤               │
         │                          │               │
         ├──────────────────────────┤               │
         │ + CreateCell(row, col):  │               │
         │     Cell                 │               │
         └──────────────────────────┘               │
                    │ creates                       │
                    │                               │
                    ▼                               │
         ┌──────────────────────────┐               │
         │         Cell             │               │
         ├──────────────────────────┤               │
         │ + Pos: Position          │               │
         │ + Status: CellStatus     │               │
         │ + Ship: ShipBase?        │               │
         │ + IsRevealed: bool       │               │
         └──────────────────────────┘               │


┌─────────────────────────────────────────────────────────────────────────────┐
│                        CLIENT CLASS: Board                                    │
└─────────────────────────────────────────────────────────────────────────────┘

                    ┌──────────────────────────┐
                    │        Board             │
                    ├──────────────────────────┤
                    │ - Size: int              │
                    │ - _cells: Cell[,]        │
                    │ - _ships: List<ShipBase> │
                    │ - _cellFactory:          │
                    │     CellFactory          │
                    ├──────────────────────────┤
                    │ + Board(size, factory?)  │
                    │ + Place(ship): bool      │
                    │ + FireAt(pos): ShotResult│
                    └──────────────────────────┘
                              │ uses
                              │
                              ▼
                    ┌──────────────────────────┐
                    │     CellFactory          │
                    │   (Factory Method)       │
                    └──────────────────────────┘
```

---

## Pattern Komponentai (Pattern Components)

### 1. **Product** (Produktas)
- **Klasė**: `Cell`
- **Aprašymas**: Tai objektas, kurį sukuria Factory Method
- **Vieta**: `Domain/Cell.cs`
- **Savybės**:
  - `Pos: Position` - ląstelės pozicija
  - `Status: CellStatus` - ląstelės būsena (Empty, Ship, Hit, Miss, Sunk)
  - `Ship: ShipBase?` - laivas, esantis ląstelėje (jei yra)

### 2. **Creator** (Kūrėjas)
- **Klasė**: `CellFactory` (abstrakti klasė)
- **Aprašymas**: Apibrėžia Factory Method, kuris sukuria `Cell` produktus
- **Vieta**: `Domain/Cells/CellFactory.cs`
- **Metodai**:
  - `CreateCell(int row, int col): Cell` - **Factory Method** (abstraktus)
  - `CreateCellWithPosition(Position position): Cell` - Template Method (virtualus)

### 3. **ConcreteCreator** (Konkretus Kūrėjas)
- **Klasė**: `StandardCellFactory`
- **Aprašymas**: Konkrečiai implementuoja Factory Method, kad sukurtų standartinius `Cell` objektus
- **Vieta**: `Domain/Cells/StandardCellFactory.cs`
- **Metodai**:
  - `CreateCell(int row, int col): Cell` - sukuria standartinį `Cell` objektą

### 4. **Client** (Klientas)
- **Klasė**: `Board`
- **Aprašymas**: Naudoja `CellFactory` kurdama `Cell` objektus
- **Vieta**: `Domain/Board.cs`
- **Naudojimas**: `Board` konstruktoriuje naudojamas `_cellFactory.CreateCell()` metodas

---

## Kaip Veikia Pattern (How It Works)

### Prieš Factory Method Pattern:
```csharp
// Board.cs konstruktoriuje
for (int r = 0; r < size; r++)
    for (int c = 0; c < size; c++)
        _cells[r,c] = new Cell(r,c);  // Tiesioginis kūrimas
```

### Su Factory Method Pattern:
```csharp
// Board.cs konstruktoriuje
_cellFactory = cellFactory ?? new StandardCellFactory();
for (int r = 0; r < size; r++)
    for (int c = 0; c < size; c++)
        _cells[r,c] = _cellFactory.CreateCell(r, c);  // Factory Method
```

---

## Kodėl Naudojame Factory Method? (Why Use Factory Method?)

### Privalumai (Benefits):

1. **Lankstumas (Flexibility)**
   - Galima lengvai pridėti naujų `Cell` tipų (pvz., `SpecialCell`, `BonusCell`)
   - Nereikia keisti `Board` klasės kodo

2. **Atskyrimas (Separation of Concerns)**
   - `Board` klasė nepriklauso nuo konkretaus `Cell` kūrimo būdo
   - `Cell` kūrimo logika yra atskirta

3. **Išplėčiamumas (Extensibility)**
   - Pattern leidžia lengvai pridėti naujų factory tipų, jei prireiks

4. **Testavimas (Testability)**
   - Galima naudoti Mock Factory testuose
   - Lengviau testuoti `Board` klasę su skirtingais `Cell` tipais

---

## Pattern Struktūra (Pattern Structure)

```
┌─────────────────────────────────────────────────────────┐
│                    Factory Method Pattern                │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  Creator (CellFactory)                                   │
│    ├─ FactoryMethod: CreateCell() [abstract]           │
│    └─ TemplateMethod: CreateCellWithPosition()         │
│                                                           │
│  ConcreteCreator (StandardCellFactory)                   │
│    └─ CreateCell() [concrete implementation]            │
│                                                           │
│  Product (Cell)                                          │
│    └─ Created by FactoryMethod                           │
│                                                           │
│  Client (Board)                                          │
│    └─ Uses Creator to create Products                    │
│                                                           │
└─────────────────────────────────────────────────────────┘
```

---

## Kodo Lokacijos (Code Locations)

### Factory Method Pattern Failai:
- **Abstrakti Factory**: `BattleShips/Domain/Cells/CellFactory.cs`
- **Konkreti Factory**: `BattleShips/Domain/Cells/StandardCellFactory.cs`
- **Produktas**: `BattleShips/Domain/Cell.cs`
- **Klientas**: `BattleShips/Domain/Board.cs`

### Naudojimas:
- `Board` konstruktorius naudoja `_cellFactory.CreateCell()` metodą
- Visi `Board` objektai kuriami su `StandardCellFactory` pagal nutylėjimą
- Galima perduoti custom factory per konstruktorių

---

---

## Palyginimas su Kitu Patterns (Comparison with Other Patterns)

### Factory Method vs Abstract Factory:
- **Factory Method**: Sukuria vieną produktą (Cell)
- **Abstract Factory**: Sukuria produktų šeimą (Ship family - Classic/Modern)

### Factory Method vs Builder:
- **Factory Method**: Sukuria vieną objektą vienu žingsniu
- **Builder**: Sukuria sudėtingą objektą keliais žingsniais (fleet creation)

---

## Reziumė (Summary)

Factory Method pattern yra sėkmingai implementuotas tarp `Board` ir `Cell` klasių:

✅ **Abstrakti Creator klasė**: `CellFactory`  
✅ **Konkreti Creator klasė**: `StandardCellFactory`  
✅ **Produktas**: `Cell`  
✅ **Klientas**: `Board` (naudoja factory metodą)  

Pattern suteikia:
- Atskiria `Cell` kūrimo logiką nuo `Board` klasės
- Leidžia testuoti ir mock'inti `Cell` kūrimą
- Užtikrina lankstumą ir moduliarumą

---

## Diagramos Legendos (Diagram Legends)

- `┌─┐` - Klasė
- `├─┤` - Klasės savybės/metodai
- `△` - Inheritance (paveldėjimas)
- `│` - Uses (naudojimas)
- `└─┘` - Relationship (ryšys)
- `<<abstract>>` - Abstrakti klasė
- `<<interface>>` - Interface

---

*Sukurta: Factory Method Pattern implementacija BattleShips projekte*

