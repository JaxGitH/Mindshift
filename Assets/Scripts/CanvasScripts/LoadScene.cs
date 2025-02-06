using UnityEngine;
using UnityEngine.SceneManagement;
//This script lets you put a scene/level's name on the button this script is assigned to, so the button can load the scene.
//Last update: 2/6/25
public class LoadScene : MonoBehaviour
{
    public string levelName;
    public void OnButtonPressed()
    {
        //Searches for the name of the scene/level in the editor, click the button and change the name in the Inspector.
        AudioManager.PlayEffect(eEffects.click);
        SceneManager.LoadScene(levelName);
        if (levelName == null)
        {
            Debug.Log("Missing level to load.");
        }
    }
}
