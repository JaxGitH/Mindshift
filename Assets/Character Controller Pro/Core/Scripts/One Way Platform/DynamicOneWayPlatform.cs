using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    [AddComponentMenu("Character Controller Pro/Core/Dynamic One Way Platform")]
    public class DynamicOneWayPlatform : MonoBehaviour
    {
        public LayerMask characterLayerMask = -1;

        protected Vector3 preSimulationPosition;
        Coroutine postSimulationUpdateCoroutine = null;
        protected Dictionary<Transform, CharacterActor> characters = new Dictionary<Transform, CharacterActor>();
        ColliderComponent colliderComponent;
        PhysicsComponent physicsComponent;
        RigidbodyComponent rigidbodyComponent;

        void Awake()
        {
            colliderComponent = ColliderComponent.CreateInstance(gameObject);
            physicsComponent = PhysicsComponent.CreateInstance(gameObject);
            rigidbodyComponent = RigidbodyComponent.CreateInstance(gameObject);
        }

        void OnEnable()
        {
            postSimulationUpdateCoroutine ??= StartCoroutine(PostSimulationUpdate());
        }

        void OnDisable()
        {
            if (postSimulationUpdateCoroutine != null)
            {
                StopCoroutine(PostSimulationUpdate());
                postSimulationUpdateCoroutine = null;
            }
        }


        protected List<HitInfo> CastPlatformBody(Vector3 castDisplacement)
        {
            float backstepDistance = 0.1f;
            float skinWidth = 0f;
            Vector3 castDirection = castDisplacement.normalized;
            castDisplacement += backstepDistance * skinWidth * castDirection;
            Vector3 origin = preSimulationPosition + colliderComponent.Offset - castDirection * backstepDistance;

            HitInfoFilter filter = new HitInfoFilter(characterLayerMask, false, true);
            return physicsComponent.BoxCast(
                origin,
                colliderComponent.BoundsSize - Vector3.one * skinWidth,
                castDisplacement,
                rigidbodyComponent.Rotation,
                in filter
            );
        }
        
        protected bool ValidateOWPCollision(CharacterActor characterActor, Vector3 contactPoint) =>
            characterActor.CheckOneWayPlatformCollision(contactPoint, characterActor.Position);

        void FixedUpdate() => preSimulationPosition = rigidbodyComponent.Position;

        IEnumerator PostSimulationUpdate()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;
                UpdatePlatform();
            }
        }

        void UpdatePlatform()
        {
            Vector3 castDisplacement = rigidbodyComponent.Position - preSimulationPosition;
            var hitsList = CastPlatformBody(castDisplacement);

            if (hitsList == null)
                return;

            for (int i = 0; i < hitsList.Count; i++)
            {
                var hitInfo = physicsComponent.HitsBuffer[i];

                if (hitInfo.distance == 0f)
                    continue;

                var characterActor = characters.GetOrRegisterValue(hitInfo.transform);
                if (characterActor == null)
                    continue;

                // Ignore the character collider (hitInfo contains the collider information).
                physicsComponent.IgnoreCollision(in hitInfo, true);

                if (!characterActor.IsGrounded)
                {
                    // Check if the collision is valid
                    bool isValidCollision = ValidateOWPCollision(characterActor, hitInfo.point);

                    // If so, then move the character
                    if (isValidCollision)
                    {
                        // How much the actor needs to move            
                        Vector3 actorCastDisplacement = castDisplacement.normalized * (castDisplacement.magnitude - hitInfo.distance);
                        Vector3 destination = characterActor.Position + actorCastDisplacement;

                        // Set the collision filter
                        HitInfoFilter filter = new HitInfoFilter(characterActor.ObstaclesWithoutOWPLayerMask, false, true);
                        
                        characterActor.SweepAndTeleport(destination, in filter);
                        characterActor.ForceGrounded();
                    }
                }
            }
        }
    }

}
