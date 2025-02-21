using UnityEngine;
// This script helps manage the canvas/widget screens that'll be instantiated throughout the course of the game.
// Last update: 2/20/25
public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void ShowCanvasFE()
    {
        //Shows the "Front End" canvas.
        Instantiate(Resources.Load("Canvas/" + "Canvas_FE") as GameObject);
        SceneMgr.sceneString = "FrontEnd";
    }
    public void ShowSelectWorld()
    {
        //Shows the World Selection screen after you press Play in the main screen.
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectWorld") as GameObject);
    }
    public void ShowSelectLevel1()
    {
        //Shows World 1's levels
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectLevel1") as GameObject);
    }
    public void ShowSelectLevel2()
    {
        //Shows World 2's levels
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectLevel2") as GameObject);
    }
    public void ShowSelectLevel3()
    {
        //Shows World 3's levels
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectLevel3") as GameObject);
    }
    // v TO BE DELETED SOON v -Luis M.
    /*public void ShowCanvasHUD()
    {
        //Shows the Hud, which holds the pause button and the joystick.
        Instantiate(Resources.Load("Canvas/" + "Canvas_Hud") as GameObject);
        SceneMgr.sceneString = "zone1Level2";
        Debug.Log(SceneMgr.sceneString);
    }*/
    public void ShowPause()
    {
        //Shows the pause menu
        Instantiate(Resources.Load("Canvas/" + "Canvas_Pause") as GameObject);
    }
    public void ShowExitConfirm()
    {
        //Shows the confirm exit screen
        Instantiate(Resources.Load("Widgets/" + "Widget_Exit") as GameObject);
    }
    public void ShowOptions()
    {
        //Shows the options menu
        Instantiate(Resources.Load("Canvas/" + "Canvas_Options") as GameObject);
    }
}
