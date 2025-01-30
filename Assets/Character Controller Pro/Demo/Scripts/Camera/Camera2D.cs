using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    public enum CameraTargetMode
    {
        Bounds,
        Point
    }

    [AddComponentMenu("Character Controller Pro/Demo/Camera/Camera 2D")]
    public class Camera2D : MonoBehaviour
    {
        [Header("Target")]

        [SerializeField] Transform target = null;

        [Header("Camera size")]

        [SerializeField]
        Vector2 cameraAABBSize = new Vector2(3, 4);

        [SerializeField]
        Vector2 targetAABBSize = new Vector2(1, 1);

        [Header("Position")]

        [SerializeField]
        CameraTargetMode targetMode = CameraTargetMode.Bounds;

        [SerializeField]
        Vector3 offset = new Vector3(0f, 0f, -10f);

        [SerializeField]
        float smoothTargetTime = 0.25f;

        [Header("Rotation")]

        [SerializeField]
        bool followRotation = true;

        [Min(0.1f)]
        [SerializeField]
        float rotationSlerpSpeed = 5f;



        [Header("Look ahead")]

        [Condition("targetMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)CameraTargetMode.Bounds)]
        [SerializeField]
        float lookAheadSpeed = 4;

        [Condition("targetMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)CameraTargetMode.Bounds)]
        [SerializeField]
        float xLookAheadAmount = 1;

        [Condition("targetMode", ConditionAttribute.ConditionType.IsEqualTo, ConditionAttribute.VisibilityType.Hidden, (int)CameraTargetMode.Bounds)]
        [SerializeField]
        float yLookAheadAmount = 1;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        float xCurrentLookAheadAmount = 0;
        float yCurrentLookAheadAmount = 0;

        Vector3 targetCameraPosition;

        Vector3 smoothDampVelocity;
        Bounds cameraAABB;
        Bounds targetBounds;

        // Vector3 FinalTargetPosition => target.position + transform.TransformVector( offset );
        void Start()
        {

            if (target == null)
                Debug.Log("Missing camera target");

            Vector3 startingPosition = target.position;
            startingPosition.z = transform.position.z;
            transform.position = startingPosition;

            targetBounds = new Bounds(target.position, new Vector3(targetAABBSize.x, targetAABBSize.y, 1f));
            targetBounds.center = target.position;

            cameraAABB = new Bounds(target.position, new Vector3(cameraAABBSize.x, cameraAABBSize.y, 1f));

            targetCameraPosition = new Vector3(cameraAABB.center.x, cameraAABB.center.y, transform.position.z);
        }

        void OnDrawGizmos()
        {
            if (target == null)
                return;

            if (targetMode != CameraTargetMode.Bounds)
                return;

            Gizmos.color = new Color(0f, 0f, 1f, 0.2f);

            Bounds bounds = new Bounds(target.position, new Vector3(cameraAABBSize.x, cameraAABBSize.y, 1f));
            Gizmos.DrawCube(bounds.center, new Vector3(bounds.size.x, bounds.size.y, 1f));
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            float dt = Time.deltaTime;

            UpdateTargetAABB();
            UpdateCameraAABB(dt);

            if (followRotation)
                UpdateRotation(dt);

            UpdatePosition(dt);
        }



        void UpdateTargetAABB()
        {
            targetBounds.center = target.position;
        }

        void UpdateCameraAABB(float dt)
        {
            float deltaLookAhead = lookAheadSpeed * dt;

            //X
            if (targetBounds.max.x > cameraAABB.max.x)
            {
                float deltaX = targetBounds.max.x - cameraAABB.max.x;
                cameraAABB.center += Vector3.right * deltaX;

                if (xCurrentLookAheadAmount < xLookAheadAmount)
                {
                    xCurrentLookAheadAmount += deltaLookAhead;
                    xCurrentLookAheadAmount = Mathf.Clamp(xCurrentLookAheadAmount, -xLookAheadAmount, xLookAheadAmount);
                }


            }
            else if (targetBounds.min.x < cameraAABB.min.x)
            {
                float deltaX = cameraAABB.min.x - targetBounds.min.x;
                cameraAABB.center -= Vector3.right * deltaX;

                //Look Ahead
                if (xCurrentLookAheadAmount > -xLookAheadAmount)
                {
                    xCurrentLookAheadAmount -= deltaLookAhead;
                    xCurrentLookAheadAmount = Mathf.Clamp(xCurrentLookAheadAmount, -xLookAheadAmount, xLookAheadAmount);
                }
            }

            //Y
            if (targetBounds.max.y > cameraAABB.max.y)
            {
                float deltaY = targetBounds.max.y - cameraAABB.max.y;
                cameraAABB.center += Vector3.up * deltaY;

                //Look Ahead
                if (yCurrentLookAheadAmount < yLookAheadAmount)
                {
                    yCurrentLookAheadAmount += deltaLookAhead;
                    yCurrentLookAheadAmount = Mathf.Clamp(yCurrentLookAheadAmount, -yLookAheadAmount, yLookAheadAmount);

                }


            }
            else if (targetBounds.min.y < cameraAABB.min.y)
            {

                float deltaY = cameraAABB.min.y - targetBounds.min.y;
                cameraAABB.center -= Vector3.up * deltaY;

                //Look Ahead
                if (yCurrentLookAheadAmount > -yLookAheadAmount)
                {
                    yCurrentLookAheadAmount -= deltaLookAhead;
                    yCurrentLookAheadAmount = Mathf.Clamp(yCurrentLookAheadAmount, -yLookAheadAmount, yLookAheadAmount);
                }
            }

            targetCameraPosition.x = cameraAABB.center.x + xCurrentLookAheadAmount;
            targetCameraPosition.y = cameraAABB.center.y + yCurrentLookAheadAmount;
        }


        void UpdatePosition(float dt)
        {
            Vector3 targetPos = Vector3.zero;
            if (targetMode == CameraTargetMode.Bounds)
            {
                targetPos = Vector3.SmoothDamp(transform.position, targetCameraPosition + transform.TransformVector(offset), ref smoothDampVelocity, smoothTargetTime);
            }
            else
            {
                targetPos = Vector3.SmoothDamp(transform.position, target.position + transform.TransformVector(offset), ref smoothDampVelocity, smoothTargetTime);
            }

            transform.position = targetPos;
        }

        void UpdateRotation(float dt)
        {
            Vector3 targetUp = Vector3.ProjectOnPlane(target.up, Vector3.forward);
            Quaternion deltaRotation = Quaternion.AngleAxis(Vector3.SignedAngle(transform.up, targetUp, Vector3.forward), Vector3.forward);
            transform.rotation *= Quaternion.Slerp(Quaternion.identity, deltaRotation, rotationSlerpSpeed * dt);
        }


    }

}
