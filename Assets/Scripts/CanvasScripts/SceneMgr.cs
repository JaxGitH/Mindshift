using UnityEngine;
using UnityEngine.SceneManagement;
//This script loads the Front End scene that holds the Main Menu, and instantiating the Joystick/Pause HUD in a level.
//Last update: 2/12/2025
public enum eScene { frontEnd, inGame };
public class SceneMgr : MonoBehaviour
{
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
                break;
            case eScene.inGame:
                CanvasManager.Instance.ShowHud();
                break;
            default:
                break;
        }
    }
}
