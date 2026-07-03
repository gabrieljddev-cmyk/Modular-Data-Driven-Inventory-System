using System;

[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public InventorySlot()
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        item = null;
        quantity = 0;
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        quantity -= amount;
        if (quantity < 0)
        {
            quantity = 0;
        }
    }
}
