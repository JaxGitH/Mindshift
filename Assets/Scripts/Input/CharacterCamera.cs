using Mindshift;
using UnityEngine;

namespace Mindshift
{
    public class CharacterCamera : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset = new Vector3(0, 2, -5);
        public float smoothSpeed = 5f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
