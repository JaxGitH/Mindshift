using UnityEngine;
using Mindshift.CharacterControllerPro.Core;

namespace Mindshift.CharacterControllerPro.Demo
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
