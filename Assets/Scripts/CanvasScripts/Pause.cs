using UnityEngine;
// This is the script attached to the pause menu
// Last update: 2/12/25
public class Pause : MonoBehaviour
{
    public void OnReturnClicked()
    {
        AudioManager.PlayEffect(eEffects.click);
        AudioManager.ResumeLevelSong();
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
