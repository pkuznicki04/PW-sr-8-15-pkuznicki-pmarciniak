//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
  [TestClass]
  public class VectorUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      Random randomGenerator = new();
      double XComponent = randomGenerator.NextDouble();
      double YComponent = randomGenerator.NextDouble();
      Vector newInstance = new(XComponent, YComponent);
      Assert.AreEqual<double>(XComponent, newInstance.x);
      Assert.AreEqual<double>(YComponent, newInstance.y);
    }

    [TestMethod]
    public void Normalize_ZeroVector_ReturnsZeroVector()
    {
        Vector v = new Vector(0, 0);
        Vector n = v.Normalize();

        Assert.AreEqual(0.0, n.x);
        Assert.AreEqual(0.0, n.y);
    }

    [TestMethod]
    public void ScalarMultiplication_WorksCorrectly()
    {
        Vector v = new Vector(2, -3);

        Vector result = 2 * v;

        Assert.AreEqual(4.0, result.x);
        Assert.AreEqual(-6.0, result.y);
    }

    [TestMethod]
    public void Subtraction_WorksCorrectly()
    {
        Vector a = new Vector(5, 3);
        Vector b = new Vector(2, 1);

        Vector result = a - b;

        Assert.AreEqual(3.0, result.x);
        Assert.AreEqual(2.0, result.y);
    }

    [TestMethod]
      public void Add_WorksCorrectly()
      {
          Vector a = new Vector(1, 2);
          Vector b = new Vector(3, 4);

          a.Add(b);
          Vector result = a;

          Assert.AreEqual(4.0, result.x);
          Assert.AreEqual(6.0, result.y);
      }

      [TestMethod]
      public void Normalize_ReturnsUnitVector()
      {
          Vector v = new Vector(3, 4);
          Vector n = v.Normalize();

          Assert.AreEqual(1.0, n.Length, 1e-9);
      }

    [TestMethod]
    public void OperatorMinus_ReturnsCorrect()
    {
      Vector a = new Vector(1, 2);
      Vector b = new Vector(3, 4);

      Vector result = a - b;

      Assert.AreEqual(-2.0, result.x);
      Assert.AreEqual(-2.0, result.y);
    }
    [TestMethod]
    public void OperatorMultiply_ReturnsCorrect()
    {
      Vector a = new Vector(1, 2);
      Vector b = new Vector(3, 4);
      double scalar = 5;

      Vector result = a * b;

      Assert.AreEqual(3.0, result.x);
      Assert.AreEqual(8.0, result.y);

      result = a * scalar;

      Assert.AreEqual(5.0, result.x);
      Assert.AreEqual(10.0, result.y);

      result = scalar * a;

      Assert.AreEqual(5.0, result.x);
      Assert.AreEqual(10.0, result.y);
    }

    [TestMethod]
    public void Set_WorksCorrectly()
    {
      Vector a = new Vector(1, 2);
      double x = 10;
      double y = 5;

      a.Set(x, y);

      Assert.AreEqual(x, a.x);
      Assert.AreEqual(y, a.y);
    }
    
    [TestMethod]
    public void Length_WorksCorrectly()
    {
      Vector a = new Vector(4, 3);

      double result = a.Length;

      Assert.AreEqual(5.0, result);
    }
  }
}