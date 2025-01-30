using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

    public enum DashMode
    {
        FacingDirection,
        InputDirection
    }

    [AddComponentMenu("Character Controller Pro/Demo/Character/States/Dash")]
    public class Dash : CharacterState
    {

        [Min(0f)]
        [SerializeField]
        protected float initialVelocity = 12f;

        [Min(0f)]
        [SerializeField]
        protected float duration = 0.4f;

        [SerializeField]
        protected AnimationCurve movementCurve = AnimationCurve.Linear(0, 1, 1, 0);

        [Min(0f)]
        [SerializeField]
        protected int availableNotGroundedDashes = 1;

        [SerializeField]
        protected bool ignoreSpeedMultipliers = false;

        [SerializeField]
        protected bool forceNotGrounded = true;

        [Tooltip("Should the dash stop when we hit an obstacle (wall collision)?")]
        [SerializeField]
        protected bool cancelOnContact = true;


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        protected MaterialController materialController = null;

        protected int airDashesLeft;
        protected float dashCursor = 0;

        protected Vector3 dashDirection = Vector2.right;

        protected bool isDone = true;

        protected float currentSpeedMultiplier = 1f;

        #region Events

        /// <summary>
        /// This event is called when the dash state is entered.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnDashStart;

        /// <summary>
        /// This event is called when the dash action has ended.
        /// 
        /// The direction of the dash is passed as an argument.
        /// </summary>
        public event System.Action<Vector3> OnDashEnd;

        #endregion

        void OnEnable() => CharacterActor.OnGroundedStateEnter += OnGroundedStateEnter;
        void OnDisable() => CharacterActor.OnGroundedStateEnter -= OnGroundedStateEnter;

        public override string GetInfo()
        {
            return "This state is entirely based on particular movement, the \"dash\". This movement is normally a fast impulse along " +
            "the forward direction. In this case the movement can be defined by using an animation curve (time vs velocity)";
        }

        void OnGroundedStateEnter(Vector3 localVelocity) => airDashesLeft = availableNotGroundedDashes;

        bool EvaluateCancelOnContact() => CharacterActor.WallContacts.Count != 0;

        protected override void Awake()
        {
            base.Awake();

            materialController = this.GetComponentInBranch<CharacterActor, MaterialController>();
            airDashesLeft = availableNotGroundedDashes;
        }


        public override bool CheckEnterTransition(CharacterState fromState)
        {
            if (!CharacterActor.IsGrounded && airDashesLeft <= 0)
                return false;

            return true;
        }

        public override void CheckExitTransition()
        {
            if (isDone)
            {
                if (OnDashEnd != null)
                    OnDashEnd(dashDirection);

                CharacterStateController.EnqueueTransition<NormalMovement>();
            }
        }


        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            if (forceNotGrounded)
                CharacterActor.alwaysNotGrounded = true;

            CharacterActor.UseRootMotion = false;

            if (CharacterActor.IsGrounded)
            {

                if (!ignoreSpeedMultipliers)
                {
                    currentSpeedMultiplier = materialController != null ? materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier : 1f;
                }

            }
            else
            {

                if (!ignoreSpeedMultipliers)
                {
                    currentSpeedMultiplier = materialController != null ? materialController.CurrentVolume.speedMultiplier : 1f;
                }

                airDashesLeft--;
            }

            //Set the dash direction
            dashDirection = CharacterActor.Forward;

            ResetDash();

            //Execute the event
            OnDashStart?.Invoke(dashDirection);
        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            if (forceNotGrounded)
                CharacterActor.alwaysNotGrounded = false;
        }

        public override void UpdateBehaviour(float dt)
        {
            Vector3 dashVelocity = initialVelocity * currentSpeedMultiplier * movementCurve.Evaluate(dashCursor) * dashDirection;

            CharacterActor.Velocity = dashVelocity;

            float animationDt = dt / duration;
            dashCursor += animationDt;

            if (dashCursor >= 1)
            {
                isDone = true;
                dashCursor = 0;
            }

        }

        public override void PostUpdateBehaviour(float dt)
        {
            if (cancelOnContact)
                isDone |= EvaluateCancelOnContact();            
        }

        public virtual void ResetDash()
        {
            CharacterActor.Velocity = Vector3.zero;
            isDone = false;
            dashCursor = 0;
        }
    }

}





