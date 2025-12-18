namespace BattleShips.Services;

public record ChatMessage(string Sender, string Text, DateTime Timestamp);

public class ChatService
{
    public IReadOnlyList<ChatMessage> Messages => _messages;
    private readonly List<ChatMessage> _messages = new();
    public event Action? Updated;

    public void SendMessage(ChatMessage message)
    {
        _messages.Add(message);
        Updated?.Invoke();
    }

    public void RemoveMessage(ChatMessage message)
    {
        _messages.Remove(message);
        Updated?.Invoke();
    }

    public void ClearMessages()
    {
        _messages.Clear();
        Updated?.Invoke();
    }

    public void RestoreMessages(IEnumerable<ChatMessage> messages)
    {
        _messages.Clear();
        _messages.AddRange(messages);
        Updated?.Invoke();
    }
}