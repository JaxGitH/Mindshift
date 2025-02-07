using Lightbug.CharacterControllerPro.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSarahAnimationHandler : MonoBehaviour
{
    Animator animator;
    [SerializeField] private CharacterActor characterActor;

    private void Awake()
    {
        if (GetComponent<Animator>() != null)
            animator = GetComponent<Animator>();
        else Debug.LogError("There is no Animator Component attached to the mesh");

        /*if (GetComponent<CharacterActor>() != null)
            characterActor = GetComponent<CharacterActor>();
        else Debug.LogError("CharacterActor Component could not be retrieved");*/
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Actor V: " + characterActor.Velocity.magnitude);
        Debug.Log("Animator V:" + animator.GetFloat("groundVelocity"));
        animator.SetFloat("groundVelocity", characterActor.Velocity.magnitude);
    }
}
