using UnityEngine;

public class B_OnRbContact : MonoBehaviour
{
    [Header("Event Settings")]
    public RigidbodyEvent onCollisionEvent; // Event called on collision

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null) // Ensures we hit a Rigidbody
        {
            onCollisionEvent.Invoke(collision.rigidbody); // Call assigned functions
        }
    }
}
