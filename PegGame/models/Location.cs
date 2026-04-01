namespace PegGame.models;

public class Location
{
    public Location() {}

    public Location(int x, int y, bool hasPeg = false)
    {
        X = x;
        Y = y;
        HasPeg = hasPeg;
    }
    
    public int X { get; set; }
    public int Y { get; set; }
    public bool HasPeg { get; set; } 

    public string RenderChar() => HasPeg ? "X" : "O";
}