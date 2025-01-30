using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This component can be used to make a Transform change its scale based on the character actor height.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Graphics/Scaler")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder + 1)]
    public class CharacterGraphicsScaler : CharacterGraphics
    {

        [SerializeField]
        VectorComponent scaleHeightComponent = VectorComponent.Y;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        enum VectorComponent
        {
            X,
            Y,
            Z
        }

        Vector3 initialLocalScale = Vector3.one;


        void Start()
        {
            initialLocalScale = transform.localScale;
        }

        void Update()
        {
            if (!CharacterActor.enabled)
                return;

            Vector3 scale = Vector3.one;

            switch (scaleHeightComponent)
            {
                case VectorComponent.X:

                    scale = new Vector3(
                        initialLocalScale.x * (CharacterActor.BodySize.y / CharacterActor.DefaultBodySize.y),
                        initialLocalScale.y * (CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x),
                        initialLocalScale.z * (CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x)
                    );

                    break;
                case VectorComponent.Y:

                    scale = new Vector3(
                        initialLocalScale.x * (CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x),
                        initialLocalScale.y * (CharacterActor.BodySize.y / CharacterActor.DefaultBodySize.y),
                        initialLocalScale.z * (CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x)
                    );

                    break;
                case VectorComponent.Z:

                    scale = new Vector3(
                        initialLocalScale.x * (CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x),
                        initialLocalScale.y * (CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x),
                        initialLocalScale.z * (CharacterActor.BodySize.y / CharacterActor.DefaultBodySize.y)
                    );

                    break;
            }

            transform.localScale = scale;


        }



    }

}

