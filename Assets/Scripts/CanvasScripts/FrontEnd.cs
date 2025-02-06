using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEnd : MonoBehaviour
{
    public void OnPlayPressed()
    {
        CanvasManager.Instance.ShowSelectWorld();
    }
    public void OnOptionsPressed()
    {
        Debug.Log("<color=yellow>Options pressed. WIP</color>");
    }
    public void OnExtrasPressed()
    {
        Debug.Log("<color=yellow>Extras pressed. WIP!</color>");
    }

}
