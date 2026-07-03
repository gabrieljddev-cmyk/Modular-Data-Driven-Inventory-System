using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI textQuantity;
    private bool referencesResolved = false;

    public int SlotIndex { get; set; }
    private InventoryDisplay displayManager;

    [Header("Fallback")]
    [SerializeField] private Sprite defaultIcon;

    private void Awake()
    {
        EnsureReferences(); // Guarantee references are set up
        displayManager = GetComponentInParent<InventoryDisplay>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (displayManager == null)
        {
            Debug.LogWarning("InventorySlotUI: displayManager is not assigned.");
            return;
        }

        // notify display to start dragging
        displayManager.StartDrag(SlotIndex, eventData.position);
    
        if (itemIcon != null)
        {
            // Make icon semi-transparent, looking for juiceness and feedback
            itemIcon.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (displayManager != null)
        {
            displayManager.UpdateDrag(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        displayManager.EndDrag();
        // Restore visibility
        if (itemIcon != null) itemIcon.color = new Color(1f, 1f, 1f, 1f);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (displayManager.CurrentDragSlotIndex != -1)
        {
            displayManager.SwapSlots(displayManager.CurrentDragSlotIndex, this.SlotIndex);
        }
    }

    public void EnsureReferences()
    {
        if (referencesResolved) return;

        if (itemIcon == null)
        {
            // Try to find the Image component in children
            var imageTransform = transform.Find("itemImage");
            if (imageTransform != null) itemIcon = imageTransform.GetComponent<Image>();

            if (itemIcon == null)
            {
                foreach (Image img in GetComponentsInChildren<Image>(true))
                {
                    if (img.gameObject != this.gameObject)
                    {
                        itemIcon = img;
                        break;
                    }
                }
            }
        }

        if (textQuantity == null)
        {
            var textTransform = transform.Find("quantity_Text");
            if (textTransform != null) textQuantity = textTransform.GetComponent<TextMeshProUGUI>();

            if (textQuantity == null) textQuantity = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (itemIcon == null || textQuantity == null)
        {
            Debug.LogWarning($"InventorySlotUI could not find its UI references on {gameObject.name}. Check hierarchy names.");
        }
        else
        {
            referencesResolved = true;
        }
    }

    public void EmptySlot()
    {
        // Ensure references are resolved before clear the slot
        EnsureReferences();

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        if (textQuantity != null)
        {
            textQuantity.text = "";
            textQuantity.enabled = false;
        }
    }

    public void UpdateSlot(Sprite newIcon, int newQuantity)
    {
        //Ensure references before updating the slot
        EnsureReferences();

        if (itemIcon != null)
        {
            Sprite toApply = newIcon != null ? newIcon : defaultIcon;
            itemIcon.sprite = toApply;
            itemIcon.enabled = toApply != null;
        }

        if (textQuantity != null)
        {
            textQuantity.text = newQuantity > 1 ? newQuantity.ToString() : "";
            textQuantity.enabled = newQuantity > 1;
        }
    }
}