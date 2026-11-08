using UnityEngine;

public class DrillAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform drillBit;

    [Header("Bit Spin")]
    [SerializeField] private float bitSpinSpeed = 1200f;

    [Header("Body Motion")]
    [SerializeField] private float bobFrequency = 25f;
    [SerializeField] private float bobPositionAmount = 0.015f;
    [SerializeField] private float bobRotationAmount = 2f;

    private Vector3 restPosition;
    private Quaternion restRotation;
    private bool isDrilling;

    private void Awake()
    {
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;
    }

    public void SetDrilling(bool drilling)
    {
        isDrilling = drilling;

        if (!isDrilling)
        {
            transform.localPosition = restPosition;
            transform.localRotation = restRotation;
        }
    }

    private void Update()
    {
        if (isDrilling)
        {
            // Spin the drill bit
            if (drillBit != null)
            {
                drillBit.Rotate(Vector3.forward, bitSpinSpeed * Time.deltaTime, Space.Self);
            }

            // Subtle body bob / vibration
            float wave = Mathf.Sin(Time.time * bobFrequency);

            transform.localPosition =
                restPosition + new Vector3(0f, 0f, wave * bobPositionAmount);

            transform.localRotation =
                restRotation * Quaternion.Euler(wave * bobRotationAmount, 0f, 0f);
        }
    }

    private void OnDisable()
    {
        isDrilling = false;

        transform.localPosition = restPosition;
        transform.localRotation = restRotation;
    }
}