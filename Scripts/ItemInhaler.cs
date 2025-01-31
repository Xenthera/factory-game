using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Range = System.Range;

public partial class ItemInhaler : Node3D , IItemCarrier
{
    [Export] public Label3D countLabel;
    [Export] public Node3D output;
    private Dictionary<int, int> counts = new Dictionary<int, int>();
    private static readonly Random rng = new Random();

    private float _tick = 0f;

    private int outputBuffer;

    private int craftProgress = 0;
    
    Queue<int> craftQueue = new Queue<int>();
    
    
    public override void _Process(double delta)
    {
        string s = "";
        foreach (KeyValuePair<int, int> count in counts)
        {
            s += "ID: " + count.Key.ToString() + ": " + count.Value.ToString() + "\n";
        }

        s += "Total: " + counts.Values.Sum() + "\n";
        s += "Output craft count: " + outputBuffer + "\n";
        s += "[";
        for (int i = 0; i < 10; i++)
        {
            s += i + 1 <= craftProgress ? "=" : " ";
        }
        s += "]\n";
        countLabel.Text = s.ToString();
        
        
        _tick += (float)delta;
        
        

        if (_tick >= 0.01f)
        {
            for (int i = 0; i < 10; i++)
            {


                if (hasItemsForCraft())
                {
                    craftProgress += 2;
                }

                else
                {
                    craftProgress = 0;
                }

                if (craftProgress > 10)
                {
                    craftProgress = 0;
                    if (craft() != 0)
                    {
                        outputBuffer++;
                    }
                }


                if (output is IItemCarrier outputCarrier)
                {
                    if (outputBuffer > 0 && outputCarrier.AddItem(1))
                    {
                        outputBuffer--;
                    }
                }


            }

            _tick = 0f;
            
        }
    }

    public bool AddItem(int itemId)
    {
        if (counts.Values.Sum() > 100000)
        {
            return false;
        }
        if (itemId == 1)
        {
            counts[2]+=2;
            counts[3] += 6;
            return true;
        }
        //if (rng.Next(2) == 0)
        {
            //if (counts.Values.Sum() < 100)
            {
                if (counts.ContainsKey(itemId))
                {
                    counts[itemId]++;
                }
                else
                {
                    counts.Add(itemId, 1);
                }

                return true;
            }
        }
        

        return false;
    }

    private int craft()
    {
       
        if (counts.ContainsKey(2) && counts.ContainsKey(3))
        {
            if (counts[2] >= 1 && counts[3] >= 3)
            {
                counts[2] -= 1;
                counts[3] -= 3;
                return 1;
            }
        }

        return 0;
    }

    private bool hasItemsForCraft()
    {
        if (counts.ContainsKey(2) && counts.ContainsKey(3))
        {
            if (counts[2] >= 1 && counts[3] >= 3)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool RemoveItem()
    {
        throw new NotImplementedException();
    }

    public bool HasItem(int itemId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<int> GetItems()
    {
        throw new NotImplementedException();
    }
}
