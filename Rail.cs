using uwap.GameLibrary;

public class Rail : Thing
{
    public ConsoleColor? BackgroundColor => ConsoleColor.DarkGray;

    public Content? Content => new(ConsoleColor.Gray, "[]");
}