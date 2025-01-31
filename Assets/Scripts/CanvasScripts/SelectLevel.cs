using UnityEngine;
//This script lets you back out from the Level Select.
//Last update: 1/30/2025
public class SelectLevel : MonoBehaviour
{
    public void OnReturnPressed()
    {
        Destroy(gameObject);
    }
}
