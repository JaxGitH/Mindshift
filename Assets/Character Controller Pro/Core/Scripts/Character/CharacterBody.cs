using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    public enum CharacterBodyType
    {
        Sphere,
        Capsule
    }

    /// <summary>
    /// This component exposes all the character body properties, such as width, height, body shape, physics, etc.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Body")]
    public class CharacterBody : MonoBehaviour
    {
        [HelpBox("This component will automatically assign a Rigidbody component and a CapsuleCollider component at runtime.", HelpBoxMessageType.Info)]
        [SerializeField, BooleanButton("Physics", "3D", "2D", false)]
        bool is2D = false;

        [SerializeField, BreakVector2("Width", "Height")]
        Vector2 bodySize = new Vector2(1f, 2f);

        [SerializeField]
        float mass = 50f;

        /// <summary>
        /// Returns true if the character is governed by 2D Physics, false otherwise.
        /// </summary>
        public bool Is2D => is2D;

        /// <summary>
        /// Gets the RigidbodyComponent component associated to the character.
        /// </summary>
        public RigidbodyComponent RigidbodyComponent { get; private set; }

        /// <summary>
        /// Gets the ColliderComponent component associated to the character.
        /// </summary>
        public ColliderComponent ColliderComponent { get; private set; }

        /// <summary>
        /// Gets the mass of the character.
        /// </summary>
        public float Mass => mass;

        /// <summary>
        /// Gets the body size of the character (width and height).
        /// </summary>
        public Vector2 BodySize => bodySize;


        /// <summary>
        /// Initializes the body properties and components.
        /// </summary>
        void Awake()
        {
            if (Is2D)
            {

                ColliderComponent = gameObject.AddComponent<CapsuleColliderComponent2D>();
                RigidbodyComponent = gameObject.AddComponent<RigidbodyComponent2D>();
            }
            else
            {
                ColliderComponent = gameObject.AddComponent<CapsuleColliderComponent3D>();
                RigidbodyComponent = gameObject.AddComponent<RigidbodyComponent3D>();
            }

        }

        CharacterActor characterActor = null;


        void OnValidate()
        {
            if (characterActor == null)
                characterActor = GetComponent<CharacterActor>();

            bodySize = new Vector2(
                Mathf.Max(bodySize.x, 0f),
                Mathf.Max(bodySize.y, bodySize.x + CharacterConstants.ColliderMinBottomOffset)
            );

            if (characterActor != null)
                characterActor.OnValidate();
        }

    }

}
