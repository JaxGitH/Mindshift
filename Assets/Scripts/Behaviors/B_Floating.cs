using UnityEngine;

namespace Mindshift
{
    public class B_Floating : MonoBehaviour
    {
        [Header("Floating Settings")]
        public float floatSpeed = 2f; // Base speed at which the object floats upward
        public float liftCapacity = 5f; // Maximum lift capacity
        private B_Attachable attachedObject;

        private void OnTriggerEnter(Collider other)
        {
            if (attachedObject == null && other.TryGetComponent<B_Attachable>(out B_Attachable attachable))
            {
                AttachObject(attachable);
            }
        }

        private void AttachObject(B_Attachable attachable)
        {
            Debug.Log($"Object attached to floating: {attachable.name}");
            attachedObject = attachable;

            // Disable dragging while attached
            if (attachable.TryGetComponent<B_Draggable>(out B_Draggable draggable))
            {
                draggable.enabled = false;
            }

            // Attach via AttachmentPoint logic
            if (TryGetComponent<B_AttachmentPoint>(out B_AttachmentPoint floatingPoint) && attachable.attachmentPoint != null)
            {
                floatingPoint.Attach(attachable.attachmentPoint);
            }

            Rigidbody attachedRb = attachable.GetComponent<Rigidbody>();
            if (attachedRb != null)
            {
                attachedRb.isKinematic = true;
            }
        }

        private void FixedUpdate()
        {
            if (attachedObject != null)
            {
                // Calculate total weight of all attached objects
                float totalWeight = CalculateTotalWeight(transform);
                float effectiveWeight = totalWeight - liftCapacity;

                if (effectiveWeight <= 0)
                {
                    Vector3 floatDirection = Vector3.up * floatSpeed * Time.fixedDeltaTime;
                    transform.position += floatDirection;
                }
                else
                {
                    float adjustedSpeed = floatSpeed / effectiveWeight;
                    adjustedSpeed = Mathf.Clamp(adjustedSpeed, 0f, floatSpeed);
                    Vector3 offsetDirection = Vector3.up * adjustedSpeed * Time.fixedDeltaTime;
                    transform.position += offsetDirection;
                }
            }
        }

        // Calculate total weight of all children recursively
        private float CalculateTotalWeight(Transform parent)
        {
            float totalWeight = 0f;

            foreach (Transform child in parent)
            {
                if (child.TryGetComponent(out B_Attachable attachable))
                {
                    totalWeight += attachable.GetWeight();
                }

                totalWeight += CalculateTotalWeight(child);
            }

            return totalWeight;
        }
    }
}
