using FluentAssertions;
using PegGame.models;

namespace PegGame.Tests.models;

public class LocationTests
{
    private readonly Location _Location;
    
    public LocationTests()
    {
        _Location = new Location();
    }

    #region Constructor

    [Fact]
    public void Constructor_ValidParams_ShouldCreateInstance()
    {
        // Arrange
        const int x = 0;
        const int y = 1;
        const bool hasPeg = true;
        
        // Act
        var result = new Location(x, y, hasPeg);
        
        // Assert
        result.X.Should().Be(x);
        result.Y.Should().Be(y);
        result.HasPeg.Should().Be(hasPeg);
    }
    
    #endregion
}