using UnityEngine;
//This script lets the World Select screen show each Level Select once you click it.
//Last updated: 2/20/25
public class SelectWorld : MonoBehaviour
{
    public void OnWorld1Pressed()
    {
        //Calls the method in CanvasManager to show the World 1 level select
        AudioManager.PlayEffect(eEffects.click);
        CanvasManager.Instance.ShowSelectLevel1();
    }
    public void OnWorld2Pressed()
    {
        //Calls the method in CanvasManager to show the World 2 level select
        AudioManager.PlayEffect(eEffects.click);
        CanvasManager.Instance.ShowSelectLevel2();
    }
    public void OnWorld3Pressed()
    {
        //Calls the method in CanvasManager to show the World 3 level select
        AudioManager.PlayEffect(eEffects.click);
        CanvasManager.Instance.ShowSelectLevel3();
    }
    public void OnReturnPressed()
    {
        //Lets you go back to the previous screen.
        AudioManager.PlayEffect(eEffects.click);
        Destroy(gameObject);
    }
}
