using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// This component represents a capsule collider in a 2D world.
    /// </summary>
    public class BoxColliderComponent3D : ColliderComponent3D
    {
        BoxCollider boxCollider = null;

        public override Vector3 Size
        {
            get => boxCollider.size;
            set => boxCollider.size = value;
        }

        public override Vector3 BoundsSize => boxCollider.bounds.size;

        public override Vector3 Offset
        {
            get => boxCollider.center;
            set => boxCollider.center = value;
        }

        protected override int InternalOverlapBody(Vector3 position, Quaternion rotation, Collider[] unfilteredResults, List<Collider> filteredResults, OverlapFilterDelegate3D filter)
        {
            var center = position + rotation * boxCollider.center;
            var halfSize = boxCollider.size * 0.5f;
            var overlaps = Physics.OverlapBoxNonAlloc(
                center, 
                halfSize, 
                unfilteredResults, 
                rotation,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            return FilterValidOverlaps(overlaps, unfilteredResults, filteredResults, filter);
        }

        protected override void Awake()
        {
            boxCollider = gameObject.GetOrAddComponent<BoxCollider>(true);
            collider = boxCollider;

            base.Awake();

        }


    }



}
