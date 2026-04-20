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
    }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, double Diameter, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      Radius = Diameter / 2.0;
      Random random = new Random(); 
      for (int i = 0; i < numberOfBalls; i++)
      {
        //TUTAJ ZMIANY DO PŁYNNOŚCI RUCHU!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //Vector startingPosition = new(random.Next(100, 400 - 100), random.Next(100, 400 - 100));
        Vector startingPosition = new(random.Next(100, 300), random.Next(100, 300));

        Vector velocity = new((RandomGenerator.NextDouble()-0.5)*2, (RandomGenerator.NextDouble() -0.5)*2);
        //Ball newBall = new(startingPosition, startingPosition);

        Ball newBall = new(startingPosition, velocity, Radius);
        upperLayerHandler(startingPosition, newBall);
        BallsList.Add(newBall);
      }
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
    private Random RandomGenerator = new();
    private List<Ball> BallsList = [];


//TUTAJ OKREŚLAMY RUCH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private void Move(object? x)
    {
      foreach (Ball item in BallsList)
        item.Move((Vector)item.Velocity);
      //TUTAJ ZMIENIAM BYŁO 0.5 NIE 0.1
        //item.Move(new Vector((RandomGenerator.NextDouble() - 0.5) * 10, (RandomGenerator.NextDouble() - 0.5) * 10));
        //TUTAJ DODAJE KOLIZJE
        for (int i = 0; i < BallsList.Count; i++)
      {
        for (int j = i + 1; j < BallsList.Count; j++)
        {
          ResolveBallCollision(BallsList[i], BallsList[j]);
        }
      }

        foreach (Ball item in BallsList)
      {
        ResolveWallCollision(item);
      }


    }
//ŁUBUDUBU W ŚCIANE PYK PYK
    private void ResolveBallCollision(Ball a, Ball b)
    {
      double ax = a.PositionInternal.x + a.RadiusInternal;
      double ay = a.PositionInternal.y + a.RadiusInternal;
      double bx = b.PositionInternal.x + a.RadiusInternal;
      double by = b.PositionInternal.y + b.RadiusInternal;

      double dx = bx - ax;
      double dy = by - ay;

      double distance = Math.Sqrt(dx*dx + dy*dy);
      double minDistance = a.RadiusInternal + b.RadiusInternal;

      if (distance <= 0 || distance >= minDistance)
        return;

      double nx = dx / distance;
      double ny = dy / distance;

      double overlap = minDistance - distance;

      a.PositionInternal = new Vector(a.PositionInternal.x - nx*overlap/2.0, a.PositionInternal.y + ny*overlap/2.0);
      b.PositionInternal = new Vector(b.PositionInternal.x - nx*overlap/2.0, b.PositionInternal.y + ny*overlap/2.0);

      double vax = a.Velocity.x;
      double vay = a.Velocity.y;
      double vbx = b.Velocity.x;
      double vby = b.Velocity.y;

      double vaN = vax*nx+vay*ny;
      double vbN = vbx*nx+vby*ny;

      double ma = a.Mass;
      double mb = b.Mass;

      double vaNnew = (vaN*(ma-mb)+2*mb*vbN)/(ma+mb);
      double vbNnew = (vbN*(mb-ma)+2*ma*vaN)/(ma+mb);

      a.Velocity = new Vector(vax + (vaNnew - vaN)*nx, vay + (vaNnew - vaN)*ny);
      b.Velocity = new Vector(vbx + (vbNnew - vbN)*nx, vby + (vbNnew - vbN)*ny);
    }

    private void ResolveWallCollision(Ball ball)
    {
      const double Width = 400;
      const double Height = 420;

      Vector pos = ball.PositionInternal;
      Vector vel = (Vector)ball.Velocity;

//LEWA ŚCIANA
      if(pos.x - ball.RadiusInternal < 0)
      {
        pos = new Vector(ball.RadiusInternal, pos.y);
        vel = Reflect(vel, new Vector(1, 0));
      }

// PRAWA ŚCIANA
      if (pos.x + ball.RadiusInternal > Width)
      {
        pos = new Vector(Width-ball.RadiusInternal, pos.y);
        vel = Reflect(vel, new Vector(-1,0));
      }

//GÓRNA ŚCIANA
      if (pos.y - ball.RadiusInternal < 0)
      {
        pos = new Vector(pos.x, ball.RadiusInternal);
        vel = Reflect(vel, new Vector(0,1));
      }

//DOLNA ŚCIANA
      if (pos.y + ball.RadiusInternal > Height)
      {
        pos = new Vector(pos.x, Height - ball.RadiusInternal);
        vel = Reflect(vel, new Vector(0,-1));
      }

      ball.PositionInternal = pos;
      ball.Velocity = vel;

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