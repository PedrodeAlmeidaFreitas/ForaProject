namespace ForaProject.Domain.Exceptions;

/// <summary>
/// Exception thrown when company data is invalid or incomplete.
/// </summary>
public sealed class InvalidCompanyDataException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCompanyDataException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidCompanyDataException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidCompanyDataException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidCompanyDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
