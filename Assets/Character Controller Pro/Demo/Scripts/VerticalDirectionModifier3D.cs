using UnityEngine;
using Lightbug.CharacterControllerPro.Core;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class VerticalDirectionModifier3D : VerticalDirectionModifier
    {
        void OnTriggerEnter(Collider other)
        {
            if (!isReady)
                return;

            CharacterActor characterActor = GetCharacter(other.transform);
            if (characterActor != null)
            {

                HandleUpDirection(characterActor);
                characterActor.Teleport(reference.referenceTransform);
            }
        }
    }



}
