using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text interactionText;

    public void Show(string message, Color? color = null)
    {
        if (interactionText == null)
            return;

        interactionText.text = message;
        interactionText.color = color ?? Color.white;
        interactionText.enabled = true;
    }

    public void Hide()
    {
        if (interactionText == null)
            return;

        interactionText.enabled = false;
    }

    private void Start()
    {
        Hide();
    }
}