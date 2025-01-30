using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// Struct that contains the information of the contact, gathered from the collision message ("enter" and "stay").
    /// </summary>
    public readonly struct Contact
    {
        /// <summary>
        /// Flag that indicates the enter state (OnContactEnter) of the contact.
        /// </summary>
        public readonly bool firstContact;

        /// <summary>
        /// The contact point.
        /// </summary>
        public readonly Vector3 point;

        /// <summary>
        /// The contact normal.
        /// </summary>
        public readonly Vector3 normal;

        /// <summary>
        /// The 2D collider component associated with the collided object.
        /// </summary>
        public readonly Collider2D collider2D;

        /// <summary>
        /// The 3D collider component associated with the collided object.
        /// </summary>
        public readonly Collider collider3D;

        /// <summary>
        /// Flag that indicates if the collided object is a rigidbody or not.
        /// </summary>
        public readonly bool isRigidbody;

        /// <summary>
        /// Flag that indicates if the collided object is a kinematic rigidbody or not.
        /// </summary>
        public readonly bool isKinematicRigidbody;

        /// <summary>
        /// The relative velocity of the rigidbody associated. 
        /// </summary>
        public readonly Vector3 relativeVelocity;

        /// <summary>
        /// The contact point velocity. This value corresponds to the ground rigidbody velocity.
        /// </summary>
        public readonly Vector3 pointVelocity;

        /// <summary>
        /// The gameObject representing the collided object.
        /// </summary>
        public readonly GameObject gameObject;

        public Contact(bool firstContact, ContactPoint2D contact, Collision2D collision) : this()
        {
            this.firstContact = firstContact;
            this.collider2D = contact.collider;
            this.point = contact.point;
            this.normal = contact.normal;
            this.gameObject = this.collider2D.gameObject;

            var contactRigidbody = this.collider2D.attachedRigidbody;

            this.relativeVelocity = collision.relativeVelocity;

            if (this.isRigidbody = contactRigidbody != null)
            {
                this.isKinematicRigidbody = contactRigidbody.bodyType == RigidbodyType2D.Kinematic;
                this.pointVelocity = contactRigidbody.GetPointVelocity(this.point);
            }
        }


        public Contact(bool firstContact, ContactPoint contact, Collision collision) : this()
        {
            this.firstContact = firstContact;
            this.collider3D = contact.otherCollider;
            this.point = contact.point;
            this.normal = contact.normal;
            this.gameObject = this.collider3D.gameObject;

            var contactRigidbody = this.collider3D.attachedRigidbody;

            this.relativeVelocity = collision.relativeVelocity;

            if (this.isRigidbody = contactRigidbody != null)
            {
                this.isKinematicRigidbody = contactRigidbody.isKinematic;
                this.pointVelocity = contactRigidbody.GetPointVelocity(this.point);
            }
        }

        public Contact(Vector3 point, Vector3 normal, Vector3 pointVelocity, Vector3 relativeVelocity) : this()
        {
            this.point = point;
            this.normal = normal;
            this.pointVelocity = pointVelocity;
            this.relativeVelocity = relativeVelocity;
        }
    }

}

