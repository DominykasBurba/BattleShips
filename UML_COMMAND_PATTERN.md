# Command Pattern - Class Diagram

## Apžvalga (Overview)

Command Design Pattern yra sukurtas tarp `ChatService` (Chat) ir `ChatMessage` klasių. `ChatService` naudoja Command pattern vykdydamas operacijas su chat pranešimais, o ne tiesiogiai manipuliudamas `ChatMessage` objektais.

---

## UML Klasės Diagrama (UML Class Diagram)

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        COMMAND DESIGN PATTERN                                 │
│                    ChatService → ChatMessage Operations                      │
└─────────────────────────────────────────────────────────────────────────────┘


    ┌──────────────────────────┐
    │       Client              │
    │   (ChatBox.razor)        │
    ├──────────────────────────┤
    │                          │
    │ + Send(): void            │
    └──────────────────────────┘
                │ creates & passes
                │
                ▼
    ┌──────────────────────────┐
    │    ChatInvoker           │
    │   (Invoker)              │
    ├──────────────────────────┤
    │ - _command: ICommand?     │
    ├──────────────────────────┤
    │ + SetCommand(cmd): void  │
    │ + Run(): void            │
    │ + RunCommand(cmd): void  │
    └──────────────────────────┘
                │ holds & executes
                │
                ▼
                            ┌──────────────────────────┐
                            │    <<interface>>         │
                            │    ICommand              │
                            ├──────────────────────────┤
                            │                          │
                            │ + Execute(): void        │
                            │ + Undo(): void           │
                            └──────────────────────────┘
                                       △
                                       │implements
                                       │
                    ┌──────────────────┴──────────────────┐
                    │                                      │
                    │                                      │
    ┌───────────────┴──────────┐       ┌──────────────────┴──────────┐
    │                          │       │                              │
    │ SendMessageCommand       │       │ ClearChatCommand             │
    ├──────────────────────────┤       ├──────────────────────────────┤
    │ - _chatService:          │       │ - _chatService:              │
    │     ChatService           │       │     ChatService              │
    │ - _sender: string         │       │ - _backup: List<ChatMessage> │
    │ - _text: string           │       ├──────────────────────────────┤
    │ - _message: ChatMessage?  │       │ + Execute(): void            │
    ├──────────────────────────┤       │ + Undo(): void                │
    │ + Execute(): void         │       └──────────────────────────────┘
    │ + Undo(): void            │
    └──────────────────────────┘
                │ calls receiver.action()
                │
                ▼
    ┌──────────────────────────┐
    │     ChatService           │
    │   (Receiver)              │
    ├──────────────────────────┤
    │ - _messages: List<...>    │
    │ + Messages: IReadOnlyList│
    │ + Updated: event          │
    ├──────────────────────────┤
    │ + SendMessage(msg): void  │
    │ + RemoveMessage(msg): void│
    │ + ClearMessages(): void   │
    │ + RestoreMessages(): void │
    └──────────────────────────┘
                │ manages
                │
                ▼
    ┌──────────────────────────┐
    │      ChatMessage         │
    ├──────────────────────────┤
    │ + Sender: string          │
    │ + Text: string            │
    │ + Timestamp: DateTime     │
    └──────────────────────────┘
