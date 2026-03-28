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
        locationCount.Should().Be(45);
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
    
    #region GetPossibleMovesForSlot

    [Fact]
    public void GetPossibleMovesForSlot_ValidSlot_ShouldReturnAllPossibleMoves()
    {
        // Arrange
        
        // testing a spot other than 0,0
        Location slot = _board.GetSlot(2, 2);
        slot.HasPeg = false;
        
        _board.GetSlot(0, 0).HasPeg = true; 
        _board.GetSlot(4, 0).HasPeg = false; // excludes moves without a valid From location
        _board.GetSlot(1, 1).HasPeg = false; // excludes moves without a valid To location

        var expected = new List<Move>
        {
            new() {To = slot, From = _board.GetSlot(0, 4), Over = _board.GetSlot(1, 3)},
            new() {To = slot, From = _board.GetSlot(-2, 2), Over = _board.GetSlot(0, 2)},
        };
        
        // Act
        List<Move> result = _board.GetPossibleMovesForSlot(slot);
        
        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void GetPossibleMovesForSlot_NullLocation_ShouldThrow()
    {
        // Arrange/Act
        Action act = () => _board.GetPossibleMovesForSlot(null!);
        
        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("slot");
    }

    [Fact]
    public void GetPossibleMovesForSlot_LocationHasNoSlot_ShouldThrow()
    {
        // Arrange
        var location = new Location(0, 0, false);
        
        // Act
        Action act = () => _board.GetPossibleMovesForSlot(location);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("slot");
    }
    
    [Fact]
    public void GetPossibleMovesForSlot_LocationHasPeg_ShouldThrow()
    {
        // Arrange
        var location = new Location(0, 0, true, true);
        
        // Act
        Action act = () => _board.GetPossibleMovesForSlot(location);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("slot");
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
    
    #region GetSlot
    
    [Fact]
    public void GetSlot_ValidLocation_ShouldReturnALocation()
    {
        // Act
        Location result = _board.GetSlot(0, 0);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetSlot_LocationDoesNotExist_ShouldThrow()
    {
        // Act
        Action act = () => _board.GetSlot(100, 100);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetSlot_LocationDoesNotHaveSlot_ShouldThrow()
    {
        // Act
        Action act = () => _board.GetSlot(1, 0);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
    
    #endregion
    
    #region GetSlotOrDefault
    
    [Fact]
    public void GetSlotOrDefault_ValidLocation_ShouldReturnALocation()
    {
        // Act
        Location? result = _board.GetSlotOrDefault(0, 0);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void GetSlotOrDefault_LocationDoesNotExist_ShouldReturnNull()
    {
        // Act
        Location? result = _board.GetSlotOrDefault(100, 100);

        // Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public void GetSlotOrDefault_LocationDoesNotHaveSlot_ShouldReturnNull()
    {
        // Act
        Location? result = _board.GetSlotOrDefault(1, 0);

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
        newBoard.GetSlot(0, 0).HasPeg.Should().BeTrue();
        _board.GetSlot(0, 0).HasPeg.Should().BeFalse();
    }
    
    #endregion
    
}