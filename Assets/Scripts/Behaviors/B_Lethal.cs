using UnityEngine;
using Mindshift.CharacterControllerPro.Core;

public class B_Lethal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CharacterActor character = other.GetComponent<CharacterActor>();
        if (character != null)
        {
            Die(character);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CharacterActor character = collision.collider.GetComponent<CharacterActor>();
        if (character != null)
        {
            Die(character);
        }
    }

    private void Die(CharacterActor character)
    {
        Debug.Log(character.name + " has died! Respawning at last checkpoint...");
        character.Respawn();
    }
}
