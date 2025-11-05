namespace BattleShips.Services;

public record ChatMessage(string Sender, string Text, DateTime Timestamp);

/// <summary>
/// Receiver class in Command Design Pattern.
/// Knows how to perform operations on ChatMessage objects.
/// Contains all message operations that commands can call.
/// </summary>
public class ChatService
{
    public IReadOnlyList<ChatMessage> Messages => _messages;
    private readonly List<ChatMessage> _messages = new();
    public event Action? Updated;

    /// <summary>
    /// Action method: Adds a message to the chat.
    /// Called by SendMessageCommand when executing.
    /// </summary>
    public void SendMessage(ChatMessage message)
    {
        _messages.Add(message);
        Updated?.Invoke();
    }

    /// <summary>
    /// Action method: Removes a message from the chat.
    /// Called by SendMessageCommand when undoing.
    /// </summary>
    public void RemoveMessage(ChatMessage message)
    {
        _messages.Remove(message);
        Updated?.Invoke();
    }

    /// <summary>
    /// Action method: Clears all messages from the chat.
    /// Called by ClearChatCommand when executing.
    /// </summary>
    public void ClearMessages()
    {
        _messages.Clear();
        Updated?.Invoke();
    }

    /// <summary>
    /// Action method: Restores messages to the chat.
    /// Called by ClearChatCommand when undoing.
    /// </summary>
    public void RestoreMessages(IEnumerable<ChatMessage> messages)
    {
        _messages.Clear();
        _messages.AddRange(messages);
        Updated?.Invoke();
    }
}