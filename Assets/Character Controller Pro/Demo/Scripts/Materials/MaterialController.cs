using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{

    [AddComponentMenu("Character Controller Pro/Demo/Material Controller")]
    [DefaultExecutionOrder(-10)]
    public class MaterialController : MonoBehaviour
    {
        [SerializeField]
        MaterialsProperties materialsProperties = null;

        CharacterActor characterActor = null;

        /// <summary>
        /// This event is called when the character enters a volume. 
        /// 
        /// The volume is passed as an argument.
        /// </summary>
        public event System.Action<Volume> OnVolumeEnter;

        /// <summary>
        /// This event is called when the character exits a volume. 
        /// 
        /// The volume is passed as an argument.
        /// </summary>
        public event System.Action<Volume> OnVolumeExit;

        /// <summary>
        /// This event is called when the character step on a surface. 
        /// 
        /// The surface is passed as an argument.
        /// </summary>
        public event System.Action<Surface> OnSurfaceEnter;

        /// <summary>
        /// This event is called when the character step off a surface. 
        /// 
        /// The surface is passed as an argument.
        /// </summary>
        public event System.Action<Surface> OnSurfaceExit;

        // Environment ------------------------------------------------------
        Volume currentVolume = null;
        Surface currentSurface = null;


        /// <summary>
        /// Gets the surface the character is colliding with. If this returns null the surface will be considered as "default".
        /// </summary>
        public Surface CurrentSurface => currentSurface;

        /// <summary>
        /// Gets the volume the character is colliding with. If this returns null the volume will be considered as "default".
        /// </summary>
        public Volume CurrentVolume => currentVolume;


        void GetSurfaceData()
        {
            if (!characterActor.IsGrounded)
            {
                SetCurrentSurface(materialsProperties.DefaultSurface);
            }
            else
            {
                var ground = characterActor.GroundObject;
                if (ground != null)
                { 
                    bool validSurface = materialsProperties.GetSurface(ground, out Surface surface);

                    if (validSurface)
                    {
                        SetCurrentSurface(surface);
                    }
                    else
                    {
                        // Untagged ground
                        if (ground.CompareTag("Untagged"))
                        {
                            SetCurrentSurface(materialsProperties.DefaultSurface);
                        }
                    }
                }
            }
        }

        void SetCurrentSurface(Surface surface)
        {
            if (surface != currentSurface)
            {
                if (OnSurfaceExit != null)
                    OnSurfaceExit(currentSurface);

                if (OnSurfaceEnter != null)
                    OnSurfaceEnter(surface);
            }

            currentSurface = surface;
        }


        void GetVolumeData()
        {
            var triggerObject = characterActor.CurrentTrigger.gameObject;
            if (triggerObject == null)
            {
                if (currentVolume != materialsProperties.DefaultVolume)
                {
                    if (OnVolumeExit != null)
                        OnVolumeExit(currentVolume);

                    SetCurrentVolume(materialsProperties.DefaultVolume);

                }
            }
            else
            {
                bool validVolume = materialsProperties.GetVolume(triggerObject, out Volume volume);

                if (validVolume)
                {
                    SetCurrentVolume(volume);
                }
                else
                {
                    // If the current trigger is not a valid volume, then search for one that is.

                    int currentTriggerIndex = characterActor.Triggers.Count - 1;

                    for (int i = currentTriggerIndex; i >= 0; i--)
                    {
                        validVolume = materialsProperties.GetVolume(characterActor.Triggers[i].gameObject, out volume);

                        if (validVolume)
                        {
                            SetCurrentVolume(volume);
                        }
                    }

                    if (!validVolume)
                    {
                        SetCurrentVolume(materialsProperties.DefaultVolume);
                    }
                }


            }
        }

        void SetCurrentVolume(Volume volume)
        {
            if (volume != currentVolume)
            {
                if (OnVolumeExit != null)
                    OnVolumeExit(currentVolume);

                if (OnVolumeEnter != null)
                    OnVolumeEnter(volume);
            }

            currentVolume = volume;
        }

        void Awake()
        {
            characterActor = this.GetComponentInBranch<CharacterActor>();

            if (characterActor == null)
            {
                this.enabled = false;
                return;
            }

            SetCurrentSurface(materialsProperties.DefaultSurface);
            SetCurrentVolume(materialsProperties.DefaultVolume);
        }

        void FixedUpdate()
        {
            GetSurfaceData();
            GetVolumeData();
        }


    }


}