```

---

## Pattern Komponentai (Pattern Components)

### 1. **Command** (Komanda)
- **Interface**: `ICommand`
- **Aprašymas**: Apibrėžia sąsają vykdymo operacijoms
- **Vieta**: `Domain/Commands/ICommand.cs`
- **Metodai**:
  - `Execute(): void` - vykdo komandą
  - `Undo(): void` - atšaukia komandą

### 2. **ConcreteCommand** (Konkreti Komanda)
- **Klase**: `SendMessageCommand`, `ClearChatCommand`
- **Aprašymas**: Konkrečiai implementuoja komandas
- **Vieta**: 
  - `Domain/Commands/SendMessageCommand.cs`
  - `Domain/Commands/ClearChatCommand.cs`
- **Funkcionalumas**:
  - `SendMessageCommand` - siunčia pranešimą į chat
  - `ClearChatCommand` - išvalo visus chat pranešimus

### 3. **Receiver** (Gavėjas)
- **Klasė**: `ChatService`
- **Aprašymas**: Žino, kaip atlikti operacijas su `ChatMessage` objektais
- **Vieta**: `Services/ChatService.cs`
- **Metodai**:
  - `SendMessage(ChatMessage)` - prideda pranešimą
  - `RemoveMessage(ChatMessage)` - pašalina pranešimą
  - `ClearMessages()` - išvalo visus pranešimus
  - `RestoreMessages(IEnumerable<ChatMessage>)` - atkuria pranešimus

### 4. **Invoker** (Iškvietėjas)
- **Klasė**: `ChatInvoker`
- **Aprašymas**: Holds a command and executes it when Run() is called
- **Vieta**: `Domain/Commands/ChatInvoker.cs`
- **Metodai**:
  - `SetCommand(ICommand)` - nustato komandą vykdymui
  - `Run()` - vykdo nustatytą komandą
  - `RunCommand(ICommand)` - vykdo komandą tiesiogiai

### 5. **Product** (Produktas)
- **Klasė**: `ChatMessage`
- **Aprašymas**: Objektas, su kuriuo atliekamos operacijos
- **Vieta**: `Services/ChatService.cs` (record definition)
- **Savybės**:
  - `Sender: string` - siuntėjo vardas
  - `Text: string` - pranešimo tekstas
  - `Timestamp: DateTime` - laiko žymė

### 6. **Client** (Klientas)
- **Klasė**: `ChatBox.razor` (UI component)
- **Aprašymas**: Creates commands and passes them to the invoker
- **Vieta**: `Components/ChatBox.razor`
- **Funkcionalumas**: Creates `SendMessageCommand` and passes it to `ChatInvoker`

---

## Kaip Veikia Pattern (How It Works)

### Prieš Command Pattern:
```csharp
// ChatService.cs
public void Send(string sender, string text)
{
    if (string.IsNullOrWhiteSpace(text)) return;
    _messages.Add(new ChatMessage(sender, text.Trim(), DateTime.Now));
    Updated?.Invoke();
}
```

### Su Command Pattern (Klasikinė struktūra):
```csharp
// Client (ChatBox.razor)
private void Send()
{
    // Client creates command
    var command = new SendMessageCommand(Chat, "Player 1", text);
    
    // Client passes command to invoker
    _invoker.RunCommand(command);
}

// Invoker (ChatInvoker.cs)
public void RunCommand(ICommand command)
{
    command.Execute();  // Calls command.Execute()
}

// Command (SendMessageCommand.cs)
public void Execute()
{
    if (string.IsNullOrWhiteSpace(_text)) return;
    var trimmedText = _text.Trim();
    _message = new ChatMessage(_sender, trimmedText, DateTime.Now);
    _chatService.SendMessage(_message);  // Calls receiver.action()
}

