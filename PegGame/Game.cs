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
        List<Board> allBoards = CalculateMoves([board]);
        
        var winningBoard = allBoards.FirstOrDefault(b => b.IsComplete());
        if (winningBoard == null)
        {
            Console.WriteLine("No winning solution found...");
            throw new Exception("I broke!");
        }
        
        return winningBoard.Moves.ToList();
    }

    private List<Board> CalculateMoves(List<Board> boards, bool removeDuplicates = true, int callCount = 1)
    {
        if (callCount == 14)
            throw new Exception("Too many moves! A solution should have been found by now!");

        Console.WriteLine($"Moves calculated: {callCount}");
        callCount++;
        Console.WriteLine($"Board Count: {boards.Count}");

        // Remove duplicate boards for significant performance gains
        if (removeDuplicates)
        {
            boards = boards.DistinctBy(b => b.GetRenderString()).ToList();
            Console.WriteLine($"Board Count: {boards.Count}");
        }
        
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
            return CalculateMoves(newBoards, removeDuplicates, callCount);
    }
    
}