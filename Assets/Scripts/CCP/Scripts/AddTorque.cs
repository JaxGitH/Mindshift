using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mindshift.Utilities;

namespace Mindshift.CharacterControllerPro.Demo
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
