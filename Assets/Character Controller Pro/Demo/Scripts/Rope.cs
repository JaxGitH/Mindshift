using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class Rope : MonoBehaviour
    {

        [Header("Debug")]
        [SerializeField]
        bool showGizmos = true;


        [Header("Properties")]


        [SerializeField]
        Vector3 topLocalPosition = Vector3.zero;


        [SerializeField]
        Vector3 bottomLocalPosition = Vector3.zero;



        public Vector3 TopPosition
        {
            get
            {
                return transform.position + transform.TransformVectorUnscaled(topLocalPosition);
            }
        }

        public Vector3 BottomPosition
        {
            get
            {
                return transform.position + transform.TransformVectorUnscaled(bottomLocalPosition);
            }
        }

        public Vector3 BottomToTop
        {
            get
            {
                return TopPosition - BottomPosition;
            }
        }


        public bool IsInRange(Vector3 referencePosition)
        {
            Vector3 bottomToReference = referencePosition - BottomPosition;

            if (Vector3.Angle(BottomToTop, bottomToReference) > 90f)
                return false;

            Vector3 topToReference = referencePosition - TopPosition;
            if (Vector3.Angle(BottomToTop, topToReference) < 90f)
                return false;

            return true;
        }



        void OnDrawGizmos()
        {
            if (!showGizmos)
                return;

            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawSphere(TopPosition, 0.25f);

            Gizmos.color = new Color(0f, 0f, 1f, 0.2f);
            Gizmos.DrawSphere(BottomPosition, 0.25f);



        }

    }

}
