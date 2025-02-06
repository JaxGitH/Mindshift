using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectLevel : MonoBehaviour
{
    public void OnLevelPressed()
    {
        Debug.Log("Level pressed! WIP");
    }
    public void OnReturnPressed()
    {
        Destroy(gameObject);
    }
}
