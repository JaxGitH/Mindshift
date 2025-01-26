﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KinematicCharacterController
{
    [CustomEditor(typeof(B_KinematicCharacterMotor))]
    public class KinematicCharacterMotorEditor : Editor
    {
        protected virtual void OnSceneGUI()
        {            
            B_KinematicCharacterMotor motor = (target as B_KinematicCharacterMotor);
            if (motor)
            {
                Vector3 characterBottom = motor.transform.position + (motor.Capsule.center + (-Vector3.up * (motor.Capsule.height * 0.5f)));

                Handles.color = Color.yellow;
                Handles.CircleHandleCap(
                    0, 
                    characterBottom + (motor.transform.up * motor.MaxStepHeight), 
                    Quaternion.LookRotation(motor.transform.up, motor.transform.forward), 
                    motor.Capsule.radius + 0.1f, 
                    EventType.Repaint);
            }
        }
    }
}