using UnityEngine;
using System.Collections;

public class B_ForceInDirection : MonoBehaviour
{
    private Rigidbody otherRb;
    [SerializeField] Vector3 direction = Vector3.up;
    [SerializeField] float power = 3f;
    [SerializeField] ForceMode forceType = ForceMode.Impulse;


    
    public void ApplyForce(Rigidbody rb)
    {
        if (rb == null)
        {
            Debug.LogError("No Rigidbody detected");
        }

        if (forceType == ForceMode.Impulse) Upforce(rb);
        else if (forceType == ForceMode.Force)  ConstantForce(rb);
        else Debug.LogError("No method has been made for this ForceMode");
    }
    private void Upforce(Rigidbody _rb)
    {
        _rb.AddForce(direction * power, forceType);
    }

    private void ConstantForce(Rigidbody _rb)
    {
        StartCoroutine(ApplyForceOverTime(_rb));
    }

    private IEnumerator ApplyForceOverTime(Rigidbody rb)
    {
        while (true) // Infinite loop until stopped
        {
            rb.AddForce(direction.normalized * power, forceType);
            yield return new WaitForFixedUpdate(); // Apply force every physics update
        }
    }
}
