///////////////////////////////////////////////////////////////////////
//                                                   41 Post                                       //
// Created by DimasTheDriver on June/22/2011                                    //
// Part of 'Unity: Normal Walker' post.                          		 		 //
// Available at:     http://www.41post.com/?p=4123                              //
/////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[RequireComponent (typeof (CharacterController))]

public class NormalWalker : MonoBehaviour 
{
	//this game object's Transform
	private Transform goTransform;
	
	//the speed to move the game object
	private float speed = 6.0f;
	//the gravity
	private float gravity = 50.0f;
	
	//the direction to move the character
	private Vector3 moveDirection = Vector3.zero;
	//the attached character controller
	private CharacterController cController;
	
	//a ray to be cast 
	private Ray ray;
	//A class that stores ray collision info
	private RaycastHit hit;
	
	//a class to store the previous normal value
	private Vector3 oldNormal;
	//the threshold, to discard some of the normal value variations
	public float threshold = 0.009f;

	// Use this for initialization
	void Start () 
	{
		//get this game object's Transform
		goTransform = this.GetComponent<Transform>();
		//get the attached CharacterController component
		cController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//cast a ray from the current game object position downward, relative to the current game object orientation
		ray = new Ray(goTransform.position, -goTransform.up);  
		
		//if the ray has hit something
		if(Physics.Raycast(ray.origin,ray.direction, out hit, 5))//cast the ray 5 units at the specified direction  
		{  
			//if the current goTransform.up.y value has passed the threshold test
			if(oldNormal.y >= goTransform.up.y + threshold || oldNormal.y <= goTransform.up.y - threshold)
			{
				//set the up vector to match the normal of the ray's collision
				goTransform.up = hit.normal;
			}
			//store the current hit.normal inside the oldNormal
			oldNormal =  hit.normal;
		}  
		
		//move the game object based on keyboard input
		moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		//apply the movement relative to the attached game object orientation
		moveDirection = goTransform.TransformDirection(moveDirection);
		//apply the speed to move the game object
		moveDirection *= speed;
		
		// Apply gravity downward, relative to the containing game object orientation
		moveDirection.y -= gravity * Time.deltaTime * goTransform.up.y;
		
		// Move the game object
		cController.Move(moveDirection * Time.deltaTime);

        Debug.DrawRay(ray.origin, ray.direction, Color.green);
	}
}
