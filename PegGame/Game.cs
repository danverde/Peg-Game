using System.Diagnostics;
using Ardalis.GuardClauses;
using PegGame.models;

namespace PegGame;

public class Game
{
    public List<Move> Solve(int x, int y)
    {
        Guard.Against.OutOfRange(x, nameof(x), -4, 4);
        Guard.Against.OutOfRange(y, nameof(y), -4, 4);
        
        var board = new Board(x, y);
        board.Render();
        
        // Recursively do the thing!
        var allBoards = DoTheThing(new List<Board> {board});
        
        var winningBoard = allBoards.FirstOrDefault(b => b.IsComplete());

        if (winningBoard == null)
        {
            Console.WriteLine("No winning solution found");
            Debugger.Break();
            throw new Exception("I broke");
        }
            
        
        return winningBoard.Moves.ToList();
    }

    // private List<Board> DoTheThing(Board board, List<Board> allBoards)
    private List<Board> DoTheThing(List<Board> boards, int callCount = 0)
    {
        if (callCount == 20)
            throw new Exception("infinite loop detected");

        Console.WriteLine($"Call Count: {callCount}");
        callCount++;

        var newBoards = new List<Board>();
        
        foreach (var board in boards)
        {
            List<Location> emptySlots = board.GetEmptyLocations();

            foreach (Location emptySlot in emptySlots)
            {
                List<Move> possibleMoves = board.GetPossibleMovesForLocation(emptySlot);
                
                foreach (Move possibleMove in possibleMoves)
                {
                    var newBoard = board.Clone();
                    newBoard.MakeMove(possibleMove);
                    // newBoard.Render();
                    newBoards.Add(newBoard);
                }
            }
        }

        if (newBoards.Any(b => b.IsComplete()))
            return boards;
        else
            return DoTheThing(newBoards, callCount);
    }
    
}