using UnityEngine;
using UnityEngine.SceneManagement;
// This script lets you put a scene/level's name on the button this script is assigned to, so the button can load the scene.
// Last update: 2/12/25
public class LoadScene : MonoBehaviour
{
    public string levelName;
    public void OnButtonPressed()
    {
        //Searches for the name of the scene/level in the editor, click the button and change the name in the Inspector.
        AudioManager.PlayEffect(eEffects.click);

        // Add this line below this
        AudioManager.PlaySong(eSongs.level);
        // Add the line above, it'll play music during the level

        //Replace this below with whatever new code you built
        SceneManager.LoadScene(levelName);
        if (levelName == null)
        {
            Debug.Log("Missing level to load.");
        }
    }
}
