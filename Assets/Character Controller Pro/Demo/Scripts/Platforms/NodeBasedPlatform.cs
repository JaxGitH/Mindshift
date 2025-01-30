using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    /// <summary>
    /// A "KinematicPlatform" implementation whose movement and rotation are purely based on nodes. Use this component to create platforms that moves and 
    /// rotate precisely following a predefined path.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Demo/Dynamic Platform/Node Based Platform")]
    public class NodeBasedPlatform : Platform
    {

        public enum SequenceType
        {
            Rewind,
            Loop,
            OneWay
        }


        enum ActionState
        {
            Idle,
            Ready,
            Waiting,
            Working,
            Done
        }


        //[Header("Debug Options")]
        [SerializeField] bool drawHandles = true;
        public bool DrawHandles
        {
            get
            {
                return drawHandles;
            }
        }

        //[Header("Actions")]
        public bool move = true;
        public bool rotate = false;

        [SerializeField]
        List<PlatformNode> actionsList = new List<PlatformNode>();

        public List<PlatformNode> ActionsList
        {
            get
            {
                return actionsList;
            }
        }


        public SequenceType sequenceType;
        public bool positiveSequenceDirection = true;

        [Range(0.1f, 50)]
        [SerializeField]
        float globalSpeedModifier = 1;

        ActionState actionState;

        Vector3 targetPosition;
        Vector3 targetRotation;

        Vector3 startingPosition;
        Vector3 startingRotation;


        bool updateInitialPosition = true;
        public bool UpdateInitialPosition
        {
            get
            {
                return updateInitialPosition;
            }
        }


        Vector3 initialPosition;
        public Vector3 InitialPosition
        {
            get
            {
                return initialPosition;
            }
        }

        float time = 0;

        PlatformNode currentAction = null;

        int currentActionIndex = 0;
        public int CurrentActionIndex
        {
            get
            {
                return currentActionIndex;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            updateInitialPosition = false;
            initialPosition = transform.position;

            actionState = ActionState.Ready;

            currentActionIndex = 0;
            currentAction = actionsList[0];


        }

        void FixedUpdate()
        {
            float dt = Time.deltaTime;

            switch (actionState)
            {
                case ActionState.Idle:

                    break;
                case ActionState.Ready:

                    SetTargets();


                    actionState = ActionState.Working;


                    break;

                case ActionState.Working:

                    time += dt * globalSpeedModifier;
                    if (time >= currentAction.targetTime)
                    {

                        actionState = ActionState.Done;

                        time = 0;
                    }
                    else
                    {
                        if (move)
                            RigidbodyComponent.Move(CalculatePosition());

                        Quaternion rotation = RigidbodyComponent.Rotation;

                        if (rotate)
                            RigidbodyComponent.Rotate(CalculateRotation());

                    }


                    break;
                case ActionState.Done:

                    time = 0;

                    if (positiveSequenceDirection)
                    {

                        if (currentActionIndex != (actionsList.Count - 1))
                        {
                            currentActionIndex++;
                            actionState = ActionState.Ready;
                        }
                        else
                        {
                            switch (sequenceType)
                            {
                                case SequenceType.Loop:

                                    currentActionIndex = 0;
                                    actionState = ActionState.Ready;

                                    break;
                                case SequenceType.Rewind:

                                    currentActionIndex--;
                                    positiveSequenceDirection = false;
                                    actionState = ActionState.Ready;

                                    break;
                                case SequenceType.OneWay:

                                    actionState = ActionState.Idle;

                                    break;
                            }



                        }


                    }
                    else
                    {
                        if (currentActionIndex != 0)
                        {
                            currentActionIndex--;
                            actionState = ActionState.Ready;
                        }
                        else
                        {

                            switch (sequenceType)
                            {
                                case SequenceType.Loop:

                                    currentActionIndex = actionsList.Count - 1;
                                    actionState = ActionState.Ready;

                                    break;
                                case SequenceType.Rewind:

                                    currentActionIndex++;
                                    positiveSequenceDirection = true;
                                    actionState = ActionState.Ready;

                                    break;
                                case SequenceType.OneWay:

                                    actionState = ActionState.Idle;

                                    break;
                            }


                        }
                    }

                    currentAction = actionsList[currentActionIndex];

                    break;
            }

        }


        public override string ToString()
        {
            return "Current Index = " + currentActionIndex + '\n' +
            "State = " + actionState;
        }

        void SetTargets()
        {
            startingPosition = transform.position;
            startingRotation = transform.eulerAngles;

            targetPosition = initialPosition + currentAction.position;
            targetRotation = currentAction.eulerAngles;

        }


        Vector3 CalculatePosition()
        {

            float curveTime = time / currentAction.targetTime;

            Vector3 position = Vector3.Lerp(
                startingPosition,
                this.targetPosition,
                currentAction.movementCurve.Evaluate(curveTime)
            );

            return position;

        }

        Quaternion CalculateRotation()
        {

            float curveTime = time / currentAction.targetTime;

            float curveResult = currentAction.rotationCurve.Evaluate(curveTime);

            Vector3 finalAngle;

            finalAngle.x = Mathf.LerpAngle(startingRotation.x, this.targetRotation.x, curveResult);
            finalAngle.y = Mathf.LerpAngle(startingRotation.y, this.targetRotation.y, curveResult);
            finalAngle.z = Mathf.LerpAngle(startingRotation.z, this.targetRotation.z, curveResult);


            Quaternion rotation = Quaternion.Euler(finalAngle);

            return rotation;

        }


    }

}
