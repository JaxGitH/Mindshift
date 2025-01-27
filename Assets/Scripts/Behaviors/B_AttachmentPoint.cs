using UnityEngine;

namespace Mindshift
{
    public class B_AttachmentPoint : MonoBehaviour
    {
        [Header("Attachment Settings")]
        public bool isOccupied = false; // Tracks whether this AttachmentPoint is occupied
        [SerializeField] private Transform attachedParent; // Reference to the parent object after attachment

        private void Start()
        {
            Debug.Log($"{gameObject.name}: Initialized with isOccupied = {isOccupied}");
        }

        private void Update()
        {
            if (isOccupied)
            {
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isOccupied) return; // Skip if already occupied

            Debug.Log($"{gameObject.name}: Trigger detected with {other.name}");

            // Check if the other object is an AttachmentPoint
            if (other.TryGetComponent<B_AttachmentPoint>(out B_AttachmentPoint otherPoint) && !otherPoint.isOccupied)
            {
                Debug.Log($"{gameObject.name} attaching to {otherPoint.name}");
                Attach(otherPoint);
            }
        }

        public void Attach(B_AttachmentPoint otherPoint)
        {
            if (isOccupied || otherPoint.isOccupied) return;

            Debug.Log($"{gameObject.name} attaching to {otherPoint.name}");

            // Align the parent object to match the attachment points
            Transform thisParent = transform.parent;
            Transform otherParent = otherPoint.transform.parent;

            // Ensure the Balloon (B_Floating) is the parent
            if (thisParent.TryGetComponent<B_Floating>(out _) && !otherParent.TryGetComponent<B_Floating>(out _))
            {
                // Balloon is the parent
                Vector3 alignmentOffset = transform.position - thisParent.position;
                thisParent.position = otherPoint.transform.position - alignmentOffset;
                otherParent.SetParent(thisParent, worldPositionStays: true);
            }
            else if (!thisParent.TryGetComponent<B_Floating>(out _) && otherParent.TryGetComponent<B_Floating>(out _))
            {
                // The other object is a Balloon; reverse roles
                Vector3 alignmentOffset = otherPoint.transform.position - otherParent.position;
                otherParent.position = transform.position - alignmentOffset;
                thisParent.SetParent(otherParent, worldPositionStays: true);
            }
            else
            {
                Debug.LogError($"{gameObject.name} and {otherPoint.name}: No valid parent found for attachment.");
                return;
            }

            Debug.Log($"Aligned {thisParent.name} to {otherParent.name}");

            // Mark points as occupied
            isOccupied = true;
            otherPoint.isOccupied = true;
        }

        private void LateUpdate()
        {
            // Ensure the parent object stays locked
            if (attachedParent != null)
            {
                transform.position = attachedParent.position;
            }
        }
    }
}
