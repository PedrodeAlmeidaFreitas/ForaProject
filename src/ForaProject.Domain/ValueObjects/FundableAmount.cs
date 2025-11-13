namespace ForaProject.Domain.ValueObjects;

/// <summary>
/// Represents the fundable amounts calculated for a company.
/// Immutable value object containing both standard and special fundable amounts.
/// </summary>
public sealed class FundableAmount : IEquatable<FundableAmount>
{
    /// <summary>
    /// Gets the standard fundable amount.
    /// </summary>
    public decimal StandardAmount { get; private set; }

    /// <summary>
    /// Gets the special fundable amount (with adjustments).
    /// </summary>
    public decimal SpecialAmount { get; private set; }

    /// <summary>
    /// Gets the date when the amounts were calculated.
    /// </summary>
    public DateTime CalculationDate { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FundableAmount"/> class.
    /// </summary>
    /// <param name="standardAmount">The standard fundable amount.</param>
    /// <param name="specialAmount">The special fundable amount.</param>
    private FundableAmount(decimal standardAmount, decimal specialAmount)
    {
        if (standardAmount < 0)
            throw new ArgumentException("Standard amount cannot be negative.", nameof(standardAmount));

        if (specialAmount < 0)
            throw new ArgumentException("Special amount cannot be negative.", nameof(specialAmount));

        StandardAmount = Math.Round(standardAmount, 2);
        SpecialAmount = Math.Round(specialAmount, 2);
        CalculationDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new FundableAmount instance.
    /// </summary>
    /// <param name="standardAmount">The standard fundable amount.</param>
    /// <param name="specialAmount">The special fundable amount.</param>
    /// <returns>A new FundableAmount instance.</returns>
    public static FundableAmount Create(decimal standardAmount, decimal specialAmount)
    {
        return new FundableAmount(standardAmount, specialAmount);
    }

    public bool Equals(FundableAmount? other)
    {
        if (other is null)
            return false;

        return StandardAmount == other.StandardAmount &&
               SpecialAmount == other.SpecialAmount;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FundableAmount);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StandardAmount, SpecialAmount);
    }

    public override string ToString()
    {
        return $"Standard: ${StandardAmount:N2}, Special: ${SpecialAmount:N2}";
    }

    public static bool operator ==(FundableAmount? left, FundableAmount? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(FundableAmount? left, FundableAmount? right)
    {
        return !(left == right);
    }
}
