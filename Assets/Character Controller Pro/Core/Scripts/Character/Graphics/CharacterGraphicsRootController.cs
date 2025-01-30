using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This component is responsible for smoothing out the graphics-related elements (under the root object) based on the character movement and rotation.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Graphics/Graphics Root Controller (obsolete)")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder)]
    public class CharacterGraphicsRootController : CharacterGraphics
    {
        [HelpBox("This component is obsolete. It has been separated into two new components: Step Lerper and Rotation Lerper.", HelpBoxMessageType.Warning)]
        [Tooltip("Whether or not interpolate the rotation of the character.")]
        [SerializeField]
        bool lerpRotation = false;

        [Condition("lerpRotation", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [SerializeField]
        float rotationLerpSpeed = 25f;

        [Space(10f)]

        [Tooltip("Whether or not to interpolate the vertical displacement change of the character. A vertical displacement happens everytime the character " +
        "increase/decrease its vertical position (slopes, step up, step down, etc.). This feature does not work with rigidbodies (if this is required use the new VerticalDisplacementLerper component instead).")]
        [SerializeField]
        bool lerpVerticalDisplacement = true;

        [Tooltip("How fast the step up interpolation is going to be.")]
        [SerializeField]
        float positiveDisplacementSpeed = 10f;

        [Tooltip("How fast the step down interpolation is going to be.")]
        [SerializeField]
        float negativeDisplacementSpeed = 40f;


        [Tooltip("Having a character that is being interpolated all the time is not ideal, especially when walking on slopes, being not grounded, or maybe using a moving platform. " +
        "For those cases, the character should be allowed to smoothly go back to its original local position over time. This field represents the duration of this process (in seconds).")]
        [SerializeField]
        float recoveryDuration = 1f;

        [Tooltip("The maximum speed used for the recovery process (see recoveryDuration tooltip).")]
        [SerializeField]
        float maxRecoverySpeed = 200f;
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        Vector3 previousPosition = default(Vector3);
        Quaternion previousRotation = default(Quaternion);

        Vector3 initialLocalForward = default(Vector3);

        protected override void OnValidate()
        {
            base.OnValidate();

            CustomUtilities.SetPositive(ref rotationLerpSpeed);
            CustomUtilities.SetPositive(ref positiveDisplacementSpeed);
            CustomUtilities.SetPositive(ref negativeDisplacementSpeed);
        }

        void Start()
        {
            initialLocalForward = CharacterActor.transform.InverseTransformDirection(transform.forward);

            previousPosition = transform.position;
            previousRotation = transform.rotation;
        }


        void OnEnable()
        {
            CharacterActor.OnTeleport += OnTeleport;
        }

        void OnDisable()
        {
            CharacterActor.OnTeleport -= OnTeleport;
        }

        bool teleportFlag = false;

        void OnTeleport(Vector3 position, Quaternion rotation)
        {
            teleportFlag = true;
        }


        void Update()
        {
            if (CharacterActor == null)
            {
                this.enabled = false;
                return;
            }

            float dt = Time.deltaTime;

            HandleRotation(dt);
            HandleVerticalDisplacement(dt);

            if (teleportFlag)
                teleportFlag = false;

        }

        float recoveryTimer = 0f;

        void HandleVerticalDisplacement(float dt)
        {
            if (!lerpVerticalDisplacement)
                return;

            if (teleportFlag)
            {
                previousPosition = transform.position;
                transform.position = CharacterActor.Position;
                return;
            }

            Vector3 planarDisplacement = Vector3.ProjectOnPlane(CharacterActor.transform.position - previousPosition, CharacterActor.Up);
            Vector3 verticalDisplacement = Vector3.Project(CharacterActor.transform.position - previousPosition, CharacterActor.Up);

            float groundProbingDisplacement = CharacterActor.transform.InverseTransformVectorUnscaled(CharacterActor.GroundProbingDisplacement).y;

            if (Mathf.Abs(groundProbingDisplacement) < 0.01f)
                recoveryTimer += dt;
            else
                recoveryTimer = 0f;

            // Choose between positive and negative displacement speed based on the vertical displacement.
            bool upwardsLerpDirection = CharacterActor.transform.InverseTransformVectorUnscaled(verticalDisplacement).y >= 0f;
            float displacementTSpeed = upwardsLerpDirection ? positiveDisplacementSpeed : negativeDisplacementSpeed;


            // Calculate the lerp t speed as a function of the recovery timer.
            float lerpTSpeedOutput = Mathf.Min(displacementTSpeed + ((maxRecoverySpeed - displacementTSpeed) / recoveryDuration) * recoveryTimer, maxRecoverySpeed);

            transform.position = previousPosition + planarDisplacement + Vector3.Lerp(Vector3.zero, verticalDisplacement, lerpTSpeedOutput * dt);

            previousPosition = transform.position;
        }


        void HandleRotation(float dt)
        {
            if (!lerpRotation)
                return;

            if (teleportFlag)
            {
                transform.localRotation = Quaternion.identity;
                previousRotation = transform.rotation;

                return;
            }

            transform.rotation = Quaternion.Slerp(previousRotation, CharacterActor.Rotation, rotationLerpSpeed * dt);

            previousRotation = transform.rotation;
        }



    }

}



