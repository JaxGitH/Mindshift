using UnityEngine;

namespace Mindshift
{
    public class B_Draggable : B_DragAndDrop
    {
        [Header("Draggable Settings")]
        [SerializeField] private bool isResettable = true;
        [SerializeField] private PhysicMaterial physicMaterial; // Reference to the material

        protected override void Start()
        {
            base.Start();
            UpdateFrictionBasedOnWeight();
        }
        protected override void OnLongPress()
        {
            base.OnLongPress();
        }

        protected override void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    isBeingDragged = true;
                    objectRb.isKinematic = true; // Disable physics while dragging
                    dragHoldTime = 0f;

                    Debug.Log($"Started dragging {gameObject.name}");
                }
            }

            if (Input.GetMouseButton(0) && isBeingDragged)
            {
                dragHoldTime += Time.deltaTime;

                // Convert screen position to world position
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z; // Maintain current Z depth
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                // Move the object directly to the cursor
                transform.position = worldPosition;

                if (dragHoldTime >= longPressThreshold)
                {
                    OnLongPress();
                }
            }

            if (Input.GetMouseButtonUp(0) && isBeingDragged)
            {
                isBeingDragged = false;
                objectRb.isKinematic = false; // Re-enable physics after dragging
                Debug.Log($"Stopped dragging {gameObject.name}");
            }
        }

        public void SyncWeight(float weight)
        {
            dragWeight = weight;
            UpdateFrictionBasedOnWeight();
            Debug.Log($"Weight synchronized: {dragWeight}");
        }

        private void UpdateFrictionBasedOnWeight()
        {
            if (physicMaterial != null)
            {
                float friction = Mathf.Clamp(dragWeight / 10f, 0.1f, 1.0f); // Scale friction with weight
                physicMaterial.dynamicFriction = friction;
                physicMaterial.staticFriction = friction;
                Debug.Log($"Friction updated for {gameObject.name}: {friction}");
            }
        }
        public void ResetPosition()
        {
            transform.position = Vector3.zero;
            Debug.Log($"Object position reset to origin: {gameObject.name}");
        }
    }
}
