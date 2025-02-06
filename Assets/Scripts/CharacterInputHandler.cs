using UnityEngine;
using UnityEngine.InputSystem; // New Input System
using Lightbug.CharacterControllerPro.Core;

public class CharacterInputHandler : MonoBehaviour
{
    private CharacterActor characterActor;
    public Joystick joystick;  // On-screen joystick reference
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private void Awake()
    {
        characterActor = GetComponent<CharacterActor>();
    }

    private void FixedUpdate()
    {
        if (characterActor == null) return;

        // Read joystick input (if joystick exists)
        float horizontalInput = joystick != null ? joystick.Horizontal : 0f;
//        float verticalInput = joystick != null ? joystick.Vertical : 0f;

        // Read keyboard input (WASD / Arrow Keys)
        horizontalInput += Keyboard.current != null ? Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0 : 0;
//        verticalInput += Keyboard.current != null ? Keyboard.current.sKey.isPressed ? -1 : Keyboard.current.wKey.isPressed ? 1 : 0 : 0;

        // Normalize input to prevent diagonal speed boost
        Vector2 moveInput = new Vector2(horizontalInput, 0).normalized;

        // Convert 2D input into a 3D movement direction
        Vector3 movementDirection = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        // Apply movement to CharacterActor
        characterActor.Velocity = new Vector3(movementDirection.x, characterActor.Velocity.y, movementDirection.z);

        // Jump Handling
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (characterActor.IsGrounded)
        {
            characterActor.Velocity += Vector3.up * jumpForce;
        }
    }
}
