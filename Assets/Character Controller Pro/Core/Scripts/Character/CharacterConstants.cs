namespace Lightbug.CharacterControllerPro.Core
{


    /// <summary>
    /// This class contain all the constants used for collision detection, steps detection, ground probing, etc. All the values were carefully chosen, this means that it is not recommended to modify these values at all,
    /// however if you need to do it do so at your own risk (make a backup before).
    /// </summary>
    public class CharacterConstants
    {
        /// <summary>
        /// Offset (towards the ground) applied to the ground trigger.
        /// </summary>
        public const float GroundTriggerOffset = 0.05f;

        /// <summary>
        /// This value represents the time (in seconds) that a jumping character can remain unstable while touching the ground. This becomes useful to prevent the character 
        /// to be stuck in this unstable state for too long.
        /// </summary>
        public const float MaxUnstableGroundContactTime = 0.25f;

        /// <summary>
        /// Distance between the origins of the upper and lower edge detection rays.
        /// </summary>
        public const float EdgeRaysSeparation = 0.005f;

        /// <summary>
        /// Cast distance used for the raycasts in the edge detection algorithm.
        /// </summary>
        public const float EdgeRaysCastDistance = 2f;

        /// <summary>
        /// Space between the collider and the collision shape (used by physics queries).
        /// </summary>
        public const float SkinWidth = 0.005f;

        /// <summary>
        /// Minimum offset applied to the bottom of the capsule (upwards) to avoid contact with the ground.
        /// </summary>
        public const float ColliderMinBottomOffset = 0.1f;

        /// <summary>
        /// Minimum angle between upper and lower normals (from the edge detector) that defines an edge.
        /// </summary>
        public const float MinEdgeAngle = 0.5f;

        /// <summary>
        /// Maximum angle between upper and lower normals (from the edge detector) that defines an edge.
        /// </summary>
        public const float MaxEdgeAngle = 170f;

        /// <summary>
        /// Minimum angle between upper and lower normals (from the edge detector) that defines a step.
        /// </summary>
        public const float MinStepAngle = 85f;

        /// <summary>
        /// Maximum angle between upper and lower normals (from the edge detector) that defines a step.
        /// </summary>
        public const float MaxStepAngle = 95f;

        /// <summary>
        /// Base distance used for ground probing.
        /// </summary>
        public const float GroundCheckDistance = 0.1f;

        /// <summary>
        /// Maximum number of iterations available for the collide and slide algorithm.
        /// </summary>
        public const int MaxSlideIterations = 3;

        /// <summary>
        /// Maximum number of iterations available for the collide and slide algorithm used after the simulation (dynamic ground processing).
        /// </summary>
        public const int MaxPostSimulationSlideIterations = 2;

        /// <summary>
        /// The default gravity value used by the weight function.
        /// </summary>
        public const float DefaultGravity = 9.8f;

        /// <summary>
        /// Minimum angle value considered when choosing the "head contact". The angle is measured between the contact normal and the "Up" vector.
        /// The valid range goes from "MinHeadContactAngle" to 180 degrees.
        /// </summary>
        public const float HeadContactMinAngle = 100f;

        /// <summary>
        /// Tolerance value considered when choosing the "wall contact". The angle is measured between the contact normal and the "Up" vector.
        /// The valid range goes from 90 - "WallContactAngleTolerance" to 90 degrees.
        /// </summary>
        public const float WallContactAngleTolerance = 10f;

        /// <summary>
        /// Distance used to predict the ground below the character.
        /// </summary>
        public const float GroundPredictionDistance = 10f;


    }


}