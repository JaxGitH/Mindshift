using UnityEngine;

[AddComponentMenu("Custom Character Controller/Rigidbody Character")]
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class RigidbodyCharacter : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider col;
    private Vector3 customGravity;
    private bool isGrounded;
    private bool disableGroundCheck = false;
    private float lastBounceTime;
    private Vector3 moveDirection;
    private Rigidbody platformRb;
    private Vector3 platformVelocity;

    [Header("Rigidbody Settings")]
    [SerializeField] private RigidbodyInterpolation interpolation;
    [SerializeField] private CollisionDetectionMode collisionDetectionMode;
    [SerializeField] private FreezePosition freezePosition;
    [SerializeField] private bool isKinematic = false;
    [SerializeField] private float mass = 3f;
    [SerializeField] private float linearDrag = 0f;
    [SerializeField] private float angularDrag = 0.05f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool rotateForwardDirection = true;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool allowAirControl = true;
    [SerializeField, Range(0f,1f)] private float airControlFactor = 0.1f;

    [Header("Jump & Gravity")]
    [SerializeField] private bool gravityOn = true;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float groundCheckCooldown = 0.2f;

    [Header("Ground Detection")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Step Handling")]
    [SerializeField] private float stepHeight = 0.5f;
    [SerializeField] private float stepCheckDistance = 0.5f;
    [SerializeField] private float stepSmooth = 0.1f;

    [Header("Slope Handling")]
    [SerializeField, Range(1f, 89f)] private float slopeLimit = 55f;

    [Header("Moving Platform Support")]
    [SerializeField] private LayerMask dynamicGroundLayerMask;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        rb.isKinematic = isKinematic;
        rb.useGravity = gravityOn;
        rb.mass = mass;
        rb.drag = linearDrag;
        rb.angularDrag = angularDrag;
        rb.collisionDetectionMode = collisionDetectionMode;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (freezePosition.x) rb.constraints |= RigidbodyConstraints.FreezePositionX;
        if (freezePosition.y) rb.constraints |= RigidbodyConstraints.FreezePositionY;
        if (freezePosition.z) rb.constraints |= RigidbodyConstraints.FreezePositionZ;
    }

    private void Awake()
    {
        customGravity = Vector3.down * gravityScale * 9.81f;
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    public void DisableGroundCheckTemporarily()
    {
        disableGroundCheck = true;
        isGrounded = false;
        Invoke(nameof(EnableGroundCheck), groundCheckCooldown);
    }

    private void EnableGroundCheck()
    {
        disableGroundCheck = false;
    }

    private void CheckGrounded()
    {
        if (disableGroundCheck) return;

        Vector3 sphereOrigin = transform.position + Vector3.up * (col.radius * 0.9f + 0.05f);
        isGrounded = Physics.SphereCast(sphereOrigin, col.radius * 0.9f, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);

        if (!isGrounded)
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hit, groundCheckDistance + 0.1f, groundLayer);

        if (isGrounded && hit.collider.attachedRigidbody)
        {
            platformRb = hit.collider.attachedRigidbody;
            platformVelocity = platformRb.velocity;
        }
        else
        {
            platformRb = null;
        }
    }

    public void MoveCharacter(float inputX, float inputZ)
    {
        moveDirection = cameraTransform != null ?
            (cameraTransform.forward * inputZ + cameraTransform.right * inputX).normalized
            : new Vector3(inputX, 0, inputZ).normalized;

        if (isGrounded)
        {
            Vector3 moveVelocity = moveDirection * moveSpeed;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer)
                && Vector3.Angle(hit.normal, Vector3.up) < slopeLimit)
            {
                moveVelocity = Vector3.ProjectOnPlane(moveVelocity, hit.normal);
            }

            rb.velocity = moveVelocity + platformVelocity;

            HandleStepping(); // Call step handling
        }
        else if (allowAirControl)
        {
            Vector3 airVelocity = moveDirection * moveSpeed * airControlFactor;
            rb.velocity = new Vector3(airVelocity.x, rb.velocity.y, airVelocity.z);
        }

        AutoRotate();
    }


    private void HandleStepping()
    {
        if (moveDirection == Vector3.zero) return; // Don't step if not moving

        Vector3 footPosition = transform.position + Vector3.up * 0.05f;
        Vector3 stepPosition = footPosition + Vector3.up * stepHeight;

        // Check if there's an obstacle in front
        bool lowerRayHit = Physics.Raycast(footPosition, moveDirection, out RaycastHit lowerHit, stepCheckDistance, groundLayer);
        bool upperRayHit = Physics.Raycast(stepPosition, moveDirection, out RaycastHit upperHit, stepCheckDistance, groundLayer);

        if (lowerRayHit && !upperRayHit)
        {
            float stepHeightDifference = lowerHit.point.y - transform.position.y;
            if (stepHeightDifference > 0.05f && stepHeightDifference <= stepHeight) // Ignore micro-steps
            {
                rb.MovePosition(rb.position + new Vector3(0, stepHeightDifference * stepSmooth, 0));
            }
        }
    }


    public void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            isGrounded = false;
        }
    }

    public void AutoRotate()
    {
        if (!rotateForwardDirection || moveDirection == Vector3.zero) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z)), Time.deltaTime * rotationSpeed);
    }
}

[System.Serializable]
public class FreezePosition
{
    public bool x, y, z;
}
