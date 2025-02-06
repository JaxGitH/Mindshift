using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    public void ShowCanvasFE()
    {
        Instantiate(Resources.Load("Canvas/" + "Canvas_FE") as GameObject);
    }
    public void ShowSelectWorld()
    {
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectWorld") as GameObject);
    }
    public void ShowSelectLevel1()
    {
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectLevel1") as GameObject);
    }
    public void ShowSelectLevel2()
    {
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectLevel2") as GameObject);
    }
    public void ShowSelectLevel3()
    {
        Instantiate(Resources.Load("Canvas/" + "Canvas_SelectLevel3") as GameObject);
    }
}
