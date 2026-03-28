using Newtonsoft.Json;

namespace PegGame.models;

public class Location
{
    public Location() {}

    [JsonConstructor]
    public Location(int x, int y, bool hasSlot, bool hasPeg = false)
    {
        X = x;
        Y = y;
        HasSlot = hasSlot;
        HasPeg = hasSlot && hasPeg;
    }
    
    public int X { get; set; }
    public int Y { get; set; }

    public bool HasPeg { get; set; } 

    public bool HasSlot { get; }

    public string RenderChar()
    {
        if (HasPeg)
            return "X";
        if (HasSlot)
            return "O";
        else return " ";
    }
}