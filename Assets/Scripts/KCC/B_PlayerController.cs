using Mindshift;
using UnityEngine;
using KinematicCharacterController;

public class B_PlayerController : MonoBehaviour, ICharacterController
{
    [Header("Movement Settings")]
    public float WalkSpeed = 5f;
    public float SprintSpeed = 10f;
    public float RotationSpeed = 10f;

    [Header("References")]
    public B_KinematicCharacterMotor Motor;
    public Joystick Joystick;

    [Header("Player State")]
    public bool IsAlive = true;

    private Vector3 _moveInput;
    private bool _isSprinting;

    private void Start()
    {
        if (Motor == null)
        {
            Motor = GetComponent<B_KinematicCharacterMotor>();
        }
        Motor.CharacterController = this;

        if (Joystick == null)
        {
            Debug.LogError("Joystick reference not assigned!");
        }
    }

    private void Update()
    {
        if (!IsAlive)
        {
            // Disable input and movement if the player is dead
            _moveInput = Vector3.zero;
            return;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        // Check for keyboard input
        float keyboardHorizontal = Input.GetAxis("Horizontal");
//        float keyboardVertical = Input.GetAxis("Vertical");

        // Check for joystick input
        float joystickHorizontal = Joystick != null ? Joystick.Horizontal : 0f;
//        float joystickVertical = Joystick != null ? Joystick.Vertical : 0f;

        // Combine inputs, prioritizing joystick if available
        float horizontal = Mathf.Abs(joystickHorizontal) > Mathf.Epsilon ? joystickHorizontal : keyboardHorizontal;
//        float vertical = Mathf.Abs(joystickVertical) > Mathf.Epsilon ? joystickVertical : keyboardVertical;

        _moveInput = new Vector3(horizontal, 0f, 0f).normalized;

        // Gather sprint input
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        // Custom logic before character updates (if needed)
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        // Custom logic after character updates (if needed)
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_moveInput.sqrMagnitude > 0f)
        {
            // Rotate towards movement direction
            Quaternion targetRotation = Quaternion.LookRotation(_moveInput, Vector3.up);
            currentRotation = Quaternion.Slerp(currentRotation, targetRotation, RotationSpeed * deltaTime);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (!IsAlive)
        {
            currentVelocity = Vector3.zero;
            return;
        }

        // Determine target speed
        float targetSpeed = _isSprinting ? SprintSpeed : WalkSpeed;

        // Calculate movement direction in the local space of the motor
        Vector3 targetVelocity = _moveInput * targetSpeed;

        // Apply velocity
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, deltaTime * 10f);
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // Custom grounding logic if needed
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        // Example: Ignore specific colliders if necessary
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // Handle ground hit logic (e.g., effects or sounds)
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        // Handle movement hit logic (e.g., collision effects)
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        // Process stability report if needed
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        // Handle discrete collisions if needed
    }

    public void Die()
    {
        IsAlive = false;
        Debug.Log("Player has died.");
        // Additional logic for death (e.g., triggering animations, game over screen, etc.)
    }
}