// Receiver (ChatService.cs)
public void SendMessage(ChatMessage message)
{
    _messages.Add(message);  // Performs the actual operation
    Updated?.Invoke();
}
```

---

## Kodėl Naudojame Command Pattern? (Why Use Command Pattern?)

### Privalumai (Benefits):

1. **Atskyrimas (Separation of Concerns)**
   - `ChatService` nepriklauso nuo konkretaus operacijos vykdymo būdo
   - Komandos logika yra atskirta nuo `ChatService`

2. **Undo/Redo Funkcionalumas**
   - Lengvai galima pridėti undo/redo funkcionalumą
   - Kiekviena komanda žino, kaip atsaukti savo veiksmą

3. **Lankstumas (Flexibility)**
   - Galima lengvai pridėti naujų komandų tipų
   - Komandas galima vykdyti vėlavus (queue, log)

4. **Testavimas (Testability)**
   - Lengviau testuoti - komandos yra atskiri objektai
   - Galima mock'inti komandas

---

## Pattern Struktūra (Pattern Structure)

```
┌─────────────────────────────────────────────────────────┐
│                    Command Pattern                       │
├─────────────────────────────────────────────────────────┤
│                                                           │
│  Command (ICommand)                                      │
│    ├─ Execute(): void                                   │
│    └─ Undo(): void                                      │
│                                                           │
│  ConcreteCommand (SendMessageCommand, ClearChatCommand) │
│    ├─ Execute() [concrete implementation]               │
│    └─ Undo() [concrete implementation]                 │
│                                                           │
│  Receiver (ChatService)                                  │
│    └─ Knows how to perform operations on ChatMessage    │
│                                                           │
│  Invoker (ChatService.ExecuteCommand)                    │
│    └─ Calls Execute() on commands                       │
│                                                           │
│  Product (ChatMessage)                                   │
│    └─ Object being manipulated                          │
│                                                           │
└─────────────────────────────────────────────────────────┘
```

---

## Kodo Lokacijos (Code Locations)

### Command Pattern Failai:
- **Command Interface**: `BattleShips/Domain/Commands/ICommand.cs`
- **SendMessageCommand**: `BattleShips/Domain/Commands/SendMessageCommand.cs`
- **ClearChatCommand**: `BattleShips/Domain/Commands/ClearChatCommand.cs`
- **Invoker**: `BattleShips/Domain/Commands/ChatInvoker.cs`
- **Receiver**: `BattleShips/Services/ChatService.cs`
- **Client**: `BattleShips/Components/ChatBox.razor`
- **Product**: `ChatMessage` record in `BattleShips/Services/ChatService.cs`

### Naudojimas:
- **Client** (`ChatBox.razor`) sukuria komandas ir perduoda jas **Invoker** (`ChatInvoker`)
- **Invoker** (`ChatInvoker.RunCommand()`) vykdo komandas
- **Commands** (`SendMessageCommand`, `ClearChatCommand`) iškviečia **Receiver** (`ChatService`) metodus
- **Receiver** (`ChatService`) atlieka faktines operacijas su `ChatMessage` objektais

---

## Palyginimas su Kitu Patterns (Comparison with Other Patterns)

### Command vs Strategy:
- **Command**: Apkapsuliuoja užklausą kaip objektą (execute/undo)
- **Strategy**: Apkapsuliuoja algoritmą (vienas metodas, be undo)

### Command vs Observer:
- **Command**: Komanda vykdoma vieną kartą, gali būti atšaukiama
- **Observer**: Stebimas objektas praneša apie pakeitimus daugeliui stebėtojų

---

## Reziumė (Summary)

Command Design Pattern yra sėkmingai implementuotas tarp `ChatService` ir `ChatMessage` su klasikine struktūra:

✅ **Command Interface**: `ICommand`  
✅ **Concrete Commands**: `SendMessageCommand`, `ClearChatCommand`  
✅ **Invoker**: `ChatInvoker` (atskirta klasė)  
✅ **Receiver**: `ChatService` (tik receiver funkcijos)  
✅ **Client**: `ChatBox.razor` (sukuria komandas)  
✅ **Product**: `ChatMessage`  

Pattern struktūra:
- **Client** → **Invoker** → **Command** → **Receiver**

Pattern suteikia:
- Atskiria roles: Client, Invoker, Command, Receiver
- Leidžia lengvai pridėti undo/redo funkcionalumą
- Užtikrina lankstumą ir moduliarumą
- Atitinka klasikinį Command Pattern (B) dizainą

---

## Diagramos Legendos (Diagram Legends)

- `┌─┐` - Klasė
- `├─┤` - Klasės savybės/metodai
- `△` - Inheritance (paveldėjimas)
- `│` - Uses (naudojimas)
- `└─┘` - Relationship (ryšys)
- `<<interface>>` - Interface
- `<<abstract>>` - Abstrakti klasė

---

## Operacijų Pavyzdys (Operation Example)

### Send Message Flow (Client → Invoker → Command → Receiver):
```
1. Client (ChatBox.razor) sukuria SendMessageCommand(Chat, "Player 1", text)
2. Client perduoda komandą Invoker (ChatInvoker.RunCommand(command))
3. Invoker iškviečia command.Execute()
4. SendMessageCommand.Execute() sukuria ChatMessage objektą
5. SendMessageCommand iškviečia receiver.action() → ChatService.SendMessage(message)
6. ChatService (Receiver) prideda pranešimą į sąrašą
```

### Clear Chat Flow (Client → Invoker → Command → Receiver):
```
1. Client sukuria ClearChatCommand(Chat)
2. Client perduoda komandą Invoker (ChatInvoker.RunCommand(command))
3. Invoker iškviečia command.Execute()
4. ClearChatCommand.Execute() išsaugo backup ir iškviečia ChatService.ClearMessages()
5. ChatService (Receiver) išvalo visus pranešimus
```

### Pattern Flow Diagram:
```
Client (ChatBox)
    ↓ creates
    ↓
SendMessageCommand(command)
    ↓ passes to
    ↓
Invoker (ChatInvoker)
    ↓ calls
    ↓
Command.Execute()
    ↓ calls
    ↓
Receiver.action() (ChatService.SendMessage)
    ↓ performs
    ↓
ChatMessage (Product)
```

---

*Sukurta: Command Design Pattern implementacija BattleShips projekte*

