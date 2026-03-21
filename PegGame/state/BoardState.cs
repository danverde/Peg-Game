using System.Runtime.Serialization;
using Ardalis.GuardClauses;
using Newtonsoft.Json;
using PegGame.models;

namespace PegGame.state;

public class BoardState
{
    public IReadOnlyList<Location> Locations => _locations;
    
    private readonly List<Location> _locations;
    
    public BoardState(int x, int y)
    {
        Guard.Against.NegativeOrZero(x);
        Guard.Against.NegativeOrZero(y);
        
        _locations = LoadLocations();
        SetInitialPeg(x, y);
    }

    protected BoardState()
    {
        _locations = LoadLocations();
    }

    #region Setup

    private List<Location> LoadLocations()
    {
        // Parse JSON board
        string locationsFile = File.ReadAllText("state/locations.json");
        var locations = JsonConvert.DeserializeObject<List<Location>>(locationsFile);
        if (locations == null)
            throw new SerializationException("Error While Deserializing Locations");
        
        return locations;
    }
    
    protected void SetInitialPeg(int x, int y)
    {
        // Set initial empty location
        var startingLocation = GetLocation(x, y);
        if (startingLocation == null)
            throw new Exception($"Invalid Starting Location: X:{x}, Y:{y}");
        
        startingLocation.HasPeg = false;
    }
    
    #endregion
    
    public List<Location> GetAvailablePegs(Location location)
    {
        Guard.Against.Null(location);
        
        if (location.HasPeg)
            throw new ArgumentException("Location has Peg");
        
        var locationsWithPegs = _locations.Where(l => l.HasPeg).ToList();

        return locationsWithPegs.Where(l =>
                l.HasPeg
                && ()
            )
            .ToList();

        // return _locations.Where(l => 
        //         l.HasPeg 
        //         && (l.X + 2 == location.X 
        //             || l.X - 2 == location.X 
        //             || l.Y + 2 == location.Y 
        //             || l.Y - 2 == location.Y)
        //     )
        //     .ToList();
    }
    
    public void MakeMove(Move move)
    {
        Guard.Against.Null(move);

        var from = GetLocation(move.From);
        var to = GetLocation(move.To);
        var jumped = GetLocation(move);
        
        if (from == null || to == null || jumped == null)
            throw new Exception("Invalid Move");
        
        from.HasPeg = false;
        to.HasPeg = true;
        jumped.HasPeg = false;
    }

    #region GetLocation

    private Location? GetLocation(int x, int y) => _locations.SingleOrDefault(l => l.X == x && l.Y == y);
    
    private Location? GetLocation(Location location) => GetLocation(location.X, location.Y);
    
    private Location? GetLocation(Move move)
    {
        // TODO verify this...
        var x = Math.Abs(move.From.X - move.To.X);
        var y = Math.Abs(move.From.Y - move.To.Y);

        return GetLocation(x, y);
    }
    
    #endregion

}