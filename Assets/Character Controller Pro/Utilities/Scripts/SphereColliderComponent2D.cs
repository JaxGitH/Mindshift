using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


namespace Lightbug.Utilities
{

    /// <summary>
    /// This component represents a sphere collider in a 2D world (better known as a circle).
    /// </summary>
    public class SphereColliderComponent2D : ColliderComponent2D
    {
        CircleCollider2D circleCollider = null;

        public override Vector3 Size
        {
            get => Vector2.one * 2f * circleCollider.radius;
            set => circleCollider.radius = value.x / 2f;
        }

        public override Vector3 BoundsSize => circleCollider.bounds.size;

        public override Vector3 Offset
        {
            get => circleCollider.offset;
            set => circleCollider.offset = value;
        }

        protected override int InternalOverlapBody(Vector3 position, Quaternion rotation, Collider2D[] unfilteredResults, List<Collider2D> filteredResults, OverlapFilterDelegate2D filter)
        {
            var center = position + rotation * circleCollider.offset;

            var qht = Physics2D.queriesHitTriggers;
            Physics2D.queriesHitTriggers = false;

            ContactFilter.layerMask = Physics2D.DefaultRaycastLayers;
            var overlaps = Physics2D.OverlapCircle(
                center,
                circleCollider.radius,
                ContactFilter,
                unfilteredResults
            );

            Physics2D.queriesHitTriggers = qht;

            return FilterValidOverlaps(overlaps, unfilteredResults, filteredResults, filter);
        }

        protected override void Awake()
        {

            circleCollider = gameObject.GetOrAddComponent<CircleCollider2D>(true);
            collider = circleCollider;

            base.Awake();
        }


    }

}