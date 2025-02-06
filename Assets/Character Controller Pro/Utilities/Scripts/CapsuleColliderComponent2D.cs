using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// This component represents a capsule collider in a 2D world.
    /// </summary>
    public class CapsuleColliderComponent2D : ColliderComponent2D
    {
        CapsuleCollider2D capsuleCollider = null;

        public override Vector3 Size
        {
            get => capsuleCollider.size;
            set => capsuleCollider.size = value;
        }

        public override Vector3 BoundsSize => capsuleCollider.bounds.size;


        public override Vector3 Offset
        {
            get => capsuleCollider.offset;
            set => capsuleCollider.offset = value;
        }

        protected override int InternalOverlapBody(Vector3 position, Quaternion rotation, Collider2D[] unfilteredResults, List<Collider2D> filteredResults, OverlapFilterDelegate2D filter)
        {
            var up = rotation * Vector3.up;
            var center = position + rotation * capsuleCollider.offset;
            var angle = rotation.eulerAngles.z;

            var qht = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;
            
            ContactFilter.layerMask = Physics2D.DefaultRaycastLayers;
            var overlaps = Physics2D.OverlapCapsule(
                center,
                capsuleCollider.size,
                CapsuleDirection2D.Vertical,
                angle,
                ContactFilter,
                unfilteredResults
            );

            Physics2D.queriesHitTriggers = qht;

            return FilterValidOverlaps(overlaps, unfilteredResults, filteredResults, filter);
        }


        protected override void Awake()
        {

            capsuleCollider = gameObject.GetOrAddComponent<CapsuleCollider2D>();
            collider = capsuleCollider;

            base.Awake();

        }


    }



}
