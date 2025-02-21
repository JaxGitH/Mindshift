using UnityEngine;
// This script allows you to pause the game
// Last update: 2/20/25
public class Hud : MonoBehaviour
{
    public void OnPausePressed()
    {
        AudioManager.PlayEffect(eEffects.click);
        Time.timeScale = 0;
        AudioManager.PauseLevelSong();
        CanvasManager.Instance.ShowPause();
    }
}
