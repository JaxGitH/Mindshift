using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

    /// <summary>
    /// A "KinematicPlatform" implementation whose movement and rotation is defined by an action (movement and/or rotation).
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Demo/Dynamic Platform/Action Based Platform")]
    public class ActionBasedPlatform : Platform
    {
        [SerializeField]
        protected MovementAction movementAction = new MovementAction();

        [SerializeField]
        protected RotationAction rotationAction = new RotationAction();

        void Start()
        {
            movementAction.Initialize(transform);
            rotationAction.Initialize(transform);
        }

        void FixedUpdate()
        {
            float dt = Time.deltaTime;

            Vector3 position = RigidbodyComponent.Position;
            Quaternion rotation = RigidbodyComponent.Rotation;

            movementAction.Tick(dt, ref position);
            rotationAction.Tick(dt, ref position, ref rotation);
            RigidbodyComponent.MoveAndRotate(position, rotation);
        }
    }

}
