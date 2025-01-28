using UnityEngine;

namespace Mindshift
{
    public class B_Mantleable : MonoBehaviour
    {
        [Header("Mantling Settings")]
        [Tooltip("The maximum height from the Player's position that this object can be mantled.")]
        public float mantleHeight = 2f;

        [Tooltip("The maximum distance from the Player at which this object can be mantled.")]
        public float mantleDistance = 1.5f;

        [Tooltip("Optional offset for the mantle target position, if needed.")]
        public Vector3 mantleOffset = Vector3.zero;

        /// <summary>
        /// Determines if the object is mantleable by checking height and distance constraints.
        /// </summary>
        /// <param name="playerPosition">The Player's current position.</param>
        /// <param name="playerDirection">The direction the Player is facing.</param>
        /// <param name="mantleTarget">The calculated mantle target position, if mantleable.</param>
        /// <returns>True if the object can be mantled, otherwise false.</returns>
        public bool IsMantleable(Vector3 playerPosition, Vector3 playerDirection, out Vector3 mantleTarget)
        {
            mantleTarget = Vector3.zero;

            // Perform a raycast to check if the Player is within range
            Vector3 rayOrigin = playerPosition + Vector3.up * 0.5f; // Slightly above Player position to avoid ground checks
            if (Physics.Raycast(rayOrigin, playerDirection, out RaycastHit hit, mantleDistance))
            {
                // Check if the hit object is this mantleable object
                if (hit.collider.gameObject == gameObject)
                {
                    float heightDifference = hit.point.y - playerPosition.y;

                    // Ensure the height is within mantleable range
                    if (heightDifference > 0f && heightDifference <= mantleHeight)
                    {
                        // Calculate the mantle target position
                        mantleTarget = hit.point + mantleOffset;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Draw gizmos to visualize the mantleable area in the Scene view.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            // Draw a sphere to represent the mantleable range
            Gizmos.DrawWireSphere(transform.position, mantleDistance);

            // Draw a line to represent the mantle height
            Vector3 topPoint = transform.position + Vector3.up * mantleHeight;
            Gizmos.DrawLine(transform.position, topPoint);
        }
    }
}