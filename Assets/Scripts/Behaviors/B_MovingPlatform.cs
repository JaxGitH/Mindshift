using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;

public struct B_MovingPlatformState
{
    public PhysicsMoverState MoverState;
    public float DirectorTime;
}

public class B_MovingPlatform : MonoBehaviour, IMoverController
{
    public PhysicsMover Mover;

    public PlayableDirector Director;

    private Transform _transform;

    private void Start()
    {
        _transform = this.transform;

        Mover.MoverController = this;
    }

    // This is called every FixedUpdate by our PhysicsMover in order to tell it what pose it should go to
    public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
    {
        // Remember pose before animation
        Vector3 _positionBeforeAnim = _transform.position;
        Quaternion _rotationBeforeAnim = _transform.rotation;

        // Update animation
        EvaluateAtTime(Time.time);

        // Set our platform's goal pose to the animation's
        goalPosition = _transform.position;
        goalRotation = _transform.rotation;

        // Reset the actual transform pose to where it was before evaluating. 
        // This is so that the real movement can be handled by the physics mover; not the animation
        _transform.position = _positionBeforeAnim;
        _transform.rotation = _rotationBeforeAnim;
    }

    public void EvaluateAtTime(double time)
    {
        if (Director == null)
        {
            Debug.LogError("PlayableDirector is not assigned!");
            return;
        }

        double clampedTime = time % Director.duration;

        if (clampedTime < 0 || clampedTime > Director.duration)
        {
            Debug.LogWarning($"Attempted to evaluate timeline at an out-of-range time: {time}. Clamping to {clampedTime}.");
            clampedTime = Mathf.Clamp((float)clampedTime, 0f, (float)Director.duration);
        }

        Director.time = clampedTime;
        Director.Evaluate();
    }
}
