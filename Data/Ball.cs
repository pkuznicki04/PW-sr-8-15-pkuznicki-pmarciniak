//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

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

    private Vector Position;

    private Vector _velocity;
//TUTAJ DODAJE I PROMIEŃ I MASE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private readonly double Radius;
    public double Mass {get; }
    public double Time { get; set; }

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }
//TUTAJ ZMIANY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!-> ograniczenie ruchu do boarda
    internal void Move(Vector delta)
    {
      Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      RaiseNewPositionChangeNotification();
    }
    #endregion private
  }
}