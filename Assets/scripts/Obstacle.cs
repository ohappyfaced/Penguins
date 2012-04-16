using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {
	
	private float radius;
	private GameObject cam;
	
	// Use this for initialization
	void Start ()
	{
		
		
	}
	
	public float Radius {
		get 
		{
			Mesh mesh = GetComponent<MeshFilter> ().mesh;
			//Debug.Log (mesh.bounds.size.x * transform.localScale.x);
			float x = mesh.bounds.size.x * transform.localScale.x;
			float z = mesh.bounds.size.z * transform.localScale.z;
			return Mathf.Sqrt (x * x + z*z);
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}
}
