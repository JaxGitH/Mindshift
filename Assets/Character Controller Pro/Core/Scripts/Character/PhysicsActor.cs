using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    /// <summary>
    /// A physics-based actor that represents a custom 2D/3D interpolated rigidbody.
    /// </summary>
    public abstract class PhysicsActor : MonoBehaviour
    {
        [Header("Rigidbody")]

        [Tooltip("Interpolates the Transform component associated with this actor during Update calls. This is a custom implementation, the actor " +
        "does not use Unity's default interpolation.")]
        public bool interpolateActor = true;

        [Tooltip("Whether or not to use continuous collision detection (rigidbody property). " +
        "This won't affect character vs static obstacles interactions, but it will affect character vs dynamic rigidbodies.")]
        public bool useContinuousCollisionDetection = true;

        [Header("Root motion")]

        [Tooltip("This option activates root motion for the character. With root motion enabled, position and rotation are handled exclusively by the animation system.")]
        public bool UseRootMotion = false;

        [Tooltip("Whether or not to transfer position data from the root motion animation to the character.")]
        [Condition("UseRootMotion", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public bool UpdateRootPosition = true;

        [Tooltip("How the root velocity data is going to be applied to the actor.")]
        [Condition(
            new string[] { "UpdateRootPosition", "UseRootMotion" },
            new ConditionAttribute.ConditionType[] { ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.ConditionType.IsTrue },
            new float[] { 0f, 0f },
            ConditionAttribute.VisibilityType.NotEditable)]
        public RootMotionVelocityType rootMotionVelocityType = RootMotionVelocityType.SetVelocity;

        [Tooltip("Whether or not to transfer rotation data from the root motion animation to the character.")]
        [Condition("UseRootMotion", ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.VisibilityType.NotEditable)]
        public bool UpdateRootRotation = true;

        [Tooltip("How the root velocity data is going to be applied to the actor.")]
        // [Condition( "UpdateRootRotation" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
        [Condition(
            new string[] { "UpdateRootRotation", "UseRootMotion" },
            new ConditionAttribute.ConditionType[] { ConditionAttribute.ConditionType.IsTrue, ConditionAttribute.ConditionType.IsTrue },
            new float[] { 0f, 0f },
            ConditionAttribute.VisibilityType.NotEditable)]
        public RootMotionRotationType rootMotionRotationType = RootMotionRotationType.AddRotation;



        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Defines how the root velocity data is going to be applied to the actor.
        /// </summary>
        public enum RootMotionVelocityType
        {
            /// <summary>
            /// The root motion velocity will be applied as velocity.
            /// </summary>
            SetVelocity,
            /// <summary>
            /// The root motion velocity will be applied as planar velocity.
            /// </summary>
            SetPlanarVelocity,
            /// <summary>
            /// The root motion velocity will be applied as vertical velocity.
            /// </summary>
            SetVerticalVelocity,
        }


        /// <summary>
        /// Defines how the root rotation data is going to be applied to the actor.
        /// </summary>
        public enum RootMotionRotationType
        {
            /// <summary>
            /// The root motion rotation will override the current rotation.
            /// </summary>
            SetRotation,
            /// <summary>
            /// The root motion rotation will be added to the current rotation.
            /// </summary>
            AddRotation
        }

        /// <summary>
        /// This event is called prior to the physics simulation.
        /// </summary>
        public event System.Action<float> OnPreSimulation;

        /// <summary>
        /// This event is called after the physics simulation.
        /// </summary>
        public event System.Action<float> OnPostSimulation;

        Vector3 startingPosition;
        Vector3 targetPosition;
        Quaternion startingRotation;
        Quaternion targetRotation;
        Coroutine postSimulationUpdateCoroutine;
        AnimatorLink animatorLink = null;
        bool wasInterpolatingActor = false;

        /// <summary>
        /// Gets the RigidbodyComponent component associated with the character.
        /// </summary>
        public abstract RigidbodyComponent RigidbodyComponent { get; }

        /// <summary>
        /// Gets the Animator component associated with the actor.
        /// </summary>
        public Animator Animator { get; private set; }
                
        /// <summary>
        /// Gets/Sets the rigidbody velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get => RigidbodyComponent.Velocity;
            set => RigidbodyComponent.Velocity = value;
        }

        /// <summary>
        /// Gets/Sets the rigidbody velocity projected onto a plane formed by its up direction.
        /// </summary>
        public Vector3 PlanarVelocity
        {
            get => transform.TransformDirection(LocalPlanarVelocity);
            set => LocalPlanarVelocity = transform.InverseTransformDirection(value);
        }

        /// <summary>
        /// Gets/Sets the rigidbody velocity projected onto its up direction.
        /// </summary>
        public Vector3 VerticalVelocity
        {
            get => transform.TransformDirection(LocalVerticalVelocity);
            set => LocalVerticalVelocity = transform.InverseTransformDirection(value);
        }

        /// <summary>
        /// Gets/Sets the rigidbody local velocity.
        /// </summary>
        public Vector3 LocalVelocity
        {
            get => transform.InverseTransformDirection(RigidbodyComponent.Velocity);
            set => RigidbodyComponent.Velocity = transform.TransformDirection(value);
        }

        /// <summary>
        /// Gets/Sets the rigidbody local planar velocity.
        /// </summary>
        public Vector3 LocalPlanarVelocity
        {
            get
            {
                Vector3 localVelocity = LocalVelocity;
                localVelocity.y = 0f;
                return localVelocity;
            }
            set
            {
                value.y = 0f;
                LocalVelocity = value + LocalVerticalVelocity;
            }
        }

        /// <summary>
        /// Gets/Sets the rigidbody local vertical velocity.
        /// </summary>
        public Vector3 LocalVerticalVelocity
        {
            get
            {
                Vector3 localVelocity = LocalVelocity;
                localVelocity.x = localVelocity.z = 0f;
                return localVelocity;
            }
            set
            {
                value.x = value.z = 0f;
                LocalVelocity = LocalPlanarVelocity + value;
            }
        }

        /// <summary>
        /// Returns true if the character local vertical velocity is less than zero. 
        /// </summary>
        public bool IsFalling => LocalVelocity.y < 0f;

        /// <summary>
        /// Returns true if the character local vertical velocity is greater than zero.
        /// </summary>
        public bool IsAscending => LocalVelocity.y > 0f;

        /// <summary>
        /// Gets the CharacterBody component associated with this character actor.
        /// </summary>
        public bool Is2D => RigidbodyComponent.Is2D;

        /// <summary>
        /// Gets/Sets the current rigidbody position. This action will produce an "interpolation reset", meaning that (visually) the object will move instantly to the target.
        /// </summary>
        public Vector3 Position
        {
            get => RigidbodyComponent.Position;
            set
            {
                RigidbodyComponent.Position = value;
                targetPosition = value;
            }
        }

        /// <summary>
        /// Gets/Sets the current rigidbody rotation. This action will produce an "interpolation reset", meaning that (visually) the object will rotate instantly to the target.
        /// </summary>
        public Quaternion Rotation
        {
            get => transform.rotation;
            set
            {
                transform.rotation = value;
                targetRotation = value;
            }
        }


        public bool IsKinematic
        {
            get => RigidbodyComponent.IsKinematic;
            set => RigidbodyComponent.IsKinematic = value;
        }

        /// <summary>
        /// Sets the rigidbody velocity based on a target position. The same can be achieved by setting the velocity value manually.
        /// </summary>
        public void Move(Vector3 position) => RigidbodyComponent.Move(position);

        public event System.Action<Vector3, Quaternion> OnTeleport;
        public event System.Action OnAnimatorMoveEvent;
        public event System.Action<int> OnAnimatorIKEvent;

        /// <summary>
        /// Sets the teleportation position. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Vector3 position) => Teleport(position, Rotation);

        /// <summary>
        /// Sets the teleportation position and rotation using an external Transform reference. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Transform reference) => Teleport(reference.position, reference.rotation);

        /// <summary>
        /// Sets the teleportation position and rotation. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;

            ResetInterpolationPosition();
            ResetInterpolationRotation();

            OnTeleport?.Invoke(Position, Rotation);
        }

        /// <summary>
        /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.up).
        /// </summary>
        public virtual Vector3 Up
        {
            get
            {
                return Rotation * Vector3.up;
            }
            set
            {
                Quaternion deltaRotation = Quaternion.FromToRotation(Up, value);
                Rotation = deltaRotation * Rotation;
            }
        }


        /// <summary>
        /// Gets/Sets the current forward direction based on the rigidbody rotation (not necessarily transform.forward).
        /// </summary>
        public virtual Vector3 Forward
        {
            get
            {
                return Is2D ? Rotation * Vector3.right : Rotation * Vector3.forward;
            }
            set
            {
                Quaternion deltaRotation = Quaternion.FromToRotation(Forward, value);
                Rotation = deltaRotation * Rotation;
            }
        }

        /// <summary>
        /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right)
        /// </summary>
        public virtual Vector3 Right
        {
            get
            {
                return Is2D ? Rotation * Vector3.forward : Rotation * Vector3.right;
            }
            set
            {
                Quaternion deltaRotation = Quaternion.FromToRotation(Right, value);
                Rotation = deltaRotation * Rotation;
            }
        }

        #region Rotation

        /// <summary>
        /// Sets a rotation based on "forward" and "up". This is equivalent to Quaternion.LookRotation.
        /// </summary>   
        public virtual void SetRotation(Vector3 forward, Vector3 up)
        {
            Rotation = Quaternion.LookRotation(forward, up);
        }

        /// <summary>
        /// Rotates the character by doing yaw rotation (around its "up" axis).
        /// </summary>         
        public virtual void RotateAround(Quaternion deltaRotation, Vector3 pivot)
        {
            Vector3 preReferenceToPivot = pivot - Position;
            Rotation = deltaRotation * Rotation;
            Vector3 postReferenceToPivot = deltaRotation * preReferenceToPivot;
            Position += preReferenceToPivot - postReferenceToPivot;
        }

        /// <summary>
        /// Rotates the character by doing yaw rotation (around its "up" axis) based on a given "forward" vector.
        /// </summary>
        /// <param name="angle">The desired forward vector.</param> 
        public virtual void SetYaw(Vector3 forward)
        {
            Rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Forward, forward, Up), Up) * Rotation;
        }

        [System.Obsolete]
        /// <summary>
        /// Rotates the character by doing yaw rotation (around its "up" axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        public void SetYaw(float angle)
        {
            Rotation = Quaternion.AngleAxis(angle, Up) * Rotation;
        }

        /// <summary>
        /// Rotates the character by doing yaw rotation (around its "up" axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param> 
        public virtual void RotateYaw(float angle)
        {
            Rotation = Quaternion.AngleAxis(angle, Up) * Rotation;
        }

        /// <summary>
        /// Rotates the character by doing yaw rotation (around its "up" axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>  
        /// <param name="pivot">The rotation pivot in space.</param>
        public virtual void RotateYaw(float angle, Vector3 pivot)
        {
            Quaternion deltaRotation = Quaternion.AngleAxis(angle, Up);
            RotateAround(deltaRotation, pivot);
        }

        /// <summary>
        /// Rotates the character by doing pitch rotation (around its "right" axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>   
        public virtual void RotatePitch(float angle)
        {
            Rotation = Quaternion.AngleAxis(angle, Right) * Rotation;
        }

        /// <summary>
        /// Rotates the character by doing pitch rotation (around its "right" axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>       
        /// <param name="pivot">The rotation pivot in space.</param>      
        public virtual void RotatePitch(float angle, Vector3 pivot)
        {
            Quaternion deltaRotation = Quaternion.AngleAxis(angle, Right);
            RotateAround(deltaRotation, pivot);
        }

        /// <summary>
        /// Rotates the character by doing roll rotation (around its "forward" axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>  
        public virtual void RotateRoll(float angle)
        {
            Rotation = Quaternion.AngleAxis(angle, Forward) * Rotation;
        }

        /// <summary>
        /// Rotates the character by doing roll rotation (around its "forward" axis).
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>        
        /// <param name="pivot">The rotation pivot in space.</param>     
        public virtual void RotateRoll(float angle, Vector3 pivot)
        {
            Quaternion deltaRotation = Quaternion.AngleAxis(angle, Forward);
            RotateAround(deltaRotation, pivot);
        }

        /// <summary>
        /// Rotates the character by performing 180 degrees of yaw rotation (around its vertical axis). Also, interpolation (rotation) gets automatically reset 
        /// just to prevent weird visual artifacts.
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        public virtual void TurnAround()
        {
            ResetInterpolationRotation();
            RotateYaw(180f);
        }

        #endregion

        /// <summary>
        /// Configures all the animation-related components based on a given Animator component. The Animator provides root motion data along 
        /// </summary>
        [System.Obsolete]
        public void InitializeAnimation()
        {
            if (Animator == null)
                return;

#if UNITY_2023_1_OR_NEWER
            Animator.updateMode = AnimatorUpdateMode.Fixed;
#else
            Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
#endif

            if (!Animator.TryGetComponent(out animatorLink))
                animatorLink = Animator.gameObject.AddComponent<AnimatorLink>();
        }

        public void SetAnimator(Animator animator)
        {
            if (animator == null)
                return;
                        
            Animator = animator;

#if UNITY_2023_1_OR_NEWER
            Animator.updateMode = AnimatorUpdateMode.Fixed;
#else
            Animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
#endif
            if (!Animator.TryGetComponent(out animatorLink))
                animatorLink = Animator.gameObject.AddComponent<AnimatorLink>();
        }

        public void ResetIKWeights()
        {
            if (animatorLink != null)
                animatorLink.ResetIKWeights();
        }

        protected virtual void PreSimulationUpdate(float dt) { }
        protected virtual void PostSimulationUpdate(float dt) { }

        protected virtual void UpdateKinematicRootMotionPosition()
        {
            if (!UpdateRootPosition)
                return;

            Position += Animator.deltaPosition;
        }

        protected virtual void UpdateKinematicRootMotionRotation()
        {
            if (!UpdateRootRotation)
                return;

            if (rootMotionRotationType == RootMotionRotationType.AddRotation)
                Rotation *= Animator.deltaRotation;
            else
                Rotation = Animator.rootRotation;
        }

        protected virtual void UpdateDynamicRootMotionPosition()
        {
            if (!UpdateRootPosition)
                return;

            RigidbodyComponent.Move(Position + Animator.deltaPosition);
        }

        protected virtual void UpdateDynamicRootMotionRotation()
        {
            if (!UpdateRootRotation)
                return;

            if (rootMotionRotationType == RootMotionRotationType.AddRotation)
                Rotation *= Animator.deltaRotation;
            else
                Rotation = Animator.rootRotation;
        }

        void PreSimulationRootMotionUpdate()
        {
            if (RigidbodyComponent.IsKinematic)
            {
                if (UpdateRootPosition)
                    UpdateKinematicRootMotionPosition();

                if (UpdateRootRotation)
                    UpdateKinematicRootMotionRotation();
            }
            else
            {
                if (UpdateRootPosition)
                    UpdateDynamicRootMotionPosition();

                if (UpdateRootRotation)
                    UpdateDynamicRootMotionRotation();
            }
        }


        void OnAnimatorIKLinkMethod(int layerIndex) => OnAnimatorIKEvent?.Invoke(layerIndex);

        #region Interpolation

        bool internalResetFlag = true;

        bool IsReadyForInterpolation()
        {
            if (!interpolateActor)
                return false;

            if (Is2D)
            {
                return Physics2D.simulationMode == SimulationMode2D.FixedUpdate;
            }
            else
            {
#if UNITY_2022_2_OR_NEWER
                return Physics.simulationMode == SimulationMode.FixedUpdate;
#else
                return true;
#endif
            }
        }

        public void SyncBody()
        {
            if (!IsReadyForInterpolation())
                return;

            if (!wasInterpolatingActor)
                return;

            // Since the Transform component has been modified during Update calls due to the interpolation process, this will affect the body position/rotation (Rigidbody).
            // It is important to re-define the rigidbody properties with the previous physics frame targets data.
            Position = startingPosition = targetPosition;
            Rotation = startingRotation = targetRotation;

            if (internalResetFlag)
            { 
                internalResetFlag = false;
                resetPositionFlag = false;
                resetRotationFlag = false;
            }
        }


        public void InterpolateBody()
        {
            if (!IsReadyForInterpolation())
                return;

            if (wasInterpolatingActor)
            {
                float interpolationFactor = (Time.time - Time.fixedTime) / Time.fixedDeltaTime;

                transform.SetPositionAndRotation(
                    resetPositionFlag ? targetPosition : Vector3.Lerp(startingPosition, targetPosition, interpolationFactor), 
                    resetRotationFlag ? targetRotation : Quaternion.Slerp(startingRotation, targetRotation, interpolationFactor)
                );
            }
            else    // interpolation has been enabled
            {
                ResetInterpolationPosition();
                ResetInterpolationRotation();
            }
        }

        public void UpdateInterpolationTargets()
        {
            if (!IsReadyForInterpolation())
                return;

            targetPosition = Position;
            targetRotation = Rotation;

            if (resetPositionFlag)
            {
                startingPosition = targetPosition;
            }

            if (resetRotationFlag)
            {                
                startingRotation = targetRotation;
            }
        }

        bool resetPositionFlag = false;
        bool resetRotationFlag = false;

        /// <summary>
        /// Prevents the body from getting its position interpolated during one physics update.
        /// </summary>
        public void ResetInterpolationPosition() => resetPositionFlag = true;

        /// <summary>
        /// Prevents the body from getting its rotation interpolated during one physics update.
        /// </summary>
        public void ResetInterpolationRotation() => resetRotationFlag = true;

        /// <summary>
        /// Prevents the body from getting interpolated during one physics update.
        /// </summary>
        public void ResetInterpolation()
        {
            ResetInterpolationPosition();
            ResetInterpolationRotation();
        }

