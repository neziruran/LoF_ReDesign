using UnityEngine;
using System.Collections;

//public class UserSwarmController : MonoBehaviour {
public class UserSwarmController : MessageBehaviour
{
	public Transform my_user;
	public Transform other_user;
	public Transform other_insects;
	
	private Transform myTransform;
	
	private Vector3 move;
	
	public float moveSpeed = 0.01f;
	public float rotationSpeed = 10;
	public float maxDistance;


	public float pasivetime1 = 5f;
	public float pasivetime2 = 5f;
	public float pasivetime3 = 5f;

	public float insectsHeight;

	private float timer;
	
	private SwarmState mySwarmState;

	enum SwarmState
	{
		Idle,
		Following_user,
		Guiding_hunter_to_other_user,
		Guiding_hunter_to_other_insects
	}
	
	void Awake ()
	{
		myTransform = transform;
	}
	
	
	// Use this for initialization
	void OnStart () 
	{
		mySwarmState = SwarmState.Idle;	
	}
	
	// Update is called once per frame
	void Update () {
		
		
		switch(mySwarmState)
		{

		case SwarmState.Idle:
			
			if (Vector3.Distance (my_user.position, myTransform.position) > 0.2)
			{
				mySwarmState = SwarmState.Following_user;	
			}

		break;
			
		case SwarmState.Following_user:

			if (Vector3.Distance (my_user.position, myTransform.position) < 0.007)
			{
				mySwarmState = SwarmState.Idle;	
				
			}else
			{
				//myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (my_user.position - myTransform.position), 80 * Time.deltaTime);				
				myTransform.position = my_user.position;//myTransform.forward * moveSpeed * Time.deltaTime;
			}

		break;

		case SwarmState.Guiding_hunter_to_other_user:

			if (Vector3.Distance (my_user.position, myTransform.position) < 5)
			{
				myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (other_user.position - myTransform.position), rotationSpeed * Time.deltaTime);				
				myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
			}
			
			timer += Time.deltaTime;
			
			if (timer > pasivetime2 )
			{
				mySwarmState = SwarmState.Guiding_hunter_to_other_insects;
				timer = 0 ;
			}

		break;	

		case SwarmState.Guiding_hunter_to_other_insects:
			
			if (Vector3.Distance (my_user.position, myTransform.position) < 4)
			{
				myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (other_insects.position - myTransform.position), rotationSpeed * Time.deltaTime);				
				myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
			}
			
			timer += Time.deltaTime;
			
			if (timer > pasivetime3 )
			{
				mySwarmState = SwarmState.Following_user;
				timer = 0 ;
			}
			
		break;	
		}

		myTransform.position = new Vector3(myTransform.position.x, insectsHeight ,myTransform.position.z);
		
		myTransform.Rotate(new Vector3(-transform.eulerAngles.x,0,-transform.eulerAngles.z));
		
		
	}
}
