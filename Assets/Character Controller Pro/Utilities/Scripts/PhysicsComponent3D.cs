using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Lightbug.Utilities
{

    /// <summary>
    /// An implementation of a PhysicsComponent for 3D physics.
    /// </summary>
    public sealed class PhysicsComponent3D : PhysicsComponent
    {
        /// <summary>
        /// Gets the Rigidbody associated with this object.
        /// </summary>
        public Rigidbody Rigidbody { get; private set; } = null;

        /// <summary>
        /// Gets an array with all the colliders associated with this object.
        /// </summary>
        public Collider Collider { get; private set; } = null;

        readonly ContactPoint[] _contactsBuffer = new ContactPoint[10];
        readonly RaycastHit[] _hitsBuffer = new RaycastHit[10];
        readonly Collider[] _overlapsBuffer = new Collider[10];

        protected override void Awake()
        {
            base.Awake();
            Collider = GetComponent<Collider>();
        }

        protected override void Start()
        {
            base.Start();
            Rigidbody = GetComponent<Rigidbody>();
        }

        void OnTriggerStay(Collider other)
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

        void OnTriggerExit(Collider other)
        {
            if (ignoreCollisionMessages)
                return;

            for (int i = Triggers.Count - 1; i >= 0; i--)
            {
                if (Triggers[i].collider3D == other)
                {
                    Triggers.RemoveAt(i);
                    break;
                }
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (ignoreCollisionMessages)
                return;

            int bufferHits = collision.GetContacts(_contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint contact = _contactsBuffer[i];
                Contact outputContact = new Contact(true, contact, collision);
                Contacts.Add(outputContact);
            }
        }

        void OnCollisionStay(Collision collision)
        {
            if (ignoreCollisionMessages)
                return;

            int bufferHits = collision.GetContacts(_contactsBuffer);

            // Add the contacts to the list
            for (int i = 0; i < bufferHits; i++)
            {
                ContactPoint contact = _contactsBuffer[i];
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
                bool exist = !Physics.GetIgnoreLayerCollision(i, objectLayer);
                if (exist)
                    output.value |= 1 << i;
            }

            return output;
        }

        public override void IgnoreCollision(in HitInfo hitInfo, bool ignore)
        {
            if (hitInfo.collider3D == null)
                return;

            Physics.IgnoreCollision(Collider, hitInfo.collider3D, ignore);
        }

        public override void IgnoreCollision(Transform otherTransform, bool ignore)
        {
            if (otherTransform == null)
                return;

            if (!otherTransform.TryGetComponent(out Collider collider))
                return;

            Physics.IgnoreCollision(Collider, collider, ignore);
        }

        public void IgnoreCollision(Collider collider, bool ignore)
        {
            if (collider == null)
                return;

            Physics.IgnoreCollision(Collider, collider, ignore);
        }

        [System.Obsolete]
        public void IgnoreCollision(Collider collider, bool ignore, int layerMask)
        {
            if (collider == null)
                return;

            if (!CustomUtilities.BelongsToLayerMask(collider.gameObject.layer, layerMask))
                return;

            Physics.IgnoreCollision(Collider, collider, ignore);
        }

        public override void IgnoreLayerCollision(int targetLayer, bool ignore)
        {
            Physics.IgnoreLayerCollision(gameObject.layer, targetLayer, ignore);

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
            return Physics.RaycastNonAlloc(
                origin,
                Vector3.Normalize(castDisplacement),
                _hitsBuffer,
                castDisplacement.magnitude,
                layerMask,
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );
        }

        protected override int InternalSphereCast(Vector3 center, float radius, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps)
        {
            return Physics.SphereCastNonAlloc(
                center,
                radius,
                Vector3.Normalize(castDisplacement),
                _hitsBuffer,
                castDisplacement.magnitude,
                layerMask,
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );
        }

        protected override int InternalCapsuleCast(Vector3 bottom, Vector3 top, float radius, Vector3 castDisplacement, int layerMask, bool ignoreTriggers, bool allowOverlaps)
        {
            return Physics.CapsuleCastNonAlloc(
                bottom,
                top,
                radius,
                Vector3.Normalize(castDisplacement),
                _hitsBuffer,
                castDisplacement.magnitude,
                layerMask,
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );
        }

        protected override int InternalBoxCast(Vector3 center, Vector3 size, Vector3 castDisplacement, Quaternion orientation, int layerMask, bool ignoreTriggers, bool allowOverlaps)
        {
            return Physics.BoxCastNonAlloc(
                center,
                size / 2f,
                Vector3.Normalize(castDisplacement),
                _hitsBuffer,
                orientation,
                castDisplacement.magnitude,
                layerMask,
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );
        }

        #endregion

        #region Overlaps

        protected override int InternalOverlapSphere(Vector3 center, float radius, int layerMask, bool ignoreTriggers)
        {
            return Physics.OverlapSphereNonAlloc(
                center,
                radius,
                _overlapsBuffer,
                layerMask,
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );
        }

        protected override int InternalOverlapCapsule(Vector3 bottom, Vector3 top, float radius, int layerMask, bool ignoreTriggers)
        {
            return Physics.OverlapCapsuleNonAlloc(
                bottom,
                top,
                radius,
                _overlapsBuffer,
                layerMask,
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );
        }

        protected override int InternalOverlapBox(Vector3 center, Vector3 size, Quaternion orientation, int layerMask, bool ignoreTriggers)
        {
            return Physics.OverlapBoxNonAlloc(
                center,
                size / 2f,
                _overlapsBuffer,
                orientation,
                layerMask,
                ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
            );
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
            var closestRaycastHit = new RaycastHit();
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

        bool PerformHitCheck(ref RaycastHit raycastHit, in HitInfoFilter filter, bool allowOverlaps, HitFilterDelegate hitFilter)
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
            if (!gameObject.TryGetComponent(out Collider collider))
                return false;

            return !Physics.GetIgnoreCollision(Collider, collider);
        }
    }

}