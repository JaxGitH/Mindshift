using UnityEngine;

public class B_Float : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatPower = 5f;
    [SerializeField] private ForceMode forceType = ForceMode.Acceleration;
    [SerializeField] private float forceRampTime = 1f; // Time to ramp up force

    private Rigidbody rb;
    private float currentPower = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on " + gameObject.name);
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            // Gradually increase force to prevent sudden jolts
            currentPower = Mathf.Lerp(currentPower, floatPower, Time.fixedDeltaTime / forceRampTime);
            rb.AddForce(Vector3.up * currentPower, forceType);
        }
    }
}
