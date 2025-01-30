using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{


    [AddComponentMenu("Character Controller Pro/Demo/Character/States/Jet Pack")]
    public class JetPack : CharacterState
    {
        [Header("Planar movement")]

        [SerializeField]
        protected float targetPlanarSpeed = 5f;

        [Header("Planar movement")]

        [SerializeField]
        protected float targetVerticalSpeed = 5f;

        [SerializeField]
        protected float duration = 1f;


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        protected Vector3 smoothDampVelocity = Vector3.zero;


        public override string GetInfo()
        {
            return "This state allows the character to imitate a \"JetPack\" type of movement. Basically the character can ascend towards the up direction, " +
            "but also move in the local XZ plane.";
        }

        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            CharacterActor.alwaysNotGrounded = true;
            CharacterActor.UseRootMotion = false;

            smoothDampVelocity = CharacterActor.VerticalVelocity;

        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            CharacterActor.alwaysNotGrounded = false;
        }

        public override void UpdateBehaviour(float dt)
        {

            // Vertical movement
            CharacterActor.VerticalVelocity = Vector3.SmoothDamp(CharacterActor.VerticalVelocity, targetVerticalSpeed * CharacterActor.Up, ref smoothDampVelocity, duration);

            // Planar movement
            CharacterActor.PlanarVelocity = Vector3.Lerp(CharacterActor.PlanarVelocity, targetPlanarSpeed * CharacterStateController.InputMovementReference, 7f * dt);

            // Looking direction
            CharacterActor.SetYaw(CharacterActor.PlanarVelocity);
        }

        public override void CheckExitTransition()
        {
            if (CharacterActor.Triggers.Count != 0)
            {

                if (CharacterActions.interact.Started)
                    CharacterStateController.EnqueueTransition<LadderClimbing>();
            }
            else if (!CharacterActions.jetPack.value)
            {
                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
        }

    }

}
