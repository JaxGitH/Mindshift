using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// An implementation of a PhysicsComponent for 2D physics.
    /// </summary>
    public sealed class PhysicsComponent2D : PhysicsComponent
    {
        /// <summary>
        /// Gets the Rigidbody associated with this object.
        /// </summary>
        public Rigidbody2D Rigidbody { get; private set; } = null;

        /// <summary>
        /// Gets an array with all the colliders associated with this object.
        /// </summary>
        //public Collider2D[] Colliders { get; private set; } = null;
        public Collider2D Collider { get; private set; } = null;

        readonly ContactPoint2D[] _contactsBuffer = new ContactPoint2D[10];
        readonly RaycastHit2D[] _hitsBuffer = new RaycastHit2D[10];
        readonly Collider2D[] _overlapsBuffer = new Collider2D[10];
        ContactFilter2D _contactFilter;

        protected override void Awake()
        {
            base.Awake();
            Collider = GetComponent<Collider2D>();
            _contactFilter = (new ContactFilter2D()).NoFilter();
            _contactFilter.useLayerMask = true;
        }

        protected override void Start()
        {
            base.Start();
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (ignoreCollisionMessages)
                return;

            if (!other.isTrigger)
                return;

            bool found = false;
            float fixedTime = Time.fixedTime;

            // Check if the trigger is contained inside the list
            for (int i = 0; i < Triggers.Count && !found; i++)
            {
                if (Triggers[i] != other)
                    continue;

                found = true;
                var trigger = Triggers[i];
                trigger.Update(fixedTime);
                Triggers[i] = trigger;
            }

            if (!found)
                Triggers.Add(new Trigger(other, Time.fixedTime));
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (ignoreCollisionMessages)
                return;

            for (int i = Triggers.Count - 1; i >= 0; i--)
            {
                if (Triggers[i].collider2D == other)
                {
                    Triggers.RemoveAt(i);
                    break;
                }
            }
        }



        void OnCollisionEnter2D(Collision2D collision)
        {
            if (ignoreCollisionMessages)
                return;


            int bufferHits = collision.GetContacts(_contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint2D contact = _contactsBuffer[i];

                Contact outputContact = new Contact(true, contact, collision);

                Contacts.Add(outputContact);
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            if (ignoreCollisionMessages)
                return;


            int bufferHits = collision.GetContacts(_contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint2D contact = _contactsBuffer[i];

                Contact outputContact = new Contact(false, contact, collision);

                Contacts.Add(outputContact);
            }
        }


        protected override LayerMask GetCollisionLayerMask()
        {
            int objectLayer = gameObject.layer;
            LayerMask output = 0;

            for (int i = 0; i < 32; i++)
            {
                bool exist = !Physics2D.GetIgnoreLayerCollision(i, objectLayer);

                if (exist)
                    output.value |= 1 << i;
            }

            return output;
        }

        public override void IgnoreCollision(in HitInfo hitInfo, bool ignore)
        {
            if (hitInfo.collider2D == null)
                return;

            Physics2D.IgnoreCollision(Collider, hitInfo.collider2D, ignore);
        }

        public override void IgnoreCollision(Transform otherTransform, bool ignore)
        {
            if (otherTransform == null)
                return;

            if (!otherTransform.TryGetComponent(out Collider2D collider))
                return;

            Physics2D.IgnoreCollision(Collider, collider, ignore);
        }

        public void IgnoreCollision(Collider2D collider, bool ignore)
        {
            if (collider == null)
                return;

            Physics2D.IgnoreCollision(Collider, collider, ignore);
        }

        [System.Obsolete]
        public void IgnoreCollision(Collider2D collider, bool ignore, int layerMask)
        {
            if (collider == null)
                return;

            if (!CustomUtilities.BelongsToLayerMask(collider.gameObject.layer, layerMask))
                return;

            Physics2D.IgnoreCollision(Collider, collider, ignore);
        }


        public override void IgnoreLayerCollision(int targetLayer, bool ignore)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, targetLayer, ignore);

            if (ignore)
                CollisionLayerMask &= ~(1 << targetLayer);
            else
                CollisionLayerMask |= (1 << targetLayer);
        }

        public override void IgnoreLayerMaskCollision(LayerMask layerMask, bool ignore)
        {

            int layerMaskValue = layerMask.value;
            int currentLayer = 1;

            for (int i = 0; i < 32; i++)
            {
                bool exist = (layerMaskValue & currentLayer) > 0;

                if (exist)
                    IgnoreLayerCollision(i, ignore);

                currentLayer <<= 1;
            }

            if (ignore)
                CollisionLayerMask &= ~(layerMask.value);
            else
                CollisionLayerMask |= (layerMask.value);
        }

        #region Physics queries

        protected override int InternalRaycast(Vector3 origin, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps)
        {
            var qht = Physics2D.queriesHitTriggers;
            var qsic = Physics2D.queriesStartInColliders;

            Physics2D.queriesHitTriggers = !ignoreTriggers;
            Physics2D.queriesStartInColliders = true;

            var hits = Physics2D.RaycastNonAlloc(
                origin,
                Vector3.Normalize(castDisplacement),
                _hitsBuffer,
                castDisplacement.magnitude,
                layerMask
            );

            Physics2D.queriesHitTriggers = qht;
            Physics2D.queriesStartInColliders = qsic;

            return hits;
        }

        protected override int InternalSphereCast(Vector3 center, float radius, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps)
        {
            var qht = Physics2D.queriesHitTriggers;
            var qsic = Physics2D.queriesStartInColliders;

            Physics2D.queriesHitTriggers = !ignoreTriggers;
            Physics2D.queriesStartInColliders = true;

            _contactFilter.layerMask = layerMask;
            var hits = Physics2D.CircleCast(
                center,
                radius,
                Vector3.Normalize(castDisplacement),
                _contactFilter,
                _hitsBuffer,
                castDisplacement.magnitude
            );

            Physics2D.queriesHitTriggers = qht;
            Physics2D.queriesStartInColliders = qsic;

            return hits;
        }

        protected override int InternalCapsuleCast(Vector3 bottom, Vector3 top, float radius, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps)
        {
            Vector3 bottomToTop = top - bottom;
            Vector3 center = bottom + CustomUtilities.Multiply(bottomToTop, 0.5f);

            Vector2 size;
            size.x = 2f * radius;
            size.y = bottomToTop.magnitude + size.x;

            float castAngle = Vector2.SignedAngle(bottomToTop, Vector2.up);

            var qht = Physics2D.queriesHitTriggers;
            var qsic = Physics2D.queriesStartInColliders;

            Physics2D.queriesHitTriggers = !ignoreTriggers;
            Physics2D.queriesStartInColliders = true;

            _contactFilter.layerMask = layerMask;
            var hits = Physics2D.CapsuleCast(
                center,
                size,
                CapsuleDirection2D.Vertical,
                castAngle,
                Vector3.Normalize(castDisplacement),
                _contactFilter,
                _hitsBuffer,
                castDisplacement.magnitude
            );

            Physics2D.queriesHitTriggers = qht;
            Physics2D.queriesStartInColliders = qsic;

            return hits;
        }

        protected override int InternalBoxCast(Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, int layerMask, bool ignoreTriggers, bool allowOverlaps)
        {
            var qht = Physics2D.queriesHitTriggers;
            var qsic = Physics2D.queriesStartInColliders;

            Physics2D.queriesHitTriggers = !ignoreTriggers;
            Physics2D.queriesStartInColliders = true;

            _contactFilter.layerMask = layerMask;
            var hits = Physics2D.BoxCast(
                center,
                size,
                orientation.eulerAngles.z,
                Vector3.Normalize(castDisplacement),
                _contactFilter,
                _hitsBuffer,
                castDisplacement.magnitude
            );

            Physics2D.queriesHitTriggers = qht;
            Physics2D.queriesStartInColliders = qsic;

            return hits;
        }

        #endregion

        #region Overlaps

        protected override int InternalOverlapSphere(Vector3 center, float radius, int layerMask, bool ignoreTriggers)
        {
            var qht = Physics2D.queriesHitTriggers;
            var qsic = Physics2D.queriesStartInColliders;

            Physics2D.queriesHitTriggers = !ignoreTriggers;
            Physics2D.queriesStartInColliders = true;

            _contactFilter.layerMask = layerMask;
            var overlaps = Physics2D.OverlapCircle(
                center,
                radius,
                _contactFilter,
                _overlapsBuffer
            );

            Physics2D.queriesHitTriggers = qht;
            Physics2D.queriesStartInColliders = qsic;

            return overlaps;
        }

        protected override int InternalOverlapCapsule(Vector3 bottom, Vector3 top, float radius, int layerMask, bool ignoreTriggers)
        {
            Vector3 bottomToTop = top - bottom;
            Vector3 center = bottom + 0.5f * bottomToTop;
            Vector2 size = new Vector2(2f * radius, bottomToTop.magnitude + 2f * radius);

            float castAngle = Vector2.SignedAngle(bottomToTop, Vector2.up);

            var qht = Physics2D.queriesHitTriggers;
            var qsic = Physics2D.queriesStartInColliders;

            Physics2D.queriesHitTriggers = !ignoreTriggers;
            Physics2D.queriesStartInColliders = true;

            _contactFilter.layerMask = layerMask;
            var overlaps = Physics2D.OverlapCapsule(
                center,
                size,
                CapsuleDirection2D.Vertical,
                castAngle,
                _contactFilter,
                _overlapsBuffer
            );

            Physics2D.queriesHitTriggers = qht;
            Physics2D.queriesStartInColliders = qsic;

            return overlaps;
        }

        protected override int InternalOverlapBox(Vector3 center, Vector3 size, Quaternion orientation, int layerMask, bool ignoreTriggers)
        {
            var qht = Physics2D.queriesHitTriggers;
            var qsic = Physics2D.queriesStartInColliders;

            Physics2D.queriesHitTriggers = !ignoreTriggers;
            Physics2D.queriesStartInColliders = true;

            var angle = orientation.eulerAngles.z;
            _contactFilter.layerMask = layerMask;
            var overlaps = Physics2D.OverlapBox(
                center,
                size,
                angle,
                _contactFilter,
                _overlapsBuffer
            );

            Physics2D.queriesHitTriggers = qht;
            Physics2D.queriesStartInColliders = qsic;

            return overlaps;
        }

        #endregion

        protected override int FilterOverlaps(int overlaps, LayerMask ignoredLayerMask, HitFilterDelegate hitFilter)
        {
            int filteredOverlaps = overlaps;
            for (int i = 0; i < overlaps; i++)
            {
                var hitCollider = _overlapsBuffer[i];

                if (hitCollider.transform.IsChildOf(transform))
                {
                    filteredOverlaps--;
                    continue;
                }

                if (hitFilter != null)
                {
                    if (!hitFilter.Invoke(hitCollider.transform))
                    {
                        filteredOverlaps--;
                        continue;
                    }
                }

                // Tell the physics engine to ignore this collider if it belongs to the "ignoredLayerMask" mask.
                if (CustomUtilities.BelongsToLayerMask(hitCollider.gameObject.layer, ignoredLayerMask))
                    IgnoreCollision(hitCollider, true);
            }

            return filteredOverlaps;
        }

        protected override void GetClosestHit(out HitInfo hitInfo, int hits, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps, HitFilterDelegate hitFilter)
        {
            var closestRaycastHit = new RaycastHit2D();
            closestRaycastHit.distance = Mathf.Infinity;

            var hit = false;
            HitsBuffer.Clear();

            for (int i = 0; i < hits; i++)
            {
                var raycastHit = _hitsBuffer[i];
                if (!PerformHitCheck(ref raycastHit, in filter, allowOverlaps, hitFilter))
                    continue;

                hit = true;

                var thisHitInfo = new HitInfo(ref raycastHit, Vector3.Normalize(castDisplacement));
                HitsBuffer.Add(thisHitInfo);

                if (raycastHit.distance < closestRaycastHit.distance)
                {
                    closestRaycastHit = raycastHit;
                }
            }

            if (hit)
                hitInfo = new HitInfo(ref closestRaycastHit, Vector3.Normalize(castDisplacement));
            else
                hitInfo = new HitInfo();
        }

        protected override List<HitInfo> GetAllHits(int hits, Vector3 castDisplacement, in HitInfoFilter filter, bool allowOverlaps, HitFilterDelegate hitFilter)
        {
            HitsBuffer.Clear();

            for (int i = 0; i < hits; i++)
            {
                var raycastHit = _hitsBuffer[i];
                if (!PerformHitCheck(ref raycastHit, in filter, allowOverlaps, hitFilter))
                    continue;

                var thisHitInfo = new HitInfo(ref raycastHit, Vector3.Normalize(castDisplacement));
                HitsBuffer.Add(thisHitInfo);
            }

            return HitsBuffer;
        }

        bool PerformHitCheck(ref RaycastHit2D raycastHit, in HitInfoFilter filter, bool allowOverlaps, HitFilterDelegate hitFilter)
        {
            var raycastHitDistance = raycastHit.distance;
            var raycastHitCollider = raycastHit.collider;

            if (raycastHitCollider.transform.IsChildOf(transform))
                return false;

            if (hitFilter != null)
                if (!hitFilter.Invoke(raycastHitCollider.transform))
                    return false;

            if (!allowOverlaps && raycastHitDistance == 0)
                return false;

            if (raycastHitDistance < filter.minimumDistance || raycastHitDistance > filter.maximumDistance)
                return false;

            if (filter.ignoreRigidbodies && raycastHitCollider.attachedRigidbody != null)
                return false;

            return true;
        }

        public override bool CheckCollisionsWith(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent(out Collider2D collider))
                return false;

            return !Physics2D.GetIgnoreCollision(Collider, collider);
        }
    }

}
