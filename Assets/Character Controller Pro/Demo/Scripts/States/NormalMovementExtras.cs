using UnityEngine;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

    [System.Serializable]
    public class PlanarMovementParameters
    {
        [Min(0f)]
        public float baseSpeedLimit = 6f;

        [Header("Run (boost)")]

        public bool canRun = true;

        [Tooltip("\"Toggle\" will activate/deactivate the action when the input is \"pressed\". On the other hand, \"Hold\" will activate the action when the input is pressed, " +
        "and deactivate it when the input is \"released\".")]
        public InputMode runInputMode = InputMode.Hold;


        [Min(0f)]
        public float boostSpeedLimit = 10f;


        [Header("Stable grounded parameters")]
        public float stableGroundedAcceleration = 50f;
        public float stableGroundedDeceleration = 40f;
        public AnimationCurve stableGroundedAngleAccelerationBoost = AnimationCurve.EaseInOut(0f, 1f, 180f, 2f);

        [Header("Unstable grounded parameters")]
        public float unstableGroundedAcceleration = 10f;
        public float unstableGroundedDeceleration = 2f;
        public AnimationCurve unstableGroundedAngleAccelerationBoost = AnimationCurve.EaseInOut(0f, 1f, 180f, 1f);

        [Header("Not grounded parameters")]
        public float notGroundedAcceleration = 20f;
        public float notGroundedDeceleration = 5f;
        public AnimationCurve notGroundedAngleAccelerationBoost = AnimationCurve.EaseInOut(0f, 1f, 180f, 1f);

        [System.Serializable]
        public struct PlanarMovementProperties
        {
            [Tooltip("How fast the character increses its current velocity.")]
            public float acceleration;

            [Tooltip("How fast the character reduces its current velocity.")]
            public float deceleration;

            [Tooltip("How fast the character reduces its current velocity.")]
            public float angleAccelerationMultiplier;

            public PlanarMovementProperties(float acceleration, float deceleration, float angleAccelerationBoost)
            {
                this.acceleration = acceleration;
                this.deceleration = deceleration;
                this.angleAccelerationMultiplier = angleAccelerationBoost;
            }
        }

    }


    [System.Serializable]
    public class VerticalMovementParameters
    {

        public enum UnstableJumpMode
        {
            Vertical,
            GroundNormal
        }


        [Header("Gravity")]

        [Tooltip("It enables/disables gravity. The gravity value is calculated based on the jump apex height and duration.")]
        public bool useGravity = true;

        [Header("Jump")]

        public bool canJump = true;

        [Space(10f)]

        [Tooltip("The gravity magnitude and the jump speed will be automatically calculated based on the jump apex height and duration. Set this to false if you want to manually " +
        "set those values.")]
        public bool autoCalculate = true;

        [Condition("autoCalculate", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("The height reached at the apex of the jump. The maximum height will depend on the \"jumpCancellationMode\".")]
        [Min(0f)]
        public float jumpApexHeight = 2.25f;

        [Condition("autoCalculate", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Tooltip("The amount of time to reach the \"base height\" (apex).")]
        [Min(0f)]
        public float jumpApexDuration = 0.5f;

        [Condition("autoCalculate", ConditionAttribute.ConditionType.IsFalse, ConditionAttribute.VisibilityType.NotEditable)]
        public float jumpSpeed = 10f;

        [Condition("autoCalculate", ConditionAttribute.ConditionType.IsFalse, ConditionAttribute.VisibilityType.NotEditable)]
        public float gravity = 10f;


        [Space(10f)]

        [Tooltip("Reduces the vertical velocity when the jump action is canceled.")]
        public bool cancelJumpOnRelease = true;

        [Tooltip("How much the vertical velocity is reduced when canceling the jump (0 = no effect , 1 = zero velocity).")]
        [Range(0f, 1f)]
        public float cancelJumpMultiplier = 0.5f;

        [Tooltip("When canceling the jump (releasing the action), if the jump time is less than this value nothing is going to happen. Only when the timer is greater than this \"min time\" the jump will be affected.")]
        public float cancelJumpMinTime = 0.1f;

        [Tooltip("When canceling the jump (releasing the action), if the jump time is less than this value (and greater than the \"min time\") the velocity will be affected.")]
        public float cancelJumpMaxTime = 0.3f;

        [Space(10f)]

        [Tooltip("This will help to perform the jump action after the actual input has been started. This value determines the maximum time between input and ground detection.")]
        [Min(0f)]
        public float preGroundedJumpTime = 0.2f;

        [Tooltip("If the character is not grounded, and the \"not grounded time\" is less or equal than this value, the jump action will be performed as a grounded jump. This is also known as \"coyote time\".")]
        [Min(0f)]
        public float postGroundedJumpTime = 0.1f;

        [Space(10f)]

        [Min(0)]
        [Tooltip("Number of jumps available for the character in the air.")]
        public int availableNotGroundedJumps = 1;

        [Space(10f)]

        public bool canJumpOnUnstableGround = false;


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        // public float GravityMagnitude { get; private set; } = 9.8f;


        public void UpdateParameters()
        {
            if (autoCalculate)
            {
                gravity = (2 * jumpApexHeight) / Mathf.Pow(jumpApexDuration, 2);
                jumpSpeed = gravity * jumpApexDuration;
            }
        }

        public void OnValidate()
        {
            if (autoCalculate)
            {
                gravity = (2 * jumpApexHeight) / Mathf.Pow(jumpApexDuration, 2);
                jumpSpeed = gravity * jumpApexDuration;
            }
            else
            {
                jumpApexDuration = jumpSpeed / gravity;
                jumpApexHeight = gravity * Mathf.Pow(jumpApexDuration, 2) / 2f;

            }
        }

        [Header("Jump Down (One Way Platforms)")]

        public bool canJumpDown = true;

        [Space(10f)]

        public bool filterByTag = false;

        public string jumpDownTag = "JumpDown";

        [Space(10f)]

        [Min(0f)]
        public float jumpDownDistance = 0.05f;

        [Min(0f)]
        public float jumpDownVerticalVelocity = 0.5f;

    }

    [System.Serializable]
    public class CrouchParameters
    {

        public bool enableCrouch = true;

        public bool notGroundedCrouch = false;

        [Tooltip("This multiplier represents the crouch ratio relative to the default height.")]
        [Condition("enableCrouch", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Min(0f)]
        public float heightRatio = 0.75f;

        [Tooltip("How much the crouch action affects the movement speed?.")]
        [Condition("enableCrouch", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [Min(0f)]
        public float speedMultiplier = 0.3f;

        [Tooltip("\"Toggle\" will activate/deactivate the action when the input is \"pressed\". On the other hand, \"Hold\" will activate the action when the input is pressed, " +
        "and deactivate it when the input is \"released\".")]
        [Condition("enableCrouch", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public InputMode inputMode = InputMode.Hold;

        [Tooltip("This field determines an anchor point in space (top, center or bottom) that can be used as a reference during size changes. " +
        "For instance, by using \"top\" as a reference, the character will shrink/grow my moving only the bottom part of the body.")]
        public CharacterActor.SizeReferenceType notGroundedReference = CharacterActor.SizeReferenceType.Top;

        [Min(0f)]
        public float sizeLerpSpeed = 8f;
    }

    [System.Serializable]
    public class LookingDirectionParameters
    {
        public bool changeLookingDirection = true;


        [Header("Lerp properties")]

        public float speed = 10f;

        [Header("Target Direction")]

        public LookingDirectionMode lookingDirectionMode = LookingDirectionMode.Movement;

        [Condition("lookingDirectionMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)LookingDirectionMode.Target)]
        [Space(5f)]
        public Transform target = null;

        [Space(5f)]
        [Condition("lookingDirectionMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)LookingDirectionMode.Movement)]
        public LookingDirectionMovementSource stableGroundedLookingDirectionMode = LookingDirectionMovementSource.Input;

        [Condition("lookingDirectionMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)LookingDirectionMode.Movement)]
        public LookingDirectionMovementSource unstableGroundedLookingDirectionMode = LookingDirectionMovementSource.Velocity;

        [Condition("lookingDirectionMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)LookingDirectionMode.Movement)]
        public LookingDirectionMovementSource notGroundedLookingDirectionMode = LookingDirectionMovementSource.Input;


        public enum LookingDirectionMode
        {
            Movement,
            Target,
            ExternalReference
        }


        public enum LookingDirectionMovementSource
        {
            Velocity,
            Input
        }



    }


    public enum InputMode
    {
        Toggle,
        Hold
    }


}
