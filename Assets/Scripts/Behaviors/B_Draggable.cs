using UnityEngine;

public class B_Draggable : MonoBehaviour
{
    [Header("Draggable Settings")]
    [SerializeField] private bool isResettable = true;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 offset;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"B_Draggable: No Rigidbody found on {gameObject.name}");
            enabled = false;
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            DragObject();
        }
    }

    private void OnMouseDown()
    {
        StartDragging();
    }

    private void OnMouseUp()
    {
        StopDragging();
    }

    private void StartDragging()
    {
        if (rb != null)
        {
            isDragging = true;
            rb.isKinematic = true; // Temporarily disable physics for smooth dragging
            offset = transform.position - GetMouseWorldPosition();
        }
    }

    private void StopDragging()
    {
        if (rb != null)
        {
            isDragging = false;
            rb.isKinematic = false; // Re-enable physics when dropped
        }
    }

    private void DragObject()
    {
        Vector3 newPosition = GetMouseWorldPosition() + offset;
        transform.position = newPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
