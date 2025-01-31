using UnityEngine;
//This script is for the FrontEnd scene.
//Last update: 1/30/25
public class FrontEnd : MonoBehaviour
{
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
