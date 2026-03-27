using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using PegGame.models;

namespace PegGame.Tests.models;

[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
public class BoardTests
{
    private readonly Board _board;

    public BoardTests()
    {
        _board = new Board(0, 0);
    }

    #region Constructor

    [Fact]
    public void Constructor_ShouldSetLocations()
    {
        // Arrange
        Board board = new BoardProctor();
        
        // Assert
        board.Locations.Should().HaveCount(45);
        board.Locations.Where(l => l.HasSlot && l.HasPeg).Count().Should().Be(15);
    }
    
    [Theory]
    [InlineData(1,1)]
    [InlineData(3,1)]
    public void Constructor_ValidCoordinates_ShouldSetInitialPeg(int x, int y)
    {
        // Arrange
        Board board = new Board(x, y);
        
        // Assert
        board.Locations.Where(l => l.HasPeg).Should().HaveCount(14);
        board.Locations.Single(l => l.X == x && l.Y == y).HasPeg.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(6)]
    [InlineData(5)]
    [InlineData(-5)]
    [InlineData(-6)]
    public void Constructor_XCoordinateOutOfRange_ShouldThrow(int x)
    {
        // Arrange
        Action act = () => new Board(x, 0);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName(nameof(x));
    }
    
    [Theory]
    [InlineData(6)]
    [InlineData(5)]
    [InlineData(-5)]
    [InlineData(-6)]
    public void Constructor_YCoordinateOutOfRange_ShouldThrow(int y)
    {
        // Arrange
        Action act = () => new Board(0, y);
        
        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName(nameof(y));
    }

    #endregion

    #region GetEmptySlots

    [Fact]
    public void GetEmptySlots_ShouldReturnEmptySlots()
    {
        // Arrange
        var expected = _board.Locations.Single(l => l.X == 0 && l.Y == 0); 
        
        // Act
        var result =  _board.GetEmptySlots();
        
        // Arrange
        result.Should().HaveCount(1);
        result.Should().Contain(expected);
    }

    #endregion
    
    #region GetLocation
    
    [Fact]
    public void GetLocation_ValidLocation_ShouldReturnALocation()
    {
        // Act
        Location result = _board.GetLocation(0, 0);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetLocation_InvalidLocation_ShouldThrow()
    {
        // Act
        Action act = () => _board.GetLocation(100, 100);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
    
    #endregion
    
    #region GetLocationOrDefault
    
    [Fact]
    public void GetLocationOrDefault_ValidLocation_ShouldReturnALocation()
    {
        // Act
        Location? result = _board.GetLocationOrDefault(0, 0);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetLocationOrDefault_InvalidLocation_ShouldReturnNull()
    {
        // Act
        Location? result = _board.GetLocationOrDefault(100, 100);

        // Assert
        result.Should().BeNull();
    }
    
    #endregion
    
    #region Clone
    
    [Fact]
    public void Clone_ShouldCloneBoard()
    {
        // Arrange
        
        // Act
        var newBoard = _board.Clone();
        newBoard.Locations.Single(l => l.X == 0 && l.Y == 0).HasPeg = true;
        
        // Assert
        newBoard.GetLocation(0, 0).HasPeg.Should().BeTrue();
        _board.GetLocation(0, 0).HasPeg.Should().BeFalse();
    }
    
    #endregion
    
}