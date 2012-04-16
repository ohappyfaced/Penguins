using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;
using System;

public class Flocking : MonoBehaviour
{
	// Each vehicle contains a CharacterController which
	// makes it easier to deal with the relationship between
	// movement initiated by the character and the forces
	// generated by contact with the terrain & other game objects.
	private CharacterController myCharacterController = null;
	private Steering steerer = null;
	private FlockManager flockMan = null;

	// a unique identification number assigned by the flock manager 
	private int index = -1;
	public int Index {
		get { return index; }
		set { index = value; }
	}
	
	//movement variables
	private float gravity = 20.0f;
	private Vector3 moveDirection;

	//steering variable
	private Vector3 steeringForce;

	//list of nearby flockers
	private List<GameObject> nearFlockers = new List<GameObject> ();
	private List<float> nearFlockersDistances = new List<float> ();
	
	public void Start ()
	{
		//get component reference
		myCharacterController = gameObject.GetComponent<CharacterController> ();
		steerer = gameObject.GetComponent<Steering> ();
		moveDirection = transform.forward;		
	}


	public void setFlockManager (GameObject fManager)
	{
		flockMan = fManager.GetComponent<FlockManager> ();
	}

	
	private Vector3 Alignment ()
	{
		return steerer.alignTo (flockMan.FlockDirection);
	}

	
	private Vector3 Cohesion ()
	{
		return steerer.seek (flockMan.Centroid);
	}


	private Vector3 Separation ()
	{
		//empty our lists
		nearFlockers.Clear ();
		nearFlockersDistances.Clear ();
		
		float dist;
		//write this - it won't work like this
		dist = flockMan.separationDist;
		
		// create a Vector3
		Vector3 dv = Vector3.zero;
		
		// get the closest flocker
		for(int i = 0; i < flockMan.numberOfFlockers; i++)
		{
			if(flockMan.getDistance(index, i) < dist)
			{
				dist = flockMan.getDistance(index, i);
				// set the desired velocity temporarily to the position
				dv = flockMan.Flockers[i].transform.position;
			}
		}
		
		// set the desired velocity to flee the position of the closest obstacle
		dv = steerer.flee(dv);

		return steerer.alignTo(dv);
	}

		
	private GameObject GetClosestObstacle ()
	{
		GameObject closest = flockMan.Obstacles[0];
		float closestDist = Vector3.Distance (transform.position, closest.transform.position);
		float nextDist;
		for (int i = 1; i < flockMan.Obstacles.Length; i++) {
			nextDist = Vector3.Distance (transform.position, flockMan.Obstacles[i].transform.position);
			if (nextDist < closestDist) {
				closest = flockMan.Obstacles[i];
				closestDist = nextDist;
			}
		}
		return closest;
	}



	private void ClampSteering ()
	{
		if (steeringForce.magnitude > steerer.maxForce) {
			steeringForce.Normalize ();
			steeringForce *= steerer.maxForce;
		}
	}

	private Vector3 stayInBounds ( float radius, Vector3 center)
	{
		if(Vector3.Distance(transform.position, center) > radius)
			return steerer.seek (center);
		else
			return Vector3.zero;
	}
	
	
	// Update is called once per frame
	public void FixedUpdate ()
	{
		CalcSteeringForce ();
		ClampSteering ();
		
		moveDirection = transform.forward * steerer.Speed;
		// movedirection equals velocity
		//add acceleration
		moveDirection += steeringForce * Time.deltaTime;
		//modified for dt
		//update speed
		steerer.Speed = moveDirection.magnitude;
		if (steerer.Speed != moveDirection.magnitude) {
			moveDirection = moveDirection.normalized * steerer.Speed;
		}
		//orient transform
		if (moveDirection != Vector3.zero)
			transform.forward = moveDirection;
		
		// Apply gravity
		moveDirection.y -= gravity;
		
		// the CharacterController moves us subject to physical constraints
		myCharacterController.Move (moveDirection * Time.deltaTime);
	}



	private void CalcSteeringForce ()
	{
		steeringForce = Vector3.zero;
		steeringForce += flockMan.inBoundsWt * stayInBounds (140, GameObject.Find("Ground").transform.position);
		steeringForce += flockMan.alignmentWt * Alignment ();
		steeringForce += flockMan.cohesionWt * Cohesion ();
		steeringForce += flockMan.separationWt * Separation ();
		steeringForce += flockMan.avoidWt * steerer.AvoidObstacle (GetClosestObstacle (), flockMan.avoidDist);
	}
	
}