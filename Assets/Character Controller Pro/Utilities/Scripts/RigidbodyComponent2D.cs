using System.Collections;
using UnityEngine;

namespace Lightbug.Utilities
{
    /// <summary>
    /// An implementation of RigidbodyComponent for 2D rigidbodies.
    /// </summary>
    public sealed class RigidbodyComponent2D : RigidbodyComponent
    {
        new Rigidbody2D rigidbody = null;
        RaycastHit2D[] sweepBuffer = new RaycastHit2D[10];

        protected override bool IsUsingContinuousCollisionDetection => rigidbody.collisionDetectionMode > 0;

        public override HitInfo Sweep(Vector3 position, Vector3 direction, float distance)
        {
            var p = Position;
            Position = position;
            int length = rigidbody.Cast(direction, sweepBuffer, distance);
            Position = p;

            sweepBuffer.GetClosestHit(out RaycastHit2D raycastHit, length, null);

            return new HitInfo(ref raycastHit, direction);
        }

        protected override void Awake()
        {
            base.Awake();
            rigidbody = gameObject.GetOrAddComponent<Rigidbody2D>();
            rigidbody.hideFlags = HideFlags.NotEditable;

            previousContinuousCollisionDetection = IsUsingContinuousCollisionDetection;
        }


        public override bool Is2D => true;

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
            get => rigidbody.bodyType == RigidbodyType2D.Kinematic;
            set
            {
                if (value == IsKinematic)
                    return;

                // Since CCD can't be true for kinematic bodies, the body type must change to dynamic before setting CCD
                if (value)
                {
                    ContinuousCollisionDetection = false;
                    rigidbody.bodyType = RigidbodyType2D.Kinematic;
                }
                else
                {
                    
                    rigidbody.bodyType = RigidbodyType2D.Dynamic;
                    ContinuousCollisionDetection = previousContinuousCollisionDetection;
                }

                InvokeOnBodyTypeChangeEvent();
            }
        }

        public override bool UseGravity
        {
            get => rigidbody.gravityScale != 0f;
            set => rigidbody.gravityScale = value ? 1f : 0f;
        }

        public override bool UseInterpolation
        {
            get => rigidbody.interpolation == RigidbodyInterpolation2D.Interpolate;
            set => rigidbody.interpolation = value ? RigidbodyInterpolation2D.Interpolate : RigidbodyInterpolation2D.None;
        }

        public override bool ContinuousCollisionDetection
        {
            get => rigidbody.collisionDetectionMode == CollisionDetectionMode2D.Continuous;
            set => rigidbody.collisionDetectionMode = value ? CollisionDetectionMode2D.Continuous : CollisionDetectionMode2D.Discrete;
        }

        public override RigidbodyConstraints Constraints
        {
            get
            {
                switch (rigidbody.constraints)
                {
                    case RigidbodyConstraints2D.None:
                        return RigidbodyConstraints.None;

                    case RigidbodyConstraints2D.FreezeAll:
                        return RigidbodyConstraints.FreezeAll;

                    case RigidbodyConstraints2D.FreezePosition:
                        return RigidbodyConstraints.FreezePosition;

                    case RigidbodyConstraints2D.FreezePositionX:
                        return RigidbodyConstraints.FreezePositionX;

                    case RigidbodyConstraints2D.FreezePositionY:
                        return RigidbodyConstraints.FreezePositionY;

                    case RigidbodyConstraints2D.FreezeRotation:
                        return RigidbodyConstraints.FreezeRotationZ;

                    default:
                        return RigidbodyConstraints.None;
                }

            }
            set
            {
                switch (value)
                {
                    case RigidbodyConstraints.None:
                        rigidbody.constraints = RigidbodyConstraints2D.None;
                        break;
                    case RigidbodyConstraints.FreezeAll:
                        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                        break;
                    case RigidbodyConstraints.FreezePosition:
                        rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
                        break;
                    case RigidbodyConstraints.FreezePositionX:
                        rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
                        break;
                    case RigidbodyConstraints.FreezePositionY:
                        rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
                        break;
                    case RigidbodyConstraints.FreezeRotation:
                        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                        break;
                    case RigidbodyConstraints.FreezeRotationZ:
                        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                        break;
                    default:
                        rigidbody.constraints = RigidbodyConstraints2D.None;
                        break;
                }
            }
        }

        public override Vector3 Position
        {
            get => new Vector3(rigidbody.position.x, rigidbody.position.y, transform.position.z);
            set => rigidbody.position = value;
        }

        public override Quaternion Rotation
        {
            get => Quaternion.Euler(0f, 0f, rigidbody.rotation);
            set => rigidbody.rotation = value.eulerAngles.z;
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
            get => new Vector3(0f, 0f, rigidbody.angularVelocity);
            set => rigidbody.angularVelocity = value.z;
        }

        public override void Interpolate(Vector3 position) => rigidbody.MovePosition(position);
        public override void Interpolate(Quaternion rotation) => rigidbody.MoveRotation(rotation.eulerAngles.z);

        public override Vector3 GetPointVelocity(Vector3 point) => rigidbody.GetPointVelocity(point);

        public override void AddForceToRigidbody(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            ForceMode2D forceMode2D = ForceMode2D.Force;

            if (forceMode == ForceMode.Impulse || forceMode == ForceMode.VelocityChange)
                forceMode2D = ForceMode2D.Impulse;

            rigidbody.AddForce(force, forceMode2D);
        }

        public override void AddExplosionForceToRigidbody(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0) 
            => Debug.LogWarning("AddExplosionForce is not available for 2D physics");

    }

}
