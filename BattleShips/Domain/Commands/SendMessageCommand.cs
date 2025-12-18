using BattleShips.Services;

namespace BattleShips.Domain.Commands;


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

    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(_text)) return;
        
        var trimmedText = _text.Trim();
        _message = new ChatMessage(_sender, trimmedText, DateTime.Now);
        _chatService.SendMessage(_message);
    }

    public void Undo()
    {
        if (_message != null)
        {
            _chatService.RemoveMessage(_message);
        }
    }
}










