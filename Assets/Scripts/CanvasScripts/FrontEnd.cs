using UnityEngine;
//This script is for the FrontEnd scene.
//Last update: 2/6/25
public class FrontEnd : MonoBehaviour
{
    private void Awake()
    {
        //Test to see if AudioManager loads the song on start
        AudioManager.PlaySong(eSongs.song1);
    }
    public void OnPlayPressed()
    {
        //"OnPlayPressed()" is for the Play button in the main screen, and it'll load the World/Lab Select canvas.
        CanvasManager.Instance.ShowSelectWorld();
    }
    public void OnOptionsPressed()
    {
        //"OnOptionsPressed()" displays the Options menu (CURRENTLY WIP)
        Debug.Log("<color=yellow>Options pressed. WIP</color>");
    }
    public void OnExtrasPressed()
    {
        //"OnExtrasPressed()" displays the Extras menu (CURRENTLY WIP)
        Debug.Log("<color=yellow>Extras pressed. WIP!</color>");
    }

}
