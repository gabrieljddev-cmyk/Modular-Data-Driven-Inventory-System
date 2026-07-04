using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    [Header("Inventory Display Settings")]
    public InventorySystem inventorySystem;
    [SerializeField] private Transform inventorySlotContainer; // Container to slots
    [SerializeField] private GameObject inventorySlotPrefab; // Prefab for a single slot

    [Header("Drag Drop Settings")]
    [SerializeField] private Image dragIcon;
    public int CurrentSlotIndex { get; private set; } = -1;

    private List<InventorySlotUI> inventoryUISlot = new List<InventorySlotUI>();

    private void OnDisable()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged -= UpdateInterface;
        }
    }

    private void Start()
    {
        inventorySystem = FindFirstObjectByType<InventorySystem>();
    
        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged -= UpdateInterface; // Avoid duplicate subs
            inventorySystem.OnInventoryChanged += UpdateInterface;
        }

        if (inventorySystem == null)
        {
            Debug.LogError("InventoryDisplay: inventorySystem is not assigned.");
            return;
        }

        InitializeInventoryUI();
        UpdateInterface();
    }
    
    void OnDestroy()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventoryChanged -= UpdateInterface;
        }
    }

    public void StartDrag(int slotIndex, Vector2 mousePos)
    {
        CurrentSlotIndex = slotIndex;
        
        var itemData = inventorySystem.GetItemAtSlot(slotIndex);
        if (itemData != null)
        {
            dragIcon.sprite = itemData.itemIcon;
            dragIcon.gameObject.SetActive(true);
            dragIcon.transform.position = mousePos;
        }
    }

    public void UpdateDrag(Vector2 mousePos)
    {
        if (CurrentSlotIndex == -1) return;
        
        RectTransform rectTransform = dragIcon.GetComponent<RectTransform>();
        Canvas canvas = dragIcon.GetComponentInParent<Canvas>();

        if (dragIcon != null)
        {
            dragIcon.transform.position = mousePos;
        }
    }

    public void EndDrag()
    {
        CurrentSlotIndex = -1;
        dragIcon.gameObject.SetActive(false);
    }

    public void SwapSlots(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;
        
        inventorySystem.SwapSlots(fromIndex, toIndex);
    }

    public void StartToolTip(int slotIndex, Vector2 mousePos)
    {
        CurrentSlotIndex = slotIndex;

        var itemData = inventorySystem.GetItemAtSlot(slotIndex);

        if (itemData != null)
        {
            ToolTipManager.ShowToolTip(itemData.itemName, itemData.itemDescription, mousePos);
        }
    }

    public void EndToolTip()
    {
        ToolTipManager.HideToolTip();
    }

    private void InitializeInventoryUI()
    {
        if (inventorySlotContainer == null || inventorySlotPrefab == null)
        {
            Debug.LogError("InventoryDisplay: References missing in Inspector.");
            return;
        }

        // Garbage Collection: avoid dupplicate slots
        foreach (Transform child in inventorySlotContainer)
        {
            Destroy(child.gameObject);
        }

        inventoryUISlot.Clear();

        if (inventorySystem.inventorySlots.Count == 0)
        {
            inventorySystem.InitializeInventorySlots();
        }

        int targetSlotCount = Mathf.Max(inventorySystem.inventorySlots.Count, inventorySystem.inventorySlots.Count);

        for (int i = 0; i < targetSlotCount; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventorySlotContainer);
            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

            if (slotUI == null)
            {
                Debug.LogError("InventoryDisplay: inventorySlotPrefab must contain an InventorySlotUI component.");
                Destroy(slotObj);
                continue;
            }

            slotUI.EnsureReferences(); 

            inventoryUISlot.Add(slotUI);
            slotUI.SlotIndex = i;
            slotUI.EmptySlot();
        }
    }

    private void UpdateInterface()
    {
        if (inventorySystem == null || inventoryUISlot == null || inventoryUISlot.Count == 0)
        {
            Debug.LogWarning("InventoryDisplay: not updating interface");
            return;
        }

        int slotCount = Mathf.Min(inventorySystem.inventorySlots.Count, inventoryUISlot.Count);

        for (int i = 0; i < slotCount; i++)
        {
            var logicalSlot = inventorySystem.inventorySlots[i];

            if (logicalSlot != null && logicalSlot.item != null)
            {
                inventoryUISlot[i].UpdateSlot(logicalSlot.item.itemIcon, logicalSlot.quantity);
            }
            else
            {
                inventoryUISlot[i].EmptySlot();
            }
        }

        if (inventoryUISlot.Count > 0)
        {
            Canvas.ForceUpdateCanvases();
        }
    }
}