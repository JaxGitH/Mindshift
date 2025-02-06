using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{

    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererUtility : MonoBehaviour
    {
        public Transform target = null;

        LineRenderer lineRenderer = null;

        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        void Update()
        {
            if (target == null)
                return;

            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(
                new Vector3[] {
                    transform.position,
                    target.position
                }
            );
        }
    }

}
