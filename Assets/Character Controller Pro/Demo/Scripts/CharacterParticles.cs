using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    /// <summary>
    /// This component handles all the particles effects associated with the character.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Demo/Character/Character Particles")]
    public class CharacterParticles : MonoBehaviour
    {
        [HelpBox("This script contains a \"PlayFootstep\" method, which is intended to be used as an animation event function. " +
        "Please make sure the animation clip does trigger this event (if you want to see the effect in action).", HelpBoxMessageType.Warning)]
        [Tooltip("This prefab will be used by the grounded and the footsteps particles.")]
        [SerializeField]
        GameObject groundParticlesPrefab = null;

        [Header("Grounded particles")]

        [Tooltip("The character vertical speed at the moment of impact (on the horizontal axis) vs the particle system main module start speed (on the vertical axis).")]
        [SerializeField]
        AnimationCurve groundParticlesSpeed = AnimationCurve.Linear(0f, 0.5f, 10f, 3f);

        [Header("Footsteps particles")]

        [Tooltip("The character on ground speed (on the horizontal axis) vs the particle system main module start speed (on the vertical axis).")]
        [SerializeField]
        AnimationCurve footstepParticleSpeed = AnimationCurve.Linear(0f, 0.5f, 10f, 3f);

        [Tooltip("The character on ground speed (on the horizontal axis) vs the particle system main module start size (on the vertical axis).")]
        [SerializeField]
        AnimationCurve footstepParticleSize = AnimationCurve.Linear(0f, 0.5f, 10f, 3f);



        ParticleSystem[] groundParticlesArray = new ParticleSystem[10];

        ParticleSystemPooler particlesPooler = null;

        MaterialController materialController = null;

        CharacterActor CharacterActor = null;


        void Awake()
        {
            CharacterActor = this.GetComponentInBranch<CharacterActor>();


            materialController = this.GetComponentInBranch<CharacterActor, MaterialController>();

            if (materialController == null)
            {
                Debug.Log("CharacterMaterial component missing");
                this.enabled = false;
                return;
            }



            if (groundParticlesPrefab != null)
                particlesPooler = new ParticleSystemPooler(groundParticlesPrefab, CharacterActor.transform.position, CharacterActor.transform.rotation, 10);

        }

        void OnEnable() => CharacterActor.OnGroundedStateEnter += OnGroundedStateEnter;
        void OnDisable() => CharacterActor.OnGroundedStateEnter -= OnGroundedStateEnter;

        void OnGroundedStateEnter(Vector3 localVelocity)
        {
            Vector3 particlePosition = CharacterActor.transform.position;
            Quaternion particleRotation = Quaternion.LookRotation(CharacterActor.GroundContactNormal);

            float fallingSpeed = Mathf.Abs(localVelocity.y);

            float particlesStartSpeed = groundParticlesSpeed.Evaluate(fallingSpeed);

            particlesPooler.Instantiate(
                particlePosition,
                particleRotation,
                materialController.CurrentSurface.color,
                particlesStartSpeed
            );

        }

        /// <summary>
        /// Public method used by the animation events to play the footsteps particles.
        /// </summary>
        public void PlayFootstep()
        {
            if (!enabled)
                return;

            Vector3 particlePosition = CharacterActor.transform.position;
            Quaternion particleRotation = CharacterActor.GroundContactNormal != Vector3.zero ? Quaternion.LookRotation(CharacterActor.GroundContactNormal) : Quaternion.identity;

            float groundedSpeed = CharacterActor.Velocity.magnitude;
            particlesPooler.Instantiate(
                particlePosition,
                particleRotation,
                materialController.CurrentSurface.color,
                footstepParticleSpeed.Evaluate(groundedSpeed),
                footstepParticleSize.Evaluate(groundedSpeed)
            );
        }

        void Update()
        {
            particlesPooler.Update();
        }

    }


    /// <summary>
    /// This class implements a simple "Particle System Pooler". By using this system it is possible to reuse a fixed number of particles, avoiding runtime instantiation (thus improving performance).
    /// </summary>
    public class ParticleSystemPooler
    {
        List<ParticleSystem> activeList = new List<ParticleSystem>();
        List<ParticleSystem> inactiveList = new List<ParticleSystem>();

        public ParticleSystemPooler(GameObject particlePrefab, Vector3 position, Quaternion rotation, int bufferLength)
        {
            for (int i = 0; i < bufferLength; i++)
            {
                GameObject gameObject = GameObject.Instantiate<GameObject>(particlePrefab, position, rotation);
                ParticleSystem particleSystem = gameObject.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule mainModule = particleSystem.main;
                mainModule.playOnAwake = false;
                particleSystem.Stop(true);

                if (particleSystem != null)
                    inactiveList.Add(particleSystem);
            }
        }

        ParticleSystem SelectParticle()
        {
            ParticleSystem selectedParticle = null;

            if (inactiveList.Count == 0)
            {
                selectedParticle = activeList[0];
            }
            else
            {
                selectedParticle = inactiveList[0];
                inactiveList.RemoveAt(0);
                activeList.Add(selectedParticle);
            }

            return selectedParticle;


        }

        /// <summary>
        /// Puts a particle from the pool into the scene.
        /// </summary>
        public void Instantiate(Vector3 position, Quaternion rotation)
        {
            ParticleSystem particleSystem = SelectParticle();

            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;

            particleSystem.Play(true);


        }

        /// <summary>
        /// Puts a particle from the pool into the scene.
        /// </summary>
        public void Instantiate(Vector3 position, Quaternion rotation, Color color)
        {
            ParticleSystem particleSystem = SelectParticle();

            ParticleSystem.MainModule mainModule = particleSystem.main;

            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;

            Color particleColor = mainModule.startColor.color;
            particleColor.r = color.r;
            particleColor.g = color.g;
            particleColor.b = color.b;

            mainModule.startColor = particleColor;

            particleSystem.Play(true);

            activeList.Add(particleSystem);


        }

        /// <summary>
        /// Puts a particle from the pool into the scene.
        /// </summary>
        public void Instantiate(Vector3 position, Quaternion rotation, Color color, float startSpeed)
        {
            ParticleSystem particleSystem = SelectParticle();

            ParticleSystem.MainModule mainModule = particleSystem.main;

            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;

            Color particleColor = mainModule.startColor.color;
            particleColor.r = color.r;
            particleColor.g = color.g;
            particleColor.b = color.b;

            mainModule.startColor = particleColor;
            mainModule.startSpeed = startSpeed;

            particleSystem.Play(true);

            activeList.Add(particleSystem);

        }

        /// <summary>
        /// Puts a particle from the pool into the scene.
        /// </summary>
        public void Instantiate(Vector3 position, Quaternion rotation, Color color, float startSpeed, float startSize)
        {
            ParticleSystem particleSystem = SelectParticle();

            ParticleSystem.MainModule mainModule = particleSystem.main;

            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;

            Color particleColor = mainModule.startColor.color;
            particleColor.r = color.r;
            particleColor.g = color.g;
            particleColor.b = color.b;

            mainModule.startColor = particleColor;
            mainModule.startSpeed = startSpeed;
            mainModule.startSize = startSize;

            particleSystem.Play(true);

            activeList.Add(particleSystem);

        }

        /// <summary>
        /// Updates the system.
        /// </summary>
        public void Update()
        {
            for (int i = activeList.Count - 1; i >= 0; i--)
            {
                ParticleSystem particleSystem = activeList[i];

                if (!particleSystem.isPlaying)
                {
                    activeList.RemoveAt(i);

                    inactiveList.Add(particleSystem);
                }

            }

        }
    }



}
