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
      set => Position = value;
    }

    internal double RadiusInternal => Radius;
    internal double MassInternal => Mass;

    #region private

    private Vector Position;

    private Vector _velocity;
//TUTAJ DODAJE I PROMIEŃ I MASE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    private readonly double Radius;
    public double Mass {get; }

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, Position);
    }
//TUTAJ ZMIANY!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!-> ograniczenie ruchu do boarda
    internal void Move(Vector delta)
    {
      /*Position = new Vector(Position.x + delta.x, Position.y + delta.y);
      RaiseNewPositionChangeNotification();
      */

      double newX = Position.x + delta.x;
      double newY = Position.y + delta.y;

      const double Width = 400;
      const double Height = 420;

      double dx = delta.x;
      double dy = delta.y;

/*
      if (newX - Radius <= 0 || newX + Radius >= Width)
        dx = -dx;

      if (newY - Radius <= 0 || newY + Radius >= Height)
        dy = -dy;  
*/
//LEWA
      if (newX < 0)
      {
        newX = 0;
        dx = -dx;
      }
//PRAWA
      if (newX + (Radius*2) > Width)
      {
        newX = Width - (Radius*2);
        dx = -dx;
      }

//GÓRA
      if (newY < 0)
      {
        newY = 0;
        dy = -dy;
      }

//DOLNA
      if (newY + (Radius*2) > Height)
      {
        newY = Height - (Radius*2);
        dy = -dy;
      }

      _velocity = new Vector(dx, dy);

      Position = new Vector(newX, newY);
      RaiseNewPositionChangeNotification();
    }

    #endregion private
  }
}