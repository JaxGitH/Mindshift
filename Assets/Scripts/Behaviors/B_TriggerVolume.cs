using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class B_TriggerVolume : MonoBehaviour
{
    [Header("Scene Loading")]
    public string sceneToLoad; // Set in Inspector

    [Header("Player Detection")]
    public bool isEnabled = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
    }

    private void LoadScene()
    {
        Debug.Log($"Loading scene: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

}
