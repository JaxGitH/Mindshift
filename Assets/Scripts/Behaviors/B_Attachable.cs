using UnityEngine;

namespace Mindshift
{
    public class B_Attachable : MonoBehaviour
    {
        [Header("Attachment Settings")]
        [SerializeField] private bool canBeAttached = true;
        [SerializeField] private float weight = 1.0f; // Weight of the object
        [SerializeField] private float liftOffset = 0.5f; // Modifier affecting how much lift is required
        public B_AttachmentPoint attachmentPoint; // Reference to the AttachmentPoint

        private void Start()
        {
            if (attachmentPoint == null)
            {
                Debug.LogError($"{gameObject.name}: AttachmentPoint is missing or not assigned!");
            }
            else
            {
                Debug.Log($"{gameObject.name}: AttachmentPoint assigned to {attachmentPoint.name}");
            }
        }

        public bool IsAttachable()
        {
            return canBeAttached;
        }

        public float GetWeight()
        {
            return weight;
        }

        public float GetLiftOffset()
        {
            return liftOffset;
        }
    }
}
