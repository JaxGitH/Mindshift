using UnityEngine;
using UnityEngine.InputSystem;
using Mindshift.CharacterControllerPro.Core;

namespace Mindshift
{
    public class CharacterInputHandler : MonoBehaviour
    {
        public static CharacterInputHandler Instance;

        [SerializeField] private CharacterActor characterActor;
        [SerializeField] private float maxMoveSpeed = 5f;
        [SerializeField] private float moveSpeed = 5f;
        [Range(0f, 1f)]
        [SerializeField] private float inAirControl = 0.5f;
        [SerializeField] private float jumpForce = 10f;

        [Header("Joystick Reference")]
        [SerializeField] private VariableJoystick joystick;

        private PlayerInputActions inputActions;
        private Vector2 moveInput;

        private void Awake()
        {
            characterActor = GetComponent<CharacterActor>();

            inputActions = new PlayerInputActions();

            // ✅ Movement Input - Disables mouse click when active
            inputActions.GamePlay.Move.performed += ctx =>
            {
                moveInput = ctx.ReadValue<Vector2>();
                GameStateManager.Instance.IsKeyboardInputActive = moveInput.magnitude > 0.1f;
            };

            inputActions.GamePlay.Move.canceled += _ =>
            {
                moveInput = Vector2.zero;
                GameStateManager.Instance.IsKeyboardInputActive = false;
            };

            // ✅ Left Click Action - Only allowed if no movement input
            inputActions.GamePlay.Click.performed += _ =>
            {
                if (GameStateManager.Instance.CanUseMouseInput())
                {
                    HandleMouseClick();
                }
            };

            if (joystick == null)
            {
                Debug.LogWarning("VariableJoystick is not assigned in CharacterInputHandler.");
            }

            moveSpeed = maxMoveSpeed;
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

            if (GameStateManager.Instance.IsDraggingObject) return;

            if (characterActor.isKinematic) return;

            Vector2 joystickInput = joystick != null ? joystick.Direction : Vector2.zero;
            Vector2 finalInput = joystickInput.magnitude > 0.1f ? joystickInput : moveInput;

            if (!characterActor.IsGrounded)
            {
                moveSpeed = maxMoveSpeed * inAirControl;
            }
            else moveSpeed = maxMoveSpeed;
            
            Vector3 movementDirection = new Vector3(finalInput.x * moveSpeed, 0, finalInput.y * moveSpeed);

            characterActor.Velocity = new Vector3(movementDirection.x, characterActor.Velocity.y, movementDirection.z);
        }

        // ✅ Mouse Click Handler
        private void HandleMouseClick()
        {
            Debug.Log("Mouse click processed!");
        }

        // ✅ Determine if mouse input should be used based on GameStateManager
        public bool CanUseMouseInput()
        {
            return GameStateManager.Instance.CanUseMouseInput();
        }

        public bool IsPlayerMoving()
        {
            return moveInput.magnitude > 0.1f;
        }

        public bool IsMovementInputActive()
        {
            return moveInput.magnitude > 0.1f || (joystick != null && joystick.Direction.magnitude > 0.1f);
        }

        public void ResetMovement()
        {
            moveInput = Vector2.zero;
            GameStateManager.Instance.IsKeyboardInputActive = false;
        }
    }
}
