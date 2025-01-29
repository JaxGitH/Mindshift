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
    public float Gravity = -9.81f;
    public float JumpForce = 10f;

    [Header("References")]
    public B_KinematicCharacterMotor Motor;
    public Joystick Joystick;
    public Transform CameraTransform;

    [Header("Player State")]
    public bool IsAlive = true;
    private Vector3 _moveInput;
    private bool _isSprinting;
    private bool _isJumping;

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

    private void Update()
    {
        if (!IsAlive) return;

        HandleInput();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Motor.CheckForMantle(out Vector3 mantleTarget))
            {
                StartCoroutine(PerformMantle(mantleTarget));
            }
            else if (Motor.GroundingStatus.IsStableOnGround)
            {
                _isJumping = true;
            }
        }
    }

    private IEnumerator PerformMantle(Vector3 mantleTarget)
    {
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

        transform.position = mantleTarget;
        Motor.SetMovementCollisionsSolvingActivation(true);
    }

    private void HandleInput()
    {
        float keyboardHorizontal = Input.GetAxis("Horizontal");
        float joystickHorizontal = Joystick != null ? Joystick.Horizontal : 0f;

        float horizontal = Mathf.Abs(joystickHorizontal) > Mathf.Epsilon ? joystickHorizontal : keyboardHorizontal;

        _moveInput = new Vector3(horizontal, 0f, 0f).normalized;
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
        if (!IsAlive) return;

        float targetSpeed = _isSprinting ? SprintSpeed : WalkSpeed;
        Vector3 targetVelocity = _moveInput * targetSpeed;

        if (Motor.GroundingStatus.IsStableOnGround)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, deltaTime * 10f);
            _isJumping = false;
        }
        else
        {
            currentVelocity.y += Gravity * deltaTime;

            if (_isJumping)
            {
                currentVelocity.y = JumpForce;
                _isJumping = false;
            }
        }
    }

    public void PostGroundingUpdate(float deltaTime) { }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void Die()
    {
        IsAlive = false;
        Debug.Log("Player has died.");
    }
}
