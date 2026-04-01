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
        int locationCount = _board.Locations.Count;
        int pegCount = _board.Locations.Count(l => l.HasPeg);
        
        // Assert
        locationCount.Should().Be(15);
        pegCount.Should().Be(14);
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

    // TODO IsComplete
    
    // TODO MakeMove
    
    #region GetPossibleMovesForLocation

    [Fact]
    public void GetPossibleMovesForLocation_ValidLocation_ShouldReturnAllPossibleMoves()
    {
        // Arrange
        
        // testing a spot other than 0,0
        Location location = _board.GetLocation(2, 2);
        location.HasPeg = false;
        
        _board.GetLocation(0, 0).HasPeg = true; 
        _board.GetLocation(4, 0).HasPeg = false; // excludes moves without a valid From location
        _board.GetLocation(1, 1).HasPeg = false; // excludes moves without a valid To location

        var expected = new List<Move>
        {
            new() {To = location, From = _board.GetLocation(0, 4), Over = _board.GetLocation(1, 3)},
            new() {To = location, From = _board.GetLocation(-2, 2), Over = _board.GetLocation(0, 2)},
        };
        
        // Act
        List<Move> result = _board.GetPossibleMovesForLocation(location);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetPossibleMovesForLocation_NullLocation_ShouldThrow()
    {
        // Arrange/Act
        Action act = () => _board.GetPossibleMovesForLocation(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("location");
    }

    [Fact]
    public void GetPossibleMovesForLocation_LocationHasPeg_ShouldThrow()
    {
        // Arrange
        var location = new Location(0, 0, true);
        
        // Act
        Action act = () => _board.GetPossibleMovesForLocation(location);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("location");
    }
    
    #endregion
    
    #region GetEmptyLocations

    [Fact]
    public void GetEmptyLocations_ShouldReturn()
    {
        // Arrange
        var expected = _board.Locations.Single(l => l.X == 0 && l.Y == 0); 
        
        // Act
        var result =  _board.GetEmptyLocations();
        
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
    public void GetLocation_LocationDoesNotExist_ShouldThrow()
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
    public void GetLocationOrDefault_LocationDoesNotExist_ShouldReturnNull()
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