using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class B_PingPong : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Vector3 direction = Vector3.right;
    private float distanceTraveled = 0f;
    Vector3 startPos;
    Vector3 endOffset;
    Vector3 endPos;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        startPos = transform.position;
        endPos = startPos + endOffset;
    }

    private void FixedUpdate()
    {
        /*if (Mathf.Abs(startPos.magnitude - endPos.magnitude) <= 0)
        {
            direction *= -1f;
        }*/
        
        rb.AddForce(direction, ForceMode.Force);
    }
}


