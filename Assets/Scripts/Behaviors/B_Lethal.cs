using UnityEngine;

namespace Mindshift
{
    public class B_Lethal : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            B_PlayerController player = other.GetComponent<B_PlayerController>();
            if (player != null && player.IsAlive)
            {
                player.Die();
            }
        }
    }
}

