using System.Collections.Generic;

[System.Serializable]
public class SlotSaveData
{
    public int itemID;
    public int amount;
}

[System.Serializable]
public class InventorySaveData
{
    public List<SlotSaveData> savedSlots = new List<SlotSaveData>();
}