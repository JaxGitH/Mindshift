using KinematicCharacterController;
using UnityEngine;

public class B_Player : MonoBehaviour
{
    public Transform CameraFollowPoint;
    public B_PlayerController PlayerController;
    public B_CharacterCamera CharacterCamera;

    private void Start()
    {
        if (PlayerController == null)
        {
            PlayerController = GetComponent<B_PlayerController>();
        }

        if (CharacterCamera == null)
        {
            CharacterCamera = FindObjectOfType<B_CharacterCamera>();
        }

        if (CharacterCamera != null && CameraFollowPoint != null)
        {
            CharacterCamera.SetFollowTransform(CameraFollowPoint);
//            CharacterCamera.IgnoredColliders.AddRange(GetComponentsInChildren<Collider>());
        }
        else
        {
            Debug.LogError("CharacterCamera or CameraFollowPoint is not set!");
        }
    }
}
