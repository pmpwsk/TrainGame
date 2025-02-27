using uwap.GameLibrary;

public class StationDetector(int index, bool isLast = false) : Thing
{
    public ConsoleColor? BackgroundColor => ConsoleColor.DarkGray;

    public Content? Content => new(ConsoleColor.DarkRed, "[]");

    public int Index = index;
    
    public bool IsLast = isLast;
}