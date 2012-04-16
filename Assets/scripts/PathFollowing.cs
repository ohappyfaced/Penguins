using UnityEngine;
using System.Collections;

public class PathFollowing : MonoBehaviour {
	// get the waypoint you have just gone to
	public Waypoint currentWaypoint;
	private CharacterController myCharacterController = null;
	private Steering steer;
	public Steering[] penguins;
	
	//steering variable
	private Vector3 steeringForce;
	
	//movement variables
	private float gravity = 0.0f;
	private Vector3 moveDirection;
	
	// Use this for initialization
	void Start () {
		myCharacterController = gameObject.GetComponent<CharacterController> ();
		steer = gameObject.GetComponent<Steering> ();
		moveDirection = transform.forward;	
	}
	
	// Update is called once per frame
	void Update () {
		CalcSteeringForce ();
		ClampSteering ();
		
		moveDirection = transform.forward * steer.Speed;
		// movedirection equals velocity
		//add acceleration
		moveDirection += steeringForce * Time.deltaTime;
		//modified for dt
		//update speed
		steer.Speed = moveDirection.magnitude;
		if (steer.Speed != moveDirection.magnitude) {
			moveDirection = moveDirection.normalized * steer.Speed;
		}
		//orient transform
		if (moveDirection != Vector3.zero)
			transform.forward = moveDirection;
		
		// Apply gravity
		moveDirection.y -= gravity;
		
		// the CharacterController moves us subject to physical constraints
		myCharacterController.Move (moveDirection * Time.deltaTime);
		
		float dist;
		dist = Mathf.Sqrt(Mathf.Pow(transform.position.x - currentWaypoint.transform.position.x,2) + Mathf.Pow(transform.position.z - currentWaypoint.transform.position.z,2));
		
		if(dist <= currentWaypoint.Radius)
		{
			changeWaypoint();	
		}
	}
	
	private void CalcSteeringForce ()
	{
		steeringForce = Vector3.zero;
		steeringForce += steer.seek(currentWaypoint.transform.position);
		steeringForce += this.collisionDetection(50) * 10;
		
		steeringForce.y = 0;
	}
	
	private void ClampSteering ()
	{
		if (steeringForce.magnitude > steer.maxForce) {
			steeringForce.Normalize ();
			steeringForce *= steer.maxForce;
		}
	}
	
	private void changeWaypoint()
	{
		currentWaypoint = currentWaypoint.next;
	}
	
	// unaligned collision detection
	private Vector3 collisionDetection(float safeDist)
	{
		Vector3 avoidCollide = Vector3.zero;
		// loop through penguins to avoid
		for(int i = 0; i < penguins.Length; i++)
		{
			// get the time
			float time = steer.timeOfClosestApproach(penguins[i]);
			
			// if time is negative, doesn't matter
			if(time < 0)
			{
				continue;
			}
			
			// get the position at that time of both objects
			Vector3 position1 = this.steer.Speed * this.transform.forward * time;
			Vector3 position2 = penguins[i].Speed * penguins[i].transform.forward * time;
			
			// if the position < safe distance, avoid the object
			if(Mathf.Abs(position1.x - position2.x) < safeDist || Mathf.Abs(position1.z - position2.z) < safeDist)
			{
				// get the dot product of the fwd and right vectors
				float fwdDot = this.steer.transform.forward.x * penguins[i].transform.forward.x +
					this.steer.transform.forward.y * penguins[i].transform.forward.y;
				// positive fwd product = slow down
				// negative fwd product = speed up
				if(fwdDot > 0)
				{
					avoidCollide += this.steer.transform.forward;	
				}
				if(fwdDot < 0)
				{
					avoidCollide -= this.steer.transform.forward;	
				}
				
				float rightDot = this.steer.transform.right.x * penguins[i].transform.right.x +
					this.steer.transform.right.y * penguins[i].transform.right.y;
				// positive right product = turn left
				// negative right product = turn right
				if(rightDot > 0)
				{
					avoidCollide += this.steer.transform.right;	
				}
				if(rightDot < 0)
				{
					avoidCollide -= this.steer.transform.right;	
				}
				
				avoidCollide.Normalize();
			}
		}
		
		// return what to do
		Debug.Log(avoidCollide);
		return avoidCollide;
	}
}
