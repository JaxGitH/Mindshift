using UnityEngine;
// This is the script attached to the pause menu
// Last update: 2/20/25
public class Pause : MonoBehaviour
{
    public void OnReturnClicked()
    {
        AudioManager.PlayEffect(eEffects.click);
        AudioManager.ResumeLevelSong();
        Time.timeScale = 1;
        Destroy(this.gameObject);
    }
    public void OnOptionsClicked()
    {
        AudioManager.PlayEffect(eEffects.click);
        AudioManager.PlaySong(eSongs.options);
        CanvasManager.Instance.ShowOptions();
    }
    public void OnExitClicked()
    {
        AudioManager.PlayEffect(eEffects.click);
        CanvasManager.Instance.ShowExitConfirm();
    }
}
