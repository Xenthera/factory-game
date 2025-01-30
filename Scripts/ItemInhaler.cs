using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ItemInhaler : Node3D , IItemCarrier
{
    [Export] public Label3D countLabel;
    private Dictionary<int, int> counts = new Dictionary<int, int>();
    private static readonly Random rng = new Random(); 


    public override void _Process(double delta)
    {
        string s = "";
        foreach (KeyValuePair<int, int> count in counts)
        {
            s += "ID: " + count.Key.ToString() + ": " + count.Value.ToString() + "\n";
        }

        s += "Total: " + counts.Values.Sum();
        countLabel.Text = s.ToString();
    }

    public bool AddItem(int itemId)
    {
        if (rng.Next(2) == 0)
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
