using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This component can be used to rotate a 2D sprite based on the character forward direction.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Graphics/Sprite Rotator")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder)]
    public class CharacterSpriteRotator : CharacterGraphics
    {
        [SerializeField]
        VectorComponent scaleAffectedComponent = VectorComponent.X;

        enum VectorComponent
        {
            X,
            Y,
            Z
        }

        Vector3 initialScale;
        Vector3 initialForward;

        void HandleRotation(float dt)
        {
            transform.rotation = Quaternion.LookRotation(initialForward, CharacterActor.Up);

            float signedAngle = Vector3.SignedAngle(CharacterActor.Forward, CharacterActor.Up, Vector3.forward);
            bool shouldBeFacingRight = signedAngle > 0f;

            switch (scaleAffectedComponent)
            {
                case VectorComponent.X:

                    transform.localScale = new Vector3(
                        shouldBeFacingRight ? initialScale.x : -initialScale.x,
                        transform.localScale.y,
                        transform.localScale.z
                    );

                    break;
                case VectorComponent.Y:

                    transform.localScale = new Vector3(
                        transform.localScale.x,
                        shouldBeFacingRight ? initialScale.y : -initialScale.y,
                        transform.localScale.z
                    );

                    break;
                case VectorComponent.Z:

                    transform.localScale = new Vector3(
                        transform.localScale.x,
                        transform.localScale.y,
                        shouldBeFacingRight ? initialScale.z : -initialScale.z
                    );

                    break;
            }            
        }

        #region Messages

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!CharacterBody.Is2D)
                Debug.Log("Warning: CharacterBody is not 2D. This component is intended to be used with a 2D physics character.");

            SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
                Debug.Log("Warning: \"Scale\" facing direction mode is intended to work with sprites, not with humanoid characters, choose \"Rotation\" instead.");
        }

        protected override void Awake()
        {
            base.Awake();

            if (!CharacterBody.Is2D)
                enabled = false;
        }

        void Start()
        {
            initialScale = transform.localScale;
            initialForward = transform.forward;
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;
            HandleRotation(dt);
        }

        #endregion

    }

}
