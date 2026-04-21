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
  public class DataImplementationUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        IEnumerable<IBall>? ballsList = null;
        newInstance.CheckBallsList(x => ballsList = x);
        Assert.IsNotNull(ballsList);
        int numberOfBalls = 0;
        newInstance.CheckNumberOfBalls(x => numberOfBalls = x);
        Assert.AreEqual<int>(0, numberOfBalls);
      }
    }

    [TestMethod]
    public void DisposeTestMethod()
    {
      DataImplementation newInstance = new DataImplementation();
      bool newInstanceDisposed = false;
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsFalse(newInstanceDisposed);
      newInstance.Dispose();
      newInstance.CheckObjectDisposed(x => newInstanceDisposed = x);
      Assert.IsTrue(newInstanceDisposed);
      IEnumerable<IBall>? ballsList = null;
      newInstance.CheckBallsList(x => ballsList = x);
      Assert.IsNotNull(ballsList);
      newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(0, x));
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
      Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Start(0, 20.0, (position, ball) => { }));
    }

    [TestMethod]
    public void StartTestMethod()
    {
      using (DataImplementation newInstance = new DataImplementation())
      {
        int numberOfCallbackInvoked = 0;
        int numberOfBalls2Create = 10;
        double Diameter = 20.0;
        newInstance.Start(
          numberOfBalls2Create,
          Diameter,
          (startingPosition, ball) =>
          {
            numberOfCallbackInvoked++;
            Assert.IsTrue(startingPosition.x >= 0);
            Assert.IsTrue(startingPosition.y >= 0);
            Assert.IsNotNull(ball);
          });
        Assert.AreEqual<int>(numberOfBalls2Create, numberOfCallbackInvoked);
        newInstance.CheckNumberOfBalls(x => Assert.AreEqual<int>(10, x));
      }
    }

    [TestMethod]
      public void BallPositionChangesOverTime()
      {
          using var data = new DataImplementation();

          IVector? first = null;
          IVector? second = null;

          data.Start(1, 20.0, (pos, ball) =>
          {
              ball.NewPositionNotification += (_, p) =>
              {
                  if (first == null)
                      first = p;
                  else
                      second = p;
              };
          });

          Thread.Sleep(50);

          Assert.IsNotNull(first);
          Assert.IsNotNull(second);
          Assert.AreNotEqual(first!.x, second!.x);
      }

[TestMethod]
        public void BallStaysInsideBoard()
        {
            using var data = new DataImplementation();

            IVector? last = null;

            data.Start(1, 20.0, (pos, ball) =>
            {
                ball.NewPositionNotification += (_, p) => last = p;
            });

            Thread.Sleep(200);

            Assert.IsNotNull(last);
            Assert.IsTrue(last!.x >= 0 && last.x <= 400);
            Assert.IsTrue(last.y >= 0 && last.y <= 420);
        }
  }
}