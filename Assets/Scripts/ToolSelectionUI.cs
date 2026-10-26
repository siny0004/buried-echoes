
using TMPro;
using UnityEngine;

public class ToolSelectionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text toolText;

    public void Refresh(bool shovelEquipped, int shovelLevel)
    {
        if (toolText == null)
        {
            Debug.LogWarning("ToolText TMP reference is NOT assigned!");
            return;
        }

        string shovelColor = shovelEquipped ? "#E6C38D" : "#888888";
        string drillColor = shovelEquipped ? "#888888" : "#E6C38D";

        toolText.text =
            $"<color={shovelColor}>[1] Shovel Lv. {shovelLevel}</color>\n" +
            $"<color={drillColor}>[2] Drill</color>";
    }
}
