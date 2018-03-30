using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuttTheBall : MonoBehaviour 
{
    // We have to use a pivot point empty game object because Unity 
    // puts the pivots in the center of objects.
    // If we could customize the pivot of our aim indicator,
    // we could skip this.
    // Instead, we use an empty game object where we want the pivot point to be,
    // which is in the center of the ball (not the center of the aim indicator).
    [Tooltip("The object we rotate to show which direction the player is aiming.")]
    [SerializeField]
    private Transform pivotPoint;

    // The built-in deadzone on the Input Manager is weird and doesn't 
    // work how I'd expect. Manually handling it gives results I understand.
    [Tooltip("Ignore analog stick input below this threshold.")]
    [SerializeField]
    private float deadzone = 0.1f;

    [Tooltip("Fully extending the analog stick will achieve max force.")]
    [SerializeField]
    private float puttMaxForce = 10f;

    private float xInput;
    private float yInput;
    Vector3 aimDirection;
    private Rigidbody rigidbody;

	// Use this for initialization
	void Start () 
	{
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () 
	{
        GetAimInput();
        UpdateAimIndicator();
        Putt();
	}

    /// <summary>
    /// Use the analog stick input to create an aim direction vector.
    /// Because our game is 3D and we want to aim along our ground XZ plane, we use the horizontal input for the X value,
    /// and the vertical input for the Z value. 
    /// </summary>
    private void GetAimInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        aimDirection = new Vector3(xInput, 0, yInput);
    }

    /// <summary>
    /// Rotate and scale the pivot point based on the aim direction.
    /// Our aim indicator is a child of the pivot point game object,
    /// so it will rotate and scale along with it's parent.
    /// </summary>
    private void UpdateAimIndicator()
    {
        // Magnitude is the "length" of the vector.
        // If we're not pushing past our deadzone, ignore input.
        // If we are, rotate and scale the pivot point based on the input.
        // The rotation will indicate the direction we will putt in.
        // The scale will indicate the strength of the putt.
        if (aimDirection.magnitude > deadzone)
        {
            pivotPoint.rotation = Quaternion.LookRotation(aimDirection, transform.up);
            pivotPoint.localScale = new Vector3(pivotPoint.localScale.x, pivotPoint.localScale.y, aimDirection.magnitude);
        }
        else
        {
            pivotPoint.localScale = new Vector3(pivotPoint.localScale.x, pivotPoint.localScale.y, 0);
        }
    }
    private void Putt()
    {
        // Because our anlog stick input is recorded from zero to one,
        // the magnitude will top out around 1. Which is nice for "100%" force...
        // If we push the analog stick half way, it should be around mangitude 0.5,
        // or 50% force.
        if (Input.GetButtonDown("Fire1"))
        {
            float force = puttMaxForce * aimDirection.magnitude;

            // We can add force in the direction of our pivotpoint's forward, because
            // we are rotating it based on the aim input every frame.
            rigidbody.AddForce(pivotPoint.transform.forward * force, ForceMode.Impulse);
        }
    }
}