#endregion

        /// <summary>
        /// Checks if the Animator component associated with the character is valid or not. An Animator is valid if it 
        /// is active and its internal references are not null.
        /// </summary>
        /// <returns>True if the Animator is valid, false otherwise.</returns>
        public bool IsAnimatorValid()
        {
            if (Animator == null)
                return false;

            if (Animator.runtimeAnimatorController == null)
                return false;

            if (!Animator.gameObject.activeSelf)
                return false;

            return true;
        }

        #region Messages

        public virtual void OnValidate() { }

        protected virtual void Awake()
        {
            gameObject.GetOrAddComponent<PhysicsActorSync>();

            var animator = this.GetComponentInBranch<CharacterActor, Animator>();            
            SetAnimator(animator);
        }

        protected virtual void OnEnable()
        {
            postSimulationUpdateCoroutine ??= StartCoroutine(PostSimulationUpdate());

            if (animatorLink != null)
            {
                animatorLink.OnAnimatorMoveEvent += OnAnimatorMoveLinkMethod;
                animatorLink.OnAnimatorIKEvent += OnAnimatorIKLinkMethod;
            }

            startingPosition = targetPosition = transform.position;
            startingRotation = targetRotation = transform.rotation;

            ResetInterpolationPosition();
            ResetInterpolationRotation();
        }

        protected virtual void OnDisable()
        {
            if (postSimulationUpdateCoroutine != null)
            {
                StopCoroutine(postSimulationUpdateCoroutine);
                postSimulationUpdateCoroutine = null;
            }

            if (animatorLink != null)
            {
                animatorLink.OnAnimatorMoveEvent -= OnAnimatorMoveLinkMethod;
                animatorLink.OnAnimatorIKEvent -= OnAnimatorIKLinkMethod;
            }
        }

        protected virtual void Start()
        {
            RigidbodyComponent.ContinuousCollisionDetection = useContinuousCollisionDetection;
            RigidbodyComponent.UseInterpolation = false;

            // Interpolation
            targetPosition = startingPosition = transform.position;
            targetRotation = startingRotation = transform.rotation;
        }

        void Update()
        {
            InterpolateBody();

            wasInterpolatingActor = interpolateActor;
            internalResetFlag = true;
        }

        void OnAnimatorMoveLinkMethod()
        {
            if (!enabled)
                return;

            if (!UseRootMotion)
                return;

            float dt = Time.deltaTime;
            OnAnimatorMoveEvent?.Invoke();

            PreSimulationRootMotionUpdate();
            PreSimulationUpdate(dt);
            OnPreSimulation?.Invoke(dt);

            // 2D Physics (Box2D) requires transform.forward to be Vector3.forward/back, otherwise the simulation will ignore
            // the body due to the thinness of it.
            if (Is2D)
            {
                if (Right.z > 0f)
                    Right = Vector3.forward;
                else
                    Right = Vector3.back;
            }

            // Manual sync in case the Transform component is "dirty".
            transform.SetPositionAndRotation(Position, Rotation);
        }


        void FixedUpdate()
        {
            if (UseRootMotion)
                return;

            float dt = Time.deltaTime;

            PreSimulationUpdate(dt);
            OnPreSimulation?.Invoke(dt);

            // 2D Physics (Box2D) requires transform.forward to be Vector3.forward/back, otherwise the simulation will ignore
            // the body due to the thinness of it.
            if (Is2D)
            {
                if (Right.z > 0f)
                    Right = Vector3.forward;
                else
                    Right = Vector3.back;
            }

            // Manual sync in case the Transform component is "dirty".
            transform.SetPositionAndRotation(Position, Rotation);            
        }

        IEnumerator PostSimulationUpdate()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;

                float dt = Time.deltaTime;

                if (enabled)
                {
                    PostSimulationUpdate(dt);
                    OnPostSimulation?.Invoke(dt);
                    UpdateInterpolationTargets();
                }

            }
        }

        #endregion


    }

}
