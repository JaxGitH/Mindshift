using Mindshift;
using System;
using UnityEngine;

namespace KinematicCharacterController
{
    public class B_CharacterCamera : MonoBehaviour
    {
        [Header("Framing")]
        public Camera Camera;
        public Vector2 FollowPointFraming = Vector2.zero;
        public float FollowingSharpness = 10f;

        [Header("Distance")]
        public float DefaultDistance = 6f;
        public float MinDistance = 2f;
        public float MaxDistance = 10f;
        public float DistanceMovementSpeed = 5f;
        public float DistanceMovementSharpness = 10f;

        [Header("Rotation")]
        public bool InvertX = false;
        public bool InvertY = false;
        [Range(-90f, 90f)] public float DefaultVerticalAngle = 20f;
        [Range(-90f, 90f)] public float MinVerticalAngle = -60f;
        [Range(-90f, 90f)] public float MaxVerticalAngle = 60f;
        public float RotationSpeed = 1f;
        public float RotationSharpness = 15f;

        [Header("Obstruction")]
        public float ObstructionCheckRadius = 0.2f;
        public LayerMask ObstructionLayers = -1;
        public float ObstructionSharpness = 10f;

        public Transform FollowTransform { get; private set; }

        private Vector3 _planarDirection;
        private float _targetVerticalAngle;
        private float _currentDistance;
        private float _targetDistance;
        private Vector3 _currentFollowPosition;
        private bool _distanceIsObstructed;
        private const int MaxObstructions = 32;
        private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];

        private void Awake()
        {
            _currentDistance = DefaultDistance;
            _targetDistance = DefaultDistance;
//            _planarDirection = Vector3.forward;
//            _targetVerticalAngle = DefaultVerticalAngle;

            // Automatically find the Player's CameraTransform
            B_PlayerController playerController = FindObjectOfType<B_PlayerController>();
            if (playerController != null && playerController.CameraTransform != null)
            {
                SetFollowTransform(playerController.CameraTransform);
                Debug.Log("Camera follow target set to: " + playerController.name);
            }
            else
            {
                Debug.LogError("PlayerController or CameraTransform not found!");
            }
        }

        public void SetFollowTransform(Transform followTransform)
        {
            FollowTransform = followTransform;
            _planarDirection = followTransform.forward;
            _currentFollowPosition = followTransform.position;
        }

        private void Update()
        {
            // Example inputs for testing
            float zoomInput = 0f; // Replace with real input if needed
            Vector2 rotationInput = Vector2.zero; // Replace with real input if needed

            UpdateCamera(Time.deltaTime, zoomInput, rotationInput);
        }

        public void UpdateCamera(float deltaTime, float zoomInput, Vector2 rotationInput)
        {
            if (FollowTransform == null)
            {
                Debug.LogWarning("FollowTransform is not assigned. Camera cannot follow.");
                return;
            }

            // Handle input inversion
            if (InvertX) rotationInput.x *= -1f;
            if (InvertY) rotationInput.y *= -1f;

            // Update rotation
            Quaternion rotationFromInput = Quaternion.Euler(0f, rotationInput.x * RotationSpeed, 0f);
            _planarDirection = rotationFromInput * _planarDirection;
            _planarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(_planarDirection, FollowTransform.up)).normalized;

            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle - rotationInput.y * RotationSpeed, MinVerticalAngle, MaxVerticalAngle);
//            Quaternion planarRotation = Quaternion.LookRotation(_planarDirection, FollowTransform.up);
  //          Quaternion verticalRotation = Quaternion.Euler(_targetVerticalAngle, 0f, 0f);
    //        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, planarRotation * verticalRotation, 1f - Mathf.Exp(-RotationSharpness * deltaTime));
      //      transform.rotation = targetRotation;

            // Update distance
            _targetDistance = Mathf.Clamp(_targetDistance + zoomInput * DistanceMovementSpeed, MinDistance, MaxDistance);
            _currentDistance = Mathf.Clamp(_currentDistance, MinDistance, MaxDistance);

            if (float.IsNaN(_currentDistance) || float.IsInfinity(_currentDistance))
            {
                Debug.LogError($"Invalid _currentDistance detected: {_currentDistance}. Resetting to DefaultDistance.");
                _currentDistance = DefaultDistance;
            }

            // Follow the target smoothly
            _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

            if (float.IsNaN(_currentFollowPosition.x) || float.IsNaN(_currentFollowPosition.y) || float.IsNaN(_currentFollowPosition.z))
            {
                Debug.LogError($"Invalid _currentFollowPosition detected: {_currentFollowPosition}. Resetting to FollowTransform position.");
                _currentFollowPosition = FollowTransform.position;
            }

            // Handle obstructions
            float adjustedDistance = _targetDistance;
            RaycastHit closestHit;
            int obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -transform.forward, _obstructions, _targetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
            if (obstructionCount > 0)
            {
                closestHit = FindClosestObstruction(obstructionCount);
                adjustedDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1f - Mathf.Exp(-ObstructionSharpness * deltaTime));
                _distanceIsObstructed = true;
            }
            else
            {
                adjustedDistance = Mathf.Lerp(_currentDistance, _targetDistance, 1f - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
                _distanceIsObstructed = false;
            }

            adjustedDistance = Mathf.Clamp(adjustedDistance, MinDistance, MaxDistance);

            if (float.IsNaN(adjustedDistance) || float.IsInfinity(adjustedDistance))
            {
                Debug.LogError($"Invalid adjustedDistance detected: {adjustedDistance}. Resetting to DefaultDistance.");
                adjustedDistance = DefaultDistance;
            }

            _currentDistance = adjustedDistance;

            // Update position
            Vector3 targetPosition = _currentFollowPosition - (transform.forward * _currentDistance);
            targetPosition += transform.right * FollowPointFraming.x;
            targetPosition += transform.up * FollowPointFraming.y;

            if (float.IsNaN(targetPosition.x) || float.IsNaN(targetPosition.y) || float.IsNaN(targetPosition.z))
            {
                Debug.LogError($"Invalid targetPosition detected: {targetPosition}. Resetting to _currentFollowPosition.");
                targetPosition = _currentFollowPosition;
            }

            transform.position = targetPosition;
        }

        private RaycastHit FindClosestObstruction(int obstructionCount)
        {
            RaycastHit closestHit = new RaycastHit { distance = Mathf.Infinity };

            for (int i = 0; i < obstructionCount; i++)
            {
                if (_obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0)
                {
                    closestHit = _obstructions[i];
                }
            }

            return closestHit;
        }
    }
}
