using Newtonsoft.Json;

namespace PegGame.models;

public class Location
{
    public Location() {}

    [JsonConstructor]
    public Location(int x, int y, bool hasPeg, bool hasSlot)
    {
        X = x;
        Y = y;
        HasSlot = hasSlot;
        HasPeg = hasSlot && hasPeg;
    }
    
    public int X { get; set; }
    public int Y { get; set; }

    public bool HasPeg { get; set; } 

    public bool HasSlot { get; set; }

    public string RenderChar()
    {
        if (HasPeg)
            return "X";
        if (HasSlot)
            return "O";
        else return " ";
    }
}