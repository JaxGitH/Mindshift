using UnityEngine;
using KinematicCharacterController;

namespace Mindshift
{
    [RequireComponent(typeof(PhysicsMover))]
    public class B_BouncePad : MonoBehaviour, IMoverController
    {
        [Header("Bounce Settings")]
        public float bounceForce = 30f;
        public float bounceCooldown = 0.2f; // Prevents repeated bounces
        public LayerMask playerLayer;

        private PhysicsMover physicsMover;
        private float lastBounceTime;

        private void Awake()
        {
            physicsMover = GetComponent<PhysicsMover>();
            physicsMover.MoverController = this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Time.time - lastBounceTime < bounceCooldown) return; // Prevent double bounces

            if ((playerLayer & (1 << other.gameObject.layer)) != 0)
            {
                if (other.TryGetComponent<B_KinematicCharacterMotor>(out B_KinematicCharacterMotor motor))
                {
                    Debug.Log($"Bouncing {other.name} with force {bounceForce}");
                    motor.BaseVelocity = new Vector3(motor.BaseVelocity.x, bounceForce, motor.BaseVelocity.z);
                    lastBounceTime = Time.time;
                }
            }
        }

        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            // BouncePad should remain stationary
            goalPosition = transform.position;
            goalRotation = transform.rotation;
        }
    }
}
