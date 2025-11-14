using BattleShips.Services;

namespace BattleShips.Domain.Commands;

/// <summary>
/// Concrete Command class for sending a chat message.
/// Implements Command pattern for sending messages to chat.
/// </summary>
public class SendMessageCommand : ICommand
{
    private readonly ChatService _chatService;
    private readonly string _sender;
    private readonly string _text;
    private ChatMessage? _message;

    public SendMessageCommand(ChatService chatService, string sender, string text)
    {
        _chatService = chatService;
        _sender = sender;
        _text = text;
    }

    /// <summary>
    /// Executes the command by sending the message to chat.
    /// </summary>
    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(_text)) return;
        
        var trimmedText = _text.Trim();
        _message = new ChatMessage(_sender, trimmedText, DateTime.Now);
        _chatService.SendMessage(_message);
    }

    /// <summary>
    /// Undoes the command by removing the last sent message.
    /// </summary>
    public void Undo()
    {
        if (_message != null)
        {
            _chatService.RemoveMessage(_message);
        }
    }
}








