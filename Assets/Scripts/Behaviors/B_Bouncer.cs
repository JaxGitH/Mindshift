using UnityEngine;

namespace Mindshift
{
    public class B_BouncePad : MonoBehaviour
    {
        [Header("Bounce Settings")]
        public float bounceForce = 30f;
        public float bounceCooldown = 0.2f; // Prevents repeated bounces
        public LayerMask playerLayer;

        private float lastBounceTime;


        private void OnTriggerEnter(Collider other)
        {

        }

        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            // BouncePad should remain stationary
            goalPosition = transform.position;
            goalRotation = transform.rotation;
        }
    }
}
