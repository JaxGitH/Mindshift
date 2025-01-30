using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{
    public class ZeroGravity : CharacterState
    {
        [Header("Movement")]
        public float baseSpeed = 10f;
        public float acceleration = 20f;
        public float deceleration = 20f;

        [Header("Pitch")]
        public bool invertPitch = false;
        public float pitchAngularSpeed = 180f;

        [Min(0f)]
        public float pitchLerpAcceleration = 5f;

        [Header("Roll")]
        public bool invertRoll = false;
        public float rollAngularSpeed = 180f;

        [Min(0f)]
        public float rollLerpAcceleration = 5f;

        float pitchModifier = 1f;
        float rollModifier = 1f;
        Vector3 targetVerticalVelocity;
        float pitchValue;
        float rollValue;

        protected override void Awake()
        {
            base.Awake();

            pitchModifier = -(invertPitch ? 1f : -1f);
            rollModifier = invertRoll ? 1f : -1f;
        }

        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            CharacterActor.alwaysNotGrounded = true;
            CharacterActor.UseRootMotion = false;
            CharacterActor.constraintRotation = false;
            targetVerticalVelocity = CharacterActor.VerticalVelocity;
        }

        public override void UpdateBehaviour(float dt)
        {
            ProcessRotation(dt);
            ProcessVelocity(dt);
        }

        private void ProcessRotation(float dt)
        {
            pitchValue = Mathf.Lerp(pitchValue, pitchModifier * CharacterActions.pitch.value * pitchAngularSpeed * dt, pitchLerpAcceleration * dt);
            rollValue = Mathf.Lerp(rollValue, rollModifier * CharacterActions.roll.value * rollAngularSpeed * dt, rollLerpAcceleration * dt);

            CharacterActor.RotatePitch(pitchValue, CharacterActor.Center);
            CharacterActor.RotateRoll(rollValue, CharacterActor.Center);


            Vector3 forward = Vector3.Lerp(CharacterActor.Forward, Vector3.ProjectOnPlane(CharacterStateController.ExternalReference.forward, CharacterActor.Up), 5f * dt);
            CharacterActor.SetYaw(forward);
        }

        private void ProcessVelocity(float dt)
        {
            Vector3 targetVelocity = CharacterStateController.InputMovementReference * baseSpeed;
            CharacterActor.Velocity = Vector3.MoveTowards(CharacterActor.Velocity, targetVelocity, (CharacterActions.movement.Detected ? acceleration : deceleration) * dt);

            if (CharacterActions.jump.value)
            {
                targetVerticalVelocity = CharacterActor.Up * baseSpeed;
                CharacterActor.VerticalVelocity = Vector3.MoveTowards(CharacterActor.VerticalVelocity, targetVerticalVelocity, acceleration * dt);
            }
            else if (CharacterActions.crouch.value)
            {
                targetVerticalVelocity = -CharacterActor.Up * baseSpeed;
                CharacterActor.VerticalVelocity = Vector3.MoveTowards(CharacterActor.VerticalVelocity, targetVerticalVelocity, acceleration * dt);
            }
        }
    }
}