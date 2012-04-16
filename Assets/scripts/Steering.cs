using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;

public class Steering : MonoBehaviour
{

	private CharacterController myCharacterController = null;

	//movement variables
	private float speed = 0.0f;
	public float maxForce = 35;
	public float maxSpeed = 30;
	
	
	public float Speed {
		get { return speed; }
		set { speed = Mathf.Clamp (value, 0, maxSpeed); }
	}
	
	public void Start ()
	{
		//get component reference
		myCharacterController = gameObject.GetComponent<CharacterController> ();
	}
	public float Radius {
		get {
			Mesh mesh = GetComponent<MeshFilter> ().mesh;
			//Debug.Log (mesh.bounds.size.x * transform.localScale.x);
			float x = mesh.bounds.size.x * transform.localScale.x;
			float z = mesh.bounds.size.z * transform.localScale.z;
			return Mathf.Sqrt (x * x + z * z);
		}
	}

	
	public Vector3 seek (Vector3 pos)
	{
		// find dv, the desired velocity
		Vector3 dv = pos - transform.position;
		dv.y = 0; //only steer in the x/z plane
		dv = dv.normalized * maxSpeed;//scale by maxSpeed
		dv -= transform.forward * speed;//subtract velocity to get vector in that direction
		return dv;
	}
	
	// same as seek pos above, but parameter is game object
	public Vector3 seek (GameObject gO)
	{
		return seek(gO.transform.position);
	}

	public Vector3 flee (Vector3 pos)
	{
		Vector3 dv = transform.position - pos;//opposite direction from seek 
		dv.y = 0;
		dv = dv.normalized * maxSpeed;
		dv -= transform.forward * speed;
		return dv;
	}
	
	public Vector3 flee (GameObject go)
	{
		Vector3 targetPos = go.transform.position;
		targetPos.y = transform.position.y;
		Vector3 dv = transform.position - targetPos;
		dv = dv.normalized * maxSpeed;
		return dv - transform.forward * speed;
	}

	public Vector3 alignTo (Vector3 direction)
	{
		// useful for aligning with flock direction
		Vector3 dv = direction.normalized;
		dv.y = 0; //stay in x/z plane
		return dv * maxSpeed - transform.forward * speed;
		
	}
	
	public float timeOfClosestApproach(Steering go)
	{
		float t, b, c, e, f;
		
		// find the difference in positions
		Vector3 posDif = this.transform.position - go.transform.position;
		Vector3 velDif = (speed * this.transform.forward) - (go.Speed * go.transform.forward);
		
		b = 2 * velDif.x * posDif.x;
		c = velDif.x * velDif.x;
		e = 2 * velDif.z * posDif.z;
		f = velDif.z * velDif.z;
		
		// t is the time when an object will be closest to another object
		// assuming velocity remains constant the time will be equal to
		// the derivative of the distance squared between two wanderers
		t = -(b + e) / (2 * (c + f));
		
		return t;
	}

	//Assumptions:
	// we can access radius of obstacle
	// we have CharacterController component
	public Vector3 AvoidObstacle (GameObject obst, float safeDistance)
	{
		Vector3 dv = Vector3.zero;
		//compute a vector from character to center of obstacle
		Vector3 vecToCenter = obst.transform.position - transform.position;
		//eliminate y component so we have a 2D vector in the x, z plane
		vecToCenter.y = 0;
		float dist = vecToCenter.magnitude;
		
		//return zero vector if too far to worry about
		if (dist > safeDistance + obst.GetComponent<Obstacle> ().Radius + Radius)
			return dv;
		
		//return zero vector if behind us
		if (Vector3.Dot (vecToCenter, transform.forward) < 0)
			return dv;
		
		//return zero vector if we can pass safely
		float rightDotVTC = Vector3.Dot (vecToCenter, transform.right);
		//if (Mathf.Abs (rightDotVTC) > obst.GetComponent<Obstacle> ().Radius + GetComponent<CharacterController> ().radius)
		if (Mathf.Abs (rightDotVTC) > obst.GetComponent<Obstacle> ().Radius + Radius)
			return dv;
		
		//obstacle on right so we steer to left
		if (rightDotVTC > 0)
			dv = transform.right * -maxSpeed * safeDistance / dist;
		else
		//obstacle on left so we steer to right
			dv = transform.right * maxSpeed * safeDistance / dist;
		
		//stay in x/z plane
		dv.y = 0;
		
		//compute the force
		dv -= transform.forward * speed;
		return dv;
	}


}