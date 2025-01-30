using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.Utilities
{

    /// <summary>
    /// This component represents a sphere collider in a 3D world.
    /// </summary>
    public class SphereColliderComponent3D : ColliderComponent3D
    {
        SphereCollider sphereCollider = null;

        public override Vector3 Size
        {
            get => Vector3.one * 2f * sphereCollider.radius;
            set => sphereCollider.radius = value.x / 2f;
        }

        public override Vector3 BoundsSize => sphereCollider.bounds.size;


        public override Vector3 Offset
        {
            get => sphereCollider.center;
            set => sphereCollider.center = value;
        }

        protected override int InternalOverlapBody(Vector3 position, Quaternion rotation, Collider[] unfilteredResults, List<Collider> filteredResults, OverlapFilterDelegate3D filter)
        {
            var center = position + rotation * sphereCollider.center;
            var up = rotation * Vector3.up;

            var overlaps = Physics.OverlapSphereNonAlloc(
                center,
                sphereCollider.radius,
                unfilteredResults,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore
            );

            return FilterValidOverlaps(overlaps, unfilteredResults, filteredResults, filter);
        }

        protected override void Awake()
        {
            sphereCollider = gameObject.GetOrAddComponent<SphereCollider>(true);
            collider = sphereCollider;

            base.Awake();
        }


    }

}
