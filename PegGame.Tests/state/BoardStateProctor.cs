using PegGame.state;

namespace PegGame.Tests.state;

public class BoardStateProctor : BoardState
{
    public new void SetInitialPeg(int x, int y) => base.SetInitialPeg(x, y);
}