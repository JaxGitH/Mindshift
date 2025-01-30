using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;

namespace Lightbug.CharacterControllerPro.Demo
{

    [AddComponentMenu("Character Controller Pro/Demo/Character/States/Ledge Hanging")]
    public class LedgeHanging : CharacterState
    {

        [Header("Filter")]

        [SerializeField]
        protected LayerMask layerMask = 0;

        [SerializeField]
        protected bool filterByTag = false;

        [SerializeField]
        protected string tagName = "Untagged";

        [SerializeField]
        protected bool detectRigidbodies = false;

        [Header("Detection")]

        [SerializeField]
        protected bool groundedDetection = false;

        [Tooltip("How far the hands are from the character along the forward direction.")]
        [Min(0f)]
        [SerializeField]
        protected float forwardDetectionOffset = 0.5f;
        
        [Tooltip("How far the hands are from the character along the up direction.")]
        [Min(0.05f)]
        [SerializeField]
        protected float upwardsDetectionOffset = 1.8f;

        [Min(0.05f)]
        [SerializeField]
        protected float separationBetweenHands = 1f;

        [Tooltip("The distance used by the raycast methods.")]
        [Min(0.05f)]
        [SerializeField]
        protected float ledgeDetectionDistance = 0.05f;

        [Header("Offset")]

        [SerializeField]
        protected float verticalOffset = 0f;

        [SerializeField]
        protected float forwardOffset = 0f;

        [Header("Movement")]

        public float ledgeJumpVelocity = 10f;

        [SerializeField]
        protected bool autoClimbUp = true;

        [Tooltip("If the previous state (\"fromState\") is contained in this list the autoClimbUp flag will be triggered.")]
        [SerializeField]
        protected CharacterState[] forceAutoClimbUpStates = null;

        [Header("Animation")]

        [SerializeField]
        protected string topUpParameter = "TopUp";



        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        protected const float MaxLedgeVerticalAngle = 50f;


        public enum LedgeHangingState
        {
            Idle,
            TopUp
        }

        protected LedgeHangingState state;


        protected bool forceExit = false;
        protected bool forceAutoClimbUp = false;


        protected override void Awake()
        {
            base.Awake();

        }

        protected override void Start()
        {
            base.Start();

            if (CharacterActor.Animator == null)
            {
                Debug.Log("The LadderClimbing state needs the character to have a reference to an Animator component. Destroying this state...");
                Destroy(this);
            }

        }

        public override void CheckExitTransition()
        {
            if (forceExit)
                CharacterStateController.EnqueueTransition<NormalMovement>();

        }

        HitInfo leftHitInfo = new HitInfo();
        HitInfo rightHitInfo = new HitInfo();


        public override bool CheckEnterTransition(CharacterState fromState)
        {
            if (!groundedDetection && CharacterActor.IsAscending)
                return false;

            if (!groundedDetection && CharacterActor.IsGrounded)
                return false;

            if (!IsValidLedge(CharacterActor.Position))
                return false;


            return true;
        }

        Vector3 initialPosition;

        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            forceExit = false;
            initialPosition = CharacterActor.Position;
            CharacterActor.alwaysNotGrounded = true;
            CharacterActor.Velocity = Vector3.zero;
            CharacterActor.IsKinematic = true;

            // Set the size as the default one (CharacterBody component)
            CharacterActor.SetSize(CharacterActor.DefaultBodySize, CharacterActor.SizeReferenceType.Top);
            
            // Look towards the wall
            CharacterActor.SetYaw(Vector3.ProjectOnPlane(-CharacterActor.WallContact.normal, CharacterActor.Up));

            Vector3 referencePosition = 0.5f * (leftHitInfo.point + rightHitInfo.point);
            Vector3 headToReference = referencePosition - CharacterActor.Top;
            Vector3 correction = Vector3.Project(headToReference, CharacterActor.Up) +
                verticalOffset * CharacterActor.Up +
                forwardOffset * CharacterActor.Forward;

            CharacterActor.Position = CharacterActor.Position + correction;

            state = LedgeHangingState.Idle;

            // Determine if the character should skip the "hanging" state and go directly to the "climbing" state.
            for (int i = 0; i < forceAutoClimbUpStates.Length; i++)
            {
                CharacterState state = forceAutoClimbUpStates[i];
                if (fromState == state)
                {
                    forceAutoClimbUp = true;
                    break;
                }
            }
        }

        public override void ExitBehaviour(float dt, CharacterState toState)
        {
            CharacterActor.IsKinematic = false;
            CharacterActor.alwaysNotGrounded = false;
            forceAutoClimbUp = false;

            if (ledgeJumpFlag)
            {
                ledgeJumpFlag = false;

                CharacterActor.Position = initialPosition;
                CharacterActor.Velocity = CharacterActor.Up * ledgeJumpVelocity;
            }
            else
            {
                CharacterActor.Velocity = Vector3.zero;
            }
        }

