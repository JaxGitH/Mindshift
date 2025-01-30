using UnityEngine;


namespace Lightbug.CharacterControllerPro.Demo
{

    public class AddTorque3D : AddTorque
    {
        new Rigidbody rigidbody = null;


        protected override void Awake()
        {
            base.Awake();

            rigidbody = GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = maxAngularVelocity;
        }

        protected override void AddTorqueToRigidbody()
        {
            rigidbody.AddRelativeTorque(torque);
        }


    }

}
