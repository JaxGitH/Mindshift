using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class RigidbodyEvent : UnityEvent<Rigidbody>
{

}

[System.Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

