using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Utilities
{
    /// <summary>
    /// An implementation of a ColliderComponent for 3D colliders.
    /// </summary>
    public abstract class ColliderComponent3D : ColliderComponent
    {

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
        protected Collider collider = null;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

        public RaycastHit[] UnfilteredHits { get; protected set; } = new RaycastHit[20];
        public List<RaycastHit> FilteredHits { get; protected set; } = new List<RaycastHit>(10);

        public Collider[] UnfilteredOverlaps { get; protected set; } = new Collider[20];
        public List<Collider> FilteredOverlaps { get; protected set; } = new List<Collider>(10);

#if UNITY_6000_0_OR_NEWER
        public PhysicsMaterial Material
        {
            get => collider.sharedMaterial;
            set => collider.sharedMaterial = value;
        }
#else
        public PhysicMaterial Material
        {
            get => collider.sharedMaterial;
            set => collider.sharedMaterial = value;
        }
#endif

        protected abstract int InternalOverlapBody(Vector3 position, Quaternion rotation, Collider[] unfilteredResults, List<Collider> filteredResults, OverlapFilterDelegate3D filter);

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

            Vector3 penetration = Vector3.zero;
            for (int i = 0; i < overlaps; i++)
            {
                var otherCollider = FilteredOverlaps[i];

                if (otherCollider.transform.IsChildOf(collider.transform))
                    continue;

                if (otherCollider.isTrigger)
                    continue;

                var overlapped = Physics.ComputePenetration(
                    collider,
                    position,
                    rotation,
                    otherCollider,
                    otherCollider.transform.position,
                    otherCollider.transform.rotation,
                    out Vector3 direction,
                    out float distance
                );

                if (!overlapped)
                    continue;

                penetration += direction * distance;

                Action?.Invoke(ref position, ref rotation, otherCollider.transform, direction, distance);
            }

            return penetration;
        }

        protected bool InternalHitFilter(RaycastHit raycastHit)
        {
            if (raycastHit.collider == collider)
                return false;

            if (raycastHit.collider.isTrigger)
                return false;

            return true;
        }

        protected bool InternalOverlapFilter(Collider collider)
        {
            if (collider == this.collider)
                return false;

            if (collider.isTrigger)
                return false;

            return true;
        }

        protected int FilterValidOverlaps(int hits, Collider[] unfilteredOverlaps, List<Collider> filteredOverlaps, OverlapFilterDelegate3D Filter)
        {
            filteredOverlaps.Clear();

            for (int i = 0; i < hits; i++)
            {
                Collider collider = unfilteredOverlaps[i];

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

#if UNITY_6000_0_OR_NEWER
            PhysicsMaterial material = new PhysicsMaterial("Frictionless 3D");
            material.frictionCombine = PhysicsMaterialCombine.Minimum;
            material.bounceCombine = PhysicsMaterialCombine.Minimum;
#else
            PhysicMaterial material = new PhysicMaterial("Frictionless 3D");
            material.frictionCombine = PhysicMaterialCombine.Minimum;
            material.bounceCombine = PhysicMaterialCombine.Minimum;
#endif

            material.dynamicFriction = 0f;
            material.staticFriction = 0f;
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
