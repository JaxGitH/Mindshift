using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

    [System.Serializable]
    public class MovementReferenceParameters
    {
        [Tooltip("Select what type of movement reference the player should be using. Should the character use its own transform, the world coordinates, or an external transform?")]
        public MovementReferenceMode movementReferenceMode = MovementReferenceMode.World;

        [Tooltip("The Transform component used by the \"External\" movement reference.")]
        public Transform externalReference = null;

        CharacterActor characterActor = null;

        public enum MovementReferenceMode
        {
            World,
            External,
            Character
        }

        /// <summary>
        /// Gets a vector that is the product of the input axes (taken from the character actions) and the movement reference. 
        /// The magnitude of this vector is always less than or equal to 1.
        /// </summary>
        public Vector3 InputMovementReference { get; private set; }

        /// <summary>
        /// Forward vector used by the movement reference.
        /// </summary>
        public Vector3 MovementReferenceForward { get; private set; }


        /// <summary>
        /// Right vector used by the movement reference.
        /// </summary>
        public Vector3 MovementReferenceRight { get; private set; }

        Vector3 characterInitialForward;
        Vector3 characterInitialRight;

        public void Initialize(CharacterActor characterActor)
        {
            if (characterActor == null)
            {
                Debug.Log("CharacterActor component is null!");
                return;
            }

            this.characterActor = characterActor;
            characterInitialForward = this.characterActor.Forward;
            characterInitialRight = this.characterActor.Right;

        }

        public void UpdateData(Vector2 movementInput)
        {
            UpdateMovementReferenceData();

            if (characterActor.Is2D)
            {
                InputMovementReference = CustomUtilities.Multiply(MovementReferenceRight, movementInput.x);
            }
            else
            {
                Vector3 inputMovementReference = CustomUtilities.Multiply(MovementReferenceRight, movementInput.x) +
                    CustomUtilities.Multiply(MovementReferenceForward, movementInput.y);

                InputMovementReference = Vector3.ClampMagnitude(inputMovementReference, 1f);
            }

            // Debug ---------------------------------------------
            // Debug.DrawRay( characterActor.Position , MovementReferenceForward * 2f , Color.blue );
            // Debug.DrawRay( characterActor.Position , MovementReferenceRight * 2f , Color.red );
        }

        void UpdateMovementReferenceData()
        {
            // Forward
            switch (movementReferenceMode)
            {
                case MovementReferenceMode.World:

                    MovementReferenceForward = Vector3.forward;
                    MovementReferenceRight = Vector3.right;

                    break;

                case MovementReferenceMode.Character:

                    MovementReferenceForward = characterInitialForward;
                    MovementReferenceRight = characterInitialRight;
                    break;

                case MovementReferenceMode.External:


                    if (externalReference != null)
                    {
                        // MovementReferenceForward = CustomUtilities.ProjectOnTangent( externalReference.forward , characterActor.GroundStableNormal , characterActor.Up );
                        // MovementReferenceRight = CustomUtilities.ProjectOnTangent( externalReference.right , characterActor.GroundStableNormal , characterActor.Up );
                        MovementReferenceForward = Vector3.Normalize(Vector3.ProjectOnPlane(externalReference.forward, characterActor.Up));
                        MovementReferenceRight = Vector3.Normalize(Vector3.ProjectOnPlane(externalReference.right, characterActor.Up));
                    }
                    else
                        Debug.Log("CharacterStateController: the external reference is null! assign a Transform.");

                    break;
            }


        }
    }

}
