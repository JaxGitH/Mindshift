using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class AddTorque2D : AddTorque
    {
        new Rigidbody2D rigidbody = null;

        protected override void Awake()
        {
            base.Awake();

            rigidbody = GetComponent<Rigidbody2D>();
        }

        protected override void AddTorqueToRigidbody()
        {
            rigidbody.AddTorque(torque.z);
            rigidbody.angularVelocity = Mathf.Clamp(rigidbody.angularVelocity, -maxAngularVelocity, maxAngularVelocity);
        }


    }

}
