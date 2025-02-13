using uwap.GameLibrary;

public class StationDetector : Thing
{
    public ConsoleColor? BackgroundColor => ConsoleColor.DarkGray;

    public Content? Content => new(ConsoleColor.DarkRed, "[]");
}