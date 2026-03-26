using Ardalis.GuardClauses;
using PegGame.models;
using PegGame.state;

namespace PegGame;

public class Game
{
    public List<Move> Solve(int x, int y)
    {
        // TODO input validation
        Guard.Against.OutOfRange(x, nameof(x), -5, 5);
        Guard.Against.OutOfRange(y, nameof(y), -4, 4);
        
        var moves = new List<Move>();
        var boardState = new BoardState(x, y);
        
        boardState.RenderState();
        
        // Recursively do the thing!
        
        
        return moves;
    }

}