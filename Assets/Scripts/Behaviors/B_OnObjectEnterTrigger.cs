using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class B_OnObjectEnterTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent triggerEvent;
    [SerializeField] UnityEvent<string> triggerEventString;
    [SerializeField] bool useStringTrigger = false;
    [SerializeField] string _name;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision Detected");
        if (useStringTrigger && triggerEventString != null)
        {
            Debug.Log("Will send string");
            triggerEventString.Invoke(_name);
        }
        else if (triggerEvent != null)
        {
            Debug.Log("No string");
            triggerEvent.Invoke();
        }
    }
}
