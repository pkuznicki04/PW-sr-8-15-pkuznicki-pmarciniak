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
  public abstract class DataAbstractAPI : IDisposable
  {
    #region Layer Factory

    public static DataAbstractAPI GetDataLayer()
    {
      return modelInstance.Value;
    }

    #endregion Layer Factory

    #region public API
//TUTAJ DODAŁEM ŚREDNICE DO PODAWANYCH DANYCH!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public abstract void Start(int numberOfBalls, double Diameter, Action<IVector, IBall> upperLayerHandler);

    #endregion public API

    #region IDisposable

    public abstract void Dispose();

    #endregion IDisposable

    #region private

    private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

    #endregion private
  }

  public interface IVector
  {
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    double x { get;}

    /// <summary>
    /// The y component of the vector.
    /// </summary>
    double y { get;}
  }

  public interface IBall
  {
    event EventHandler<IVector> NewPositionNotification;

    IVector Velocity { get; set; }
  }

  public interface IBinaryTree
  {
    internal Ball Ball { get; set; }

    internal BinaryTree BinaryTreeLeft {  get; set; }
    internal BinaryTree BinaryTreeRight {  get; set; }
  }
}