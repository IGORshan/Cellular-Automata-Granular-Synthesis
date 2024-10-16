using CellularAutomataUI.Service;

namespace CellularAutomataUI.Helpers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Avalonia.Controls;
using Avalonia.Media;



//Used for the legacy rules that existed in preset for user to choose.
public class LegacyRule
{
    public string Name { get; set; }
    public int A { get; set; }
    public int B { get; set; }
}

//The RuleSet class holds the job of manipulating the cells and applying the rules of life/death/birth.
//This will be an abstract base class, which are used by all the different versions of rule sets implemented by concrete classes.
//It only handles the common code like for a single cell to be able to calculate how many neighbors it has, etc.
public abstract class CaRuleSet
{
    protected int _maxX = 0; 
    protected int _maxY = 0; 
    protected int[,] _field;

    //instantiates the RuleSet with a copy of the game field & its max X and Y boundaries.
    public CaRuleSet(int[,] field, int maxX, int maxY)
    {
        _field = field;
        _maxX = maxX;
        _maxY = maxY;
    }

    //return the numbers of neighbors that the cell has at coordinate(X,Y)
    protected int GetNumberOfNeighbors(int x, int y)
    {
        int neighbors = 0;

        //Checking: Up & Down & Right & Left -- four of them
        if (x + 1 < _maxX && _field[x + 1, y] == 1)
            neighbors++;

        if (x - 1 >= 0 && _field[x - 1, y] == 1)
            neighbors++;

        if (y + 1 < _maxY && _field[x, y + 1] == 1)
            neighbors++;
    
        if (y - 1 >= 0 && _field[x, y - 1] == 1)
            neighbors++;
        
        //Checking: diaganols -- 4 of them
        if (x + 1 < _maxX && y + 1 < _maxY && _field[x + 1, y + 1] == 1)
            neighbors++;
        
        if (x + 1 < _maxX && y - 1 >= 0 && _field[x + 1, y - 1] == 1)
            neighbors++;
        
        if (x - 1 >= 0 && y + 1 < _maxY && _field[x - 1, y + 1] == 1)
            neighbors++;
        
        if (x - 1 >= 0 && y - 1 >= 0 && _field[x - 1, y - 1] == 1)
            neighbors++;

        return neighbors;
    }

    //Execute one generation of the game field, causing the cells to live die or give birth
    //As a template method, it calls the concrete method TickAlgorithm() to execute the cell movement details.
    public void Tick()
    {
        int[,] field2 = TickAlgorithm();
        
        //Copy the result field2 to field, equivalent as: field = field2;
        Array.Copy(field2, _field, field2.Length);
    }
    
    protected abstract int[,] TickAlgorithm();
    
    public static List<LegacyRule> LoadRules(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var rules = JsonConvert.DeserializeObject<List<LegacyRule>>(json);
        return rules;
    }
}


//Below a concrete class of a generic ruleset implementation that notation form of the rule can be passed to implement all different behaviors or rules.
//This helps us to get rid of having to implement individual ruleset classes for each one and makes it easier to try out different CA rule sets.
public class RuleGeneric : CaRuleSet
{
    //Generic rule a/b, where a represents the digits before the slash, and b represents those after. 
    private List<int> _continueDigits;
    private List<int> _birthDigits;

    public RuleGeneric(int[,] field, int maxX, int maxY, int a, int b)
        : base(field, maxX, maxY)
    {
        _continueDigits = ToDigitArray(a);
        _birthDigits = ToDigitArray(b);
    }

    protected List<int> ToDigitArray(int digits)
    {
        return digits.ToString().Select(digit => (int)char.GetNumericValue(digit)).ToList();
    }

    protected override int[,] TickAlgorithm()
    {
        int[,] field2 = new int[_maxX, _maxY];
        
        // A/B
        // The first number(s) before the slash is what it requires for a cell to continue
        // The second number(s) after the slash is what it requires for a cell's birth.
        string pitchArrayPerCol = "";
        for (int y = 0; y < _maxY; ++y)
        {
            for (int x = 0; x < _maxX; ++x)
            {
                int neighbors = GetNumberOfNeighbors(x, y);

                //Check birth conditions
                if (_birthDigits.Contains(neighbors))
                {
                    //cell is born
                    field2[x, y] = 1;
                }

                //Check living / continuing condition
                else if (_continueDigits.Contains(neighbors))
                {
                    //cell continues to live
                    field2[x, y] = _field[x, y];
                }

                else
                {
                    field2[x, y] = 0;
                }
                pitchArrayPerCol += x + " " + y + " " + field2[x, y] + ", "; //col + row + cond
            }
        }
        OSCService.SendMatrixMessage(pitchArrayPerCol);
        Console.WriteLine(pitchArrayPerCol);
        return field2;
    }
} 