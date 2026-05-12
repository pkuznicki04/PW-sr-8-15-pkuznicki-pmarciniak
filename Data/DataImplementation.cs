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

    private readonly object Lock = new object();


//TUTAJ OKREŚLAMY RUCH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private void Move(object? x)
    {
      //foreach (Ball item in BallsList.ToList())
        //item.Move((Vector)item.Velocity);
      //TUTAJ ZMIENIAM BYŁO 0.5 NIE 0.1
        //item.Move(new Vector((RandomGenerator.NextDouble() - 0.5) * 10, (RandomGenerator.NextDouble() - 0.5) * 10));
        //TUTAJ DODAJE KOLIZJE

      var LocalList = BallsList;
      
      for (int i = 0; i < LocalList.Count; i++)
      {
        ResolveWallCollision(LocalList[i]);
        for (int j = i + 1; j < LocalList.Count; j++)
        {
          ResolveBallCollision(LocalList[i], LocalList[j]);
        }
      }

      /*foreach (Ball ball in LocalList)
      {
        ResolveWallCollision(ball);
        //item.Move();
        foreach (Ball collisionBall in LocalList)
        {

        }
      }*/
    }
//ŁUBUDUBU W ŚCIANE PYK PYK
    private void ResolveBallCollision(Ball a, Ball b)
    {
      /*double ax = a.PositionInternal.x;
      double ay = a.PositionInternal.y;
      double bx = b.PositionInternal.x;
      double by = b.PositionInternal.y;

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
      b.Velocity = new Vector(vbx + (vbNnew - vbN)*nx, vby + (vbNnew - vbN)*ny);*/

      
      Vector pa = a.PositionInternal;
      Vector pb = b.PositionInternal;

      Vector va = (Vector)a.Velocity;
      Vector vb = (Vector)b.Velocity;

      double dx = pb.x - pa.x;
      double dy = pb.y - pa.y;

      double distance = Math.Sqrt(dx * dx + dy * dy);
      double minDist = a.RadiusInternal + b.RadiusInternal;

      if (distance <= 0 || distance >= minDist)
          return;

      double nx = dx / distance;
      double ny = dy / distance;

      double rv =
          (vb.x - va.x) * nx +
          (vb.y - va.y) * ny;

      if (rv > 0)
          return;

      
      double penetration = minDist - distance;
      double correction = penetration / 2.0;

      a.PositionInternal = new Vector(
          pa.x - correction * nx,
          pa.y - correction * ny
      );

      b.PositionInternal = new Vector(
          pb.x + correction * nx,
          pb.y + correction * ny
      );

      
      double restitution = 0.9; 

      double j = -(1 + restitution) * rv;
      j /= (1 / a.Mass + 1 / b.Mass);

      Vector impulse = new Vector(j * nx, j * ny);

      a.Velocity = new Vector(
          va.x - impulse.x / a.Mass,
          va.y - impulse.y / a.Mass
      );

      b.Velocity = new Vector(
          vb.x + impulse.x / b.Mass,
          vb.y + impulse.y / b.Mass
      );


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