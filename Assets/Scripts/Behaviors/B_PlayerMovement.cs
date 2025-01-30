using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class B_PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private float maxSlopeAngle = 45f;


    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 7f; // Force for ledge jump
#pragma warning disable 0414
    [SerializeField] private float ledgeDetectionDistance = 0.7f; // Distance ahead to check for ledges
#pragma warning restore 0414
    [SerializeField] private LayerMask ignoredLayers; // Set layers to ignore in Inspector


    [Header("Step Settings")]
    [SerializeField] private Transform stepChecker; // Assign the Empty GameObject in the Inspector
    [SerializeField] private float stepHeight = 0.5f;
    //[SerializeField] private float footStartHeight = 0.1f; // Manually adjust foot ray start height
    [SerializeField] private float stepSmooth = 5f;

    [Header("Friction & Slope Control")]
    [SerializeField] private PhysicMaterial highFrictionMaterial;
    [SerializeField] private PhysicMaterial lowFrictionMaterial;
    [SerializeField] private float stopDrag = 5f;
    [SerializeField] private float moveDrag = 0f;

    private Rigidbody rb;
    private CapsuleCollider col;
    private float moveInput;
    private bool onSlope = false;
    private Vector3 slopeMoveDirection;
    private Rigidbody platformRb = null;
    private Transform platformTransform = null; // Stores the platform's Transform

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        rb.freezeRotation = true; // Prevent unwanted Rigidbody rotation
    }

    private void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (moveInput != 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0f, moveInput > 0 ? 0f : 180f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        AdjustFriction();
        CheckForLedge();
    }

    private void FixedUpdate()
    {
        if (platformRb != null)
        {
            // Match platform's velocity but allow player input to override
            rb.AddForce(platformRb.velocity, ForceMode.VelocityChange);
            //rb.velocity += platformRb.velocity;
        }
        SlopeCheck();
        MoveCharacter();

        if (Mathf.Abs(moveInput) > 0.1f)
        {
            HandleStepping();
        }

        ApplyPlatformMovement();
    }

    private void MoveCharacter()
    {
        Vector3 movementDirection = new Vector3(moveInput, 0f, 0f).normalized;

        if (onSlope)
        {
            rb.MovePosition(rb.position + slopeMoveDirection * (moveSpeed * Time.fixedDeltaTime));
        }
        else
        {
            rb.velocity = new Vector3(movementDirection.x * moveSpeed, rb.velocity.y, 0f);
        }
    }

    private void SlopeCheck()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, slopeCheckDistance))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle > 0f && slopeAngle <= maxSlopeAngle)
            {
                onSlope = true;
                slopeMoveDirection = Vector3.ProjectOnPlane(new Vector3(moveInput, 0f, 0f), hit.normal);
            }
            else
            {
                onSlope = false;
            }
        }
        else
        {
            onSlope = false;
        }
    }

    private void HandleStepping()
    {
        if (Mathf.Abs(moveInput) < 0.1f) return; // Exit if not moving

        Vector3 moveDirection = new Vector3(Mathf.Sign(moveInput), 0f, 0f);

        // Use `stepChecker.position` instead of `transform.position`
        Vector3 footPosition = stepChecker.position + moveDirection * 0.5f;
        Vector3 stepPosition = footPosition + Vector3.up * stepHeight;

        RaycastHit lowerHit, upperHit;

        bool lowerRayHit = Physics.Raycast(footPosition, moveDirection, out lowerHit, 0.5f);
        bool upperRayHit = Physics.Raycast(stepPosition, moveDirection, out upperHit, 0.5f);

        Debug.DrawRay(footPosition, moveDirection * 0.5f, Color.red);
        Debug.DrawRay(stepPosition, moveDirection * 0.5f, Color.green);

        Debug.Log($"Lower Ray Hit: {lowerRayHit} | Upper Ray Hit: {upperRayHit}");

        if (lowerRayHit && !upperRayHit)
        {
            float stepHeightDifference = lowerHit.point.y + stepChecker.position.y; // Compare to StepChecker instead of Player Position
            Debug.Log($"Step Height Difference: {stepHeightDifference}");

            if (stepHeightDifference <= stepHeight && stepHeightDifference > 0.05f)
            {
                Debug.Log("Step-Up Applied!");
                rb.velocity = new Vector3(rb.velocity.x, stepHeightDifference * stepSmooth, rb.velocity.z);
            }
        }
    }



    private void AdjustFriction()
    {
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            col.material = lowFrictionMaterial;
            rb.drag = moveDrag;
        }
        else
        {
            col.material = highFrictionMaterial;
            rb.drag = stopDrag;
        }
    }

    private void ApplyPlatformMovement()
    {
        if (platformRb != null)
        {
            rb.velocity += platformRb.velocity * Time.fixedDeltaTime;
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;
        if (otherRb != null && !otherRb.isKinematic)
        {
            platformRb = otherRb;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody == platformRb)
        {
            platformRb = null;
        }
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        //Check if the object has a Rigidbody and is NOT kinematic
        Rigidbody detectedRb = collision.gameObject.GetComponent<Rigidbody>();

        if (detectedRb != null && !detectedRb.isKinematic)
        {
            platformRb = detectedRb; // Store platform Rigidbody
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //Remove velocity inheritance when stepping off
        if (collision.rigidbody == platformRb)
        {
            platformRb = null;
        }
    }



    private void CheckForLedge()
    {
        float ledgeDetectionDistance = 0.7f; // Distance ahead to check for ledges
        float groundCheckDistance = 1f; // Distance downward to confirm ground presence

        // Step 1: Check if ground exists directly ahead
        Vector3 ledgeRayStart = transform.position + transform.right * ledgeDetectionDistance;
        RaycastHit hitAhead;
        bool groundAhead = Physics.Raycast(ledgeRayStart, Vector3.down, out hitAhead, groundCheckDistance);

        // Step 2: Check if the player is grounded at their current position
        bool isOnGround = IsGrounded(); // Using the updated IsGrounded() function

        // Step 3: If there is no ground ahead, but the player is currently on the ground, trigger a jump
        if (!groundAhead && isOnGround && moveInput != 0)
        {
            Jump();
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    private bool IsGrounded()
    {
        float checkDistance = 0.1f; // Small distance to detect the ground
        RaycastHit hit;

        // Raycast slightly downward from the player's position
        if (Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance))
        {
            return true; // Ground detected
        }

        return false; // No ground detected
    }
}