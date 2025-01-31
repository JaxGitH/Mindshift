using UnityEngine;
using UnityEngine.SceneManagement;
//This script lets you put a scene/level's name on the button this script is assigned to, so the button can load the scene.
//Last update: 1/30/25
public class LoadScene : MonoBehaviour
{
    public string levelName;
    public void OnButtonPressed()
    {
        //Searches for the name of the scene/level in the editor, click the button and change the name in the Inspector.
        SceneManager.LoadScene(levelName);
        if (levelName == null)
        {
            Debug.Log("Missing level to load.");
        }
    }
}
