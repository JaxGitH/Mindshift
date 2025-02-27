﻿using UnityEngine;
using Mindshift.CharacterControllerPro.Core;

namespace Mindshift.CharacterControllerPro.Demo
{

    public class VerticalDirectionModifier2D : VerticalDirectionModifier
    {

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!isReady)
                return;

            CharacterActor characterActor = GetCharacter(other.transform);

            if (characterActor != null)
            {
                HandleUpDirection(characterActor);
                characterActor.Up = reference.referenceTransform.up;
            }
        }
    }



}
