# Abstract Factory Pattern - Class Diagram

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        ABSTRACT FACTORY PATTERN                              │
└─────────────────────────────────────────────────────────────────────────────┘


                            ┌──────────────────────────┐
                            │   <<interface>>          │
                            │   IShipFactory           │
                            ├──────────────────────────┤
                            │ + CreateBattleship():    │
                            │     ShipBase             │
                            │ + CreateSubmarine():     │
                            │     ShipBase             │
                            │ + CreateDestroyer():     │
                            │     ShipBase             │
                            │ + CreateCruiser():       │
                            │     ShipBase             │
                            │ + CreateShip(kind):      │
                            │     ShipBase             │
                            └──────────────────────────┘
                                       △
                                       │implements
                                       │
                         ┌─────────────┴─────────────┐
                         │                           │
                         │                           │
         ┌───────────────┴──────────┐   ┌───────────┴──────────────┐
         │ ClassicShipFactory       │   │ ModernShipFactory        │
         ├──────────────────────────┤   ├──────────────────────────┤
         │ + CreateBattleship():    │   │ + CreateBattleship():    │
         │     ShipBase             │   │     ShipBase             │
         │ + CreateSubmarine():     │   │ + CreateSubmarine():     │
         │     ShipBase             │   │     ShipBase             │
         │ + CreateDestroyer():     │   │ + CreateDestroyer():     │
         │     ShipBase             │   │     ShipBase             │
         │ + CreateCruiser():       │   │ + CreateCruiser():       │
         │     ShipBase             │   │     ShipBase             │
         │ + CreateShip(kind):      │   │ + CreateShip(kind):      │
         │     ShipBase             │   │     ShipBase             │
         └──────────────────────────┘   └──────────────────────────┘
                    │ creates                    │ creates
                    │                            │
                    │                            │
                    ▼                            ▼
         ┌──────────────────┐         ┌──────────────────┐
         │  Classic Family  │         │  Modern Family   │
         └──────────────────┘         └──────────────────┘


                        ┌──────────────────────────┐
                        │    <<abstract>>          │
                        │    ShipBase              │
                        ├──────────────────────────┤
                        │ + Name: string           │
                        │ + Length: int            │
                        │ + Kind: ShipKind         │
                        │ + Start: Position        │
                        │ + Orientation: Orientation│
                        ├──────────────────────────┤
                        │ + Cells(): Position[]    │
                        │ + RegisterHit(pos): void │
                        │ + Reposition(): void     │
                        │ + IsSunk: bool           │
                        └──────────────────────────┘
                                   △
                                   │extends
                                   │
              ┌────────────────────┴────────────────────┐
              │                                         │
    ┌─────────┴──────────┐                  ┌──────────┴─────────┐
    │                    │                  │                    │
┌───▼─────────┐  ┌───────▼──┐      ┌───────▼──────┐  ┌─────────▼───────┐
│ Battleship  │  │ Submarine│      │ModernBattleship│ │ModernSubmarine │
├─────────────┤  ├──────────┤      ├────────────────┤ ├─────────────────┤
│+ Name       │  │+ Name    │      │+ Name          │ │+ Name           │
│+ Length = 4 │  │+ Length=3│      │+ Length = 5    │ │+ Length = 4     │
│+ Kind       │  │+ Kind    │      │+ Kind          │ │+ Kind           │
└─────────────┘  └──────────┘      └────────────────┘ └─────────────────┘

┌─────────────┐  ┌──────────┐      ┌────────────────┐ ┌─────────────────┐
│ Destroyer   │  │ Cruiser  │      │ModernDestroyer │ │ModernCruiser    │
├─────────────┤  ├──────────┤      ├────────────────┤ ├─────────────────┤
│+ Name       │  │+ Name    │      │+ Name          │ │+ Name           │
│+ Length = 2 │  │+ Length=1│      │+ Length = 3    │ │+ Length = 2     │
│+ Kind       │  │+ Kind    │      │+ Kind          │ │+ Kind           │
└─────────────┘  └──────────┘      └────────────────┘ └─────────────────┘


┌────────────────────────────────────────────────────────────────────────┐
│                    CLIENT & BUILDER INTEGRATION                         │
└────────────────────────────────────────────────────────────────────────┘

        ┌──────────────────────────┐
        │ PlacementService         │
        ├──────────────────────────┤
        │ - _director: FleetDirector│
        │ - _shipType: ShipType    │
        ├──────────────────────────┤
        │ + SetShipType(type)      │
        │ + RandomizeFleet(board)  │
        └──────────────┬───────────┘
                       │ uses
                       │
                       ▼
        ┌──────────────────────────┐
        │   <<interface>>          │
        │   IBoardBuilder          │
        ├──────────────────────────┤
        │ + BuildPart(kind)        │
        │ + GetResult(): Board     │
        └──────────────────────────┘
                       △
                       │implements
         ┌─────────────┴──────────────┐
         │                            │
┌────────▼────────────┐    ┌──────────▼──────────┐
│RandomFleetBuilder   │    │ManualFleetBuilder   │
├─────────────────────┤    ├─────────────────────┤
│- _board: Board      │    │- _board: Board      │
│- _shipFactory:      │    │- _shipFactory:      │
│    IShipFactory     │    │    IShipFactory     │
│- _rng: Random       │    │                     │
├─────────────────────┤    ├─────────────────────┤
│+ BuildPart(kind)    │    │+ BuildPart(kind,    │
│+ GetResult(): Board │    │    pos, orient)     │
└─────────────────────┘    │+ TryBuildPart()     │
         │                 └─────────────────────┘
         │ uses                      │ uses
         │                           │
         └────────────┬──────────────┘
                      │
                      ▼
        ┌──────────────────────────┐
        │   IShipFactory           │
        │   (injected dependency)  │
        └──────────────────────────┘


┌─────────────────────┐
│ <<enumeration>>     │
│ ShipType            │
├─────────────────────┤
│ Classic             │
│ Modern              │
└─────────────────────┘

┌─────────────────────┐
│ <<enumeration>>     │
│ ShipKind            │
├─────────────────────┤
│ Battleship          │
│ Submarine           │
│ Destroyer           │
│ Cruiser             │
└─────────────────────┘


┌────────────────────────────────────────────────────────────────────────┐
│ RELATIONSHIPS                                                           │
├────────────────────────────────────────────────────────────────────────┤
│ • IShipFactory ──creates──> ShipBase                                   │
│ • ClassicShipFactory ──creates──> Battleship, Submarine, etc.          │
│ • ModernShipFactory ──creates──> ModernBattleship, ModernSubmarine...  │
│ • RandomFleetBuilder ──uses──> IShipFactory                            │
│ • ManualFleetBuilder ──uses──> IShipFactory                            │
│ • PlacementService ──selects──> IShipFactory (based on ShipType)       │
└────────────────────────────────────────────────────────────────────────┘
```
