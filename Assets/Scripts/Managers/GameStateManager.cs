using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

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

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (!IsDraggingObject || !cannotAcceptInput)  // Allow input switching only when not dragging
        {
            DetectInput();
        }

    }

    private void DetectInput()
    {
        bool keyboardInput = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 ||
                             Input.GetButton("Jump") || Input.GetButton("Fire1");

        bool mouseInput = Input.GetMouseButton(0) || Input.touchCount > 0;

        if (keyboardInput && inputPriority == InputPriority.KeyboardJoystick)
        {
            RegisterKeyboardJoystickInput();
        }
        else if (mouseInput && inputPriority == InputPriority.MouseTouch)
        {
            RegisterMouseTouchInput();
        }
        else if (keyboardInput && !isUsingMouseTouch)
        {
            RegisterKeyboardJoystickInput();
        }
        else if (mouseInput && !isUsingKeyboardJoystick)
        {
            RegisterMouseTouchInput();
        }
    }

    public void RegisterKeyboardJoystickInput()
    {
        isUsingKeyboardJoystick = true;
        isUsingMouseTouch = false;
    }

    public void RegisterMouseTouchInput()
    {
        isUsingMouseTouch = true;
        isUsingKeyboardJoystick = false;
    }

    public bool CanUseKeyboardJoystick()
    {
        return !isUsingMouseTouch && (inputPriority == InputPriority.KeyboardJoystick || !isUsingMouseTouch);
    }

    public bool CanUseMouseTouch()
    {
        return !isUsingKeyboardJoystick && (inputPriority == InputPriority.MouseTouch || !isUsingKeyboardJoystick);
    }

    public void SetDraggingState(bool isDragging)
    {
        IsDraggingObject = isDragging;
    }

    public void ResetInput() // Useful if needed to manually reset inputs
    {
        isUsingKeyboardJoystick = false;
        isUsingMouseTouch = false;
    }
}
