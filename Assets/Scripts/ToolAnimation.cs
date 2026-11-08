
using System.Collections;
using UnityEngine;

public class ToolAnimation : MonoBehaviour
{
    [Header("Dig Animation")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField]
    private Vector3 digPositionOffset =
        new Vector3(-0.08f, -0.05f, 0.2f);

    [SerializeField]
    private Vector3 digRotationOffset =
        new Vector3(15f, 0f, -12f);

    private Vector3 restPosition;
    private Quaternion restRotation;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        // Remember the tool's original position and rotation.
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;
    }

    public void PlayDigAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(AnimateDig());
    }

    private IEnumerator AnimateDig()
    {
        Vector3 digPosition = restPosition + digPositionOffset;

        Quaternion digRotation =
            restRotation * Quaternion.Euler(digRotationOffset);

        float halfDuration = animationDuration / 2f;
        float elapsed = 0f;

        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        // Swing forward.
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / halfDuration);

            transform.localPosition =
                Vector3.Lerp(startPosition, digPosition, t);

            transform.localRotation =
                Quaternion.Slerp(startRotation, digRotation, t);

            yield return null;
        }

        elapsed = 0f;

        // Return to resting position.
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / halfDuration);

            transform.localPosition =
                Vector3.Lerp(digPosition, restPosition, t);

            transform.localRotation =
                Quaternion.Slerp(digRotation, restRotation, t);

            yield return null;
        }

        transform.localPosition = restPosition;
        transform.localRotation = restRotation;

        animationCoroutine = null;
    }

    private void OnDisable()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        transform.localPosition = restPosition;
        transform.localRotation = restRotation;
    }
}
