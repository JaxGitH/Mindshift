using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

public class B_Bouncer : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float bouncePower = 15f; // Base bounce force
    [SerializeField] private float fallMultiplier = 1.5f; // Extra force when falling
    [SerializeField] private LayerMask playerLayer; // Layer assigned to the Player

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has B_Link and is attached
        B_Link linkScript = other.GetComponentInParent<B_Link>();
        if (linkScript != null && linkScript.isAttached)
        {
            Debug.Log($"Bouncer disabled because {other.gameObject.name} is linked.");
            return;
        }

        // Check if the collided object is in the specified Player Layer
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            CharacterActor character = other.GetComponentInParent<CharacterActor>();
            if (character != null)
            {
                ApplyCharacterBounce(character);
                return;
            }
        }
        else
        {
            // If not in Player Layer, apply bounce to Rigidbody
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                ApplyRigidbodyBounce(rb);
            }
        }
    }

    private void ApplyCharacterBounce(CharacterActor character)
    {
        Debug.Log($"Bouncing CCP Player {character.name}!");

        // Ensure the character is not grounded before applying bounce
        character.ForceNotGrounded();

        // Boost bounce effect if the player is falling
        float appliedBouncePower = (character.Velocity.y < 0) ? bouncePower * fallMultiplier : bouncePower;

        // Apply the bounce force while keeping horizontal momentum
        Vector3 newVelocity = character.Velocity;
        newVelocity.y = appliedBouncePower;
        character.Velocity = newVelocity;
    }

    private void ApplyRigidbodyBounce(Rigidbody rb)
    {
        Debug.Log($"Bouncing Rigidbody {rb.name}!");
        rb.AddForce(Vector3.up * bouncePower, ForceMode.Impulse);
    }
}
