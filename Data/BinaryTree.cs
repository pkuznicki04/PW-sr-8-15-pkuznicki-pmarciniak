using System;
using System.Collections.Generic;
using System.Text;

namespace TP.ConcurrentProgramming.Data
{
  internal class BinaryTree : IBinaryTree
  {
    #region ctor

    internal BinaryTree(Ball ball)
    {
      _ball = ball;
    }

    #endregion ctor

    #region IBinaryTree

    public Ball Ball
    {
      get => _ball;
      set => _ball = value;
    }

    public BinaryTree BinaryTreeLeft
    {
      get => _binaryTreeLeft;
      set => _binaryTreeLeft = value;
    }

    public BinaryTree BinaryTreeRight
    {
      get => _binaryTreeRight;
      set => _binaryTreeRight = value;
    }

    #endregion IBinaryTree

    #region private

    private Ball _ball;
    private BinaryTree? _binaryTreeLeft;
    private BinaryTree? _binaryTreeRight;

    #endregion private
  }
}
