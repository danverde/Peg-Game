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
        var allBoards = CalculateMove(new List<Board> {board});
        
        var winningBoard = allBoards.FirstOrDefault(b => b.IsComplete());

        if (winningBoard == null)
        {
            Console.WriteLine("No winning solution found...");
            Debugger.Break();
            throw new Exception("I broke!");
        }
            
        
        return winningBoard.Moves.ToList();
    }

    private List<Board> CalculateMove(List<Board> boards, int callCount = 1)
    {
        if (callCount == 15)
            throw new Exception("Too many moves! A solution should have been found by now!");

        Console.WriteLine($"Moves calculated: {callCount}");
        callCount++;

        // Remove duplicate boards for significant performance gains
        Console.WriteLine($"Board Count: {boards.Count}");
        boards = boards.DistinctBy(b => b.GetRenderString()).ToList();
        Console.WriteLine($"Board Count: {boards.Count}");
        

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
            return CalculateMove(newBoards, callCount);
    }
    
}