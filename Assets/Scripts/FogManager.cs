using UnityEngine;
using System.Collections;

public class FogManager : MessageBehaviour {

	public GameObject fogParticle;

	public int numberParticles;
	public float particleDistance;
	public float floorDistance;

	private GameObject[] fogParticles;
	private Vector3[] fogParticlesOriginalPosition;

	public Transform player1Position;
	public Transform player2Position;

	public Transform m;
	public float maxDistance;
	public float openSpeed;
	public float closeSpeed;
	public float percentageOfRandom;
	
	private bool freeze;

	public enum FogState
	{
		Following_players,
		Following_creatures,
		Freeze,
	};

	public FogState myFogState;
	

	// Use this for initialization
	protected override void OnStart () 
	{
		
		myFogState = FogState.Following_players;

		Messenger.RegisterListener(new Listener("creaturesarenear", gameObject, "FreezeOn"));
		Messenger.RegisterListener(new Listener("MergeNewCreature1", gameObject, "FollowingPlayers"));
		Messenger.RegisterListener(new Listener("MergeNewCreature2", gameObject, "FollowingPlayers"));
		Messenger.RegisterListener(new Listener("GoingToMerge", gameObject, "FollowingCreatures"));
		Messenger.RegisterListener(new Listener("FogFollowingCreatures", gameObject, "FollowingCreatures"));
		Messenger.RegisterListener(new Listener("FogFollowingPlayers", gameObject, "FollowingPlayers"));
		
		freeze = false;
		
		//fogParticle.layer = 11;

		// Save memory space for vectors of important data
		fogParticles = new GameObject[numberParticles];
		fogParticlesOriginalPosition = new Vector3[numberParticles];

		// Get players transforms
		GameObject tempPlayer1GameObject = GameObject.FindGameObjectWithTag ("Player");
		player1Position = tempPlayer1GameObject.transform;
		GameObject tempPlayer2GameObject = GameObject.FindGameObjectWithTag ("Player2");
		player2Position = tempPlayer2GameObject.transform;

		// Instantiate particles
		int fogParticleCreated = 0;
		float offset = (Mathf.Sqrt(numberParticles) * particleDistance)/2;
		float randomOffset = particleDistance * percentageOfRandom;
		for (int x = 0; x < (int)Mathf.Sqrt(numberParticles); x++) 
		{
			for (int y = 0; y < (int)Mathf.Sqrt(numberParticles); y++) 
			{
				Quaternion rotationTemp = Quaternion.identity;
				rotationTemp.eulerAngles = new Vector3(0, Random.Range (-percentageOfRandom*100, percentageOfRandom*100), 0);
				GameObject tempGameObject = Instantiate(fogParticle, 
				                                        new Vector3 (x*particleDistance-offset+Random.Range(-randomOffset, randomOffset), 
				            										 floorDistance+Random.Range(-randomOffset, randomOffset), 
				             										 y*particleDistance-offset+Random.Range(-randomOffset, randomOffset)), 
				                                        rotationTemp) as GameObject;
				fogParticlesOriginalPosition[fogParticleCreated] = tempGameObject.transform.position;
				fogParticles[fogParticleCreated] = tempGameObject;
				fogParticleCreated++;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
		switch(myFogState)
		{
			case FogState.Following_players:

				FogBehaviour(player1Position.position, player2Position.position);

			break;
			
			case FogState.Following_creatures:

				GameObject creature1 = GameObject.FindGameObjectWithTag("creature1");
				GameObject creature2 = GameObject.FindGameObjectWithTag("creature2");
				
				if (creature1 == null)
				{
					FogBehaviour(player1Position.position, new Vector3(creature2.transform.position.x, floorDistance, creature2.transform.position.z) );			
				}else if (creature2 == null)
				{
					FogBehaviour(new Vector3(creature1.transform.position.x, floorDistance, creature1.transform.position.z), player2Position.position);			
				}else
				{
					FogBehaviour(new Vector3(creature1.transform.position.x, floorDistance, creature1.transform.position.z), new Vector3(creature2.transform.position.x, floorDistance, creature2.transform.position.z));
				}

			break;

			case FogState.Freeze:

			break;

			}
			
		}
		
		
	void FreezeOn()
	{
		myFogState = FogState.Freeze;
	}
	void FollowingCreatures()
	{
		myFogState = FogState.Following_creatures;
	}
	void FollowingPlayers()
	{ 
		myFogState = FogState.Following_players;
	}

	void FogBehaviour(Vector3 position1, Vector3 position2)
	{
		for (int i=0; i < fogParticles.Length; i++) 
		{
			m.position = fogParticles[i].transform.position;
			
			int k = 0;
			int p = 0;
			
			if ((position1 - m.position).sqrMagnitude < maxDistance*maxDistance) 
			{
				k = 1;
				m.rotation = Quaternion.LookRotation (position1 - m.position);
				m.position -= m.forward * openSpeed * Time.deltaTime;
			}
			
			if ((position2 - m.position).sqrMagnitude < maxDistance*maxDistance) 
			{
				p = 1;
				m.rotation = Quaternion.LookRotation (position2 - m.position);
				m.position -= m.forward * openSpeed * Time.deltaTime;
			}
			
			if(k!=1 && p !=1)
			{
				if ((m.position - fogParticlesOriginalPosition[i]).sqrMagnitude > 0.001f*0.001f)
				{
					m.rotation = Quaternion.LookRotation (fogParticlesOriginalPosition[i] - m.position);
					m.position += m.forward * closeSpeed * Time.deltaTime;	
				}
				else
				{
					m.position = this.fogParticlesOriginalPosition[i];
				}
			}
			fogParticles[i].transform.position = new Vector3(m.position.x, fogParticlesOriginalPosition[i].y, m.position.z);
		}
	}
	
	
	
	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere ( player1Position.position, maxDistance-0.3f);
		Gizmos.DrawWireSphere ( player2Position.position, maxDistance-0.3f);
		
	}
}
