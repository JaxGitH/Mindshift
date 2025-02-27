using System.Collections.Generic;
using UnityEngine;
using Mindshift.CharacterControllerPro.Core;
using Mindshift.CharacterControllerPro.Implementation;
using Mindshift.Utilities;

namespace Mindshift.CharacterControllerPro.Demo
{
    public class InputBasedBehaviour : StateMachineBehaviour
    {
        CharacterBrain brain;

        [SerializeField]
        string trigger = "";

        [SerializeField]
        Vector2 movementValue = Vector2.zero;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (brain == null)
                brain = animator.GetComponentInBranch<CharacterActor, CharacterBrain>();
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (brain.CharacterActions.movement.value == movementValue)
            {
                animator.SetTrigger(trigger);
            }
        }
    }
}
