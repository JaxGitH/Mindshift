using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    public abstract class CharacterCollisions : MonoBehaviour
    { 
        public HitInfo[] HitsBuffer { get; protected set; } = new HitInfo[20];

        protected CharacterActor CharacterActor { get; private set; }
        public PhysicsComponent PhysicsComponent { get; protected set; }

        readonly CollisionInfo _collisionInfo = new CollisionInfo();

        float BackstepDistance => 2f * ContactOffset;

        public abstract float ContactOffset { get; }
        public abstract float CollisionRadius { get; }
        protected Transform Transform;

        public static CharacterCollisions CreateInstance(GameObject gameObject)
        {
            Rigidbody2D rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            Rigidbody rigidbody3D = gameObject.GetComponent<Rigidbody>();

            if (rigidbody2D != null)
                return gameObject.GetOrAddComponent<CharacterCollisions2D>();
            else if (rigidbody3D != null)
                return gameObject.GetOrAddComponent<CharacterCollisions3D>();

            return null;
        }

        /// <summary>
        /// Checks vertically for the ground using a CastSphere.
        /// </summary>
        public CollisionInfo CheckForGround(Vector3 position, float stepOffset, float stepDownDistance, in HitInfoFilter hitInfoFilter, HitFilterDelegate hitFilter = null)
        {
            float preDistance = stepOffset + BackstepDistance;
            Vector3 displacement = CustomUtilities.Multiply(-CharacterActor.Up, Mathf.Max(CharacterConstants.GroundCheckDistance, stepDownDistance));
            Vector3 origin = CharacterActor.GetBottomCenter(position, preDistance);
            Vector3 castDisplacement = displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance + ContactOffset);

            PhysicsComponent.SphereCast(
                out HitInfo hitInfo,
                origin,
                CollisionRadius,
                castDisplacement,
                in hitInfoFilter,
                false,
                hitFilter
            );

            UpdateCollisionInfo(_collisionInfo, position, in hitInfo, castDisplacement, preDistance, true, in hitInfoFilter);
            return _collisionInfo;
        }

        /// <summary>
        /// Checks vertically for the ground using a CastRay.
        /// </summary>
        public CollisionInfo CheckForGroundRay(Vector3 position, in HitInfoFilter hitInfoFilter, HitFilterDelegate hitFilter = null)
        {
            float preDistance = CharacterActor.BodySize.x / 2f;

            Vector3 origin = CharacterActor.GetBottomCenter(position);
            Vector3 displacement = CustomUtilities.Multiply(-CharacterActor.Up, Mathf.Max(CharacterConstants.GroundCheckDistance, CharacterActor.stepDownDistance));
            Vector3 castDisplacement = displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance);

            PhysicsComponent.Raycast(
                out HitInfo hitInfo,
                origin,
                castDisplacement,
                in hitInfoFilter,
                false,
                hitFilter
            );

            UpdateCollisionInfo(_collisionInfo, position, in hitInfo, castDisplacement, preDistance, false, in hitInfoFilter);
            return _collisionInfo;
        }

        /// <summary>
        /// Cast the current body shape a get the closest hit.
        /// </summary>
        public CollisionInfo CastBody(Vector3 position, Vector3 displacement, float bottomOffset, in HitInfoFilter hitInfoFilter, bool allowOverlaps = false, HitFilterDelegate hitFilter = null)
        {
            float preDistance = BackstepDistance;
            Vector3 direction = Vector3.Normalize(displacement);

            Vector3 bottom = CharacterActor.GetBottomCenter(position, bottomOffset);
            Vector3 top = CharacterActor.GetTopCenter(position);
            bottom -= CustomUtilities.Multiply(direction, preDistance);
            top -= CustomUtilities.Multiply(direction, preDistance);

            Vector3 castDisplacement = displacement + CustomUtilities.Multiply(Vector3.Normalize(displacement), preDistance + ContactOffset);
            float radius = CharacterActor.BodySize.x / 2f;

            PhysicsComponent.CapsuleCast(
                out HitInfo hitInfo,
                bottom,
                top,
                radius,
                castDisplacement,
                in hitInfoFilter,
                allowOverlaps,
                hitFilter
            );

            UpdateCollisionInfo(_collisionInfo, position, in hitInfo, castDisplacement, preDistance, false, in hitInfoFilter);
            return _collisionInfo;
        }

        /// <summary>
        /// Performs an overlap test at a given position.
        /// </summary>
        [System.Obsolete("Use CheckOverlap instead.")]
        public bool CheckOverlapWithLayerMask(Vector3 position, float bottomOffset, in HitInfoFilter hitInfoFilter, HitFilterDelegate hitFilter = null) =>
            CheckOverlap(position, bottomOffset, in hitInfoFilter, hitFilter);

        /// <summary>
        /// Performs an overlap test at a given position.
        /// </summary>
        /// <param name="position">Target position.</param>
        /// <param name="bottomOffset">Bottom offset of the capsule.</param>
        /// <param name="hitInfoFilter">Overlap filter.</param>
        /// <returns>True if the overlap test detects some obstacle.</returns>
        public bool CheckOverlap(Vector3 position, float bottomOffset, in HitInfoFilter hitInfoFilter, HitFilterDelegate hitFilter = null)
        {
            Vector3 bottom = CharacterActor.GetBottomCenter(position, bottomOffset);
            Vector3 top = CharacterActor.GetTopCenter(position);
            float radius = CharacterActor.BodySize.x / 2f - CharacterConstants.SkinWidth;

            bool overlap = PhysicsComponent.OverlapCapsule(
                bottom,
                top,
                radius,
                in hitInfoFilter,
                hitFilter
            );

            return overlap;
        }

        /// <summary>
        /// Checks if the character fits at a specific location.
        /// </summary>
        public bool CheckBodySize(Vector3 size, Vector3 position, in HitInfoFilter hitInfoFilter, HitFilterDelegate hitFilter = null)
        {
            Vector3 bottom = CharacterActor.GetBottomCenter(position, size);
            float radius = size.x / 2f;

            // BottomCenterToTopCenter = Up displacement
            Vector3 castDisplacement = CharacterActor.GetBottomCenterToTopCenter(size);

            PhysicsComponent.SphereCast(
                out HitInfo hitInfo,
                bottom,
                radius,
                castDisplacement,
                in hitInfoFilter,
                false,
                hitFilter
            );

            bool overlap = hitInfo.hit;
            return !overlap;
        }

        /// <summary>
        /// Checks if the character fits in place.
        /// </summary>
        public bool CheckBodySize(Vector3 size, in HitInfoFilter hitInfoFilter, HitFilterDelegate hitFilter = null) => CheckBodySize(size, CharacterActor.Position, in hitInfoFilter, hitFilter);


        public void UpdateCollisionInfo(
            CollisionInfo collisionInfo,
            Vector3 position,
            in HitInfo hitInfo,
            Vector3 castDisplacement,
            float preDistance,
            bool calculateEdge = true,
            in HitInfoFilter hitInfoFilter = new HitInfoFilter())
        {
            if (hitInfo.hit)
            {
                Vector3 castDirection = Vector3.Normalize(castDisplacement);

                float closestDistance = hitInfo.distance - preDistance - ContactOffset;

                var displacement = castDirection * closestDistance;

                if (calculateEdge)
                {
                    Vector3 edgeCenterReference = CharacterActor.GetBottomCenter(position + displacement, 0f);
                    PerformEdgeDetectionRaycasts(in edgeCenterReference, in hitInfo.point, in hitInfoFilter, out HitInfo upperHitInfo, out HitInfo lowerHitInfo);

                    collisionInfo.SetData(in hitInfo, CharacterActor.Up, displacement, in upperHitInfo, in lowerHitInfo);
                }
                else
                {
                    collisionInfo.SetData(in hitInfo, CharacterActor.Up, displacement);
                }
            }
            else
            {
                collisionInfo.Reset();
            }

        }

        void PerformEdgeDetectionRaycasts(in Vector3 edgeCenterReference, in Vector3 contactPoint, in HitInfoFilter hitInfoFilter, out HitInfo upperHitInfo, out HitInfo lowerHitInfo)
        {
            Vector3 castDirection = Vector3.Normalize(contactPoint - edgeCenterReference);
            Vector3 castDisplacement = CustomUtilities.Multiply(castDirection, CharacterConstants.EdgeRaysCastDistance);
            Vector3 upperHitPosition = edgeCenterReference + CustomUtilities.Multiply(CharacterActor.Up, CharacterConstants.EdgeRaysSeparation);
            Vector3 lowerHitPosition = edgeCenterReference - CustomUtilities.Multiply(CharacterActor.Up, CharacterConstants.EdgeRaysSeparation);

            PhysicsComponent.Raycast(
                out upperHitInfo,
                upperHitPosition,
                castDisplacement,
                in hitInfoFilter
            );

            PhysicsComponent.Raycast(
                out lowerHitInfo,
                lowerHitPosition,
                castDisplacement,
                in hitInfoFilter
            );
        }

        protected virtual void Awake()
        {
            CharacterActor = GetComponent<CharacterActor>();
            if (CharacterActor == null)
                Debug.Log("CharacterCollisions: CharacterComponent missing");
        }
    }
}
