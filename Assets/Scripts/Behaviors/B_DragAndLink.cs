using Mindshift;
using UnityEngine;

public class B_DragAndLink : B_Draggable
{
    [SerializeField] RigidbodyEvent RbEvent;
    [SerializeField] float breakForce = 500;
    [SerializeField] float breakTorque = 500;

    private void Awake()
    {
        if (GetComponent<B_Draggable>() != null)
        {
            
            //GetComponent<B_Draggable>().enabled = false;
            //Debug.LogError("B_Draggable cannot be on the same object as B_DragAndLink");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isDragging)
        {
            AttachToRigidbody(collision.rigidbody, collision.contacts[0].point);
        }
    }

    private void AttachToRigidbody(Rigidbody targetRb, Vector3 contactPoint)
    {
        if (targetRb == null)
        {
            Debug.Log("No Rigidbody found on the collided object.");
            return;
        }

        if (GetComponent<FixedJoint>() == null) // Ensure there isn't already a joint
        {
            FixedJoint joint = gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = targetRb;
            joint.anchor = transform.InverseTransformPoint(contactPoint);
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = targetRb.transform.InverseTransformPoint(contactPoint);
            joint.breakForce = breakForce;
            joint.breakTorque = breakTorque;

            Debug.Log($"Attached {gameObject.name} to {targetRb.gameObject.name} at {contactPoint}");
        }

        RbEvent.Invoke(targetRb);
    }
}
