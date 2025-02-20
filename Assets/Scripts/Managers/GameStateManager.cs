using Mindshift;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mindshift
{
    public class GameStateManager : MonoBehaviour
    {
        public bool IsUsingMouseTouch => isUsingMouseTouch;
        public bool IsUsingKeyboardJoystick => isUsingKeyboardJoystick;

        private PlayerInputActions inputActions;
        public static GameStateManager Instance;

        public enum InputPriority
        {
            KeyboardJoystick,
            MouseTouch
        }

        [Header("Input Settings")]
        public InputPriority inputPriority = InputPriority.KeyboardJoystick;

        private bool isUsingKeyboardJoystick = false;
        private bool isUsingMouseTouch = false;

        public bool IsDraggingObject { get; private set; }
        public bool cannotAcceptInput;

        // ✅ New: Track if keyboard input is active globally
        public bool IsKeyboardInputActive;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            inputActions = new PlayerInputActions();
            inputActions.Enable();
        }

        private void Update()
        {
            if (!IsDraggingObject || !cannotAcceptInput)
            {
                DetectInput();
            }
        }

        public bool IsAnyInputActive()
        {
            return isUsingKeyboardJoystick || isUsingMouseTouch;
        }

        public bool IsMovementInputActive()
        {
            return CharacterInputHandler.Instance != null && CharacterInputHandler.Instance.IsMovementInputActive();
        }

        private void DetectInput()
        {
            bool keyboardInput = inputActions.GamePlay.Move.ReadValue<Vector2>().magnitude > 0.1f;

            // ✅ Track keyboard input globally
            IsKeyboardInputActive = keyboardInput;
        }

        public void RegisterKeyboardJoystickInput()
        {
            isUsingKeyboardJoystick = true;
            isUsingMouseTouch = false;

            if (CharacterInputHandler.Instance != null)
            {
                CharacterInputHandler.Instance.ResetMovement();
            }
        }

        public void RegisterMouseTouchInput()
        {
            isUsingMouseTouch = true;
            isUsingKeyboardJoystick = false;

            if (CharacterInputHandler.Instance != null)
            {
                CharacterInputHandler.Instance.ResetMovement();
            }
        }

        public bool CanUseKeyboardJoystick()
        {
            return !isUsingMouseTouch && !isUsingMouseTouch;
        }

        // ✅ Mouse input is disabled if keyboard input is active
        public bool CanUseMouseInput()
        {
            return !IsKeyboardInputActive && !isUsingKeyboardJoystick;
        }

        public void SetDraggingState(bool isDragging)
        {
            IsDraggingObject = isDragging;
        }

        public void ResetInput()
        {
            isUsingKeyboardJoystick = false;
            isUsingMouseTouch = false;
            IsKeyboardInputActive = false;
        }
    }
}
