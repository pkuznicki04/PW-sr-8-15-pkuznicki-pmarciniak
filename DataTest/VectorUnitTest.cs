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

          Vector result = a.Add(b);

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
  }

      

  
}