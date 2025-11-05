# Ship Abstract Factory Pattern

## Overview

This directory implements the **Abstract Factory Pattern** for creating families of related ships in the Battleship game.

The Abstract Factory pattern provides an interface for creating families of related objects without specifying their concrete classes. In this case, we have two ship families: **Classic** and **Modern**.

## Structure

- **IShipFactory**: Abstract factory interface defining methods to create different ship types
- **ClassicShipFactory**: Concrete factory that creates classic battleship game ships
- **ModernShipFactory**: Concrete factory that creates modern naval ships with enhanced capabilities

## Ship Families

### Classic Family (ClassicShipFactory)
Traditional battleship game ships:
- **Battleship**: Length 4
- **Submarine**: Length 3
- **Destroyer**: Length 2
- **Cruiser**: Length 1

### Modern Family (ModernShipFactory)
Enhanced modern naval vessels with larger sizes:
- **Modern Battleship**: Length 5 (enhanced)
- **Modern Submarine**: Length 4 (enhanced)
- **Modern Destroyer**: Length 3 (enhanced)
- **Modern Cruiser**: Length 2 (enhanced)

## Usage

### Creating Classic Ships

```csharp
using BattleShips.Domain.Ships.Factories;

// Create a classic ship factory
IShipFactory factory = new ClassicShipFactory();

// Create ships using type-specific methods
var battleship = factory.CreateBattleship(new Position(0, 0), Orientation.Horizontal);
var submarine = factory.CreateSubmarine(new Position(2, 0), Orientation.Vertical);

// Or use the enum-based method
var ship = factory.CreateShip(ShipKind.Battleship, new Position(0, 0), Orientation.Horizontal);
```

### Creating Modern Ships

```csharp
using BattleShips.Domain.Ships.Factories;

// Create a modern ship factory
IShipFactory factory = new ModernShipFactory();

// Create modern ships (larger and more powerful)
var battleship = factory.CreateBattleship(new Position(0, 0), Orientation.Horizontal);
// This creates a ModernBattleship with length 5 instead of classic 4
```

### Switching Ship Families at Runtime

```csharp
// Start with classic ships
IShipFactory factory = new ClassicShipFactory();
var builder = new RandomFleetBuilder(board, factory);

// Later switch to modern ships
factory = new ModernShipFactory();
var modernBuilder = new RandomFleetBuilder(anotherBoard, factory);

// Or inject the factory into builders
var classicBuilder = new RandomFleetBuilder(board, new ClassicShipFactory());
var modernBuilder = new RandomFleetBuilder(board, new ModernShipFactory());
```

### Using with Builders

The factory is used internally by the Builder pattern:

```csharp
// Builders use ClassicShipFactory by default
var randomBuilder = new RandomFleetBuilder(board);  // Uses ClassicShipFactory
var manualBuilder = new ManualFleetBuilder(board);  // Uses ClassicShipFactory

// You can inject a different factory
var modernBuilder = new RandomFleetBuilder(board, new ModernShipFactory());
```

## Benefits of Abstract Factory Pattern

1. **Family Consistency**: Ensures all ships created belong to the same family (all classic or all modern)
2. **Easy to Switch Families**: Change from classic to modern ships by swapping the factory
3. **Encapsulation**: Ship creation logic is centralized in factory classes
4. **Testability**: Can inject mock factories for testing
5. **Open/Closed Principle**: Add new ship families without modifying existing code
6. **Flexibility**: Client code works with any factory implementing IShipFactory

## When to Use Each Family

- **Classic Ships**: For traditional battleship gameplay, balanced fleet
- **Modern Ships**: For a harder challenge with larger ships requiring more hits

## Integration with Other Patterns

The Abstract Factory pattern works together with:
- **Builder Pattern** (RandomFleetBuilder, ManualFleetBuilder): Uses the factory to create ships during board construction
- **Director Pattern** (FleetDirector): Orchestrates the builder which uses the factory

## Future Extensions

You could add additional ship families by implementing IShipFactory:
- **HistoricalShipFactory**: WW1/WW2 era vessels
- **SciFiShipFactory**: Futuristic spaceships
- **FantasyShipFactory**: Magical vessels and dragon ships
