//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor
//UPŁYNNIAM RUCH ZMIENIAJĄC 100 na 16 -> 60FPS
    public DataImplementation()
    {
      MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
      stopwatch = new Stopwatch();
    }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, double Diameter, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      //TMP -- Zmienić na poprawne Start / Stop dla timera
      MoveTimer.Change(Timeout.Infinite, Timeout.Infinite);
      stopwatch.Stop();

      lock (Lock)
      {
        List<Ball> NewList = [];

        Radius = Diameter / 2.0;
        Random random = new Random();
            
        for (int i = 0; i < numberOfBalls; i++)
        {
        //TUTAJ ZMIANY DO PŁYNNOŚCI RUCHU!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
          Vector startingPosition = new(random.Next(100, 300), random.Next(100, 300));

          Vector velocity = new((RandomGenerator.NextDouble()-0.5)*2*1, (RandomGenerator.NextDouble() -0.5)*2*1);
        //Ball newBall = new(startingPosition, startingPosition);

          Ball newBall = new(startingPosition, velocity, Radius);
          upperLayerHandler(startingPosition, newBall);
          NewList.Add(newBall);
        }
        BallsList = NewList;
      }
      MoveTimer.Change(0, 16);
      stopwatch.Start();
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          MoveTimer.Dispose();
          BallsList.Clear();
        }
        Disposed = true;
      }
      else
        throw new ObjectDisposedException(nameof(DataImplementation));
    }

    public override void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    //private bool disposedValue;
    private bool Disposed = false;

    private double Radius;

    private readonly Timer MoveTimer;
    private readonly Stopwatch stopwatch;
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];
    private BinaryTree binaryTree;

    private readonly object Lock = new object();


//TUTAJ OKREŚLAMY RUCH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private void Move(object? x)
    {
      var LocalList = BallsList;

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
      for (int i = 0; i < LocalList.Count; i++)
      {
        ResolveWallCollision(LocalList[i]);
        for (int j = i+1; j < LocalList.Count; j++)
        {
          ResolveBallCollision(LocalList[i], LocalList[j]);
        }
      }
    }

    private void ResolveBallCollision(Ball a, Ball b)
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

    private void ResolveWallCollision(Ball ball)
    {
      const double Width = 400;
      const double Height = 420;

      double Time = stopwatch.Elapsed.TotalSeconds;
      double deltaTime = Time - ball.Time;

      Vector bordersPositionLeftTop = new Vector( 0 + ball.RadiusInternal, 0 + ball.RadiusInternal);
      Vector bordersPositionRightBottom = new Vector( Width - ball.RadiusInternal, Height - ball.RadiusInternal);
      Vector bordersSize = bordersPositionRightBottom - bordersPositionLeftTop;

      Vector pos = ball.PositionInternal;
      Vector vel = (Vector)ball.Velocity;

      Vector nextPos = new Vector(pos.x - bordersPositionLeftTop.x + (vel.x * deltaTime * 100), pos.y - bordersPositionLeftTop.y + (vel.y * deltaTime * 100));
      
      if(nextPos.x <= 0)
      {
        vel.Set(-vel.x, vel.y);
      }
      if(nextPos.y <= 0)
      {
        vel.Set(vel.x, -vel.y);
      }

      nextPos.Set(Math.Abs(nextPos.x) % (2 * bordersSize.x), Math.Abs(nextPos.y) % (2 * bordersSize.y));

      if(nextPos.x > bordersSize.x)
      {
        nextPos.Set(bordersSize.x - (nextPos.x - bordersSize.x), nextPos.y);
        vel.Set(-vel.x, vel.y);
      }
      if(nextPos.y > bordersSize.y)
      {
        nextPos.Set(nextPos.x, bordersSize.y - (nextPos.y - bordersSize.y));
        vel.Set(vel.x, -vel.y);
      }

      nextPos.Add(bordersPositionLeftTop);

      ball.Velocity = vel;
      ball.Time = Time;
      ball.Move(nextPos - pos);
    }

    private static Vector Reflect(Vector v, Vector n)
{
    n = n.Normalize();
    return v - 2 * (v.x * n.x + v.y * n.y) * n;
}

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
    {
      returnBallsList(BallsList);
    }

    [Conditional("DEBUG")]
    internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
    {
      returnNumberOfBalls(BallsList.Count);
    }

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    #endregion TestingInfrastructure
  }
}