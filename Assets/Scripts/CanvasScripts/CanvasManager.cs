using UnityEngine;
//This script helps manage the canvas/widget screens that'll be instantiated throughout the course of the game.
//Last update: 1/30/25
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
}
