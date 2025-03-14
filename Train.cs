using System.Diagnostics;
using uwap.GameLibrary;

public class Train(TrainLevel level,
    double maxSpeed, double timeToTopSpeed, double timeToStop,
    double accelerationExponent, double decelerationExponent,
    double minAcceleration, double minDeceleration,
    List<TrainPart> trainParts)
{
    /// <summary>
    /// The real width of one field on the map, in meters.
    /// </summary>
    private const double RealWidth = 2.9;

    /// <summary>
    /// The duration of one tick, in seconds.
    /// </summary>
    private const double TickDuration = 0.05;

    private TrainLevel Level = level;
    
    private List<TrainPart> TrainParts = trainParts;

    /// <summary>
    /// The current speed, in km/h.
    /// </summary>
    private double CurrentSpeed = 0;

    /// <summary>
    /// The current offset to the field borders, from -1 to +1.
    /// </summary>
    private double PositionInField = 0;

    /// <summary>
    /// 1 for forward, -1 for reverse.
    /// </summary>
    public double Direction = 1;

    /// <summary>
    /// The desired amount of acceleration, from -1 (full braking) to +1 (full acceleration).
    /// </summary>
    public double DesiredAcceleration = 0;
    
    /// <summary>
    /// The maximum speed, in km/h.
    /// </summary>
    private double MaxSpeed = maxSpeed;

    /// <summary>
    /// Time from zero to top speed under full acceleration, in seconds.
    /// </summary>
    private double TimeToTopSpeed = timeToTopSpeed;
    
    /// <summary>
    /// Time from top speed to zero under full braking, in seconds.
    /// </summary>
    private double TimeToStop = timeToStop;

    private double AccelerationExponent = accelerationExponent;
    
    private double DecelerationExponent = decelerationExponent;
    
    private double MinAcceleration = minAcceleration;
    
    private double MinDeceleration = minDeceleration;
    
    private Timer? Timer;
    
    private int NextIndex = 0;

    private double TotalOffset = 0;
    
    public void StartTimer()
        => Timer = new Timer(MovementTick, null, TimeSpan.Zero, Timeout.InfiniteTimeSpan);
    
    private void MovementTick(object? _)
    {
        //apply speed change
        switch (DesiredAcceleration)
        {
            case > 0.001:
            {
                //acceleration
                var newSpeed = CalculateNewSpeed(AccelerationExponent, MinAcceleration, TimeToTopSpeed);
                CurrentSpeed = newSpeed > MaxSpeed ? MaxSpeed : newSpeed;
                break;
            }
            case < -0.001:
            {
                //deceleration
                var newSpeed = CalculateNewSpeed(DecelerationExponent, MinDeceleration, TimeToStop);
                CurrentSpeed = newSpeed < 0 ? 0 : newSpeed;
                break;
            }
        }
        
        //apply speed to the position
        PositionInField += Direction * CurrentSpeed * (1000 / RealWidth) / (3600 / TickDuration);
        
        //move if the position has overflown
        int posInt = (int)PositionInField;
        if (posInt > 0)
        {
            //move forwards
            for (int i = 0; i < posInt; i++)
                MoveTrain(TrainParts.First(), TrainParts.Last());
        }
        else if (posInt < 0)
        {
            //move backwards
            for (int i = 0; i < -posInt; i++)
                MoveTrain(TrainParts.Last(), TrainParts.First());
        }
        
        PositionInField -= posInt;

        Global.ConsoleLock.EnterReadLock();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, 0);
        Console.Write(Global.GameTime.IsRunning
            ? $"{(Direction > 0.9 ? "D" : Direction < -0.9 ? "R" : "N")} {Math.Round(CurrentSpeed, 0, MidpointRounding.AwayFromZero)} km/h ({DesiredAcceleration * 100}%) - Next station: {Level.Stations[NextIndex].Name}"
            : "Press enter to start!");
        Level.ResetCursor();
        Global.ConsoleLock.ExitReadLock();
        
        //check if in station
        var partOnDetector = TrainParts.FirstOrDefault(part => Level.Fields[part.X, part.Y].Any(thing => thing is StationDetector));
        if (Math.Abs(CurrentSpeed) < 0.001 && partOnDetector != null)
        {
            var detector = Level.Fields[partOnDetector.X, partOnDetector.Y].OfType<StationDetector>().First();
            if (NextIndex == detector.Index)
            {
                var detectedPartIndex = TrainParts.IndexOf(partOnDetector);
                var middlePartIndex = (TrainParts.Count - 1) / 2;
                var distance = (detectedPartIndex - middlePartIndex) + PositionInField;
                TotalOffset += Math.Abs(distance);
                if (detector.IsLast)
                {
                    Console.WriteLine($"Done! Average distance was {Math.Round(TotalOffset / (NextIndex+1), 2, MidpointRounding.AwayFromZero)}m, time taken is {Math.Round(Global.GameTime.Elapsed.TotalSeconds, 1, MidpointRounding.AwayFromZero)}s");
                    Environment.Exit(0);
                }
                else NextIndex++;
            }
        }
        
        Timer?.Change(TimeSpan.FromSeconds(TickDuration), Timeout.InfiniteTimeSpan);
    }

    private void MoveTrain(TrainPart start, TrainPart end)
    {
        List<Position> positions =
        [
            new(start.X + 1, start.Y + 0),
            new(start.X + 0, start.Y + 1),
            new(start.X + -1, start.Y + 0),
            new(start.X + 0, start.Y + -1),
        ];
        var indexWithTrain = positions.FindIndex(p => IsValidRail(p) && Level.Fields[p.X, p.Y].Any(t => t is TrainPart));
        var opposite = positions[(indexWithTrain + 2) % 4];
        MoveTo(start, end, IsEmptyRail(opposite) ? opposite : positions.First(IsEmptyRail));
    }

    private void MoveTo(TrainPart start, TrainPart end, Position position)
    {
        end.MoveTo(position.X, position.Y);
        TrainParts.Remove(end);
        if (TrainParts.First() == start)
            TrainParts.Insert(0, end);
        else TrainParts.Add(end);
    }

    private bool IsEmptyRail(Position position)
    {
        if (!IsValidRail(position))
            return false;
        
        var things = Level.Fields[position.X, position.Y];
        return things.Any(t => t is Rail) && !things.Any(t => t is TrainPart);
    }

    private bool IsValidRail(Position position)
        => position.X >= 0 && position.X < Level.Width && position.Y >= 0 && position.Y < Level.Height;

    private double CalculateNewSpeed(double exponent, double minAcceleration, double fullTime)
        => CurrentSpeed + DesiredAcceleration * MaxSpeed * (TickDuration / fullTime)
            * ((1 - (1 - minAcceleration) * Math.Pow(CurrentSpeed / MaxSpeed, exponent)) / (1 - (1 - minAcceleration) / (exponent + 1)));

    public void ChangeDirection()
    {
        if (CurrentSpeed > 0.1)
            return;
        
        CurrentSpeed = 0;
        Direction *= -1;
        DesiredAcceleration = -1;
    }
}