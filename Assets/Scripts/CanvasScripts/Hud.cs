using UnityEngine;
// This script allows you to pause the game
// Last update: 2/12/25
public class Hud : MonoBehaviour
{
    public void OnPausePressed()
    {
        AudioManager.PlayEffect(eEffects.click);
        AudioManager.PauseLevelSong();
        CanvasManager.Instance.ShowPause();
    }
}
