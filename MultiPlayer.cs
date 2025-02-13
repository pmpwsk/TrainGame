using uwap.GameLibrary;

public class MultiPlayer(Level level, int x, int y) : MovingThing(level, x, y), EnterEventThing
{
    public override ConsoleColor? BackgroundColor => _BackgroundColor;

    public override Content? Content => _Content;

    private ConsoleColor? _BackgroundColor = null;

    private Content? _Content = new(ConsoleColor.DarkBlue, "][");

    public void SetBackgroundColor(ConsoleColor? backgroundColor)
        => _BackgroundColor = backgroundColor;

    public void SetContent(Content? content)
        => _Content = content;

    public bool OnEnter(Thing sender)
    {
        FlashDone(
        [
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.Green
        ]);
        Console.ResetColor();
        Console.WriteLine();

        return true;
    }

    private void FlashDone(ConsoleColor[] colors)
    {
        foreach (ConsoleColor color in colors)
        {
            Console.CursorLeft = 0;
            Console.ForegroundColor = color;
            Console.Write("Done!");
            Thread.Sleep(500);
        }
    }
}