using Ardalis.GuardClauses;
using PegGame.models;

namespace PegGame;

public class Game
{
    public List<Move> Solve(int x, int y)
    {
        Guard.Against.OutOfRange(x, nameof(x), -5, 5);
        Guard.Against.OutOfRange(y, nameof(y), -4, 4);
        
        var moves = new List<Move>();
        var board = new Board(x, y);
        
        board.Render();
        
        List<Location> emptySlots = board.GetEmptySlots();

        // Recursively do the thing!
        foreach (Location emptySlot in emptySlots)
        {
            List<Move> possibleMoves = board.GetPossibleMovesForSlot(emptySlot);
            
            // foreach (Move possibleMove in possibleMoves)
            // {
            //     board   
            // }
        }
        
        
        return moves;
    }
    
}