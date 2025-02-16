namespace TuneTask.Shared.Exceptions;

public class UserAlreadyExistsException : BaseException
{
    public UserAlreadyExistsException(string email)
        : base($"User with email '{email}' already exists.", 400) { }
}
