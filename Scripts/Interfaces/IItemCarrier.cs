using Godot;
using System;
using System.Collections.Generic;

public interface IItemCarrier
{
    bool AddItem(int itemId);
    bool RemoveItem();
    bool HasItem(int itemId);
    IEnumerable<int> GetItems();
}
