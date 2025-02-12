using UnityEngine;

namespace Mindshift
{
    public class B_DragAndDrop : MonoBehaviour
    {
        protected Rigidbody objectRb;
        protected Camera mainCamera;
        protected bool isBeingDragged = false;
        private Vector3 initialPosition;
        private Vector3 offset;

        [Header("Dragging Settings")]
        [SerializeField] protected float dragHoldTime = 0f;
        [SerializeField] protected float longPressThreshold = 1.0f;
        [SerializeField] protected float dragWeight = 1.0f; // Weight affecting drag resistance

        protected virtual void Start()
        {
            objectRb = GetComponent<Rigidbody>();
            mainCamera = Camera.main;
        }

        protected virtual void Update()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
            HandleTouchInput();
#endif
        }

        protected virtual void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    isBeingDragged = true;
                    objectRb.isKinematic = true; // Disable physics while dragging
                    dragHoldTime = 0f;
                }
            }

            if (Input.GetMouseButton(0) && isBeingDragged)
            {
                dragHoldTime += Time.deltaTime;

                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

                // Calculate drag offset with resistance
                float resistanceFactor = Mathf.Log10(dragWeight + 1f);
                Vector3 dragOffset = (worldPosition - transform.position) / resistanceFactor;

                // Clamp drag offset to prevent excessive movement
                float maxDragDistance = 0.5f; // Maximum movement per frame
                dragOffset = Vector3.ClampMagnitude(dragOffset, maxDragDistance);

                Vector3 newPosition = transform.position + dragOffset;
                newPosition.z = transform.position.z; // Lock the Z-axis
                transform.position = newPosition;

                if (dragHoldTime >= longPressThreshold)
                {
                    OnLongPress();
                }

                Debug.Log($"Dragging {gameObject.name} with weight {dragWeight}: resistance factor {resistanceFactor}, offset {dragOffset}");
            }

            if (Input.GetMouseButtonUp(0) && isBeingDragged)
            {
                isBeingDragged = false;
                objectRb.isKinematic = false; // Re-enable physics after dragging
            }
        }

        protected virtual void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Ray ray = mainCamera.ScreenPointToRay(touch.position);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                        {
                            isBeingDragged = true;
                            objectRb.isKinematic = true;
                            dragHoldTime = 0f;
                        }
                        break;

                    case TouchPhase.Moved:
                        if (isBeingDragged)
                        {
                            dragHoldTime += Time.deltaTime;

                            Vector3 touchPosition = touch.position;
                            touchPosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
                            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
                            transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);

                            if (dragHoldTime >= longPressThreshold)
                            {
                                OnLongPress();
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                        if (isBeingDragged)
                        {
                            isBeingDragged = false;
                            objectRb.isKinematic = false;
                        }
                        break;
                }
            }
        }

        protected virtual void OnLongPress()
        {
        }
    }
}