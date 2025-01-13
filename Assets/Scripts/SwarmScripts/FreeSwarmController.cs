using UnityEngine;
using System.Collections;

public class FreeSwarmController : MessageBehaviour {

	public Transform target;

	public Transform player1;
	public Transform player2;

	public float distanceToPlayers;
	
	private Transform myTransform;

	private Transform playerTransform;
	
	private Vector3 move; 

	private Vector3 prev_pos;
	
	public float amplitude = 0.002f;
	public float frequency =  0.001f;

	public float moveSpeed = 0.01f;
	public float rotationSpeed = 0.01f;
	private float timer = 0f;
	private float escaping_time = 1f;

	public float speedMin = 0.10f;
	public float speedMax = 0.20f;
	
	public float xMax;
	public float zMax;
	public float xMin;
	public float zMin;
	
	private float x;
	private float z;
	private float tiempo;
	private float angulo;

	public float waitingNextToPlayer = 2.0f;
	private float counterNextPlayer1 = 0f;
	private float counterNextPlayer2 = 0f;
	
	private float uplimit = 0.7f;
	private float downlimit = 0.6f;

	private SwarmState mySwarmState;

	enum SwarmState
	{
		Wandering_around,
		Moving_away_player,
		Chasing_player,
		Escaping_player
	}
	
	void Awake ()
	{
		myTransform = transform;	
	} 

	protected override void OnStart () 	
	{
		GameObject go = GameObject.FindGameObjectWithTag ("Player");
		target = go.transform;
		
		x = Random.Range(-speedMax, speedMax);
		z = Random.Range(-speedMax, speedMax);
		angulo = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
		transform.localRotation = Quaternion.Euler( 0, angulo, 0);

		prev_pos = myTransform.position;

		mySwarmState = SwarmState.Wandering_around;

		Messenger.RegisterListener(new Listener("Insect_catched_player1", gameObject, "escape_player"));
		Messenger.RegisterListener(new Listener("Insect_catched_player2", gameObject, "escape_player"));
	}
	
	public void escape_player(Message m)
	{
		playerTransform = m.MessageSource.transform;
		if ( m.MessageValue == gameObject.transform.name)
		{
			mySwarmState = SwarmState.Escaping_player;	
		}
	}

