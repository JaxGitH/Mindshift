using UnityEngine;
// This script is to make an interactable object glow when you're in its vicinity.
// Make sure to attach this to a GameObject with a Box Collider that wraps around the object, but more than the Randobox's trigger.
// Also because of the commenting scheme you can tell this was written by me (Luis M.), so if you see this then hi!
// Last updated: 2/25/25

public class B_Glow : MonoBehaviour
{
    [SerializeField] private GameObject glow;

    private void OnTriggerEnter(Collider other)
    {
        glow.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        glow.SetActive(false);
    }
}
