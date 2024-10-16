using System;
using System.IO;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Transactions;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CellularAutomataUI.Helpers;
using CellularAutomataUI.Service;
using ReactiveUI;
using Avalonia.Media;


namespace CellularAutomataUI.ViewModels;

public class MainWindowViewModel : ViewModelBase, IReactiveObject, IActivatableViewModel
{
    private int[,] _field;
    private int MaxX = 9; // size of the field
    private int MaxY = 9; // size of the field
    private readonly Random _random = new Random((int)DateTime.Now.Ticks);
    private double _canvasWidth = 800;
    private double _canvasHeight = 600;
    private CaRuleSet _ruleSet;
    private int _a = 23; // The a stands for first number(s) before the slash in CA-rule and is what it requires for a cell to continue
    private int _b = 3; // The b stands for second number(s) after the slash in CA-rule and is what it requires for a cell's birth.
    private int _ruleIndex;

    private DispatcherTimer _timer;
    // private double IterateSpeedInSec = 1000;
    
    public ViewModelActivator Activator { get; }

    //Propagating of the changes of value(canvas width and height here) to the UI
    public double CanvasWidth
    {
        get => _canvasWidth;
        set => this.RaiseAndSetIfChanged(ref _canvasWidth, value);
    }
    
    public double CanvasHeight
    {
        get => _canvasHeight;
        set => this.RaiseAndSetIfChanged(ref _canvasHeight, value);
    }

    public int A
    {
        get => _a;
        set => this.RaiseAndSetIfChanged(ref _a, value);
    }

    public int B
    {
        get => _b;
        set => this.RaiseAndSetIfChanged(ref _b, value);
    }

    public int RuleIndex
    {
        get => _ruleIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _ruleIndex, value);
            SelectingLegacyRule();
        }
    }
    
    public MainWindowViewModel()
    {
        RuleIndex = 0; // Default: 0 corresponds to "Rule 1"
        Activator = new ViewModelActivator();
        this.WhenActivated((CompositeDisposable disposables) =>
        {
            /* handle activation */
            Disposable
                .Create(() => { /* handle deactivation */ })
                .DisposeWith(disposables);
        });
        
        _field = new int[MaxX, MaxY];
        _ruleSet = new RuleGeneric(_field, MaxX, MaxY, _a, _b);
        OSCService.SendGridSizeMessage(MaxX);

    }

    public void SetGridSize(int x) // MaxX and MaxY will always be the same, in the range of [8,128].
    {
        MaxX = x;
        MaxY = x;
        
        _field = new int[MaxX, MaxY];
        // InitializeRandomCellsVal();
        _ruleSet = new RuleGeneric(_field, MaxX, MaxY, _a, _b);
        OSCService.SendGridSizeMessage(MaxX);
    }
    
    //Initialize the first generation of the binary VALUE of cells
    public void InitializeRandomCellsVal()
    {
        for (int x = 0; x < MaxX; ++x)
        {
            for (int y = 0; y < MaxY; ++y)
            {
                _field[x, y] = _random.Next(0, 2); // Randomly set the cell to 0 or 1
            }
        }
    }
    
    class MyRect : Avalonia.Controls.Shapes.Rectangle
    {
        public int x;
        public int y;
        public Canvas canvas;
    }
    
    void MyClick(object sender, RoutedEventArgs e)
    {
        MyRect rect = (MyRect) sender;
        _field[rect.x, rect.y] = _field[rect.x, rect.y] == 1 ? 0 : 1;
        
        DrawCells(rect.canvas);
    }
    
    //Generating rectangles cells in the grids in the actual UI
    public void DrawCells(Canvas canvas)
    {
        double cellWidth = _canvasWidth / MaxX;
        double cellHeight = _canvasHeight / MaxY; 
        
        canvas.Children.Clear();
        
        for (int y = 0; y < MaxY; ++y)
        {
            for (int x = 0; x < MaxX; ++x)
            {
                var rect = new MyRect()
                {
                    Width = cellWidth,
                    Height = cellHeight,
                    Fill = _field[x, y] == 1 ? Brushes.Black : Brushes.Silver,
                };
                Canvas.SetLeft(rect, x * cellWidth);
                Canvas.SetTop(rect, y * cellHeight);
                canvas.Children.Add(rect);

                rect.x = x;
                rect.y = y;
                rect.canvas = canvas;
                rect.PointerPressed += MyClick;
            }
        }
    }

    //Iterating the rectangels cells by the rules of Cellular Automaton
    public void IteratingCa(Canvas canvas)
    {
        _ruleSet.Tick();
        DrawCells(canvas);
    }

    public void SelectingLegacyRule()
    {
        //Get the base directory of the project, therefore below it could be used to find the json file.
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        
        //Combine it and route to the json file properly.
        var predefinedRules = CaRuleSet.LoadRules(Path.Combine(baseDirectory, "Resources", "LegacyRules.json"));

        // var predefinedRules = CaRuleSet.LoadRules(Path.Combine(baseDirectory, "../../../", "Helpers", "LegacyRules.json"));
        var selectedRule = predefinedRules[RuleIndex];
        
         A = selectedRule.A;
         B = selectedRule.B;
    }
    
    public void ApplyRule()
    {
        //Instantiate the desired concrete RuleSet
        _ruleSet = new RuleGeneric(_field, MaxX, MaxY, _a, _b);
    }
}
