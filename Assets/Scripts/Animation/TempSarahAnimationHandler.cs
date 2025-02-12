using Mindshift.CharacterControllerPro.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSarahAnimationHandler : MonoBehaviour
{
    Animator animator;
    [SerializeField] private CharacterActor characterActor;
    bool isTestBool;

    private void Awake()
    {
        if (GetComponent<Animator>() != null)
            animator = GetComponent<Animator>();
        else Debug.LogError("There is no Animator Component attached to the mesh");

        /*if (GetComponent<CharacterActor>() != null)
            characterActor = GetComponent<CharacterActor>();
        else Debug.LogError("CharacterActor Component could not be retrieved");*/
        isTestBool = characterActor.isKinematic;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("groundVelocity", characterActor.Velocity.magnitude);
        animator.SetBool("isGrounded", characterActor.IsGrounded);
        //characterActor.isKinematic
    }
}
