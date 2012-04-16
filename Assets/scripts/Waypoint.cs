using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {
	public Waypoint next;
	public Waypoint prev;
	
	private float radius;
	
	// accessor
	public float Radius { get {return radius;} }

	// Use this for initialization
	void Start () {
		radius = 20f;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
}
