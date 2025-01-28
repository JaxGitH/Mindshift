using Mindshift;
using System.Collections;
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

    [Header("Camera Settings")]
    public Transform CameraTransform; // Assign the camera prefab here in the Inspector

    [Header("Player State")]
    public bool IsAlive = true;

    private Vector3 _moveInput;
    private bool _isSprinting;

    private Transform currentPlatform;

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

        // Assign the camera to follow the Player
        B_CharacterCamera characterCamera = FindObjectOfType<B_CharacterCamera>();
        if (characterCamera != null)
        {
            characterCamera.SetFollowTransform(this.transform);
        }
        else
        {
            Debug.LogError("Character Camera not found!");
        }
    }

    private IEnumerator PerformMantle(Vector3 mantleTarget)
    {
        Debug.Log($"Starting mantle to {mantleTarget}");

        // Disable collision solving during mantling
        Motor.SetMovementCollisionsSolvingActivation(false);

        Vector3 startPosition = transform.position;
        float mantleDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < mantleDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, mantleTarget, elapsed / mantleDuration);
            yield return null;
        }

        // Ensure alignment at the end
        transform.position = mantleTarget;
        Motor.SetMovementCollisionsSolvingActivation(true);

        Debug.Log("Mantle completed!");
    }

    private void Update()
    {
        if (!IsAlive)
        {
            _moveInput = Vector3.zero;
            return;
        }

        HandleInput();

        if (Motor.CheckForMantle(out Vector3 mantleTarget))
        {
            StartCoroutine(PerformMantle(mantleTarget));
        }
    }

    private void HandleInput()
    {
        // Check for keyboard input
        float keyboardHorizontal = Input.GetAxis("Horizontal");

        // Check for joystick input
        float joystickHorizontal = Joystick != null ? Joystick.Horizontal : 0f;

        // Combine inputs, prioritizing joystick if available
        float horizontal = Mathf.Abs(joystickHorizontal) > Mathf.Epsilon ? joystickHorizontal : keyboardHorizontal;

        _moveInput = new Vector3(horizontal, 0f, 0f).normalized;

        // Gather sprint input
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    public void BeforeCharacterUpdate(float deltaTime) { }

    public void AfterCharacterUpdate(float deltaTime) { }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_moveInput.sqrMagnitude > 0f)
        {
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

        float targetSpeed = _isSprinting ? SprintSpeed : WalkSpeed;

        Vector3 targetVelocity = _moveInput * targetSpeed;

        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, deltaTime * 10f);
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        if (Motor.GroundingStatus.GroundCollider != null &&
            Motor.GroundingStatus.GroundCollider.TryGetComponent<PhysicsMover>(out PhysicsMover mover))
        {
            // Add the platform's velocity to the Player's base velocity
            Motor.BaseVelocity += mover.Velocity;
            Debug.Log($"Player is riding platform {mover.name}. Velocity applied: {mover.Velocity}");
        }
    }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void ProcessHitStabilityReport(
        Collider hitCollider,
        Vector3 hitNormal,
        Vector3 hitPoint,
        Vector3 atCharacterPosition,
        Quaternion atCharacterRotation,
        ref HitStabilityReport hitStabilityReport)
    { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void Die()
    {
        IsAlive = false;
        Debug.Log("Player has died.");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<B_Mover>(out B_Mover platform))
        {
            Debug.Log($"Player entered platform {platform.name}");
            transform.SetParent(platform.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<B_Mover>(out B_Mover platform))
        {
            Debug.Log($"Player exited platform {platform.name}");
            transform.SetParent(null);
        }
    }
}