using UnityEngine;
// This script is for the FrontEnd scene.
// Last update: 2/12/25
public class FrontEnd : MonoBehaviour
{
    private void Awake()
    {
        //Plays the main menu song the moment the Front End is loaded, and it stops whatever music track was playing.
        AudioManager.PlaySong(eSongs.mainmenu);
    }
    public void OnPlayPressed()
    {
        //"OnPlayPressed()" is for the Play button in the main screen, and it'll load the World/Lab Select canvas.
        AudioManager.PlayEffect(eEffects.click);
        CanvasManager.Instance.ShowSelectWorld();
    }
    public void OnOptionsPressed()
    {
        //"OnOptionsPressed()" shows the Options menu.
        CanvasManager.Instance.ShowOptions();
        AudioManager.PlaySong(eSongs.options);
        AudioManager.PlayEffect(eEffects.click);
    }
    public void OnExtrasPressed()
    {
        //"OnExtrasPressed()" displays the Extras menu (CURRENTLY WIP)
        AudioManager.PlayEffect(eEffects.click);
        Debug.Log("<color=yellow>Extras pressed. WIP!</color>");
    }

}
