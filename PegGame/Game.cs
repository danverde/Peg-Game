using Ardalis.GuardClauses;
using PegGame.models;
using PegGame.state;

namespace PegGame;

public class Game
{
    public List<Move> Solve(int x, int y)
    {
        Guard.Against.NegativeOrZero(x);
        Guard.Against.NegativeOrZero(y);
        
        var moves = new List<Move>();
        var boardState = new BoardState(x, y);
        
        // Recursively do the thing!
        
        
        return moves;
    }

}