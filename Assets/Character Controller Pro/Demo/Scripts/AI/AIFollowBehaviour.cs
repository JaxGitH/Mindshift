using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

    [AddComponentMenu("Character Controller Pro/Demo/Character/AI/Follow Behaviour")]
    public class AIFollowBehaviour : CharacterAIBehaviour
    {

        [Tooltip("The target transform used by the follow behaviour.")]
        [SerializeField]
        Transform followTarget = null;

        [Tooltip("Desired distance to the target. if the distance to the target is less than this value the character will not move.")]
        [SerializeField]
        float reachDistance = 3f;

        [Tooltip("The wait time between actions updates.")]
        [Min(0f)]
        [SerializeField]
        float refreshTime = 0.65f;

        float timer = 0f;

        NavMeshPath navMeshPath = null;

        protected CharacterStateController stateController = null;

        protected override void Awake()
        {
            base.Awake();

            stateController = this.GetComponentInBranch<CharacterActor, CharacterStateController>();
            stateController.MovementReferenceMode = MovementReferenceParameters.MovementReferenceMode.World;
        }

        void OnEnable()
        {
            navMeshPath = new NavMeshPath();
            ;
        }

        public override void EnterBehaviour(float dt)
        {
            timer = refreshTime;
        }

        public override void UpdateBehaviour(float dt)
        {
            if (timer >= refreshTime)
            {
                timer = 0f;
                UpdateFollowTargetBehaviour(dt);
            }
            else
            {
                timer += dt;
            }

        }

        // Follow Behaviour --------------------------------------------------------------------------------------------------

        /// <summary>
        /// Sets the target to follow (only for the follow behaviour).
        /// </summary>
        public void SetFollowTarget(Transform followTarget, bool forceUpdate = true)
        {
            this.followTarget = followTarget;

            if (forceUpdate)
                timer = refreshTime + Mathf.Epsilon;
        }

        void UpdateFollowTargetBehaviour(float dt)
        {
            if (followTarget == null)
                return;

            characterActions.Reset();

            NavMesh.CalculatePath(transform.position, followTarget.position, NavMesh.AllAreas, navMeshPath);

            if (navMeshPath.corners.Length < 2)
                return;

            Vector3 path = navMeshPath.corners[1] - navMeshPath.corners[0];

            bool isDirectPath = navMeshPath.corners.Length == 2;
            if (isDirectPath && path.magnitude <= reachDistance)
                return;


            if (navMeshPath.corners.Length > 1)
                SetMovementAction(path);

        }



    }

}
