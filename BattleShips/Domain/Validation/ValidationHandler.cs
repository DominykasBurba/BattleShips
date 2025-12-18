namespace BattleShips.Domain.Validation;

/// <summary>
/// Base handler in a Chain of Responsibility for validating actions
/// such as ship placement or firing.
/// </summary>
public abstract class ValidationHandler
{
    private ValidationHandler? _next;

    public ValidationHandler SetNext(ValidationHandler next)
    {
        _next = next;
        return next;
    }

    public void Handle(ValidationContext context)
    {
        if (!Validate(context))
        {
            // stop chain on failure
            return;
        }

        _next?.Handle(context);
    }

    /// <summary>
    /// Returns true if validation passes and chain may continue.
    /// </summary>
    protected abstract bool Validate(ValidationContext context);
}



