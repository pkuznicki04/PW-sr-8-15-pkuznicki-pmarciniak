//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;
using System.Windows.Input;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
  public class MainWindowViewModel : ViewModelBase, IDisposable
  {
    #region ctor

    public MainWindowViewModel() : this(null)
    { }

    internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
    {
      Balls = new ObservableCollection<ModelIBall>();
      ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));

      StartCommand = new RelayCommand(Start);
    }

    #endregion ctor

    #region public API

    private int _numberOfBalls = 5;

    public int NumberOfBalls
    {
      get =>_numberOfBalls;
      set
      {
        _numberOfBalls = value;
        if (_numberOfBalls < 5)
        {
          _numberOfBalls =5;
        }
        if (_numberOfBalls > 10)
        {
          _numberOfBalls = 10;
        }
        
        RaisePropertyChanged();
      }
    }

    public double Diameter {get; set;} = 20.0;

    public ICommand StartCommand {get; }

    public void Start()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));

      Balls.Clear();
/*
      if (NumberOfBalls < 5)
      {
        NumberOfBalls=5;
      }
      if (NumberOfBalls > 10)
      {
        NumberOfBalls = 10;
      }*/

      ModelLayer.Start(NumberOfBalls, Diameter);
      //Observer.Dispose();
    }

    public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

    #endregion public API

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!Disposed)
      {
        if (disposing)
        {
          Balls.Clear();
          Observer.Dispose();
          ModelLayer.Dispose();
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        Disposed = true;
      }
    }

    public void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    #endregion IDisposable

    #region private

    private IDisposable Observer = null;
    private ModelAbstractApi ModelLayer;
    private bool Disposed = false;

    #endregion private
  }
}