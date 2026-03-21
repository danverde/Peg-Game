using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using PegGame.models;
using PegGame.state;

namespace PegGame.Tests.state;

[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
public class BoardStateTests
{
    private readonly BoardState _boardState;

    public BoardStateTests()
    {
        _boardState = new BoardState(1,1);
    }

    #region Constructor

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NegativeOrZeroX_ShouldThrow(int x)
    {
        // Arrange
        Action act = () => new BoardState(x, 1);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(x));
    } 
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NegativeOrZeroY_ShouldThrow(int y)
    {
        // Arrange
        Action act = () => new BoardState(1, y);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName(nameof(y));
    }

    [Fact]
    public void Constructor_ShouldSetLocations()
    {
        // Arrange
        BoardState board = new BoardStateProctor();
        
        // Assert
        board.Locations.Should().HaveCount(15);
        board.Locations.All(l => l.HasPeg).Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1,1)]
    [InlineData(3,1)]
    public void Constructor_ValidCoordinates_ShouldSetInitialPeg(int x, int y)
    {
        // Arrange
        BoardState board = new BoardState(x, y);
        
        // Assert
        board.Locations.Where(l => l.HasPeg).Should().HaveCount(14);
        board.Locations.Single(l => l.X == x && l.Y == y).HasPeg.Should().BeFalse();
    }
    
    [Theory]
    [InlineData(6,1)]
    [InlineData(5,2)]
    [InlineData(4,3)]
    [InlineData(3,4)]
    [InlineData(2,5)]
    [InlineData(1,6)]
    public void Constructor_CoordinatesOutOfRange_ShouldThrow(int x, int y)
    {
        // Arrange
        Action act = () => new BoardState(x, y);
        
        // Assert
        act.Should().Throw<Exception>().WithMessage($"Invalid Starting Location: X:{x}, Y:{y}");
    }

    #endregion
    
    #region GetAvailablePegs

    [Fact]
    public void GetAvailablePegs_NullLocation_ShouldThrow()
    {
        // Arrange
        Location location = null!;
        Action act = () => _boardState.GetAvailablePegs(location);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName(nameof(location));
    }
    
    [Fact]
    public void GetAvailablePegs_LocationHasPeg_ShouldThrow()
    {
        // Arrange
        Location location = new Location { X = 1, Y = 1, HasPeg = true };
        
        Action act = () => _boardState.GetAvailablePegs(location);
        
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
        List<Location> result = _boardState.GetAvailablePegs(l);
        
        // Arrange
        result.Should()
            .HaveCount(expected.Count)
            .And.BeEquivalentTo(expected);
    }
    
    #endregion
}