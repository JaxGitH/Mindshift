using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    public abstract class AddTorque : MonoBehaviour
    {
        [SerializeField]
        protected Vector3 torque;

        [SerializeField]
        protected float maxAngularVelocity = 200f;

        protected abstract void AddTorqueToRigidbody();

        protected virtual void Awake() { }



        void FixedUpdate()
        {
            AddTorqueToRigidbody();
        }


    }

}
