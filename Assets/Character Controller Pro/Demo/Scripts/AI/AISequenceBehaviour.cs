using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    [AddComponentMenu("Character Controller Pro/Demo/Character/AI/Sequence Behaviour")]
    public class AISequenceBehaviour : CharacterAIBehaviour
    {
        const float DefaultDelayTime = 0.5f;

        [SerializeField]
        List<CharacterAIAction> actionSequence = new List<CharacterAIAction>();

        // float timer = 0f;    
        float durationWaitTime = 0f;
        float wallHitWaitTime = 0f;

        int currentActionIndex = 0;

        void OnEnable()
        {
            CharacterActor.OnWallHit += OnWallHit;
        }

        void OnDisable()
        {
            CharacterActor.OnWallHit -= OnWallHit;
        }

        public override void EnterBehaviour(float dt)
        {

            currentActionIndex = 0;
            characterActions = actionSequence[currentActionIndex].action;


            if (actionSequence[currentActionIndex].sequenceType == SequenceType.Duration)
            {
                durationWaitTime = actionSequence[currentActionIndex].duration;
            }

        }

        public override void UpdateBehaviour(float dt)
        {
            // Process the timers
            if (wallHitWaitTime > 0)
                wallHitWaitTime = Mathf.Max(0f, wallHitWaitTime - dt);

            if (durationWaitTime > 0)
                durationWaitTime = Mathf.Max(0f, durationWaitTime - dt);


            switch (actionSequence[currentActionIndex].sequenceType)
            {
                case SequenceType.Duration:

                    if (durationWaitTime == 0f)
                        SelectNextSequenceElement();


                    break;
                case SequenceType.OnWallHit:

                    break;
            }

        }



        void SelectNextSequenceElement()
        {

            if (currentActionIndex == (actionSequence.Count - 1))
                currentActionIndex = 0;
            else
                currentActionIndex++;


            characterActions = actionSequence[currentActionIndex].action;
            durationWaitTime = actionSequence[currentActionIndex].duration;


        }



        void OnWallHit(Contact contact)
        {
            if (actionSequence[currentActionIndex].sequenceType != SequenceType.OnWallHit)
                return;

            if (wallHitWaitTime > 0f)
                return;

            bool characterCollision = contact.gameObject.GetComponent<CharacterActor>() != null;
            if (characterCollision)
                return;

            SelectNextSequenceElement();
            wallHitWaitTime = DefaultDelayTime;
        }



    }

}
