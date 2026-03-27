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
    
    #region GetAvailablePegs

    [Fact]
    public void GetAvailablePegs_NullLocation_ShouldThrow()
    {
        // Arrange
        Location location = null!;
        Action act = () => _board.GetAvailablePegs(location);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName(nameof(location));
    }
    
    [Fact]
    public void GetAvailablePegs_LocationHasPeg_ShouldThrow()
    {
        // Arrange
        Location location = new Location { X = 1, Y = 1, HasPeg = true };
        
        Action act = () => _board.GetAvailablePegs(location);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Location has Peg");
    }
    

    [Fact]
    public void GetAvailablePegs_ValidLocation_AllLocationsHavePegs_ShouldReturnLocations()
    {
        // Arrange
        Location l = new Location { X = 3, Y = 1, HasPeg = false };
        
        List<Location> expected =
        [
            new() {X = 1, Y = 1, HasPeg = true},
            new() {X = 5, Y = 1, HasPeg = true},
            new() {X = 1, Y = 3, HasPeg = true},
            new() {X = 3, Y = 3, HasPeg = true}
        ];
        
        // Act
        List<Location> result = _board.GetAvailablePegs(l);
        
        // Arrange
        result.Should()
            .HaveCount(expected.Count)
            .And.BeEquivalentTo(expected);
    }
    
    #endregion
}