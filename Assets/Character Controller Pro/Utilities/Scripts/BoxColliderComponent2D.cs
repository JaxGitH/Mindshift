using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// This component represents a capsule collider in a 2D world.
    /// </summary>
    public class BoxColliderComponent2D : ColliderComponent2D
    {
        BoxCollider2D boxCollider = null;

        public override Vector3 Size
        {
            get => boxCollider.size;
            set => boxCollider.size = value;
        }

        public override Vector3 BoundsSize => boxCollider.bounds.size;

        public override Vector3 Offset
        {
            get => boxCollider.offset;
            set => boxCollider.offset = value;
        }

        protected override int InternalOverlapBody(Vector3 position, Quaternion rotation, Collider2D[] unfilteredResults, List<Collider2D> filteredResults, OverlapFilterDelegate2D filter)
        {
            var up = rotation * Vector3.up;
            var center = position + rotation * boxCollider.offset;
            var angle = rotation.eulerAngles.z;

            var qht = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            
            ContactFilter.layerMask = Physics2D.DefaultRaycastLayers;
            var overlaps = Physics2D.OverlapBox(
                center,
                boxCollider.size,
                angle,
                ContactFilter,
                unfilteredResults                
            );

            Physics2D.queriesHitTriggers = qht;

            return FilterValidOverlaps(overlaps, unfilteredResults, filteredResults, filter);
        }

        protected override void Awake()
        {

            boxCollider = gameObject.GetOrAddComponent<BoxCollider2D>(true);
            collider = boxCollider;

            base.Awake();

        }


    }



}
