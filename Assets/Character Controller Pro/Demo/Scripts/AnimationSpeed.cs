using UnityEngine;

namespace Lightbug.CharacterControllerPro.Demo
{

    [RequireComponent(typeof(Animator))]
    public class AnimationSpeed : MonoBehaviour
    {
        [Min(0f)]
        public float speed = 1f;

        Animator animator = null;

        void Awake() => animator = GetComponent<Animator>();
        void Start() => animator.speed = speed;
    }

}
