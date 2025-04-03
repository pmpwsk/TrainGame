using uwap.GameLibrary;

const int width = 61;
const int height = 61;
int viewWidth = Math.Min(width, Console.WindowWidth / 2);
int viewHeight = Math.Min(height, Console.WindowHeight - 2);

//Create level object
TrainLevel level1 = new(width, height, viewWidth, viewHeight, new List<Thing>[width, height]);
level1.CursorOffsetY = 1;

//Create fields with background
for (int y = 0; y < level1.Height; y++)
    for (int x = 0; x < level1.Width; x++)
        level1.Fields[x, y] = [new Background()];

//Create rails
for (int x = 0; x <= 30; x++)
    level1.Fields[x, 0].Add(new Rail());
for (int y = 1; y <= 60; y++)
    level1.Fields[30, y].Add(new Rail());
for (int x = 31; x <= 60; x++)
    level1.Fields[x, 60].Add(new Rail());
for (int y = 30; y <= 59; y++)
    level1.Fields[60, y].Add(new Rail());
for (int x = 0; x <= 29; x++)
    level1.Fields[x, 30].Add(new Rail());
for (int x = 31; x <= 59; x++)
    level1.Fields[x, 30].Add(new Rail());
for (int y = 1; y <= 29; y++)
    level1.Fields[0, y].Add(new Rail());

//Create train station
level1.Stations.Add(new(45, 60, 0, -1, "Berlin", 43, 58));
level1.Stations.Add(new(45, 30, 0, 1, "Munich", 43, 32));
level1.Stations.Add(new(15, 30, 0, -1, "Dresden", 13, 28));
level1.Stations.Add(new(15, 0, 0, 1, "Cologne", 13, 2));

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

    string name = station.Name;
    if (name.Length % 2 != 0)
        name = name + " ";
    
    List<string> nameParts = [""];
    foreach (char letter in name)
    {
        string lastPart = nameParts.Last();
        if (lastPart.Length < 2)
        {
            nameParts[nameParts.Count - 1] = lastPart + letter;
        }
        else
        {
            nameParts.Add(letter.ToString());
        }
    }

    int x = station.NameX;
    foreach (string namePart in nameParts)
    {
        level1.Fields[x, station.NameY].Add(new Label(namePart));
        x++;
    }
}

//Create train
List<TrainPart> train1Parts = [];
for (int x = 19; x >= 11; x--)
    train1Parts.Add(new(level1, x, 0));
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