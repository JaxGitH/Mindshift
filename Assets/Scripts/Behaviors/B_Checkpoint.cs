using UnityEngine;
using Mindshift.CharacterControllerPro.Core;

public class B_Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointNumber = 0; // Unique checkpoint number

    private void OnTriggerEnter(Collider other)
    {
        CharacterActor character = other.GetComponent<CharacterActor>();
        if (character != null)
        {
            character.SetCheckpoint(transform.position, checkpointNumber);
            Debug.Log("Checkpoint reached: " + checkpointNumber);
        }
    }
}
