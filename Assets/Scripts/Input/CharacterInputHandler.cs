using UnityEngine;
using UnityEngine.InputSystem;
using Mindshift.CharacterControllerPro.Core;
using Mindshift;

public class CharacterInputHandler : MonoBehaviour
{
    [SerializeField] private CharacterActor characterActor;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    [Header("Joystick Reference")]
    [SerializeField] private VariableJoystick joystick; // <-- Reference to VariableJoystick

    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    private void Awake()
    {
        characterActor = GetComponent<CharacterActor>();

        // Initialize Input Actions
        inputActions = new PlayerInputActions();
        inputActions.GamePlay.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.GamePlay.Move.canceled += _ => moveInput = Vector2.zero;

        // Initialize joystick if available
        if (joystick == null)
        {
            Debug.LogWarning("VariableJoystick is not assigned in CharacterInputHandler.");
        }
    }

    private void OnEnable()
    {
        inputActions.GamePlay.Enable();
    }

    private void OnDisable()
    {
        inputActions.GamePlay.Disable();
    }

    private void FixedUpdate()
    {
        if (characterActor == null) return;

        // Use joystick input if it's active, otherwise use InputActions
        Vector2 joystickInput = joystick != null ? joystick.Direction : Vector2.zero;
        Vector2 finalInput = joystickInput.magnitude > 0.1f ? joystickInput : moveInput;

        // Apply Movement
        Vector3 movementDirection = new Vector3(finalInput.x, 0, finalInput.y) * moveSpeed;
        characterActor.Velocity = new Vector3(movementDirection.x, characterActor.Velocity.y, movementDirection.z);
    }

    private void Jump()
    {
        if (characterActor.IsGrounded)
        {
            characterActor.Velocity += Vector3.up * jumpForce;
        }
    }
}
