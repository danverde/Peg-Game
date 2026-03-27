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
        RemovePeg(x, y);
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

    #region RemovePeg

    private void RemovePeg(int x, int y)
    {
        // Set initial empty location
        Location? location = GetLocationOrDefault(x, y);
        if (location == null)
            throw new Exception($"Specified Location does not exist: X:{x}, Y:{y}");
        
        RemovePeg(location);
    }

    private void RemovePeg(Location location)
    {
        Guard.Against.Null(location);
        
        if (!location.HasSlot)
            throw new Exception($"Unable to remove peg from location: X:{location.X}, Y:{location.Y}");
        
        location.HasPeg = false;
    }
    
    #endregion

    #region PlacePeg
    
    private void PlacePeg(Location location)
    {
        Guard.Against.Null(location);

        if (!location.HasSlot)
            throw new Exception($"Unable to add Peg. Location missing slot: X:{location.X}, Y:{location.Y}");
        
        location.HasPeg = true;
    }
    
    #endregion
    
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

        RemovePeg(move.From);
        RemovePeg(move.Over);
        PlacePeg(move.To);
        
        _moves.Add(move);
    }

    public List<Move> GetPossibleMovesForSlot(Location slot)
    {
        Guard.Against.Null(slot);
        if (!slot.HasSlot || slot.HasPeg)
            throw new ArgumentException($"Location does not have a slot, or has a peg: X:{slot.X}, Y:{slot.Y}");
        
        // get adjacent locations with pegs
        List<Location> adjacentLocations = GetAdjacentPegs(slot);

        // Calculate moves based on slot and adjacent locations
        var moves = new List<Move>();
        foreach (Location adjacentLocation in adjacentLocations)
        {
            int xDiff = slot.X - adjacentLocation.X;
            int yDiff = slot.Y - adjacentLocation.Y;

            int x = adjacentLocation.X + xDiff;
            int y = adjacentLocation.Y + yDiff;
            
            Location? fromLocation = GetLocationOrDefault(x, y);
            
            if (fromLocation == null)
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

    public List<Location> GetEmptySlots() => _locations.Where(l => l.HasSlot && !l.HasPeg).ToList();
    
    #region GetLocation

    public Location GetLocation(int x, int y) => _locations.Single(l => l.X == x && l.Y == y);
    
    public Location? GetLocationOrDefault(int x, int y) => _locations.SingleOrDefault(l => l.X == x && l.Y == y);
    
    #endregion

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
        Console.WriteLine("Rendering Board State:");
        // throw new NotImplementedException();
        
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