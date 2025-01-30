using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{
    /// <summary>
    /// An implementation of a ColliderComponent for 2D colliders.
    /// </summary>
    public abstract class ColliderComponent2D : ColliderComponent
    {
        protected new Collider2D collider = null;

        public RaycastHit2D[] UnfilteredHits { get; protected set; } = new RaycastHit2D[20];
        public List<RaycastHit2D> FilteredHits { get; protected set; } = new List<RaycastHit2D>(10);

        public Collider2D[] UnfilteredOverlaps { get; protected set; } = new Collider2D[20];
        public List<Collider2D> FilteredOverlaps { get; protected set; } = new List<Collider2D>(10);
        protected ContactFilter2D ContactFilter;

        public PhysicsMaterial2D Material
        {
            get => collider.sharedMaterial;
            set => collider.sharedMaterial = value;
        }

        protected abstract int InternalOverlapBody(Vector3 position, Quaternion rotation, Collider2D[] unfilteredResults, List<Collider2D> filteredResults, OverlapFilterDelegate2D filter);

        public sealed override int OverlapBody(Vector3 position, Quaternion rotation)
        {
            return InternalOverlapBody(position, rotation, UnfilteredOverlaps, FilteredOverlaps, null);
        }

        public override bool ComputePenetration(ref Vector3 position, ref Quaternion rotation, PenetrationDelegate Action)
        {
            return ComputePenetrationVector(ref position, ref rotation, Action) != Vector3.zero;
        }

        public override Vector3 ComputePenetrationVector(ref Vector3 position, ref Quaternion rotation, PenetrationDelegate Action)
        {
            int overlaps = OverlapBody(position, rotation);

            if (overlaps == 0)
                return Vector3.zero;

            Vector2 penetration = Vector2.zero;
            for (int i = 0; i < overlaps; i++)
            {
                var otherCollider = FilteredOverlaps[i];

                if (otherCollider.transform.IsChildOf(collider.transform))
                    continue;

                if (otherCollider.isTrigger)
                    continue;

                var colliderDistance2D = Physics2D.Distance(collider, otherCollider);
                if (!colliderDistance2D.isOverlapped)   // only negative distances (overlaps) are allowed
                    continue;

                penetration += -colliderDistance2D.normal * colliderDistance2D.distance;
                
                Action?.Invoke(ref position, ref rotation, otherCollider.transform, -colliderDistance2D.normal, colliderDistance2D.distance);
            }

            return penetration;
        }

        protected bool InternalHitFilter(RaycastHit2D raycastHit)
        {
            if (raycastHit.collider == collider)
                return false;

            if (raycastHit.collider.isTrigger)
                return false;

            return true;
        }

        protected bool InternalOverlapFilter(Collider2D collider)
        {
            if (collider == this.collider)
                return false;

            if (collider.isTrigger)
                return false;

            return true;
        }

        protected int FilterValidOverlaps(int hits, Collider2D[] overlapsBuffer, List<Collider2D> filteredOverlaps, OverlapFilterDelegate2D Filter)
        {
            filteredOverlaps.Clear();

            for (int i = 0; i < hits; i++)
            {
                Collider2D collider = overlapsBuffer[i];

                // User-defined filter
                if (Filter != null)
                {
                    bool validHit = Filter(collider);
                    if (!validHit)
                        continue;
                }

                filteredOverlaps.Add(collider);
            }

            return filteredOverlaps.Count;
        }

        protected override void Awake()
        {
            base.Awake();

            ContactFilter = (new ContactFilter2D()).NoFilter();
            ContactFilter.useLayerMask = true;

            PhysicsMaterial2D material = new PhysicsMaterial2D("Frictionless 2D");
            material.friction = 0f;
            material.bounciness = 0f;

            collider.sharedMaterial = material;
            collider.hideFlags = HideFlags.NotEditable;
        }

        protected override void OnEnable()
        {
            collider.enabled = true;
        }

        protected override void OnDisable()
        {
            collider.enabled = false;
        }
    }

}
