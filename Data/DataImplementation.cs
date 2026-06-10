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
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace TP.ConcurrentProgramming.Data
{
  internal class DataImplementation : DataAbstractAPI
  {
    #region ctor
//UPŁYNNIAM RUCH ZMIENIAJĄC 100 na 16 -> 60FPS
    public DataImplementation()
    {
      stopwatch = new Stopwatch();

      loggerRunning = true;
      loggerThread = new Thread(LoggerLoop);
      loggerThread.Start();
    }

    #endregion ctor

    #region DataAbstractAPI

    public override void Start(int numberOfBalls, double Diameter, Action<IVector, IBall> upperLayerHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(DataImplementation));
      if (upperLayerHandler == null)
        throw new ArgumentNullException(nameof(upperLayerHandler));

      stopwatch.Stop();

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

      stopwatch.Start();
    }

    #endregion DataAbstractAPI

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      loggerRunning = false;

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
    public readonly Stopwatch stopwatch;

    public readonly Queue<string> logBuffer = new();
    public readonly object bufferLock = new();

    private Thread? loggerThread;
    private bool loggerRunning;

    //private bool disposedValue;
    private bool Disposed = false;

    private double Radius;

    private Random RandomGenerator = new();
    internal List<Ball> BallsList = [];

    private readonly object Lock = new object();

    private void LoggerLoop()
    {
      using StreamWriter writer =
          new($"{Directory.GetCurrentDirectory()}\\..\\..\\..\\..\\Logs\\{DateTime.Now:yyyy.MM.dd_HH-mm-ss}.log", true, Encoding.ASCII);

      while (loggerRunning)
      {
        string? line = null;

        lock (bufferLock)
        {
          while (logBuffer.Count == 0 && loggerRunning)
          {
            Monitor.Wait(bufferLock);
          }

          if (logBuffer.Count > 0)
          {
            line = logBuffer.Dequeue();
          }
        }

        if (line != null)
        {
          writer.WriteLine(line);
          writer.Flush();
        }
      }
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