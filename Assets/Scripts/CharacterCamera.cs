using UnityEngine;

namespace Mindshift.CharacterControllerPro.Core
{
    public class CharacterCamera : MonoBehaviour
    {
        public Transform target; // Reference to the CharacterActor's Transform
        public Vector3 offset = new Vector3(0, 2, -5); // Default camera position
        public float smoothSpeed = 5f; // How smooth the camera follows

        private void LateUpdate()
        {
            if (target == null) return;

            // Smoothly move the camera to follow the target
            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}