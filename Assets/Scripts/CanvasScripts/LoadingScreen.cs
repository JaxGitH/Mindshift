using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// This script handles loading the level scenes and the loading screen.
// Last updated: 2/20/25

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;
    public GameObject loadingScreen;
    public Image loadingBarFill;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Destroy New AudioManager");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public void LoadScene(string _levelName)
    {
        StartCoroutine(LoadSceneAsync(_levelName));
    }
    IEnumerator LoadSceneAsync(string _levelName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_levelName);
        if (_levelName == null)
        {
            Debug.Log("Missing level to load.");
        }
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBarFill.fillAmount = progressValue;
            yield return null;

        }
        //This checks what music to play when the scene loads
        if (_levelName == "FrontEnd")
        {
            AudioManager.PlaySong(eSongs.mainMenu);
        }
        else if (_levelName == "zone1Level2")
        {
            AudioManager.PlaySong(eSongs.sterileMusic);
        }
        loadingScreen.SetActive(false);
    }
}
