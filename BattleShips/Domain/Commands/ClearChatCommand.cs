using BattleShips.Services;

namespace BattleShips.Domain.Commands;

public class ClearChatCommand : ICommand
{
    private readonly ChatService _chatService;
    private List<ChatMessage>? _backup;

    public ClearChatCommand(ChatService chatService)
    {
        _chatService = chatService;
    }

    public void Execute()
    {
        _backup = new List<ChatMessage>(_chatService.Messages);
        _chatService.ClearMessages();
    }

    public void Undo()
    {
        if (_backup != null && _backup.Count > 0)
        {
            _chatService.RestoreMessages(_backup);
            _backup = null;
        }
    }
}

