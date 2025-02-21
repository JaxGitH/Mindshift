using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mindshift.CharacterControllerPro.Core;
using Mindshift.Utilities;

namespace Mindshift.CharacterControllerPro.Demo
{

    public class PerformanceCharacterBehaviour : MonoBehaviour
    {
        public CharacterActor characterActor = null;

        float sineAmplitude;
        float sineAngularSpeed;
        float sinePhase;

        void Start()
        {

            sineAmplitude = Random.Range(8f, 15f);
            sineAngularSpeed = Random.Range(0.5f, 2f);
            sinePhase = Random.Range(0f, 90f);

        }
        void FixedUpdate()
        {
            characterActor.VerticalVelocity += Vector3.down * 15f * Time.deltaTime;

            characterActor.PlanarVelocity = CustomUtilities.Multiply(Vector3.forward, sineAmplitude * Mathf.Sin(Time.time * sineAngularSpeed + sinePhase));
        }
    }

}
