using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This component can be used to rotate a 2D character based on its forward direction. For 2D, characters usually have its forward direction pointing towards Vector3.forward (or negtive).
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Graphics/2D Rotator (Obsolete)")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder)]
    public class CharacterGraphics2DRotator : CharacterGraphics
    {
        [HelpBox("This component is obsolete and has been replaced with the CharacterSpriteScaler component, which is a simplified version.", HelpBoxMessageType.Warning)]
        [Tooltip("Scale: it will flip the sprite along the horizontal axis (localScale). This works only with sprites!\nRotation: it will rotate the object towards the facing direction.")]
        public FacingDirectionMode facingDirectionMode = FacingDirectionMode.Rotation;

        [Condition("facingDirectionMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)FacingDirectionMode.Scale)]
        [SerializeField]
        VectorComponent scaleAffectedComponent = VectorComponent.X;


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Vector3 initialScale = Vector3.zero;
        Vector3 initialForward;

        enum VectorComponent
        {
            X,
            Y,
            Z
        }

        /// <summary>
        /// The method used by the CharacterGraphics component to orient the graphics object towards the facing direction vector.
        /// </summary>
        public enum FacingDirectionMode
        {
            Rotation,
            Scale
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
                
        void HandleRotation(float dt)
        {
            if (facingDirectionMode == FacingDirectionMode.Scale)
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
        }

        protected override void OnValidate()
        {
            base.OnValidate();            

            if (!CharacterBody.Is2D)
                Debug.Log("Warning: CharacterBody is not 2D. This component is intended to be used with a 2D physics character.");

            SkinnedMeshRenderer skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null && facingDirectionMode == FacingDirectionMode.Scale)
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

    }

}
