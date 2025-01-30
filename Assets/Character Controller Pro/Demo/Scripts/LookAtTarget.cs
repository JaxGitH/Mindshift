using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class LookAtTarget : MonoBehaviour
    {
        [SerializeField]
        Transform lookAtTarget = null;

        [SerializeField]
        Transform positionTarget = null;

        [SerializeField]
        bool invertForwardDirection = true;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        Vector3 initialPositionOffset = default(Vector3);

        void Start()
        {
            if (positionTarget != null)
                initialPositionOffset = positionTarget.position - transform.position;
        }

        void Update()
        {
            if (lookAtTarget != null)
            {
                transform.LookAt(lookAtTarget);

                if (invertForwardDirection)
                    transform.Rotate(Vector3.up * 180f);
            }

            if (positionTarget != null)
                transform.position = positionTarget.position + initialPositionOffset;

        }
    }

}
