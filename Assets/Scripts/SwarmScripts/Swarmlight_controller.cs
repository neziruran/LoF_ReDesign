using UnityEngine;
using System.Collections;

public class Swarmlight_controller : MonoBehaviour {

	public Transform myswarm;
	private float height = 0.4f;

	// Use this for initialization
	void Start () 
	{	

	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = new Vector3(myswarm.position.x, myswarm.position.y + height, myswarm.position.z);
	}
}
