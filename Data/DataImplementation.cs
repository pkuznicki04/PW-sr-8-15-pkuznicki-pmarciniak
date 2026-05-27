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
    }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, double Diameter, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      lock (Lock)
      {
        List<Ball> NewList = [];

        Radius = Diameter / 2.0;
        Random random = new Random();
            
        for (int i = 0; i < numberOfBalls; i++)
        {
        //Pozycja Startowa
          Vector startingPosition = new(random.Next(100, 300), random.Next(100, 300));

          //Predkosc Startowa
          Vector velocity = new((RandomGenerator.NextDouble()-0.5)*2*1, (RandomGenerator.NextDouble() -0.5)*2*1);

          Ball newBall = new(startingPosition, velocity, Radius, this);
          upperLayerHandler(startingPosition, newBall);
          NewList.Add(newBall);
        }
        foreach (Ball ball in BallsList)
        {
          ball.Stop();
        }

        BallsList = NewList;

        foreach(Ball ball in BallsList)
        {
          ball.Start();
        }
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

    private Random RandomGenerator = new();
    internal List<Ball> BallsList = [];

    private readonly object Lock = new object();

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