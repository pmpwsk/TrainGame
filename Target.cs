using uwap.GameLibrary;

public class Target : EnterEventThing
{
    public ConsoleColor? BackgroundColor => ConsoleColor.Green;

    public Content? Content => null;

    public bool OnEnter(Thing sender)
    {
        FlashDone(
        [
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Green,
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