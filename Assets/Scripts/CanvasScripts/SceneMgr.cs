using UnityEngine;
using UnityEngine.SceneManagement;
// This script loads the Front End scene that holds the Main Menu, and instantiating the Joystick/Pause HUD in a level.
// Last update: 2/25/2025
public enum eScene { frontEnd, zone1Level2, inGame };

public class SceneMgr : MonoBehaviour
{
    public static string sceneString;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        switch ((eScene)_scene.buildIndex)
        {
            case eScene.frontEnd:
                CanvasManager.Instance.ShowCanvasFE();
                sceneString = "FrontEnd";
                break;
            //  This will make loading the prefab easier since it's in a more general way instead of adding an enum type for every individual level
            //  If we need a specific song for a specific level then I don't mind adding an extra type for it I guess -Luis M.
            case eScene.inGame:
                CanvasManager.Instance.ShowCanvas_HUD();
                sceneString = "inGame";
                break;
            case eScene.zone1Level2:
                //CanvasManager.Instance.ShowCanvas_HUD();
                sceneString = "zone1Level2";
                break;
            default:
                break;
        }
    }
}