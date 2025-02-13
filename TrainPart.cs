using uwap.GameLibrary;

public class TrainPart(Level level, int x, int y) : MovingThing(level, x, y)
{
    public override ConsoleColor? BackgroundColor => ConsoleColor.Green;

    public override Content? Content => null;
}