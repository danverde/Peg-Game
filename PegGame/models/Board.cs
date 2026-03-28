using System.Runtime.Serialization;
using Ardalis.GuardClauses;
using Newtonsoft.Json;

namespace PegGame.models;

public class Board
{
    public IReadOnlyList<Location> Locations => _locations;
    public IReadOnlyList<Move> Moves => _moves;
    
    private readonly List<Location> _locations;
    private List<Move> _moves = [];
    
    public Board(int x, int y)
    {
        Guard.Against.OutOfRange(x, nameof(x), -4, 4);
        Guard.Against.OutOfRange(y, nameof(y), -4, 4);
        
        _locations = LoadLocations();
        Location startingLocation = GetSlot(x, y);
        RemovePeg(startingLocation);
    }

    protected Board()
    {
        _locations = LoadLocations();
    }

    #region Setup Methods

    private List<Location> LoadLocations()
    {
        // Parse JSON board
        string locationsFile = File.ReadAllText("state/locations.json");
        var locations = JsonConvert.DeserializeObject<List<Location>>(locationsFile);
        if (locations == null)
            throw new SerializationException("Error While Deserializing Locations");
        
        return locations;
    }
    
    #endregion

    public bool IsComplete() => _locations.Count(s => s.HasPeg) == 1;

    public List<Location> GetEmptySlots() => _locations.Where(l => l.HasSlot && !l.HasPeg).ToList();
    
    public Location GetSlot(int x, int y) => _locations.Single(l => l.HasSlot && l.X == x && l.Y == y);
    
    public Location? GetSlotOrDefault(int x, int y) => _locations.SingleOrDefault(l => l.HasSlot && l.X == x && l.Y == y);
    
    public List<Move> GetPossibleMovesForSlot(Location slot)
    {
        Guard.Against.Null(slot);
        if (!slot.HasSlot || slot.HasPeg)
            throw new ArgumentException($"Location does not have a slot, or has a peg: X:{slot.X}, Y:{slot.Y}", nameof(slot));
        
        // get adjacent locations with pegs
        List<Location> adjacentLocations = GetAdjacentPegs(slot);

        // Calculate moves based on slot and adjacent locations
        var moves = new List<Move>();
        foreach (Location adjacentLocation in adjacentLocations)
        {
            int xDiff = Math.Abs(slot.X - adjacentLocation.X);
            int yDiff = Math.Abs(slot.Y - adjacentLocation.Y);
            
            bool xIsDecreasing = slot.X > adjacentLocation.X;
            bool yIsDecreasing = slot.Y > adjacentLocation.Y;

            if (xIsDecreasing)
                xDiff *= -1;
            if (yIsDecreasing)
                yDiff *= -1;
            
            int x = adjacentLocation.X + xDiff;
            int y = adjacentLocation.Y + yDiff;
            
            Location? fromLocation = GetSlotOrDefault(x, y);
            
            if (fromLocation == null || !fromLocation.HasPeg)
                continue;
            
            moves.Add(new Move
            {
                To = slot,
                Over = adjacentLocation,
                From = fromLocation,
            });
        }
        
        return moves;
    }
    
    private bool SlotsAreAdjacent(Location l1, Location l2) => l1.Y == l2.Y && Math.Abs(l1.X - l2.X) == 2;
    
    private bool SlotsAreDiagonal(Location l1, Location l2) => Math.Abs(l1.X - l2.X) == 1 && Math.Abs(l1.Y - l2.Y) == 1;
    
    private List<Location> GetAdjacentPegs(Location location)
    {
        Guard.Against.Null(location);

        return _locations.Where(l =>
                l.HasPeg
                && (SlotsAreAdjacent(l, location)
                    || SlotsAreDiagonal(l, location)))
            .ToList();
    }
    
    public void MakeMove(Move move)
    {
        Guard.Against.Null(move);

        // Have to grab them again to make sure we're impacting the current board!
        Location from = GetSlot(move.From.X, move.From.Y);
        Location over = GetSlot(move.Over.X, move.Over.Y);
        Location to = GetSlot(move.To.X, move.From.Y);
        
        RemovePeg(from);
        RemovePeg(over);
        PlacePeg(to);
        
        _moves.Add(move);
    }
    
    private void RemovePeg(Location location)
    {
        Guard.Against.Null(location);
        
        if (!location.HasSlot)
            throw new Exception($"Unable to remove peg from location: X:{location.X}, Y:{location.Y}");
        
        location.HasPeg = false;
    }

    private void PlacePeg(Location location)
    {
        Guard.Against.Null(location);

        if (!location.HasSlot)
            throw new Exception($"Unable to add Peg. Location missing slot: X:{location.X}, Y:{location.Y}");
        
        location.HasPeg = true;
    }
    
    #region Utility

    public Board Clone()
    {
        string boardString = JsonConvert.SerializeObject(this);
        Board? boardClone = JsonConvert.DeserializeObject<Board>(boardString);
        if (boardClone == null)
            throw new SerializationException("Error While Cloning Board");
        
        return boardClone;
    }
    
    internal void Render()
    {
        Console.WriteLine("Board State:");
        
        for (int i = 4; i >= 0; i--)
        {
            var row = _locations
                .Where(l => l.Y == i)
                .Select(l => l.RenderChar())
                .ToList();

            if (row.Any())
                Console.WriteLine(string.Join("", row));
            
        }
        
        Console.WriteLine();
    }
    
    #endregion
    
}