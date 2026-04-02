using System.Runtime.Serialization;
using Ardalis.GuardClauses;
using Newtonsoft.Json;

namespace PegGame.models;

public class Board
{
    public IReadOnlyList<Location> Locations => _locations;
    public IReadOnlyList<Move> Moves => _moves;
    
    private readonly List<Location> _locations = new ();
    private List<Move> _moves = [];
    
    public Board() { }
    
    public Board(int x, int y)
    {
        Guard.Against.OutOfRange(x, nameof(x), -4, 4);
        Guard.Against.OutOfRange(y, nameof(y), -4, 4);
        
        _locations = LoadLocations();
        Location startingLocation = GetLocation(x, y);
        RemovePeg(startingLocation);
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

    public List<Location> GetEmptyLocations() => _locations.Where(l => !l.HasPeg).ToList();
    
    public Location GetLocation(int x, int y) => _locations.Single(l => l.X == x && l.Y == y);
    
    public Location? GetLocationOrDefault(int x, int y) => _locations.SingleOrDefault(l => l.X == x && l.Y == y);
    
    public List<Move> GetPossibleMovesForLocation(Location location)
    {
        Guard.Against.Null(location);
        if (location.HasPeg)
            throw new ArgumentException($"Location has a peg: X:{location.X}, Y:{location.Y}", nameof(location));
        
        // get adjacent locations with pegs
        List<Location> adjacentLocations = GetAdjacentPegs(location);

        // Calculate moves based on location and adjacent locations
        var moves = new List<Move>();
        foreach (Location adjacentLocation in adjacentLocations)
        {
            int xDiff = Math.Abs(location.X - adjacentLocation.X);
            int yDiff = Math.Abs(location.Y - adjacentLocation.Y);
            
            bool xIsDecreasing = location.X > adjacentLocation.X;
            bool yIsDecreasing = location.Y > adjacentLocation.Y;

            if (xIsDecreasing)
                xDiff *= -1;
            if (yIsDecreasing)
                yDiff *= -1;
            
            int x = adjacentLocation.X + xDiff;
            int y = adjacentLocation.Y + yDiff;
            
            Location? fromLocation = GetLocationOrDefault(x, y);
            
            if (fromLocation == null || !fromLocation.HasPeg)
                continue;
            
            moves.Add(new Move
            {
                To = location,
                Over = adjacentLocation,
                From = fromLocation,
            });
        }
        
        return moves;
    }
    
    private bool LocationsAreAdjacent(Location l1, Location l2) => l1.Y == l2.Y && Math.Abs(l1.X - l2.X) == 2;
    
    private bool LocationsAreDiagonal(Location l1, Location l2) => Math.Abs(l1.X - l2.X) == 1 && Math.Abs(l1.Y - l2.Y) == 1;
    
    private List<Location> GetAdjacentPegs(Location location)
    {
        Guard.Against.Null(location);

        return _locations.Where(l =>
                l.HasPeg
                && (LocationsAreAdjacent(l, location)
                    || LocationsAreDiagonal(l, location)))
            .ToList();
    }
    
    public void MakeMove(Move move)
    {
        Guard.Against.Null(move);

        // Have to grab them again to make sure we're impacting the current board!
        Location from = GetLocation(move.From.X, move.From.Y);
        Location over = GetLocation(move.Over.X, move.Over.Y);
        Location to = GetLocation(move.To.X, move.To.Y);
        
        RemovePeg(from);
        RemovePeg(over);
        PlacePeg(to);
        
        _moves.Add(move);
    }
    
    private void RemovePeg(Location location)
    {
        Guard.Against.Null(location);
        
        if (!location.HasPeg)
            throw new Exception($"Cannot remove peg, location does not have peg: X:{location.X}, Y:{location.Y}");
        
        location.HasPeg = false;
    }

    private void PlacePeg(Location location)
    {
        Guard.Against.Null(location);

        if (location.HasPeg)
            throw new Exception($"Location already has peg. Unable to place PegA: X:{location.X}, Y:{location.Y}");
        
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
        
        // Y
        for (int y = 4; y >= 0; y--)
        {
            var row = "";
            for (int x = -4; x <= 4; x++)
            {
                Location? l = GetLocationOrDefault(x, y);
                row += Location.RenderChar(l);
            }
            
            Console.WriteLine(row);
        }
        
        Console.WriteLine();
    }

    #endregion
}