        bool CheckValidClimb()
        {
            HitInfoFilter ledgeHitInfoFilter = new HitInfoFilter(layerMask, false, true);
            bool overlap = CharacterActor.CharacterCollisions.CheckOverlap(
                (leftHitInfo.point + rightHitInfo.point) / 2f,
                CharacterActor.StepOffset,
                in ledgeHitInfoFilter
            );

            return !overlap;
        }

        bool ledgeJumpFlag = false;

        public override void UpdateBehaviour(float dt)
        {

            switch (state)
            {

                case LedgeHangingState.Idle:

                    if (CharacterActions.jump.Started)
                    {
                        forceExit = true;
                        ledgeJumpFlag = true;
                    }
                    else if (CharacterActions.movement.Up || autoClimbUp || forceAutoClimbUp)
                    {
                        if (CheckValidClimb())
                        {
                            state = LedgeHangingState.TopUp;

                            // Root motion
                            CharacterActor.SetUpRootMotion(
                                true,
                                PhysicsActor.RootMotionVelocityType.SetVelocity,
                                false
                            );


                            CharacterActor.Animator.SetTrigger(topUpParameter);
                        }


                    }
                    else if (CharacterActions.movement.Down)
                    {
                        forceExit = true;
                    }

                    break;

                case LedgeHangingState.TopUp:

                    if (CharacterActor.Animator.GetCurrentAnimatorStateInfo(0).IsName("Exit"))
                    {
                        forceExit = true;
                        CharacterActor.ForceGrounded();
                    }


                    break;
            }


        }



        bool IsValidLedge(Vector3 characterPosition)
        {
            if (!CharacterActor.WallCollision)
                return false;

            DetectLedge(
                characterPosition,
                out leftHitInfo,
                out rightHitInfo
            );

            if (!leftHitInfo.hit || !rightHitInfo.hit)
                return false;
                        
            if (filterByTag)
                if (!leftHitInfo.transform.CompareTag(tagName) || !rightHitInfo.transform.CompareTag(tagName))
                    return false;

            Vector3 interpolatedNormal = Vector3.Normalize(leftHitInfo.normal + rightHitInfo.normal);
            float ledgeAngle = Vector3.Angle(CharacterActor.Up, interpolatedNormal);
            if (ledgeAngle > MaxLedgeVerticalAngle)
                return false;

            return true;
        }


        void DetectLedge(Vector3 position, out HitInfo leftHitInfo, out HitInfo rightHitInfo)
        {
            HitInfoFilter ledgeHitInfoFilter = new HitInfoFilter(layerMask, !detectRigidbodies, true);
            leftHitInfo = new HitInfo();
            rightHitInfo = new HitInfo();

            Vector3 forwardDirection = CharacterActor.WallCollision ? -CharacterActor.WallContact.normal : CharacterActor.Forward;
            Vector3 sideDirection = Vector3.Cross(CharacterActor.Up, forwardDirection);

            // Check if there is an object above
            Vector3 upDetection = position + CharacterActor.Up * (upwardsDetectionOffset);

            CharacterActor.PhysicsComponent.Raycast(
                out HitInfo auxHitInfo,
                CharacterActor.Center,
                upDetection - CharacterActor.Center,
                in ledgeHitInfoFilter
            );


            if (auxHitInfo.hit)
                return;

            Vector3 middleOrigin = upDetection + forwardDirection * (forwardDetectionOffset);

            Vector3 leftOrigin = middleOrigin - sideDirection * (separationBetweenHands / 2f);
            Vector3 rightOrigin = middleOrigin + sideDirection * (separationBetweenHands / 2f);

            CharacterActor.PhysicsComponent.Raycast(
                out leftHitInfo,
                leftOrigin,
                -CharacterActor.Up * ledgeDetectionDistance,
                in ledgeHitInfoFilter
            );


            CharacterActor.PhysicsComponent.Raycast(
                out rightHitInfo,
                rightOrigin,
                -CharacterActor.Up * ledgeDetectionDistance,
                in ledgeHitInfoFilter
            );



        }



#if UNITY_EDITOR

        CharacterBody characterBody = null;

        void OnValidate()
        {
            characterBody = this.GetComponentInBranch<CharacterBody>();
        }

        void OnDrawGizmos()
        {
            Vector3 forwardDirection = transform.forward;

            if (characterBody != null)
                if (characterBody.Is2D)
                    forwardDirection = transform.right;

            Vector3 sideDirection = Vector3.Cross(transform.up, forwardDirection);
            Vector3 middleOrigin = transform.position + transform.up * (upwardsDetectionOffset) + forwardDirection * (forwardDetectionOffset);
            Vector3 leftOrigin = middleOrigin - sideDirection * (separationBetweenHands / 2f);
            Vector3 rightOrigin = middleOrigin + sideDirection * (separationBetweenHands / 2f);

            CustomUtilities.DrawArrowGizmo(leftOrigin, leftOrigin - transform.up * ledgeDetectionDistance, Color.red, 0.15f);
            CustomUtilities.DrawArrowGizmo(rightOrigin, rightOrigin - transform.up * ledgeDetectionDistance, Color.red, 0.15f);
        }

#endif

    }

}

