using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectWorld : MonoBehaviour
{
    public void OnWorld1Pressed()
    {
        CanvasManager.Instance.ShowSelectLevel1();
    }
    public void OnWorld2Pressed()
    {
        CanvasManager.Instance.ShowSelectLevel2();
    }
    public void OnWorld3Pressed()
    {
        CanvasManager.Instance.ShowSelectLevel3();
    }
    public void OnReturnPressed()
    {
        Destroy(gameObject);
    }
}