	// Update is called once per frame
	void Update () 
	{	
		myTransform.position = prev_pos;

		switch(mySwarmState)
		{
			case SwarmState.Wandering_around:
				{
					WanderAround();

					UpdatePosition();
				}
				break;
			case SwarmState.Moving_away_player:
				{
					MoveAwayFromPlayers();
					
					UpdatePosition();
				}
					break;
			case SwarmState.Chasing_player:
				{
					myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (target.position - myTransform.position), rotationSpeed * Time.deltaTime);				
					myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
				}
				break;
			case SwarmState.Escaping_player:
				{
					transform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (myTransform.position - playerTransform.position), 0.001f * Time.deltaTime);				
					myTransform.position -= myTransform.forward * 1 * Time.deltaTime;
					
					timer+=Time.deltaTime;
					
					if (timer > escaping_time )
					{
						mySwarmState = SwarmState.Wandering_around;
						timer = 0 ;
					}
				}
					break;
		}
		
		prev_pos = myTransform.position;
		
		move.x = 0;
		move.y = 0.008f + amplitude*(Mathf.Sin(2*Mathf.PI*frequency*Time.time) - Mathf.Sin(2*Mathf.PI*frequency*(Time.time - Time.deltaTime)));
		move.z = 0;
		transform.position += move * 5 * Time.deltaTime;

		transform.position = new Vector3 (Mathf.Clamp (transform.position.x, -3, 3), 
		                                  Mathf.Clamp (transform.position.y, downlimit, uplimit),
		                                  Mathf.Clamp (transform.position.z, -3, 3));

		myTransform.Rotate(new Vector3(-transform.eulerAngles.x,0,-transform.eulerAngles.z));
	}

	private void WanderAround()
	{
		if ((player1.position - myTransform.position).sqrMagnitude < distanceToPlayers*distanceToPlayers || counterNextPlayer1 != 0)
		{
			mySwarmState = SwarmState.Moving_away_player;
			counterNextPlayer1 = 0f;
		}
		else 
		{
			if ((player2.position - myTransform.position).sqrMagnitude < distanceToPlayers*distanceToPlayers || counterNextPlayer2 != 0)
			{
				mySwarmState = SwarmState.Moving_away_player;
				counterNextPlayer2 = 0f;
			}
			else
			{
				tiempo += Time.deltaTime;
				
				if (transform.localPosition.x > xMax) {
					x = Random.Range(-speedMax, speedMin);
					angulo = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
					transform.localRotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler(0, angulo, 0), 0.0001f * Time.deltaTime);
					tiempo = 0.0f; 
				}
				if (transform.localPosition.x < xMin) {
					x = Random.Range(speedMin, speedMax);
					angulo = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
					transform.localRotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler(0, angulo, 0), 0.0001f * Time.deltaTime); 
					tiempo = 0.0f; 
				}
				if (transform.localPosition.z > zMax) {
					z = Random.Range(-speedMax, speedMin);
					angulo = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
					transform.localRotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler(0, angulo, 0), 0.0001f * Time.deltaTime); 
					tiempo = 0.0f; 
				}
				if (transform.localPosition.z < zMin) {
					z = Random.Range(speedMin, speedMax);
					angulo = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
					transform.localRotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler(0, angulo, 0), 0.0001f * Time.deltaTime);
					tiempo = 0.0f; 
				}
				
				if (tiempo > 5.0f) {
					x = Random.Range(-speedMax, speedMax);
					z = Random.Range(-speedMax, speedMax);
					angulo = Mathf.Atan2(x, z) * (180 / 3.141592f) + 90;
					transform.localRotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler(0, angulo, 0), 0.0001f * Time.deltaTime);
					tiempo = 0.0f;
				}
			}
		}
	}

	private void MoveAwayFromPlayers()
	{
		if ((player1.position - myTransform.position).sqrMagnitude < distanceToPlayers*distanceToPlayers || counterNextPlayer1 != 0)
		{
			if(counterNextPlayer1 == 0)
			{
				float length = Vector3.Distance(transform.position, player1.position);
				x = (transform.position.x - player1.position.x)/length;
				x *= speedMax;
				z = (transform.position.z - player1.position.z)/length;
				z *= speedMax;
			}
			counterNextPlayer1+=Time.deltaTime;
			if(counterNextPlayer1 > waitingNextToPlayer)
			{
				counterNextPlayer1 = 0.0f;
			}
		}
		else 
		{
			if ((player2.position - myTransform.position).sqrMagnitude < distanceToPlayers*distanceToPlayers || counterNextPlayer2 != 0)
			{
				if(counterNextPlayer2 == 0)
				{
					float length = Vector3.Distance(transform.position, player2.position);
					x = (transform.position.x - player2.position.x)/length;
					x *= speedMax;
					z = (transform.position.z - player2.position.z)/length;
					z *= speedMax;
				}
				counterNextPlayer2+=Time.deltaTime;
				if(counterNextPlayer2 > waitingNextToPlayer)
				{
					counterNextPlayer2 = 0.0f;
				}
			}
			else
			{
				mySwarmState = SwarmState.Wandering_around;
				timer = 0f;
			}
		}
	}

	private void UpdatePosition()
	{
		transform.localPosition = new Vector3(transform.localPosition.x + x * Time.deltaTime, transform.localPosition.y, transform.localPosition.z + z * Time.deltaTime);
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (transform.position, 0.03f);

		switch(mySwarmState)
		{
			case SwarmState.Wandering_around:
				{
					Gizmos.color = Color.white;
					Gizmos.DrawRay (transform.position, new Vector3 (x, 0, z) * 0.01f);
				}
					break;
			case SwarmState.Moving_away_player:
				{
					Gizmos.color = Color.yellow;
					Gizmos.DrawRay (transform.position, new Vector3 (x, 0, z) * 0.01f);
				}
					break;
			case SwarmState.Escaping_player:
				{
					Gizmos.color = Color.red; 
					Gizmos.DrawRay (transform.position, myTransform.forward);
				}
				break;
		}
	}
}
