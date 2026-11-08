using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text interactionText;

    public void Show(string message, Color? color = null)
    {
        if (interactionText == null)
            return;

        if (interactionText.text != message)
            interactionText.text = message;

        Color targetColor = color ?? Color.white;

        if (interactionText.color != targetColor)
            interactionText.color = targetColor;

        if (!interactionText.enabled)
            interactionText.enabled = true;
    }

    public void Hide()
    {
        if (interactionText == null || !interactionText.enabled)
            return;

        interactionText.enabled = false;
    }

    private void Start()
    {
        Hide();
    }
}