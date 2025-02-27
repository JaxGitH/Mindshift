using UnityEngine;
using UnityEngine.InputSystem;
using Mindshift.CharacterControllerPro.Core;

namespace Mindshift
{
    public class CharacterInputHandler : MonoBehaviour
    {
        public static CharacterInputHandler Instance;

        [SerializeField] private CharacterActor characterActor;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;

        [Header("Joystick Reference")]
        [SerializeField] private Joystick joystick;

        private PlayerInputActions inputActions;
        private Vector2 moveInput;
        private bool isDraggingObject = false;

        private enum InputMode
        {
            None,
            Moving,
            Dragging,
            Clicking
        }

        private InputMode currentMode = InputMode.None;
        private void Update()
        {
        }

        private void Awake()
        {
            characterActor = GetComponent<CharacterActor>();

            inputActions = new PlayerInputActions();

            // ✅ Movement Input - Switches to Moving mode
            inputActions.GamePlay.Move.performed += ctx =>
            {
                moveInput = ctx.ReadValue<Vector2>();
                GameStateManager.Instance.IsKeyboardInputActive = moveInput.magnitude > 0.1f;

                if (!isDraggingObject)
                    SwitchInputMode(InputMode.Moving);
            };

            inputActions.GamePlay.Move.canceled += _ =>
            {
                moveInput = Vector2.zero;
                GameStateManager.Instance.IsKeyboardInputActive = false;

                if (currentMode == InputMode.Moving)
                    SwitchInputMode(InputMode.None);
            };

            // ✅ Left Click Action - Only allowed if no movement input
            inputActions.GamePlay.Click.performed += _ =>
            {
                if (GameStateManager.Instance.CanUseMouseInput() && !IsMovementInputActive())
                {
                    SwitchInputMode(InputMode.Clicking);
                    HandleMouseClick();
                }
            };

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
            if (GameStateManager.Instance.IsDraggingObject) return;

            Vector2 joystickInput = joystick != null && joystick.Direction.magnitude > 0.1f ? joystick.Direction : Vector2.zero;
            Vector2 finalInput = joystickInput.magnitude > 0.1f ? joystickInput : moveInput;

            // Debug logs for joystick detection
            Debug.Log($"[CharacterInputHandler] Joystick Input: {joystickInput} | Keyboard Input: {moveInput}");

            if (finalInput.magnitude > 0.1f && currentMode != InputMode.Dragging)
            {
                SwitchInputMode(InputMode.Moving);
            }
            else if (finalInput.magnitude <= 0.1f && currentMode == InputMode.Moving)
            {
                SwitchInputMode(InputMode.None);
            }

            switch (currentMode)
            {
                case InputMode.Moving:
                    MoveCharacter(finalInput);
                    break;
                case InputMode.Dragging:
                    // Dragging is handled separately in drag scripts.
                    break;
                case InputMode.Clicking:
                    // Clicking is an instant action.
                    break;
                case InputMode.None:
                    StopCharacter();
                    break;
            }
        }

        // ✅ Switches between different input modes
        private void SwitchInputMode(InputMode newMode)
        {
            if (currentMode == newMode) return;

            Debug.Log($"[CharacterInputHandler] Switching from {currentMode} to {newMode}");
            currentMode = newMode;
        }

        // ✅ Handles player movement
        private void MoveCharacter(Vector2 input)
        {
            Vector3 movementDirection = new Vector3(input.x, 0, input.y) * moveSpeed;
            characterActor.Velocity = new Vector3(movementDirection.x, characterActor.Velocity.y, movementDirection.z);
        }

        // ✅ Stops the character
        private void StopCharacter()
        {
            characterActor.Velocity = new Vector3(0, characterActor.Velocity.y, 0);
        }

        // ✅ Handles mouse clicks
        private void HandleMouseClick()
        {
            Debug.Log("[CharacterInputHandler] Mouse Click Processed!");
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
            SwitchInputMode(InputMode.None);
        }
    }
}
