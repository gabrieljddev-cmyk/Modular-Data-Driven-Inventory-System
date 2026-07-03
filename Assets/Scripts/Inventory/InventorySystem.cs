using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class InventorySystem : MonoBehaviour
{
    public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    public int maxInventorySize = 20;

    [Header("Save/Load")]
    private string SavePath => Path.Combine(Application.persistentDataPath, "inventory.json");
    [SerializeField] private List<ItemData> itemDatabase = new List<ItemData>();

    [Header("Debug/Test")]
    [SerializeField] private ItemData debugItem;
    [SerializeField] private int debugItemQuantity = 1;

    public event Action OnInventoryChanged; // notify inventory changes

    private void Awake()
    {
        InitializeInventorySlots(); // Initialize the inventory
        LoadInventory();
    }

    public void InitializeInventorySlots()
    {
        inventorySlots.Clear(); // avoid duplicates

        for (int i = 0; i < maxInventorySize; i++) // fill empty slots
        {
            inventorySlots.Add(new InventorySlot());
        }
    }

    private void NotifyInventoryChanged()
    {
        if (OnInventoryChanged != null)
        {
            OnInventoryChanged.Invoke();
        }
        else
        {
            Debug.LogWarning($"Inventory changed event invoked with no subscribers. inventorySystem={gameObject.name} id={GetInstanceID()}");
        }
    }

    public bool AddItem(ItemData item, int quantity)
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot add null item to inventory.");
            return false;
        }

        if (item.isStackable) // Try to add to existing stack first
        {
            foreach (var inventorySlot in inventorySlots)
            {
                if (inventorySlot.item != null && inventorySlot.item.id == item.id && inventorySlot.quantity < item.maxStackSize)
                {
                    int availableSpace = item.maxStackSize - inventorySlot.quantity;
                    int quantityToAdd = Mathf.Min(quantity, availableSpace);
                    inventorySlot.AddQuantity(quantityToAdd);
                    quantity -= quantityToAdd;

                    if (quantity <= 0)
                    {
                        NotifyInventoryChanged();
                        return true;
                    }
                }
            }
        }

        while (quantity > 0)
        {
            int emptyIndex = inventorySlots.FindIndex(slot => slot.item == null);
            if (emptyIndex != -1)
            {
                InventorySlot emptySlot = inventorySlots[emptyIndex];
                emptySlot.item = item;
                int quantityToAdd = (int)(item.isStackable ? MathF.Min(quantity, item.maxStackSize) : 1);
                emptySlot.quantity = quantityToAdd;
                quantity -= quantityToAdd;
            }
            else
            {
                Debug.LogWarning($"InventorySystem ({gameObject.name}) could not add item {item.itemName}: inventory full.");
                NotifyInventoryChanged();
                return false; // Inventory full
            }
        }

        NotifyInventoryChanged();
        return true;
    }
    
    public void SwapSlots(int sourceIndex, int targetIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= inventorySlots.Count || targetIndex < 0 || targetIndex >= inventorySlots.Count)
        {
            Debug.LogWarning($"SwapSlots: Invalid indices. sourceIndex={sourceIndex}, targetIndex={targetIndex}");
            return;
        }

        var sourceSlot = inventorySlots[sourceIndex];
        var targetSlot = inventorySlots[targetIndex];

        if (sourceSlot.item != null && targetSlot.item != null && sourceSlot.item == targetSlot.item)
        {
            if (targetSlot.quantity < sourceSlot.item.maxStackSize)
            {
                int spaceInSlot = targetSlot.item.maxStackSize - targetSlot.quantity;
                int amountToTransfer = Math.Min(spaceInSlot, sourceSlot.quantity);

                targetSlot.quantity += amountToTransfer;
                sourceSlot.quantity -= amountToTransfer;

                if (sourceSlot.quantity <= 0)
                {
                    sourceSlot.ClearSlot();
                }

                NotifyInventoryChanged();
                return;
            }
        }

        inventorySlots[sourceIndex] = targetSlot;
        inventorySlots[targetIndex] = sourceSlot;
        NotifyInventoryChanged();
    }

    public ItemData GetItemAtSlot(int index)
    {
        if (index < 0 || index >= inventorySlots.Count)
        {
            Debug.LogWarning($"GetItemAtSlot: Index {index} is out of bounds.");
            return null;
        }

        return inventorySlots[index].item;
    }

    public ItemData GetItemDataByID(int itemID)
    {
        return itemDatabase.Find(item => item != null && item.id == itemID);
    }

    //Save methods
    public void SaveInventory()
    {
        InventorySaveData saveData = new InventorySaveData();

        foreach (var slot in inventorySlots)
        {
            SlotSaveData slotData = new SlotSaveData();

            if (slot.item != null)
            {
                slotData.itemID = slot.item.id;
                slotData.amount = slot.quantity;
            }
            else
            {
                slotData.itemID = 0; // void Slot
                slotData.amount = 0;
            }

            saveData.savedSlots.Add(slotData);
        }

        //Convert class to json text
        string json = JsonUtility.ToJson(saveData, true);

        // Write archive
        File.WriteAllText(SavePath, json);
        Debug.Log($"Invetory Saved in {SavePath}");
    }

    public void LoadInventory()
    {
        // Verify if exist a file
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Nenhum arquivo de save encontrado. Iniciando inventário vazio.");
            return; 
        }

        try
        {
            // read json
            string json = File.ReadAllText(SavePath);

            // covert text to data
            InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

            // Rebuild data
            for (int i = 0; i < saveData.savedSlots.Count; i++)
            {
                if (i >= inventorySlots.Count) break; // Just for safe

                SlotSaveData slotData = saveData.savedSlots[i];

                if (slotData.itemID != 0)
                {
                    ItemData itemEncontrado = GetItemDataByID(slotData.itemID);
                    if (itemEncontrado != null)
                    {
                        inventorySlots[i].item = itemEncontrado;
                        inventorySlots[i].quantity = slotData.amount;
                    }
                    else
                    {
                        inventorySlots[i].item = null;
                        inventorySlots[i].quantity = 0;
                        Debug.LogWarning($"LoadInventory: Item with ID {slotData.itemID} not found in itemDatabase.");
                    }
                }
                else
                {
                    inventorySlots[i].item = null;
                    inventorySlots[i].quantity = 0;
                }
            }

            NotifyInventoryChanged();
            Debug.Log("Inventário carregado com sucesso!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao carregar o inventário: {e.Message}");
        }
    }

    // Debug methods
    [ContextMenu("Add Test Item")]
    public void AddTestItem()
    {
        if (debugItem == null)
        {
            Debug.LogWarning("No debug item assigned in InventorySystem.");
            return;
        }

        AddItem(debugItem, debugItemQuantity);
    }

    [ContextMenu("Save Inventory")]
    public void TestSaveInventory() 
    { 
        SaveInventory();
    }

}