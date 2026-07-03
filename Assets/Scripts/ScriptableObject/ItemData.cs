using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    [TextArea(2, 5)] public string itemDescription;
    public Sprite itemIcon;
    public int itemValue;
    public int itemWeight;
    public bool isStackable;
    public int maxStackSize;
}
