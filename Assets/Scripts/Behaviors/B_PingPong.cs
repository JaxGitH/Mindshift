using UnityEngine;

public class B_PingPong : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool moveHorizontally = true; // Toggle side-to-side movement
    public bool moveVertically = false;  // Toggle up-and-down movement
    public float speed = 5f;             // Movement speed
    public float acceleration = 10f;     // Acceleration towards the target velocity
    public float maxVelocity = 5f;       // Prevents excessive speed
    public float moveDistance = 5f;      // Distance before switching direction

    private Rigidbody rb;
    private Vector3 startPosition;
    private int directionX = 1; // 1 = Right, -1 = Left
    private int directionY = 1; // 1 = Up, -1 = Down

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevents unwanted rotation
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        Vector3 targetVelocity = rb.velocity; // Keep current velocity

        // Handle horizontal movement
        if (moveHorizontally)
        {
            if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
            {
                directionX *= -1; // Flip direction
            }
            float targetVelocityX = directionX * speed;
            float velocityDifferenceX = targetVelocityX - rb.velocity.x;
            float forceX = velocityDifferenceX * acceleration;
            rb.AddForce(new Vector3(forceX, 0, 0), ForceMode.Acceleration);
        }

        // Handle vertical movement
        if (moveVertically)
        {
            if (Mathf.Abs(transform.position.y - startPosition.y) >= moveDistance)
            {
                directionY *= -1; // Flip direction
            }
            float targetVelocityY = directionY * speed;
            float velocityDifferenceY = targetVelocityY - rb.velocity.y;
            float forceY = velocityDifferenceY * acceleration;
            rb.AddForce(new Vector3(0, forceY, 0), ForceMode.Acceleration);
        }
    }
}
