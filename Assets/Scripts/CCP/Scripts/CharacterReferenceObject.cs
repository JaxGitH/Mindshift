using UnityEngine;
using Mindshift.CharacterControllerPro.Core;

namespace Mindshift.CharacterControllerPro.Demo
{

    [System.Serializable]
    public class CharacterReferenceObject
    {
        [Tooltip("This transform up direction will be used as the character up.")]
        public Transform referenceTransform;

        [Tooltip("This transform up direction will be used as the character up.")]
        public Transform verticalAlignmentReference = null;


    }

}
