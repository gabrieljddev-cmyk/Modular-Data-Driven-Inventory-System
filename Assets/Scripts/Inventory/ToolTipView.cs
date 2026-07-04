using UnityEngine;
using TMPro;

public class ToolTipView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private CanvasGroup canvas;

     // this Just care about what to show not when
    public void SetText(string name, string description)
    {
       nameText.text = name;
       descriptionText.text = description;
    }

    public void SetVisibility(bool isVisible)
    {
        canvas.alpha = isVisible? 1 : 0;
    }
}
