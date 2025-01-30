using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This component can be used to smooth out the graphics object rotation.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Graphics/Rotation Lerper")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder)]
    public class CharacterRotationLerper : CharacterGraphics
    {
        [Condition("lerpRotation", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        [SerializeField]
        float rotationLerpSpeed = 25f;
                
        Quaternion previousRotation = default;
        bool teleportFlag = false;

        #region Messages
        protected override void OnValidate()
        {
            base.OnValidate();

            CustomUtilities.SetPositive(ref rotationLerpSpeed);
        }

        void Start()
        {
            previousRotation = transform.rotation;
        }

        void OnEnable() => CharacterActor.OnTeleport += OnTeleport;
        void OnDisable() => CharacterActor.OnTeleport -= OnTeleport;

        void Update()
        {
            if (CharacterActor == null)
            {
                enabled = false;
                return;
            }

            float dt = Time.deltaTime;

            HandleRotation(dt);

            if (teleportFlag)
                teleportFlag = false;
        }

        #endregion

        void OnTeleport(Vector3 position, Quaternion rotation) => teleportFlag = true;

        void HandleRotation(float dt)
        {
            if (teleportFlag)
            {
                transform.localRotation = Quaternion.identity;
                previousRotation = transform.rotation;
                return;
            }

            transform.rotation = Quaternion.Slerp(previousRotation, CharacterActor.Rotation, rotationLerpSpeed * dt);
            previousRotation = transform.rotation;
        }

    }

}




