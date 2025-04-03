using uwap.GameLibrary;

public class Label(string textPart) : Thing
{
    public ConsoleColor? BackgroundColor => null;

    public Content? Content => new(ConsoleColor.Cyan, TextPart);

    public string TextPart = textPart;
}