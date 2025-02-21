using System.Collections;
using UnityEngine;

namespace Mindshift
{
    public class B_Magnet : MonoBehaviour
    {
        [Header("Magnet Settings")]
        [SerializeField] private float breakForce = 150000f;
        [SerializeField] private float breakTorque = 150000f;
        [SerializeField] private LayerMask attachableLayers;
        private SpringJoint joint;
        private Rigidbody rb;
        private B_Float floatScript;
        public bool isAttached = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            floatScript = GetComponent<B_Float>();

            if (rb == null)
            {
                Debug.LogError("B_Magnet: No Rigidbody found on " + gameObject.name);
                enabled = false;
            }

            if (floatScript != null)
            {
                floatScript.enabled = false; // Disable floating initially
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isAttached) return; // Prevent multiple attachments

            Transform attachPoint = FindAttachPoint(other.transform);
            if (attachPoint == null) return; // No valid attach point found

            Rigidbody targetRb = attachPoint.GetComponent<Rigidbody>();
            if (targetRb == null) targetRb = other.GetComponent<Rigidbody>(); // Fallback to parent Rigidbody

            if (targetRb == null || ((1 << other.gameObject.layer) & attachableLayers) == 0) return;

            AttachToRigidbody(targetRb);
        }

        private Transform FindAttachPoint(Transform target)
        {
            // Search all child objects for "MagnetPoint"
            foreach (Transform child in target.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "MagnetPoint")
                {
                    Debug.Log($"Found MagnetPoint on {target.name} at {child.position}");
                    return child;
                }
            }

            Debug.LogWarning($"No MagnetPoint found on {target.name}");
            return null;
        }

        private void AttachToRigidbody(Rigidbody targetRb)
        {
            if (targetRb == null) return;

            rb.isKinematic = false; // Ensure the Magnet is not kinematic before attachment

            if (joint == null)
            {
                SpringJoint springJoint = gameObject.AddComponent<SpringJoint>();
                springJoint.connectedBody = targetRb;
                springJoint.spring = 500f; // Adjust spring force as needed
                springJoint.damper = 2f; // Helps with stability
                springJoint.minDistance = 0;
                springJoint.maxDistance = 0.1f;

                joint = springJoint;
            }

            isAttached = true;
            Debug.Log($"Magnet attached to {targetRb.gameObject.name}");

            StartCoroutine(EnableFloatAfterStabilization());
        }

        private IEnumerator EnableFloatAfterStabilization()
        {
            yield return new WaitForSeconds(0.1f); // Allow time for joint stabilization

            if (floatScript != null)
            {
                floatScript.enabled = true; // Enable floating only AFTER attachment stabilizes
            }
        }

        private IEnumerator CheckJointStability()
        {
            yield return new WaitForSeconds(0.2f); // Delay to ensure Unity properly registers the joint

            while (isAttached)
            {
                if (joint == null || joint.connectedBody == null)
                {
                    Debug.Log("Magnet lost its connection! Resetting Magnet...");
                    ResetMagnet();
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ResetMagnet()
        {
            isAttached = false;
            if (floatScript != null) floatScript.enabled = false;
            joint = null; // Clear the reference
        }
    }
}
