using BattleShips.Services;

namespace BattleShips.Domain.Commands;

/// <summary>
/// Concrete Command class for clearing chat messages.
/// Implements Command pattern for clearing all messages from chat.
/// </summary>
public class ClearChatCommand : ICommand
{
    private readonly ChatService _chatService;
    private List<ChatMessage>? _backup;

    public ClearChatCommand(ChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Executes the command by clearing all messages from chat.
    /// </summary>
    public void Execute()
    {
        // Backup messages before clearing (for undo)
        _backup = new List<ChatMessage>(_chatService.Messages);
        _chatService.ClearMessages();
    }

    /// <summary>
    /// Undoes the command by restoring the cleared messages.
    /// </summary>
    public void Undo()
    {
        if (_backup != null && _backup.Count > 0)
        {
            _chatService.RestoreMessages(_backup);
            _backup = null;
        }
    }
}

