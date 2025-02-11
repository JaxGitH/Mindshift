using UnityEngine;
//This script lets you back out from the Level Select.
//Last update: 1/30/25
public class SelectLevel : MonoBehaviour
{
    public void OnReturnPressed()
    {
        AudioManager.PlayEffect(eEffects.click);
        Destroy(gameObject);
    }
}
