using KinematicCharacterController;
using UnityEngine;

namespace Mindshift
{
    [RequireComponent(typeof(Collider))]
    public class B_Bouncer : MonoBehaviour
    {
        [Header("Bounce Settings")]
        [SerializeField] private float bounceForce = 10f; // The force applied to the player
        [SerializeField] private float bounceCooldown = 0.5f; // Time before the pad can bounce again
        [SerializeField] private LayerMask bounceLayer; // Define which objects can be bounced

        private float lastBounceTime = 0f;

        private void OnTriggerEnter(Collider other)
        {
            if (Time.time < lastBounceTime + bounceCooldown)
                return; // Prevent bouncing too frequently

            // Check if the colliding object has a KinematicCharacterMotor or Rigidbody
            if (other.TryGetComponent<B_KinematicCharacterMotor>(out B_KinematicCharacterMotor characterMotor))
            {
                // Apply a vertical impulse to the character motor
                characterMotor.BaseVelocity = new Vector3(characterMotor.BaseVelocity.x, bounceForce, characterMotor.BaseVelocity.z);
                lastBounceTime = Time.time;
                Debug.Log($"Bounced {other.name} upwards!");
            }
            else if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                if ((bounceLayer.value & (1 << rb.gameObject.layer)) > 0) // Ensure it's in the correct layer
                {
                    rb.velocity = new Vector3(rb.velocity.x, bounceForce, rb.velocity.z);
                    lastBounceTime = Time.time;
                    Debug.Log($"Bounced {other.name} upwards!");
                }
            }
        }
    }
}
