using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class B_OnObjectEnterTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent triggerEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        triggerEvent.Invoke();
    }
}
