using uwap.GameLibrary;

public class TrainLevel(int width, int height, int viewWidth, int viewHeight, List<Thing>[,] fields)
    : Level(width, height, viewWidth, viewHeight, fields)
{
    public List<Station> Stations = [];
}