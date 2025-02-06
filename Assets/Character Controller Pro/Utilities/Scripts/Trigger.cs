using UnityEngine;

namespace Lightbug.Utilities
{

    /// <summary>
    /// Struct that contains the information of the contact, gathered from the collision message ("enter" and "stay").
    /// </summary>
    public struct Trigger : System.IEquatable<Trigger>, System.IEquatable<Collider>, System.IEquatable<Collider2D>
    {
        /// <summary>
        /// Flag that indicates the enter state (OnTriggerEnter) of the trigger.
        /// </summary>
        public bool firstContact;

        //public float fixedTime;

        /// <summary>
        /// The 2D collider component associated with the trigger.
        /// </summary>
        public Collider2D collider2D;

        /// <summary>
        /// The 3D collider component associated with the trigger.
        /// </summary>
        public Collider collider3D;

        /// <summary>
        /// The gameObject associated with the trigger.
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// The transform associated with the trigger.
        /// </summary>
        public Transform transform;

        float fixedTime;

        public Trigger(Collider collider, float fixedTime) : this()
        {
            this.fixedTime = fixedTime;
            firstContact = true;
            collider3D = collider;
            gameObject = collider.gameObject;
            transform = collider.transform;
        }

        public Trigger(Collider2D collider, float fixedTime) : this()
        {
            this.fixedTime = fixedTime;
            firstContact = true;
            collider2D = collider;
            gameObject = collider.gameObject;
            transform = collider.transform;
        }

        /// <summary>
        /// Updates the internal state of the trigger.
        /// </summary>
        /// <param name="fixedTime"></param>
        public void Update(float fixedTime)
        {
            // This prevents OnTrigger calls from updating the trigger more than once at the end of the simulation stage.
            if (this.fixedTime == fixedTime)
                return;
            
            firstContact = false;
        }

        /// <summary>
        /// Sets all the structs fields, based on the callback ("enter" or "stay") and the 2D collider.
        /// </summary>
        public void Set(bool firstContact, Collider2D collider)
        {
            if (firstContact)
                fixedTime = Time.fixedTime;

            this.firstContact = firstContact;
            this.collider2D = collider;
            this.gameObject = collider.gameObject;
            this.transform = collider.transform;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(Trigger))
                return false;

            return Equals((Trigger)obj);
        }

        public override int GetHashCode() => gameObject.GetHashCode();

        public bool Equals(Collider collider3D)
        {
            if (collider3D == null)
                return false;

            return this.collider3D == collider3D;
        }

        public bool Equals(Collider2D collider2D)
        {
            if (collider2D == null)
                return false;

            return this.collider2D == collider2D;
        }

        public bool Equals(Trigger trigger)
        {
            return gameObject == trigger.gameObject;
        }

        public static bool operator ==(Trigger a, Collider b) => a.Equals(b);
        public static bool operator !=(Trigger a, Collider b) => !a.Equals(b);

        public static bool operator ==(Trigger a, Collider2D b) => a.Equals(b);
        public static bool operator !=(Trigger a, Collider2D b) => !a.Equals(b);

        public static bool operator ==(Trigger a, Trigger b) => a.Equals(b);
        public static bool operator !=(Trigger a, Trigger b) => !a.Equals(b);
    }
}

