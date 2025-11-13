namespace ForaProject.Domain.Exceptions;

/// <summary>
/// Exception thrown when user-related business rules are violated.
/// </summary>
public class InvalidUserDataException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidUserDataException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidUserDataException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidUserDataException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidUserDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
