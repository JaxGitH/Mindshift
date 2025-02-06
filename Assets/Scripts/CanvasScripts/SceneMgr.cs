using UnityEngine;
using UnityEngine.SceneManagement;
//This script loads the Front End scene that holds the Main Menu and such.
//To be cleaned? Possibly??
//Last update: 1/30/25
public enum eScene { frontEnd };
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
            default:
                break;
        }
    }
}
