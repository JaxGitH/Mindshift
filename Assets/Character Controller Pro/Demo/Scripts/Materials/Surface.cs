using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{
    [System.Serializable]
    public class Surface
    {
        public string tagName = "";

        [Header("Movement")]

        [Min(0.01f)]
        public float accelerationMultiplier = 1f;

        [Min(0.01f)]
        public float decelerationMultiplier = 1f;

        [Min(0.01f)]
        public float speedMultiplier = 1f;

        [Header("Particles")]

        public Color color = Color.gray;
    }

}

