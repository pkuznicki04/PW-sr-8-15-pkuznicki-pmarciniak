//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks.Dataflow;

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double thisRadius)
    {
      Position = initialPosition;
      _velocity = initialVelocity;
      Radius = thisRadius;
      owner = null;

      Mass = thisRadius * thisRadius;
    }

    internal Ball(Vector initialPosition, Vector initialVelocity, double thisRadius, DataImplementation thisOwner)
    {
      Position = initialPosition;
      _velocity = initialVelocity;
      Radius = thisRadius;
      owner = thisOwner;

      Mass = thisRadius * thisRadius;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;
//TUTAJ MAMY PRĘDKOŚĆ!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public IVector Velocity
    {
      get  => _velocity;
      set => _velocity = (Vector)value;
    }

    #endregion IBall


    internal Vector PositionInternal
    {
      get => Position;
      set
      {
        Position = value;
        RaiseNewPositionChangeNotification();
      }
    }

    internal double RadiusInternal => Radius;
    internal double MassInternal => Mass;
    internal double TimeInternal => Time;

    #region private

    //DataImplementation - stad bieremy liste
    private readonly DataImplementation owner = null;

    //Wielowatkowosc
    private Thread? ballThread;
    private bool running;
    internal readonly object BallLock = new();

    //Pozycja i predkosc
    private Vector Position;
    private Vector _velocity;

    //promien i masa
    private readonly double Radius;
    public double Mass {get; }

    //czas ostatniego wykonania
    public double Time { get; set; }

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }

    internal void Start()
    {
      running = true;

      ballThread = new Thread(() => BallLoop());
      ballThread.IsBackground = true;
      ballThread.Start();
    }

    internal void Stop() 
    {
      running = false;
    }

    private void BallLoop()
    {
      Stopwatch stopwatch;
      if (owner != null)
      {
        stopwatch = owner.stopwatch;
      }
      else
      {
        stopwatch = Stopwatch.StartNew();
      }

      double currentTime = stopwatch.Elapsed.TotalSeconds;
      double lastTime = stopwatch.Elapsed.TotalSeconds;
      double deltaTime = currentTime - lastTime;
      double moveTime = 0;

      double repetitionTime = 0.016;

      while (running)
      {
        currentTime = stopwatch.Elapsed.TotalSeconds;
        deltaTime = currentTime - lastTime;
        lastTime = currentTime;

        moveTime += deltaTime;

        while (moveTime > repetitionTime)
        {
          Move(repetitionTime);

          moveTime -= repetitionTime;
        }

        Thread.Sleep(0);
      }
    }

    internal void Move(double deltaTime)
    {
      {
        /*
      //Sprawdzamy czy lista ma niezerowa dlugosc
      if(LocalList.Count != 0)
      {
        binaryTree = new(LocalList[0]);
      }

      //Tworzymy drzewo binarne
      for (int i = 1; i < LocalList.Count; i++)
      {
        BinaryTree currentNode = binaryTree;
        while (true)
        {
          if(LocalList[i].PositionInternal.x < currentNode.Ball.PositionInternal.x)
          {
            if(currentNode.BinaryTreeLeft != null)
            {
              currentNode = currentNode.BinaryTreeLeft;
            }
            else
            {
              currentNode.BinaryTreeLeft = new(LocalList[i]);
              break;
            }
          }
          else
          {
            if (currentNode.BinaryTreeRight != null)
            {
              currentNode = currentNode.BinaryTreeRight;
            }
            else
            {
              currentNode.BinaryTreeRight = new(LocalList[i]);
              break;
            }
          }
        }
      }*/

        //Wykonujemy ruch dla wszystkich obiektow
        /*for (int i = 0; i < LocalList.Count; i++)
        {
          ResolveWallCollision(LocalList[i]);
          BinaryTree currentNode = binaryTree;
          for (; ; )
          {
            if ((LocalList[i].RadiusInternal + currentNode.Ball.RadiusInternal) < Math.Abs(currentNode.Ball.PositionInternal.x - LocalList[i].PositionInternal.x))
            {
              ResolveBallCollision(LocalList[i], currentNode.Ball);
              break;
            }
            else
            {
              if (LocalList[i].PositionInternal.x < currentNode.Ball.PositionInternal.x)
              {

              }
            }
          }
        }*/
      }

      ResolveWallCollision(deltaTime);

      if (owner != null)
      {
        var LocalList = owner.BallsList;
        foreach (Ball ball in LocalList)
        {
          if (ball != this)
          {
            ResolveBallCollision(this, ball);
          }
        }
      }

      Log();
    }

    internal void ResolveBallCollision(Ball a, Ball b)
    {
      Ball first;
      Ball second;

      if (a.GetHashCode() < b.GetHashCode())
      {
        first = a;
        second = b;
      }
      else
      {
        first = b;
        second = a;
      }

      lock (first.BallLock)
      {
        lock (second.BallLock)
        {
          double dx = a.PositionInternal.x - b.PositionInternal.x;
          double dy = a.PositionInternal.y - b.PositionInternal.y;
          double distance = Math.Sqrt(dx * dx + dy * dy);
          double minDistance = a.RadiusInternal + b.RadiusInternal;

          if (distance <= 0 || distance >= minDistance)
            return;

          double nx = dx / distance;
          double ny = dy / distance;

          double dvx = a.Velocity.x - b.Velocity.x;
          double dvy = a.Velocity.y - b.Velocity.y;

          //jesli sie oddalaja
          double velocityAlongNormal = dvx * nx + dvy * ny;
          if (velocityAlongNormal > 0)
            return;

          double impulse = (2 * velocityAlongNormal) / (a.Mass + b.Mass);

          Vector newVelocityA = new Vector(
            a.Velocity.x - impulse * b.Mass * nx,
            a.Velocity.y - impulse * b.Mass * ny);
          Vector newVelocityB = new Vector(
            b.Velocity.x + impulse * a.Mass * nx,
            b.Velocity.y + impulse * a.Mass * ny);

          a.Velocity = newVelocityA;
          b.Velocity = newVelocityB;
        }
      }
    }

    private void ResolveWallCollision(double deltaTime)
    {
      const double Width = 400;
      const double Height = 420;
      const double Speed = 100;

      Vector bordersPositionLeftTop = new Vector(0 + Radius, 0 + Radius);
      Vector bordersPositionRightBottom = new Vector(Width - Radius, Height - Radius);
      Vector bordersSize = bordersPositionRightBottom - bordersPositionLeftTop;

      lock (BallLock)
      {
        Vector vel = (Vector)Velocity;
        Vector nextPos = new Vector(Position.x - bordersPositionLeftTop.x + (Velocity.x * deltaTime * Speed), Position.y - bordersPositionLeftTop.y + (Velocity.y * deltaTime * Speed));

        if (nextPos.x <= 0)
        {
          vel.Set(-vel.x, vel.y);
        }
        if (nextPos.y <= 0)
        {
          vel.Set(vel.x, -vel.y);
        }

        nextPos.Set(Math.Abs(nextPos.x) % (2 * bordersSize.x), Math.Abs(nextPos.y) % (2 * bordersSize.y));

        if (nextPos.x > bordersSize.x)
        {
          nextPos.Set(bordersSize.x - (nextPos.x - bordersSize.x), nextPos.y);
          vel.Set(-vel.x, vel.y);
        }
        if (nextPos.y > bordersSize.y)
        {
          nextPos.Set(nextPos.x, bordersSize.y - (nextPos.y - bordersSize.y));
          vel.Set(vel.x, -vel.y);
        }

        nextPos.Add(bordersPositionLeftTop);

        Velocity = vel;
        Position = nextPos;

        RaiseNewPositionChangeNotification();
      }
    }

    private void Log()
    {
      string entry =
          $"Time: {DateTime.Now:HH:mm:ss.fff}; " +
          $"Thread: {Thread.CurrentThread.ManagedThreadId}; " + 
          $"Position: " +
          $"{Position.x.ToString("F2", CultureInfo.InvariantCulture)}, " +
          $"{Position.y.ToString("F2", CultureInfo.InvariantCulture)}; " +
          $"Velocity: " +
          $"{Velocity.x.ToString("F2", CultureInfo.InvariantCulture)}, " +
          $"{Velocity.y.ToString("F2", CultureInfo.InvariantCulture)}";

      lock (owner.bufferLock)
      {
        owner.logBuffer.Enqueue(entry);
        Monitor.Pulse(owner.bufferLock);
      }
    }

    #endregion private
  }
}