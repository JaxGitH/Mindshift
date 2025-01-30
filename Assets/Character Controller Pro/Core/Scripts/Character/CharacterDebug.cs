using UnityEngine;
using UnityEngine.UI;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    /// <summary>
    /// This class is used for debug purposes, mainly to print information on screen about the collision flags, certain values and/or triggering events.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrder.CharacterActorOrder)]
    [AddComponentMenu("Character Controller Pro/Core/Character Debug")]
    public class CharacterDebug : MonoBehaviour
    {
        [SerializeField]
        CharacterActor characterActor = null;

        [Header("Character Info")]
        [SerializeField]
        Text text = null;

        [Header("Events")]

        [SerializeField]
        bool printEvents = false;

        [Header("Stability")]

        [SerializeField]
        Renderer stabilityIndicator;

        [Condition("stabilityIndicator", ConditionAttribute.ConditionType.IsNotNull, ConditionAttribute.VisibilityType.NotEditable)]
        [SerializeField]
        Color stableColor = new Color(0f, 1f, 0f, 0.5f);

        [Condition("stabilityIndicator", ConditionAttribute.ConditionType.IsNotNull, ConditionAttribute.VisibilityType.NotEditable)]
        [SerializeField]
        Color unstableColor = new Color(1f, 0f, 0f, 0.5f);

        int colorID = Shader.PropertyToID("_Color");
        float time = 0f;

        void UpdateCharacterInfoText()
        {
            if (text == null)
                return;

            if (time > 0.2f)
            {
                text.text = characterActor.GetCharacterInfo();
                time = 0f;
            }
            else
            {
                time += Time.deltaTime;
            }
        }

        void OnWallHit(Contact contact) => Debug.Log("OnWallHit");
        void OnGroundedStateEnter(Vector3 localVelocity) => Debug.Log("OnEnterGroundedState, localVelocity : " + localVelocity.ToString("F3"));
        void OnGroundedStateExit() => Debug.Log("OnExitGroundedState");
        void OnStableStateEnter(Vector3 localVelocity) => Debug.Log("OnStableStateEnter, localVelocity : " + localVelocity.ToString("F3"));
        void OnStableStateExit() => Debug.Log("OnStableStateExit");
        void OnHeadHit(Contact contact) => Debug.Log("OnHeadHit");
        void OnTeleportation(Vector3 position, Quaternion rotation) => Debug.Log("OnTeleportation, position : " + position.ToString("F3") + " and rotation : " + rotation.ToString("F3"));

        #region Messages
        void FixedUpdate()
        {
            if (characterActor == null)
            {
                enabled = false;
                return;
            }

            UpdateCharacterInfoText();
        }

        void Update()
        {
            if (stabilityIndicator != null)
                stabilityIndicator.material.SetColor(colorID, characterActor.IsStable ? stableColor : unstableColor);            
        }

        void OnEnable()
        {
            if (!printEvents)
                return;

            characterActor.OnHeadHit += OnHeadHit;
            characterActor.OnWallHit += OnWallHit;
            characterActor.OnGroundedStateEnter += OnGroundedStateEnter;
            characterActor.OnGroundedStateExit += OnGroundedStateExit;
            characterActor.OnStableStateEnter += OnStableStateEnter;
            characterActor.OnStableStateExit += OnStableStateExit;
            characterActor.OnTeleport += OnTeleportation;
        }

        void OnDisable()
        {
            if (!printEvents)
                return;

            characterActor.OnHeadHit -= OnHeadHit;
            characterActor.OnWallHit -= OnWallHit;
            characterActor.OnGroundedStateEnter -= OnGroundedStateEnter;
            characterActor.OnGroundedStateExit -= OnGroundedStateExit;
            characterActor.OnStableStateEnter += OnStableStateEnter;
            characterActor.OnStableStateExit += OnStableStateExit;
            characterActor.OnTeleport -= OnTeleportation;
        }
#endregion
    }
}
