﻿using UnityEngine;
using System.Collections;

namespace Mindshift
{
    public class B_Balloon : MonoBehaviour
    {
        [Header("Link Settings")]
        [SerializeField] private float breakForce = 15000f;
        [SerializeField] private float breakTorque = 15000f;
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
                Debug.LogError("B_Balloon: No Rigidbody found on " + gameObject.name);
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
            // Search all child objects for "AttachPoint"
            foreach (Transform child in target.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "AttachPoint")
                {
                    Debug.Log($"Found AttachPoint on {target.name} at {child.position}");
                    return child;
                }
            }

            Debug.LogWarning($"No AttachPoint found on {target.name}");
            return null;
        }

        private void AttachToRigidbody(Rigidbody targetRb)
        {
            if (targetRb == null) return;

            rb.isKinematic = false; // Ensure the balloon is not kinematic before attachment

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
            Debug.Log($"Balloon attached to {targetRb.gameObject.name}");

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
                    Debug.Log("Balloon lost its connection! Resetting link...");
                    ResetLink();
                    yield break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void ResetLink()
        {
            isAttached = false;
            if (floatScript != null) floatScript.enabled = false;
            joint = null; // Clear the reference
        }
    }
}