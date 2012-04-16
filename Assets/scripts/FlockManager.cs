using UnityEngine;
using System.Collections;
//including some .NET for dynamic arrays called List in C#
using System.Collections.Generic;

public class FlockManager : MonoBehaviour
{
	// weight parameters are set in editor and used by all flockers 
	// if they are initialized here, the editor will override settings	 
	// weights used to arbitrate btweeen concurrent steering forces 
	public float alignmentWt;
	public float separationWt;
	public float cohesionWt;
	public float avoidWt;
	public float inBoundsWt;

	// these distances modify the respective steering behaviors
	public float avoidDist;
	public float separationDist;
	

	// set in editor to promote reusability.
	public int numberOfFlockers;
	public Object flockerPrefab;
	public Object obstaclePrefab;
	
	//values used by all flockers that are calculated by controller on update
	private Vector3 flockDirection;
	private Vector3 centroid;
	
	//accessors
	public Vector3 FlockDirection { get {return flockDirection;} }
	public Vector3 Centroid { get {return centroid; } }
		
	// list of flockers with accessor
	private List<GameObject> flockers = new List<GameObject>();
	public List<GameObject> Flockers {get{return flockers;}}

	// array of obstacles with accessor
	private  GameObject[] obstacles;
	public GameObject[] Obstacles {get{return obstacles;}}
	
	// this is a 2-dimensional array for distances between flockers
	// it is recalculated each frame on update
	private float[,] distances;
		

	public void Start ()
	{
		
		//construct our 2d array based on the value set in the editor
		distances = new float[numberOfFlockers, numberOfFlockers];
		//reference to Vehicle script component for each flocker
		Flocking flocker; // reference to flocker scripts
	
		obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
		
		for(int i = 0; i < numberOfFlockers; i++)
		{
			//Instantiate a flocker prefab, catch the reference, cast it to a GameObject
			//and add it to our list all in one line.
			flockers.Add((GameObject)Instantiate(flockerPrefab, 
				new Vector3(50+5*i,5,50), Quaternion.identity));
			//grab a component reference
			flocker = flockers[i].GetComponent<Flocking>();
			//set values in the Vehicle script
			flocker.Index = i;
			flocker.setFlockManager(gameObject);
		}
		
	}
	public void Update( )
	{
		calcCentroid( );//find average position of each flocker 
		calcFlockDirection( );//find average "forward" for each flocker
		calcDistances( );
	}
	
	
	void calcDistances( )
	{
		float dist;
		for(int i = 0 ; i < numberOfFlockers; i++)
		{
			for( int j = i+1; j < numberOfFlockers; j++)
			{
				dist = Vector3.Distance(flockers[i].transform.position, flockers[j].transform.position);
				distances[i, j] = dist;
				distances[j, i] = dist;
			}
		}
	}
	
	public float getDistance(int i, int j)
	{
		return distances[i, j];
	}
	
	
		
	private void calcCentroid ()
	{
		// calculate the current centroid of the flock
		Vector3 cent = new Vector3();
		// use transform.position
		for(int i = 0; i < numberOfFlockers; i++)
		{
			cent += flockers[i].transform.position;
		}
		
		// get the average
		centroid = cent / numberOfFlockers;
	}
	
	private void calcFlockDirection ()
	{
		// calculate the average heading of the flock
		// use transform.
		Vector3 dir = new Vector3();
		for(int i = 0; i < numberOfFlockers; i++)
		{
			dir += flockers[i].transform.forward;
		}
		
		// get the avg
		flockDirection = dir / numberOfFlockers;
	}
	
}