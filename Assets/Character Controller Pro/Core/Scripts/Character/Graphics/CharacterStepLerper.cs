using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This component is responsible for smoothing out the graphics-related elements (under the root object) based on the character movement (CharacterActor).
    /// It allows you to modify the position and rotation accordingly, producing a great end result.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Graphics/Step Lerper")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder)]
    public class CharacterStepLerper : CharacterGraphics
    {
        [Tooltip("How fast the step up interpolation is going to be.")]
        [SerializeField]
        float positiveDisplacementSpeed = 20f;

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
        

        Vector3 previousPosition = default;
        bool teleportFlag = false;
        float recoveryTimer = 0f;

        #region Messages
        protected override void OnValidate()
        {
            base.OnValidate();

            CustomUtilities.SetPositive(ref positiveDisplacementSpeed);
            CustomUtilities.SetPositive(ref negativeDisplacementSpeed);
        }

        void Start()
        {
            previousPosition = transform.position;
        }

        void OnEnable() => CharacterActor.OnTeleport += OnTeleport;
        void OnDisable() => CharacterActor.OnTeleport -= OnTeleport;

        void Update()
        {
            if (CharacterActor == null)
            {
                enabled = false;
                return;
            }

            float dt = Time.deltaTime;

            HandleVerticalDisplacement(dt);

            if (teleportFlag)
                teleportFlag = false;
        }

        #endregion

        void OnTeleport(Vector3 position, Quaternion rotation) => teleportFlag = true;

        void HandleVerticalDisplacement(float dt)
        {
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

    }

}




