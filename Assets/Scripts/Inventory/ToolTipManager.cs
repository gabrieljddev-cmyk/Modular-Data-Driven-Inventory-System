using Unity.VisualScripting;
using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager ToolTipManagerInstance { get; private set; }

    [SerializeField] private ToolTipView toolTipView;
    [SerializeField] private Vector2 offSet = new Vector2(15, 15);
    private Vector2 mousePosition;

    private bool _isActive;

    void Awake()
    {
        if (ToolTipManagerInstance != null && ToolTipManagerInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        ToolTipManagerInstance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!_isActive) return;
        
        transform.position = mousePosition + offSet;
    }

    public static void ShowToolTip(string itemName, string itemDescription, Vector2 mousePos)
    {
        ToolTipManagerInstance.mousePosition = mousePos;
        ToolTipManagerInstance.toolTipView.SetText(itemName, itemDescription);
        ToolTipManagerInstance.toolTipView.SetVisibility(true);
        ToolTipManagerInstance._isActive = true;
    }

    public static void HideToolTip()
    {
        ToolTipManagerInstance.toolTipView.SetVisibility(false);
        ToolTipManagerInstance._isActive = false;
    }
}
