using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    [AddComponentMenu("Character Controller Pro/Demo/Character/AI/Wander Behaviour")]
    public class AIWanderBehaviour : CharacterAIBehaviour
    {
        [Min(0f)]
        [SerializeField]
        float minRandomMagnitude = 1f;

        [Min(0f)]
        [SerializeField]
        float maxRandomMagnitude = 1f;

        [Min(0f)]
        [SerializeField]
        float minRandomYawAngle = 100f;

        [Min(0f)]
        [SerializeField]
        float maxRandomYawAngle = 280f;

        [Min(0f)]
        [SerializeField]
        float waitSeconds = 3f;

        float timer = 0f;

        Vector3 initialPosition = default(Vector3);

        Vector3 target = default(Vector3);

        void OnValidate()
        {
            if (minRandomMagnitude > maxRandomMagnitude)
                minRandomMagnitude = maxRandomMagnitude;

            if (maxRandomMagnitude < minRandomMagnitude)
                maxRandomMagnitude = minRandomMagnitude;

            if (minRandomYawAngle > maxRandomYawAngle)
                minRandomYawAngle = maxRandomYawAngle;

            if (maxRandomYawAngle < minRandomYawAngle)
                maxRandomYawAngle = minRandomYawAngle;
        }



        public override void EnterBehaviour(float dt)
        {
            initialPosition = transform.position;

            target = initialPosition + transform.forward * Random.Range(minRandomMagnitude, maxRandomMagnitude);
            timer = 0f;
        }

        public override void UpdateBehaviour(float dt)
        {

            if (timer >= waitSeconds)
            {
                timer = 0f;

                SetTarget();

            }
            else
            {
                timer += dt;
            }

            float distanceToTarget = (target - CharacterActor.Position).magnitude;

            if (distanceToTarget > 0.5f)
                SetMovementAction(target - CharacterActor.Position);
            else
                characterActions.Reset();
        }


        void SetTarget()
        {
            Vector3 centerToTargetDir = target - initialPosition;
            centerToTargetDir.Normalize();

            centerToTargetDir = Quaternion.Euler(0, Random.Range(minRandomYawAngle, maxRandomYawAngle), 0f) * centerToTargetDir;

            target = initialPosition + centerToTargetDir * Random.Range(minRandomMagnitude, maxRandomMagnitude);
        }

    }

}
