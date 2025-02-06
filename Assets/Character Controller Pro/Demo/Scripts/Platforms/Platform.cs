using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{



    /// <summary>
    /// This abstract class represents a basic platform.
    /// </summary>
    public abstract class Platform : MonoBehaviour
    {

        /// <summary>
        /// Gets the RigidbodyComponent component associated to the character.
        /// </summary>
        public RigidbodyComponent RigidbodyComponent { get; protected set; }

        protected virtual void Awake()
        {
            RigidbodyComponent = RigidbodyComponent.CreateInstance(gameObject);

            if (RigidbodyComponent == null)
            {
                Debug.Log("(2D/3D)Rigidbody component not found! \nDynamic platforms must have a Rigidbody component associated.");
                this.enabled = false;
            }

        }

    }

}
