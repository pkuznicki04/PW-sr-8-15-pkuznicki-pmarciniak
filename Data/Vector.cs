//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel.DataAnnotations;

namespace TP.ConcurrentProgramming.Data
{
  /// <summary>
  ///  Two dimensions immutable vector
  /// </summary>
  internal record Vector : IVector
  {
    #region IVector

    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public double x { get; private set; }
    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public double y { get; private set; }

    public double Length => Math.Sqrt(x*x+y*y);
    public Vector Normalize()
    {
      double len = Length;
      if (len == 0)
        return new Vector(0, 0);

      return new Vector(x/len, y/len);
    }

    public void Set(double XComponent, double YComponent)
    {
      x = XComponent;
      y = YComponent;
    }

    public static Vector operator *(Vector v, Vector scalar)
    {
      return new Vector(v.x * scalar.x, v.y * scalar.y);
    }

    public static Vector operator *(double scalar, Vector v)
    {
      return new Vector(scalar*v.x, scalar*v.y);
    }

    public static Vector operator *(Vector v, double scalar)
    {
      return new Vector(v.x*scalar, v.y*scalar);
    }

    public static Vector operator -(Vector a, Vector b)
    {
      return new Vector(a.x-b.x, a.y-b.y);
    }

    public void Add(Vector other)
    {
      this.x += other.x; 
      this.y += other.y;
    }

    #endregion IVector

    /// <summary>
    /// Creates new instance of <seealso cref="Vector"/> and initialize all properties
    /// </summary>
    public Vector(double XComponent, double YComponent)
    {
      x = XComponent;
      y = YComponent;
    }
  }
}