using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{
    /// <summary>
    /// This class is responsable for detecting inputs and managing character actions.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Implementation/Character/Character Brain")]
    [DefaultExecutionOrder(int.MinValue)]
    public class CharacterBrain : MonoBehaviour
    {
        [Tooltip("Indicates when actions should be consumed.\n\n" +
            "FixedUpdate (recommended): use this when the gameplay logic needs to run during FixedUpdate.\n\n" +
            "Update: use this when the gameplay logic needs to run every frame during Update.")]
        public UpdateModeType UpdateMode = UpdateModeType.FixedUpdate;

        [BooleanButton("Brain type", "Player", "AI", true)]
        [SerializeField]
        bool isAI = false;

        [Condition("isAI", ConditionAttribute.ConditionType.IsFalse)]
        [Expand]
        [SerializeField]
        InputHandlerSettings inputHandlerSettings = new InputHandlerSettings();

        [Condition("isAI", ConditionAttribute.ConditionType.IsTrue)]
        [SerializeField]
        CharacterAIBehaviour aiBehaviour = null;

        [Expand]
        [ReadOnly]
        [SerializeField]
        CharacterActions characterActions = new CharacterActions();

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        public enum UpdateModeType { FixedUpdate, Update }
        
        CharacterAIBehaviour currentAIBehaviour = null;
        bool firstUpdateFlag = false;

        /// <summary>
        /// Gets the current brain mode (AI or Human).
        /// </summary>
        public bool IsAI => isAI;

        /// <summary>
        /// Gets the current actions values from the brain.
        /// </summary>
        public CharacterActions CharacterActions => characterActions;               

        /// <summary>
        /// Sets the internal CharacterActions value.
        /// </summary>
        public void SetAction(CharacterActions characterActions) => this.characterActions = characterActions;


        /// <summary>
        /// Sets the type of brain.
        /// </summary>
        public void SetBrainType(bool isAI)
        {
            characterActions.Reset();

            if (isAI)
                SetAIBehaviour(aiBehaviour);

            this.isAI = isAI;
        }

        /// <summary>
        /// Sets the player's input handler.
        /// </summary>
        public void SetInputHandler(InputHandler inputHandler)
        {
            if (inputHandler == null)
                return;
                        
            inputHandlerSettings.InputHandler = inputHandler;
            characterActions.Reset();
        }

        /// <summary>
        /// Sets the AI behaviour type.
        /// </summary>
        public void SetAIBehaviour(CharacterAIBehaviour aiBehaviour)
        {
            if (aiBehaviour == null)
                return;

            currentAIBehaviour?.ExitBehaviour(Time.deltaTime);
            characterActions.Reset();
            currentAIBehaviour = aiBehaviour;
            currentAIBehaviour.EnterBehaviour(Time.deltaTime);
        }

        public void UpdateBrainValues(float dt)
        {
            if (Time.timeScale == 0)
                return;

            if (IsAI)
                UpdateAIBrainValues(dt);
            else
                UpdateHumanBrainValues(dt);
        }

        void UpdateHumanBrainValues(float dt)
        {
            characterActions.SetValues(inputHandlerSettings.InputHandler);
            characterActions.Update(dt);
        }

        void UpdateAIBrainValues(float dt)
        {
            if (currentAIBehaviour != null)
            { 
                currentAIBehaviour.UpdateBehaviour(dt);
                characterActions.SetValues(currentAIBehaviour.characterActions);
            }

            characterActions.Update(dt);
        }

        #region Unity's messages

        protected virtual void Awake()
        {
            characterActions.InitializeActions();
            inputHandlerSettings.Initialize(gameObject);
        }

        protected virtual void OnEnable()
        {
            characterActions.InitializeActions();
            characterActions.Reset();
        }


        protected virtual void OnDisable()
        {
            characterActions.InitializeActions();
            characterActions.Reset();
        }

        void Start()
        {
            SetBrainType(isAI);
        }

        protected virtual void FixedUpdate()
        {
            firstUpdateFlag = true;
            if (UpdateMode == UpdateModeType.FixedUpdate)
            {
                UpdateBrainValues(0f);
            }          
        }

        protected virtual void Update()
        {
            float dt = Time.deltaTime;

            if (UpdateMode == UpdateModeType.FixedUpdate)
            {
                if (firstUpdateFlag)
                {
                    firstUpdateFlag = false;
                    characterActions.Reset();
                }
            }
            else
            {
                characterActions.Reset();
            }
            

            UpdateBrainValues(dt);
        }


        #endregion


    }

}
