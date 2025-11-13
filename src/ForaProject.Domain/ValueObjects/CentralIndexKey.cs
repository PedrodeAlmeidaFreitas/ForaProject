namespace ForaProject.Domain.ValueObjects;

/// <summary>
/// Represents a SEC Central Index Key (CIK).
/// Value object that encapsulates CIK validation and formatting.
/// </summary>
public sealed class CentralIndexKey : IEquatable<CentralIndexKey>
{
    /// <summary>
    /// Gets the CIK value.
    /// </summary>
    public int Value { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CentralIndexKey"/> class.
    /// </summary>
    /// <param name="value">The CIK value.</param>
    /// <exception cref="ArgumentException">Thrown when CIK is invalid.</exception>
    private CentralIndexKey(int value)
    {
        if (value <= 0)
            throw new ArgumentException("CIK must be a positive integer.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Creates a new CentralIndexKey instance.
    /// </summary>
    /// <param name="value">The CIK value.</param>
    /// <returns>A new CentralIndexKey instance.</returns>
    public static CentralIndexKey Create(int value)
    {
        return new CentralIndexKey(value);
    }

    /// <summary>
    /// Formats the CIK as a 10-digit string with leading zeros.
    /// </summary>
    /// <returns>Formatted CIK string.</returns>
    public string ToFormattedString()
    {
        return Value.ToString("D10");
    }

    public bool Equals(CentralIndexKey? other)
    {
        if (other is null)
            return false;

        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CentralIndexKey);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(CentralIndexKey? left, CentralIndexKey? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(CentralIndexKey? left, CentralIndexKey? right)
    {
        return !(left == right);
    }
}
