using UnityEngine;

namespace Lightbug.Utilities
{
    /// <summary>
    /// An implementation of RigidbodyComponent for 3D rigidbodies.
    /// </summary>
    public sealed class RigidbodyComponent3D : RigidbodyComponent
    {
        new Rigidbody rigidbody = null;

        protected override bool IsUsingContinuousCollisionDetection => rigidbody.collisionDetectionMode > 0;

        public override HitInfo Sweep(Vector3 position, Vector3 direction, float distance)
        {
            var p = Position;
            Position = position;
            rigidbody.SweepTest(direction, out RaycastHit raycastHit, distance);
            Position = p;
            return new HitInfo(ref raycastHit, direction);
        }

        protected override void Awake()
        {
            base.Awake();
            rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
            rigidbody.hideFlags = HideFlags.NotEditable;

            previousContinuousCollisionDetection = IsUsingContinuousCollisionDetection;
        }


        public override bool Is2D => false;

        public override float Mass
        {
            get => rigidbody.mass;
            set => rigidbody.mass = value;
        }

        public override float LinearDrag
        {
#if UNITY_6000_0_OR_NEWER
            get => rigidbody.linearDamping;
            set => rigidbody.linearDamping = value;
#else
            get => rigidbody.drag;
            set => rigidbody.drag = value;
#endif
        }

        public override float AngularDrag
        {
#if UNITY_6000_0_OR_NEWER
            get => rigidbody.angularDamping;
            set => rigidbody.angularDamping = value;
#else
            get => rigidbody.angularDrag;
            set => rigidbody.angularDrag = value;            
#endif
        }


        public override bool IsKinematic
        {
            get => rigidbody.isKinematic;
            set
            {
                if (value == IsKinematic)
                    return;

                // Since CCD can't be true for kinematic bodies, the body type must change to dynamic before setting CCD
                if (value)
                {
                    ContinuousCollisionDetection = false;
                    rigidbody.isKinematic = true;
                }
                else
                {
                    rigidbody.isKinematic = false;
                    ContinuousCollisionDetection = previousContinuousCollisionDetection;
                }

                InvokeOnBodyTypeChangeEvent();
            }
        }

        public override bool UseGravity
        {
            get => rigidbody.useGravity;
            set => rigidbody.useGravity = value;
        }

        public override bool UseInterpolation
        {
            get => rigidbody.interpolation == RigidbodyInterpolation.Interpolate;
            set => rigidbody.interpolation = value ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        }

        public override bool ContinuousCollisionDetection
        {
            get => rigidbody.collisionDetectionMode == CollisionDetectionMode.Continuous;
            set => rigidbody.collisionDetectionMode = value ? CollisionDetectionMode.Continuous : CollisionDetectionMode.Discrete;
        }

        public override RigidbodyConstraints Constraints
        {
            get => rigidbody.constraints;
            set => rigidbody.constraints = value;
        }

        public override Vector3 Position
        {
            get => rigidbody.position;
            set => rigidbody.position = value;
        }

        public override Quaternion Rotation
        {
            get => rigidbody.rotation;
            set => rigidbody.rotation = value;
        }

        public override Vector3 Velocity
        {
#if UNITY_6000_0_OR_NEWER
            get => rigidbody.linearVelocity;
            set => rigidbody.linearVelocity = value;
#else
            get => rigidbody.velocity;
            set => rigidbody.velocity = value;            
#endif
        }

        public override Vector3 AngularVelocity
        {
            get => rigidbody.angularVelocity;
            set => rigidbody.angularVelocity = value;
        }

        public override void Interpolate(Vector3 position) => rigidbody.MovePosition(position);
        public override void Interpolate(Quaternion rotation) => rigidbody.MoveRotation(rotation);
        public override Vector3 GetPointVelocity(Vector3 point) => rigidbody.GetPointVelocity(point);
        public override void AddForceToRigidbody(Vector3 force, ForceMode forceMode = ForceMode.Force) => rigidbody.AddForce(force, forceMode);
        public override void AddExplosionForceToRigidbody(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0) => rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier);
    }

}