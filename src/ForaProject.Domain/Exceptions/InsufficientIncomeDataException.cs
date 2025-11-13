namespace ForaProject.Domain.Exceptions;

/// <summary>
/// Exception thrown when a company doesn't have sufficient income data for fundable amount calculation.
/// </summary>
public sealed class InsufficientIncomeDataException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InsufficientIncomeDataException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InsufficientIncomeDataException(string message) : base(message)
    {
    }
}
