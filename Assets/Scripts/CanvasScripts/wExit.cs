using UnityEngine;
// This script handles the "Exit back to Level Select" button on the Pause menu
// Last update: 2/12/25
public class wExit : MonoBehaviour
{
    public void NoPressed()
    {
        AudioManager.PlayEffect(eEffects.click);
        Destroy(this.gameObject);
    }
}
