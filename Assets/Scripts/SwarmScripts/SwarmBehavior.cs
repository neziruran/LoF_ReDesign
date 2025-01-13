using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SwarmBehavior : MessageBehaviour {
	
	public int droneCount;
	public float spawnRadius = 1f;
	public List<GameObject> drones;

	public Vector2 swarmBounds = new Vector2(5f, 5f);

	public GameObject prefab;
	public GameObject huntedParticlesPrefab;
	
	private SkinnedMeshRenderer[] childRenderer;
	
	private int insectsCreated = 0;
	private int insectToBeCreated1 = -1;
	private int insectToBeCreated2 = -1;
	private GameObject huntedInsect1 = null;
	private GameObject huntedInsect2 = null;

	private int insectsHunted = 0;
	private int glowingInsect1Player1 = -1;
	private int glowingInsect2Player1 = -1;
	private int glowingInsect1Player2 = -1;
	private int glowingInsect2Player2 = -1;

	public bool updatingList = false;

	private float originalSteer;
	private float counter;

	private string messageSender;

	// Use this for initialization
	protected override void OnStart () {
		if (prefab == null)
		{
			Debug.Log("["+Time.time+"]Please assign a drone prefab.");
			return;
		}

		// instantiate the drones
		GameObject droneTemp;
		drones = new List<GameObject>();
		for (int i = 0; i < droneCount; i++)
		{
			droneTemp = (GameObject) GameObject.Instantiate(prefab);
			DroneBehavior db = droneTemp.GetComponent<DroneBehavior>();
			db.drones = this.drones;
			db.swarm = this;

			// spawn inside circle
			Vector2 pos = new Vector2(transform.position.x, transform.position.z) + Random.insideUnitCircle * spawnRadius;
			droneTemp.transform.position = new Vector3(pos.x, transform.position.y, pos.y);
			droneTemp.transform.parent = transform;

			originalSteer = droneTemp.GetComponent<DroneBehavior>().maxSteer;
			
			drones.Add(droneTemp);
		}

		// Resgister listener depending on who you are
		if (gameObject.tag == "SwarmPlayer1")
		{
			Messenger.RegisterListener(new Listener("Hunted_insects", gameObject, "SelectInsectsToCreate"));
		}
		if (gameObject.tag == "SwarmPlayer2")
		{
			Messenger.RegisterListener(new Listener("Hunted_insects", gameObject, "SelectInsectsToCreate"));
		}
		if (gameObject.tag == "free_swarm")
		{
			Messenger.RegisterListener(new Listener("Close_to_swarm_player1", gameObject, "SelectHuntedInsects1"));
			Messenger.RegisterListener(new Listener("Close_to_swarm_player2", gameObject, "SelectHuntedInsects2"));

			Messenger.RegisterListener(new Listener("Away_from_swarm_player1", gameObject, "FreedHuntedInsects1"));
			Messenger.RegisterListener(new Listener("Away_from_swarm_player2", gameObject, "FreedHuntedInsects2"));

			Messenger.RegisterListener(new Listener("Insect_catched_player1", gameObject, "DesactivateInsect"));
			Messenger.RegisterListener(new Listener("Insect_catched_player2", gameObject, "DesactivateInsect"));
		}

		Messenger.RegisterListener(new Listener("Creature_created", gameObject, "UpdatePlayerListsNewCreature"));
		Messenger.RegisterListener(new Listener("Texture_changed", gameObject, "UpdatePlayerListsTexture"));

		Messenger.RegisterListener(new Listener("Insects_of_swarm", gameObject, "Return_insects"));
	}
	
	public void SelectHuntedInsects1(Message m)
	{
		if (gameObject.name == m.MessageValue) {
			int n = 0;

			for (int k = insectsHunted; k < droneCount; k++) {
				if (k != glowingInsect1Player1 && k != glowingInsect1Player2 && k!= glowingInsect2Player1 && k != glowingInsect2Player2) 
				{
					GameObject droneTemp = drones [k]; 
					childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer> ();
					if(childRenderer [0].material.color.a != 0)
					{
						if(glowingInsect1Player1 == -1)
						{
							glowingInsect1Player1 = k;
							n += 1;
						}
						else
						{
							if(glowingInsect2Player1 == -1)
							{
								glowingInsect2Player1 = k;
								n += 1;
							}
						}
					}
				}

				if (n == 2) {
					break;
				}
			}
			Debug.Log ("["+Time.time+"] Insects to be hunted from "+gameObject.name+" by P1: " + glowingInsect1Player1 + " " + glowingInsect2Player1);
		}
	}

	public void SelectHuntedInsects2(Message m)
	{
		if (gameObject.name == m.MessageValue) {
			int n = 0;

			for (int k = insectsHunted; k < droneCount; k++) {
				if (k != glowingInsect1Player1 && k != glowingInsect1Player2 && k!= glowingInsect2Player1 && k != glowingInsect2Player2) 
				{
					GameObject droneTemp = drones [k]; 
					childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer> ();
					if(childRenderer [0].material.color.a != 0)
					{
						if(glowingInsect1Player2 == -1)
						{
							glowingInsect1Player2 = k;
							n += 1;
						}
						else
						{
							if(glowingInsect2Player2 == -1)
							{
								glowingInsect2Player2 = k;
								n += 1;
							}
						}
					}
				}

				if (n == 2) {
					break;
				}
			}
			Debug.Log ("["+Time.time+"] Insects to be hunted from "+gameObject.name+" by P2: " + glowingInsect1Player2 + " " + glowingInsect2Player2);
		}
	}

	public void FreedHuntedInsects1(Message m)
	{
		if (glowingInsect1Player1 != -1 && gameObject.name == m.MessageValue) 
		{
			Debug.Log ("["+Time.time+"] Insects freed from "+gameObject.name+" by P1: " + glowingInsect1Player1 + " " + glowingInsect2Player1);

			GameObject droneTemp = drones [glowingInsect1Player1]; 
			childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer> ();
			childRenderer [0].material.color = new Color (1, 1, 1, 1);
			glowingInsect1Player1 = -1;
			
			droneTemp = drones [glowingInsect2Player1]; 
			childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer> ();
			childRenderer [0].material.color = new Color (1, 1, 1, 1);
			glowingInsect2Player1 = -1;
		}
	}

	public void FreedHuntedInsects2(Message m)
	{
		if (glowingInsect1Player2 != -1 && gameObject.name == m.MessageValue) 
		{
			Debug.Log ("["+Time.time+"] Insects freed from "+gameObject.name+" by P2: " + glowingInsect1Player2 + " " + glowingInsect2Player2);

			GameObject droneTemp = drones [glowingInsect1Player2]; 
			childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer> ();
			childRenderer [0].material.color = new Color (1, 1, 1, 1);
			glowingInsect1Player2 = -1;
		
			droneTemp = drones [glowingInsect2Player2]; 
			childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer> ();
			childRenderer [0].material.color = new Color (1, 1, 1, 1);
			glowingInsect2Player2 = -1;
		}
	}

	public void DesactivateInsect(Message m)
	{
		if (gameObject.name == m.MessageValue) 
		{
			if(glowingInsect1Player1 != -1)
			{
				GameObject droneTemp1 = drones [glowingInsect1Player1]; 
				childRenderer = droneTemp1.GetComponentsInChildren<SkinnedMeshRenderer> ();
				childRenderer [0].material.color = new Color (1, 1, 1, 0);
				glowingInsect1Player1 = -1;

				GameObject droneTemp2 = drones [glowingInsect2Player1]; 
				childRenderer = droneTemp2.GetComponentsInChildren<SkinnedMeshRenderer> ();
				childRenderer [0].material.color = new Color (1, 1, 1, 0);
				glowingInsect2Player1 = -1;

				Messenger.SendToListeners(new HuntedInsectsPositions(gameObject, "Hunted_insects", "user1_swarm", droneTemp1.transform.position, droneTemp2.transform.position));

				insectsHunted += 2;
			}

			if(glowingInsect1Player2 != -1)
			{
				GameObject droneTemp1 = drones [glowingInsect1Player2]; 
				childRenderer = droneTemp1.GetComponentsInChildren<SkinnedMeshRenderer> ();
				childRenderer [0].material.color = new Color (1, 1, 1, 0);
				glowingInsect1Player2 = -1;

				GameObject droneTemp2 = drones [glowingInsect2Player2]; 
				childRenderer = droneTemp2.GetComponentsInChildren<SkinnedMeshRenderer> ();
				childRenderer [0].material.color = new Color (1, 1, 1, 0);
				glowingInsect2Player2 = -1;

				Messenger.SendToListeners(new HuntedInsectsPositions(gameObject, "Hunted_insects", "user2_swarm", droneTemp1.transform.position, droneTemp2.transform.position));

				insectsHunted += 2;
			}
		}
	}

	public void SelectInsectsToCreate(HuntedInsectsPositions m)
	{
		if (gameObject.name == m.MessageValue) {
			int n = 0;
			
			for (int k = insectsCreated; k < droneCount; k++) 
			{
				if (k != insectToBeCreated1 && k != insectToBeCreated1) 
				{
					GameObject droneTemp = drones [k]; 
					childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer> ();
					if(childRenderer[0].material.color.a == 0)
					{
						if(insectToBeCreated1 == -1)
						{
							insectToBeCreated1 = k;
							n += 1;
						}
						else
						{
							if(insectToBeCreated2 == -1 && k != insectToBeCreated1)
							{
								insectToBeCreated2 = k;
								n += 1;
							}
						}
					}
				}
				
				if (n == 2) {
					break;
				}
			}
		
			GameObject droneTemp1 = drones [insectToBeCreated1]; 
			childRenderer = droneTemp1.GetComponentsInChildren<SkinnedMeshRenderer> ();
			if (gameObject.tag == "SwarmPlayer1") {
				childRenderer [0].material.color = new Color (1, 0, 0, 1);
			}
			if (gameObject.tag == "SwarmPlayer2") {
				childRenderer [0].material.color = new Color (0, 0, 1, 1);
			}
			insectsCreated += 1;

			GameObject huntedParticles1 = (GameObject)GameObject.Instantiate (huntedParticlesPrefab);
			huntedParticles1.transform.position = droneTemp1.transform.position;
			
			Messenger.SendToListeners (new Message_transform (gameObject, "Insect_created", gameObject.tag, transform));
			Debug.Log ("[" + Time.time + "]  Insect created: " + insectToBeCreated1 + " created.");

			insectToBeCreated1 = -1;
			
			GameObject droneTemp2 = drones [insectToBeCreated2]; 
			childRenderer = droneTemp2.GetComponentsInChildren<SkinnedMeshRenderer> ();
			if (gameObject.tag == "SwarmPlayer1") {
				childRenderer [0].material.color = new Color (1, 0, 0, 1);
			}
			if (gameObject.tag == "SwarmPlayer2") {
				childRenderer [0].material.color = new Color (0, 0, 1, 1);
			}
			insectsCreated += 1;

			GameObject huntedParticles2 = (GameObject)GameObject.Instantiate (huntedParticlesPrefab);
			huntedParticles2.transform.position = droneTemp2.transform.position;
			
			Messenger.SendToListeners (new Message_transform (gameObject, "Insect_created", gameObject.tag, transform));
			Debug.Log ("[" + Time.time + "]  Insect created: " + insectToBeCreated2 + " created.");

			insectToBeCreated2 = -1;
		}

	}

	public void Return_insects(Insects_swarm m)
	{
		if (gameObject.name == "free_swarm1")
		{
			int n=0;
			for (int k=0; k<droneCount ; k++)
			{
				GameObject droneTemp = drones[k]; 
				
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
				
				int newalpha=255;	
				if (childRenderer[0].material.color.a == 0)
				{
					childRenderer[0].material.color = new Color(childRenderer[0].material.color.r, childRenderer[0].material.color.g, childRenderer[0].material.color.b, newalpha);
					n += 1;
				}
				this.drones[k] = droneTemp;
				if (n == m.Insects_swarms[0])
				{
					break;
				}
			}

			insectsHunted = 0;
		}
		if (gameObject.name == "free_swarm2")
		{
			int n=0;
			for (int k=0; k<droneCount ; k++)
			{
				GameObject droneTemp = drones[k]; 
				
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();

				int newalpha=255;	
				if (childRenderer[0].material.color.a == 0)
				{
					childRenderer[0].material.color = new Color(childRenderer[0].material.color.r, childRenderer[0].material.color.g, childRenderer[0].material.color.b, newalpha);
					n += 1;
				}
				this.drones[k] = droneTemp;
				if (n == m.Insects_swarms[1])
				{
					break;
				}
			}

			insectsHunted = 0;
		}
		if (gameObject.name == "free_swarm3")
		{
			int n=0;
			for (int k=0; k<droneCount ; k++)
			{
				GameObject droneTemp = drones[k]; 
				
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();

				int newalpha=255;	
				if (childRenderer[0].material.color.a == 0)
				{
					childRenderer[0].material.color = new Color(childRenderer[0].material.color.r, childRenderer[0].material.color.g, childRenderer[0].GetComponent<Renderer>().material.color.b, newalpha);
					n += 1;
				}
				this.drones[k] = droneTemp;
				if (n == m.Insects_swarms[2])
				{
					break;
				}
			}

			insectsHunted = 0;
		}
		if (gameObject.name == "free_swarm4")
		{
			int n=0;
			for (int k=0; k<droneCount ; k++)
			{
				GameObject droneTemp = drones[k]; 
				
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();

				int newalpha=255;	
				if (childRenderer[0].material.color.a == 0)
				{
					childRenderer[0].material.color = new Color(childRenderer[0].material.color.r, childRenderer[0].material.color.g, childRenderer[0].material.color.b, newalpha);
					n += 1;
				}
				this.drones[k] = droneTemp;
				if (n == m.Insects_swarms[3])
				{
					break;
				}
			}

			insectsHunted = 0;
		}
	}

	public void UpdatePlayerListsNewCreature(Message m)
	{
		if (gameObject.name == m.MessageValue)
		{
			updatingList = true;
			messageSender = "Main";

			insectsCreated = 0;
			
			insectToBeCreated1 = -1;
			insectToBeCreated2 = -1;

			insectsHunted = 0;
			
			glowingInsect1Player1 = -1;
			glowingInsect2Player1 = -1;
			glowingInsect1Player2 = -1;
			glowingInsect2Player2 = -1;
		}
			
	}
	public void UpdatePlayerListsTexture(Message m)
	{
		if (gameObject.name == m.MessageValue)
		{
			updatingList = true;
			messageSender = "CreatureController";

			insectsCreated = 0;

			insectToBeCreated1 = -1;
			insectToBeCreated2 = -1;

			insectsHunted = 0;
			
			glowingInsect1Player1 = -1;
			glowingInsect2Player1 = -1;
			glowingInsect1Player2 = -1;
			glowingInsect2Player2 = -1;
		}
		
	}
	

	/*public void CreateInsect(Message m)
	{
		if (insectsCreated <= droneCount-4)
		{
			int n=0;
			for (int k=0; k<droneCount ; k++)
			{
				GameObject droneTemp = drones[k]; 

				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();

				int newalpha=255;	
				if (childRenderer[0].material.color.a == 0)
				{
					childRenderer[0].material.color = new Color(childRenderer[0].material.color.r, childRenderer[0].material.color.g, childRenderer[0].material.color.b, newalpha);
					n += 1;
					insectsCreated += 1;
				}
				this.drones[k] = droneTemp;
				if (n == 2)
				{
					break;
				}
			}
		}
		else
		{
			insectsCreated = 0;
		}
	}*/

	protected virtual void Update () 
	{
			float colorPercentage = Mathf.Abs(Mathf.Sin(2*Mathf.PI*0.75f*Time.time));
			float originalPercentage = 1.0f - colorPercentage;
	
			// Glow insects for player 1
			if (glowingInsect1Player1 != -1) 
			{
				GameObject droneTemp = drones[glowingInsect1Player1]; 
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
				childRenderer[0].material.color = new Color(1, originalPercentage, originalPercentage, 255);
			}
	
			if (glowingInsect2Player1 != -1) 
			{
				GameObject droneTemp = drones[glowingInsect2Player1]; 
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
				childRenderer[0].material.color = new Color(1, originalPercentage, originalPercentage, 255);
			}
	
			// Glow insects for player 2
			if (glowingInsect1Player2 != -1) 
			{
				GameObject droneTemp = drones[glowingInsect1Player2]; 
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
				childRenderer[0].material.color = new Color(originalPercentage, originalPercentage, 1, 255);
			}
			if (glowingInsect2Player2 != -1) 
			{
				GameObject droneTemp = drones[glowingInsect2Player2]; 
				childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
				childRenderer[0].material.color = new Color(originalPercentage, originalPercentage, 1, 255);
			}
	
			/*if (huntedInsect1 != null) 
			{
				Vector3 direction = (this.drones[insectToBeCreated1].transform.position - huntedInsect1.transform.position);
				float distance = direction.magnitude;
				Vector3 directionNormalized = direction/distance;
				huntedInsect1.transform.position += directionNormalized*4.0f;
	
				childRenderer = huntedInsect1.GetComponentsInChildren<SkinnedMeshRenderer>();
				if (gameObject.tag == "SwarmPlayer1")
				{
					childRenderer[0].material.color = new Color(1, originalPercentage, originalPercentage, 255);
				}
				if (gameObject.tag == "SwarmPlayer2")
				{
					childRenderer[0].material.color = new Color(originalPercentage, originalPercentage, 1, 255);
				}
	
				if(distance < 15.0f)
				{
					GameObject huntedParticles = (GameObject) GameObject.Instantiate(huntedParticlesPrefab);
					huntedParticles.transform.position = huntedInsect1.transform.position;
	
					GameObject droneTemp = drones[insectToBeCreated1]; 
					childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
					if (gameObject.tag == "SwarmPlayer1")
					{
						childRenderer[0].material.color = new Color(1, 0, 0, 1);
					}
					if (gameObject.tag == "SwarmPlayer2")
					{
						childRenderer[0].material.color = new Color(0, 0, 1, 1);
					}
					insectsCreated += 1;
	
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_created", gameObject.tag, transform));
					Debug.Log ("["+Time.time+"]  Insect to be created: " + insectToBeCreated1 + " created.");
	
					Destroy(huntedInsect1);
					huntedInsect1 = null;
					insectToBeCreated1 = -1;
	
				}
			}*/
			/*if (huntedInsect2 != null)
			{
				Vector3 direction = (this.drones[insectToBeCreated2].transform.position - huntedInsect2.transform.position);
				float distance = direction.magnitude;
				Vector3 directionNormalized = direction/distance;
				huntedInsect2.transform.position += directionNormalized*4.0f;
	
				childRenderer = huntedInsect2.GetComponentsInChildren<SkinnedMeshRenderer>();
				if (gameObject.tag == "SwarmPlayer1")
				{
					childRenderer[0].material.color = new Color(1, originalPercentage, originalPercentage, 255);
				}
				if (gameObject.tag == "SwarmPlayer2")
				{
					childRenderer[0].material.color = new Color(originalPercentage, originalPercentage, 1, 255);
				}
	
				if(distance < 15.0f)
				{
					GameObject huntedParticles = (GameObject) GameObject.Instantiate(huntedParticlesPrefab);
					huntedParticles.transform.position = huntedInsect2.transform.position;
	
					GameObject droneTemp = drones[insectToBeCreated2]; 
					childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
					if (gameObject.tag == "SwarmPlayer1")
					{
						childRenderer[0].material.color = new Color(1, 0, 0, 1);
					}
					if (gameObject.tag == "SwarmPlayer2")
					{
						childRenderer[0].material.color = new Color(0, 0, 1, 1);
					}
					insectsCreated += 1;
	
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_created", gameObject.tag, transform));
					Debug.Log ("["+Time.time+"]  Insect to be created: " + insectToBeCreated2 + " created.");
	
					Destroy(huntedInsect2);
					huntedInsect2 = null;
					insectToBeCreated2 = -1;
				}
			}*/			
		if (updatingList == true)
		{
			if (counter<0)
			{
				counter+= Time.deltaTime;
				for (int k=0 ; k<droneCount ; k++)
				{
					GameObject droneTemp = drones[k]; 
					
					droneTemp.GetComponent<DroneBehavior>().maxSteer += 0.01f;
					
					this.drones[k] = droneTemp;
				}
			}else
			{
				updatingList = false;
				counter = 0;

				for (int k=0 ; k<droneCount ; k++)
				{
					GameObject droneTemp = drones[k]; 

					droneTemp.GetComponent<DroneBehavior>().maxSteer = originalSteer;
	
					childRenderer = droneTemp.GetComponentsInChildren<SkinnedMeshRenderer>();
					int newalpha = 0;	
					childRenderer[0].material.color = new Color(childRenderer[0].material.color.r, childRenderer[0].material.color.g, childRenderer[0].material.color.b, newalpha);
					this.drones[k] = droneTemp;	
				}
				if 	(messageSender == "Main")
				{
					Messenger.SendToListeners(new Message(gameObject, "List_updated_creature", gameObject.name));
				}
				if 	(messageSender == "CreatureController")
				{
					Messenger.SendToListeners(new Message(gameObject, "List_updated_texture", gameObject.name));
				}
			}
		}
	}

	protected virtual void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position, new Vector3(swarmBounds.x, 0f, swarmBounds.y));
		Gizmos.DrawWireSphere(transform.position, spawnRadius);
	}
}
