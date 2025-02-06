using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

    /// <summary>
    /// Interface used for objects that need to be updated in a frame by frame basis.
    /// </summary>
    public interface IUpdatable
    {
        void PreUpdateBehaviour(float dt);
        void UpdateBehaviour(float dt);
        void PostUpdateBehaviour(float dt);
    }


    /// <summary>
    /// This class handles all the involved states from the character, allowing an organized execution of events. It also contains extra information that may be required and shared between all the states.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Implementation/Character/Character State Controller")]
    public class CharacterStateController : MonoBehaviour
    {
        [Tooltip("The state used to start the state machine. It is necessary for the state to be not-null, active and enabled. Otherwise, " + 
            "the state machine will not run.")]
        [UnityEngine.Serialization.FormerlySerializedAs("currentState")]
        public CharacterState initialState = null;

        [CustomClassDrawer]
        [SerializeField]
        MovementReferenceParameters movementReferenceParameters = new MovementReferenceParameters();

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public MovementReferenceParameters MovementReferenceParameters => movementReferenceParameters;

        /// <summary>
        /// Gets the Animator component associated with the character.
        /// </summary>        
        public Animator Animator => CharacterActor.Animator;

        public bool UseRootMotion
        {
            get => CharacterActor.UseRootMotion;
            set => CharacterActor.UseRootMotion = value;
        }

        public bool UpdateRootPosition
        {
            get => CharacterActor.UpdateRootPosition;
            set => CharacterActor.UpdateRootPosition = value;
        }

        public bool UpdateRootRotation
        {
            get => CharacterActor.UpdateRootRotation;
            set => CharacterActor.UpdateRootRotation = value;
        }


        readonly Dictionary<string, CharacterState> states = new Dictionary<string, CharacterState>();


        /// <summary>
        /// Gets the brain component associated with the state controller.
        /// </summary>
        public CharacterActor CharacterActor { get; private set; }


        /// <summary>
        /// Gets the brain component associated with the state controller.
        /// </summary>
        public CharacterBrain CharacterBrain { get; private set; }


        /// <summary>
        /// This event is called when a state transition occurs. 
        /// 
        /// The "from" and "to" states are passed as arguments.
        /// </summary>
        public event System.Action<CharacterState, CharacterState> OnStateChange;



        /// <summary>
        /// Gets the current state used by the state machine.
        /// </summary>
        public CharacterState CurrentState { get; protected set; }

        /// <summary>
        /// Gets the previous state used by the state machine.
        /// </summary>
        public CharacterState PreviousState { get; protected set; }



        /// <summary>
        /// Searches for a particular state.
        /// </summary>
        public CharacterState GetState(string stateName)
        {
            states.TryGetValue(stateName, out CharacterState state);

            return state;
        }

        /// <summary>
        /// Searches for a particular state.
        /// </summary>
        public CharacterState GetState<T>() where T : CharacterState
        {
            string stateName = typeof(T).Name;
            return GetState(stateName);
        }

        /// <summary>
        /// Adds a particular state to the transition state queue (as a potential transition). The state machine will eventually check if the transition is accepted or rejected 
        /// by the target state (CheckEnterTransition). Call this method from within the CheckExitTransition method. 
        /// </summary>
        /// <example>
        /// For instance, if you need to transition to multiple states.
        /// <code>
        /// if( conditionA )
        /// {	
        /// 	EnqueueTransition<TargetStateA>();
        /// }
        /// else if( conditionB )
        /// {
        /// 	EnqueueTransition<TargetStateB>();
        /// 	EnqueueTransition<TargetStateC>(); 	
        /// }
        /// </code>	
        /// </example>
        public void EnqueueTransition<T>() where T : CharacterState
        {
            CharacterState state = GetState<T>();

            if (state == null)
                return;

            transitionsQueue.Enqueue(state);
        }

        public void EnqueueTransition(CharacterState state)
        {
            if (state == null)
                return;

            transitionsQueue.Enqueue(state);
        }

        public void EnqueueTransitionToPreviousState()
        {
            EnqueueTransition(PreviousState);
        }

        #region MovementReference

        /// <summary>
        /// Gets a vector that is the product of the input axes (taken from the character actions) and the movement reference. 
        /// The magnitude of this vector is always less than or equal to 1.
        /// </summary>
        public Vector3 InputMovementReference => movementReferenceParameters.InputMovementReference;

        public Transform ExternalReference
        {
            get => movementReferenceParameters.externalReference;
            set => movementReferenceParameters.externalReference = value;
        }

        public MovementReferenceParameters.MovementReferenceMode MovementReferenceMode
        {
            get => movementReferenceParameters.movementReferenceMode;
            set => movementReferenceParameters.movementReferenceMode = value;
        }

        /// <summary>
        /// Forward vector used by the movement reference.
        /// </summary>
        public Vector3 MovementReferenceForward => movementReferenceParameters.MovementReferenceForward;


        /// <summary>
        /// Right vector used by the movement reference.
        /// </summary>
        public Vector3 MovementReferenceRight => movementReferenceParameters.MovementReferenceRight;


        #endregion

        /// <summary>
        /// Forces the state machine to transition from the current state to a new one.
        /// </summary>
        public void ForceState(CharacterState state)
        {
            if (state == null)
                return;

            PreviousState = CurrentState;
            CurrentState = state;

            PreviousState.ExitBehaviour(Time.deltaTime, CurrentState);

            if (CanCurrentStateOverrideAnimatorController)
                Animator.runtimeAnimatorController = CurrentState.RuntimeAnimatorController;

            CurrentState.EnterBehaviour(Time.deltaTime, PreviousState);
        }

        /// <summary>
        /// Forces the state machine to transition from the current state to a new one.
        /// </summary>
        public void ForceState<T>() where T : CharacterState
        {
            CharacterState state = GetState<T>();

            if (state == null)
                return;

            ForceState(state);
        }

        void AddStates()
        {
            CharacterState[] statesArray = CharacterActor.GetComponentsInChildren<CharacterState>();
            for (int i = 0; i < statesArray.Length; i++)
            {
                CharacterState state = statesArray[i];
                string stateName = state.GetType().Name;

                // The state is already included, ignore it!
                if (GetState(stateName) != null)
                {
                    Debug.Log("Warning: GameObject " + state.gameObject.name + " has the state " + stateName + " repeated in the hierarchy.");
                    continue;
                }

                states.Add(stateName, state);
            }

        }

        /// <summary>
        /// Sets a flag that resets all the IK weights (hands and feet) during the next OnAnimatorIK call.
        /// </summary>
        public void ResetIKWeights()
        {
            CharacterActor.ResetIKWeights();
        }

        void PreCharacterSimulation(float dt) => CurrentState.PreCharacterSimulation(dt);
        void PostCharacterSimulation(float dt) => CurrentState.PostCharacterSimulation(dt);

        Queue<CharacterState> transitionsQueue = new Queue<CharacterState>();

        bool CheckForTransitions()
        {
            CurrentState.CheckExitTransition();

            CharacterState nextState = null;

            while (transitionsQueue.Count != 0)
            {
                CharacterState thisState = transitionsQueue.Dequeue();
                if (thisState == null)
                    continue;

                if (!thisState.enabled)
                    continue;

                bool success = thisState.CheckEnterTransition(CurrentState);

                if (success)
                {
                    nextState = thisState;

                    OnStateChange?.Invoke(CurrentState, nextState);

                    PreviousState = CurrentState;
                    CurrentState = nextState;

                    return true;
                }
            }

            return false;

        }

        bool CanCurrentStateOverrideAnimatorController => CurrentState.OverrideAnimatorController && Animator != null && CurrentState.RuntimeAnimatorController != null;
        bool machineStarted = false;

        #region Messages

        void Awake()
        {            
            CharacterActor = this.GetComponentInBranch<CharacterActor>();
            CharacterBrain = this.GetComponentInBranch<CharacterActor, CharacterBrain>();

            AddStates();
        }
        
        void OnEnable()
        {
            CharacterActor.OnPreSimulation += PreCharacterSimulation;
            CharacterActor.OnPostSimulation += PostCharacterSimulation;

            if (Animator != null)
                CharacterActor.OnAnimatorIKEvent += OnAnimatorIK;
        }

        void OnDisable()
        {
            CharacterActor.OnPreSimulation -= PreCharacterSimulation;
            CharacterActor.OnPostSimulation -= PostCharacterSimulation;

            if (Animator != null)
                CharacterActor.OnAnimatorIKEvent -= OnAnimatorIK;
        }

        void Start()
        {            
            movementReferenceParameters.Initialize(CharacterActor);
        }

        void FixedUpdate()
        {
            if (!machineStarted)
            {
                if (initialState == null)
                {
                    enabled = false;
                    return;
                }

                CurrentState = initialState;

                if (CharacterActor == null)
                    return;

                if (CurrentState == null)
                    return;

                if (!CurrentState.isActiveAndEnabled)
                    return;

                machineStarted = true;
                CurrentState.EnterBehaviour(0f, CurrentState);

                if (CanCurrentStateOverrideAnimatorController)
                    Animator.runtimeAnimatorController = CurrentState.RuntimeAnimatorController;

            }

            if (CharacterActor == null)
                return;

            if (CurrentState == null)
                return;

            if (!CurrentState.isActiveAndEnabled)
                return;

            if (CharacterBrain != null)
                movementReferenceParameters.UpdateData(CharacterBrain.CharacterActions.movement.value);


            if (!machineStarted)
            {
                CurrentState.EnterBehaviour(0f, CurrentState);
                machineStarted = true;
            }

            bool validTransition = CheckForTransitions();

            // Reset all transitions
            transitionsQueue.Clear();

            float dt = Time.deltaTime;
            if (validTransition)
            {
                PreviousState.ExitBehaviour(dt, CurrentState);

                if (CanCurrentStateOverrideAnimatorController)
                    Animator.runtimeAnimatorController = CurrentState.RuntimeAnimatorController;

                CurrentState.EnterBehaviour(dt, PreviousState);

            }

            CurrentState.PreUpdateBehaviour(dt);
            CurrentState.UpdateBehaviour(dt);
            CurrentState.PostUpdateBehaviour(dt);

        }

        void OnAnimatorIK(int layerIndex)
        {
            if (CurrentState == null)
                return;

            CurrentState.UpdateIK(layerIndex);
        }

        #endregion

    }

}
