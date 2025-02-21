using UnityEngine;

namespace Mindshift
{
    public class B_DragAndDrop : MonoBehaviour
    {
        protected Rigidbody objectRb;
        protected Camera mainCamera;
        protected bool isBeingDragged = false;
        public bool isPlayerContact = false;

        [Header("Dragging Settings")]
        [SerializeField] protected float dragHoldTime = 0f;
        [SerializeField] protected float longPressThreshold = 1.0f;
        [SerializeField] protected float dragWeight = 1.0f;

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
            // ✅ Mouse input is disabled if CharacterInputHandler blocks it
            if (!CharacterInputHandler.Instance.CanUseMouseInput()) return;

            if (Input.GetMouseButtonDown(0) && !isPlayerContact)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                {
                    isBeingDragged = true;
                    objectRb.isKinematic = true;
                    dragHoldTime = 0f;
                    GameStateManager.Instance.RegisterMouseTouchInput();
                }
            }

            if (Input.GetMouseButton(0) && isBeingDragged)
            {
                dragHoldTime += Time.deltaTime;
                Vector3 mousePosition = Input.mousePosition;
                mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                transform.position = new Vector3(worldPosition.x, worldPosition.y, transform.position.z);
            }

            if (Input.GetMouseButtonUp(0) && isBeingDragged)
            {
                isBeingDragged = false;
                objectRb.isKinematic = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerContact = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerContact = false;
            }
        }
    }
}
