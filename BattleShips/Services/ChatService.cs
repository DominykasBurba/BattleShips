namespace BattleShips.Services;

public record ChatMessage(string Sender, string Text, DateTime Timestamp);

public class ChatService
{
    public IReadOnlyList<ChatMessage> Messages => _messages;
    private readonly List<ChatMessage> _messages = new();
    public event Action? Updated;

    public void Send(string sender, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        _messages.Add(new ChatMessage(sender, text.Trim(), DateTime.Now));
        Updated?.Invoke();
    }

    public void Clear()
    {
        _messages.Clear();
        Updated?.Invoke();
    }
}