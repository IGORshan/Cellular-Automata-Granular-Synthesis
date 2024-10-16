using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using CellularAutomataUI.Helpers;
using CellularAutomataUI.Service;
using CellularAutomataUI.ViewModels;
using ReactiveUI;

namespace CellularAutomataUI.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private MainWindowViewModel _viewModel;
    private DispatcherTimer _timer;
    private double _speedInSec = 750;
    

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel(); //It creates a new instance of MainWindowViewModel(), this links the ViewModel to the View.
        DataContext = _viewModel; //By setting the DataContext, it is telling Avalonia that the data for this window should come from MainWindowViewModel.cs
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(_speedInSec) // Adjust the interval as needed
        };
        OSCService.SendSpeedMessage((int)_speedInSec);
        DrawInitialField();

    }
    
    //call the DrawField function to initialize the field.
    private void DrawInitialField()
    {
        var canvas = this.FindControl<Canvas>("AutomatonCanvas");
        _viewModel.DrawCells(canvas);
    }
    
    //call the function of iterating Cellular Automaton
    public void StartIterationCa(object sender, RoutedEventArgs e)
    {
        var canvas = this.FindControl<Canvas>("AutomatonCanvas");
        _timer.Start();
        _timer.Tick += (sender, e) => _viewModel.IteratingCa(canvas);
        OSCService.SendStartMessage();
    }

    public void PauseIterationCa(object sender, RoutedEventArgs e)
    {
        var canvas = this.FindControl<Canvas>("AutomatonCanvas");
        _timer.Stop();
        OSCService.SendPauseMessage();
    }

    private void ConfirmAllParamChanges(object sender, RoutedEventArgs e)
    { 
        PauseIterationCa(sender, e);
        // Retrieve the values from the NumericUpDown controls
        var value1 = NewGridSize.Value;
        _viewModel.SetGridSize((int)value1);
        
        var value2 = NewIterationSpeed.Value;
        _speedInSec = (int)value2;
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(_speedInSec) // Adjust the interval as needed
        };
        OSCService.SendSpeedMessage((int)_speedInSec);

        var value3 = GrainDurLo.Value;
        var value4 = GrainDurHi.Value;
        OSCService.SendGrainDurationLowMessage((int)value3);
        OSCService.SendGrainDurationUpperMessage((int)value4);

        _viewModel.SelectingLegacyRule();
        
        var value5 = RuleSetA.Value;
        var value6 = RuleSetB.Value;
        _viewModel.A = (int)value5;
        _viewModel.B = (int)value6;
        _viewModel.ApplyRule();
        
        // Implement your logic here using value1, value2, and selectedRule
        DrawInitialField();
    }

    public void GenerateRandomCells(object sender, RoutedEventArgs e)
    {
        var canvas = this.FindControl<Canvas>("AutomatonCanvas");
        _viewModel.InitializeRandomCellsVal();
        _viewModel.DrawCells(canvas);
    }
    
}