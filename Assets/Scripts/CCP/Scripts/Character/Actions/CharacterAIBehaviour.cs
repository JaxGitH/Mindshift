using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mindshift.CharacterControllerPro.Core;
using Mindshift.Utilities;

namespace Mindshift.CharacterControllerPro.Implementation
{

    public abstract class CharacterAIBehaviour : MonoBehaviour
    {
        public CharacterActions characterActions = new CharacterActions();


        public virtual void EnterBehaviour(float dt) { }
        public abstract void UpdateBehaviour(float dt);
        public virtual void ExitBehaviour(float dt) { }

        public CharacterActor CharacterActor { get; private set; }


        protected virtual void Awake()
        {
            CharacterActor = this.GetComponentInBranch<CharacterActor>();
        }

        protected void SetMovementAction(Vector3 direction)
        {
            Vector3 inputXZ = Vector3.ProjectOnPlane(direction, CharacterActor.Up);
            inputXZ.Normalize();
            inputXZ.y = inputXZ.z;
            inputXZ.z = 0f;

            characterActions.movement.value = inputXZ;
        }
    }

}