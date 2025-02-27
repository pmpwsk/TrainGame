using uwap.GameLibrary;

const int width = 700;
const int height = 380;
int viewWidth = Console.WindowWidth / 2;
int viewHeight = (Console.WindowHeight - 2);

//Create level object
TrainLevel level1 = new(width, height, viewWidth, viewHeight, new List<Thing>[width, height]);
level1.CursorOffsetY = 1;

//Create fields with background
for (int y = 0; y < level1.Height; y++)
    for (int x = 0; x < level1.Width; x++)
        level1.Fields[x, y] = [new Background()];

//Create rails
for (int x = 1; x <= 100; x++)
    level1.Fields[x, 1].Add(new Rail());
for (int y = 2; y <= 99; y++)
    level1.Fields[1, y].Add(new Rail());
for (int x = 1; x <= 100; x++)
    level1.Fields[x, 100].Add(new Rail());
for (int y = 2; y <= 99; y++)
    level1.Fields[100, y].Add(new Rail());

//Create train station
level1.Stations.Add(new(100, 50, 1, 0));
level1.Stations.Add(new(50, 100, 0, -1));
level1.Stations.Add(new(1, 50, -1, 0));
level1.Stations.Add(new(8, 1, 0, 1));

for (int i = 0; i < level1.Stations.Count; i++)
{
    var station = level1.Stations[i];
    level1.Fields[station.DetectorX, station.DetectorY].Add(new StationDetector(i, i == level1.Stations.Count - 1));
    if (station.PlatformOffsetX == 0)
    {
        for (int offsetX = -6; offsetX <= 6; offsetX++)
            level1.Fields[station.DetectorX + offsetX, station.DetectorY + station.PlatformOffsetY].Add(new Platform());
    }
    else
    {
        for (int offsetY = -6; offsetY <= 6; offsetY++)
            level1.Fields[station.DetectorX + station.PlatformOffsetX, station.DetectorY + offsetY].Add(new Platform());
    }
}

//Create train
List<TrainPart> train1Parts = [];
for (int x = 13; x >= 3; x--)
    train1Parts.Add(new(level1, x, 1));
foreach (var trainPart in train1Parts)
    level1.Fields[trainPart.X, trainPart.Y].Add(trainPart);
Train train1 = new(level1,
    200, 20, 15,
    0.5, 1,
    0.1, 0.9,
    train1Parts
);

//Set key function
level1.KeyFunction = KeyFunction;

//Start game
level1.Run(train1.StartTimer);
return;


//Define key function
bool KeyFunction(ConsoleKey key)
{
    switch (key)
    {
        case ConsoleKey.Escape:
            return true;
        case ConsoleKey.D1:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = -1;
            break;
        case ConsoleKey.D2:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = -0.75;
            break;
        case ConsoleKey.D3:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = -0.5;
            break;
        case ConsoleKey.D4:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = -0.25;
            break;
        case ConsoleKey.D5:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = 0;
            break;
        case ConsoleKey.D6:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = 0.25;
            break;
        case ConsoleKey.D7:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = 0.5;
            break;
        case ConsoleKey.D8:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = 0.75;
            break;
        case ConsoleKey.D9:
            if (Global.GameTime.IsRunning)
                train1.DesiredAcceleration = 1;
            break;
        case ConsoleKey.R:
            if (Global.GameTime.IsRunning)
                train1.ChangeDirection();
            break;
        case ConsoleKey.LeftArrow:
            level1.ScrollBy(-10, 0);
            break;
        case ConsoleKey.RightArrow:
            level1.ScrollBy(10, 0);
            break;
        case ConsoleKey.UpArrow:
            level1.ScrollBy(0, -10);
            break;
        case ConsoleKey.DownArrow:
            level1.ScrollBy(0, 10);
            break;
        case ConsoleKey.Enter:
            if (!Global.GameTime.IsRunning)
                Global.GameTime.Start();
            break;
    }

    return false;
}