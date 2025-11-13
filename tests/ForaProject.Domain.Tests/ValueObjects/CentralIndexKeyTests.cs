using FluentAssertions;
using ForaProject.Domain.ValueObjects;

namespace ForaProject.Domain.Tests.ValueObjects;

/// <summary>
/// Unit tests for CentralIndexKey value object.
/// </summary>
public class CentralIndexKeyTests
{
    [Fact]
    public void Create_WithValidCik_ShouldReturnCentralIndexKey()
    {
        // Arrange
        int validCik = 1234567;

        // Act
        var result = CentralIndexKey.Create(validCik);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(validCik);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-12345)]
    public void Create_WithInvalidCik_ShouldThrowArgumentException(int invalidCik)
    {
        // Act
        Action act = () => CentralIndexKey.Create(invalidCik);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*CIK must be a positive integer*");
    }

    [Theory]
    [InlineData(1, "0000000001")]
    [InlineData(123, "0000000123")]
    [InlineData(1234567, "0001234567")]
    [InlineData(1234567890, "1234567890")]
    public void ToFormattedString_ShouldReturn10DigitString(int cikValue, string expected)
    {
        // Arrange
        var cik = CentralIndexKey.Create(cikValue);

        // Act
        var result = cik.ToFormattedString();

        // Assert
        result.Should().Be(expected);
        result.Length.Should().Be(10);
    }

    [Fact]
    public void Equals_WithSameCikValue_ShouldReturnTrue()
    {
        // Arrange
        var cik1 = CentralIndexKey.Create(1234567);
        var cik2 = CentralIndexKey.Create(1234567);

        // Act
        var result = cik1.Equals(cik2);

        // Assert
        result.Should().BeTrue();
        (cik1 == cik2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentCikValue_ShouldReturnFalse()
    {
        // Arrange
        var cik1 = CentralIndexKey.Create(1234567);
        var cik2 = CentralIndexKey.Create(7654321);

        // Act
        var result = cik1.Equals(cik2);

        // Assert
        result.Should().BeFalse();
        (cik1 != cik2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);

        // Act
        var result = cik.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameCikValue_ShouldBeSame()
    {
        // Arrange
        var cik1 = CentralIndexKey.Create(1234567);
        var cik2 = CentralIndexKey.Create(1234567);

        // Act
        var hash1 = cik1.GetHashCode();
        var hash2 = cik2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void ToString_ShouldReturnValueAsString()
    {
        // Arrange
        var cik = CentralIndexKey.Create(1234567);

        // Act
        var result = cik.ToString();

        // Assert
        result.Should().Be("1234567");
    }
}
