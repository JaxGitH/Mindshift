using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lightbug.CharacterControllerPro.Demo
{

    public class MainMenuManager : MonoBehaviour
    {
        string mainMenuName = "";

        static MainMenuManager instance = null;
        public static MainMenuManager Instance => instance;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                mainMenuName = SceneManager.GetActiveScene().name;
            }
            else
            {
                Destroy(gameObject);
            }

        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        public void GoToScene(string sceneName)
        {
            if (sceneName == mainMenuName)
                Cursor.visible = true;
            else
                Cursor.visible = false;

            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }



        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SceneManager.GetActiveScene().name == mainMenuName)
                    Application.Quit();
                else
                    GoToScene(mainMenuName);
            }
        }

    }

}
