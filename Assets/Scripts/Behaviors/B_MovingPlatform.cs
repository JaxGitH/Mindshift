using UnityEngine;
using System.Collections.Generic;
using Mindshift.CharacterControllerPro.Core;

namespace Mindshift
{
    public class B_MovingPlatform : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float movementOffsetX = 5f; // Distance to move on the X-axis
        public float movementOffsetY = 5f; // Distance to move on the X-axis
        public float speed = 2f; // Movement speed
        public float waitTime = 1f; // Time to wait before switching direction

        private Vector3 startPosition;
        private Vector3 endPosition;
        private bool movingToEnd = true;
        private float waitTimer = 0f;
        private Vector3 lastPosition;
        private Rigidbody platformRigidbody;

        private void Start()
        {
            startPosition = transform.position;
            endPosition = startPosition + new Vector3(movementOffsetX, movementOffsetY, 0);
            lastPosition = transform.position;

            // Ensure the platform has a Kinematic Rigidbody (required for CCP's dynamic ground system)
            platformRigidbody = GetComponent<Rigidbody>();
            if (platformRigidbody == null)
            {
                platformRigidbody = gameObject.AddComponent<Rigidbody>();
                platformRigidbody.isKinematic = true;
                platformRigidbody.useGravity = false;
            }
        }

        private void FixedUpdate()
        {
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
                return;
            }

            // Move towards the target position
            Vector3 targetPosition = movingToEnd ? endPosition : startPosition;
            platformRigidbody.MovePosition(Vector3.MoveTowards(platformRigidbody.position, targetPosition, speed * Time.deltaTime));

            // Check if it reached the destination
            if (Vector3.Distance(platformRigidbody.position, targetPosition) < 0.01f)
            {
                movingToEnd = !movingToEnd; // Switch direction
                waitTimer = waitTime; // Set delay before moving again
            }
        }
    }
}
