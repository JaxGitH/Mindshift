using UnityEngine;
using KinematicCharacterController;

namespace Mindshift
{
    [RequireComponent(typeof(PhysicsMover))]
    public class B_Mover : MonoBehaviour, IMoverController
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveDistance = 5f; // Distance to move on the X-axis
        [SerializeField] private float moveTime = 3f; // Time to move between points
        [SerializeField] private float waitTime = 1f; // Time to wait at each point
        [SerializeField] private LayerMask allowedRiderLayers; // Layers allowed to interact with the platform

        private Vector3 pointA;
        private Vector3 pointB;
        private bool movingToB = true;
        private float timer = 0f;

        private PhysicsMover physicsMover;

        private void Awake()
        {
            // Initialize PhysicsMover and assign this script as the MoverController
            physicsMover = GetComponent<PhysicsMover>();
            physicsMover.MoverController = this;

            // Define start and end points
            pointA = transform.position;
            pointB = pointA + Vector3.right * moveDistance;
        }

        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            // Calculate movement progress
            timer += deltaTime;
            float progress = Mathf.Clamp01(timer / moveTime);

            // Determine the target position
            goalPosition = movingToB
                ? Vector3.Lerp(pointA, pointB, progress)
                : Vector3.Lerp(pointB, pointA, progress);

            // Toggle direction when reaching the destination
            if (progress >= 1f)
            {
                timer = 0f;
                movingToB = !movingToB;
            }

            // Keep the platform's rotation constant
            goalRotation = transform.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & allowedRiderLayers) != 0)
            {
                Debug.Log($"Player {other.name} entered platform {name}.");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & allowedRiderLayers) != 0)
            {
                Debug.Log($"Player {other.name} exited platform {name}.");
            }
        }
    }
}
