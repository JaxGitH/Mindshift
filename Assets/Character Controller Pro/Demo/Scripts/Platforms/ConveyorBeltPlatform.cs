using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{
    public class ConveyorBeltPlatform : Platform
    {
        [SerializeField]
        protected MovementAction movementAction = new MovementAction();

        void Start()
        {
            movementAction.Initialize(transform);
            StartCoroutine(PostSimulationUpdate());
        }

        Vector3 preSimulationPosition = Vector3.zero;

        void FixedUpdate()
        {

            float dt = Time.deltaTime;

            preSimulationPosition = RigidbodyComponent.Position;
            Vector3 position = preSimulationPosition;

            movementAction.Tick(dt, ref position);

            RigidbodyComponent.Move(position);
        }

        IEnumerator PostSimulationUpdate()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;

                RigidbodyComponent.Position = preSimulationPosition;
            }
        }

    }

}
