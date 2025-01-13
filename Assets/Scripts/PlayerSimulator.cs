using UnityEngine;
using System.Collections;

public class PlayerSimulator: MessageBehaviour 
{
	public GameObject otherplayer;
	public float distbetweenplayers;

	public float speed; //velocitat de moviment
	
	private Vector3 screenPoint;
	
	private float dist;
	
	public Transform Ground2;
	public int Number = 2;

	public float CollisionRequired = 0.5f;
	private float CollisionCurrent = 0.0f;
	private int insects = 0;

	private Transform myTransform;
	public Transform swarm1;
	public Transform swarm2;
	public Transform swarm3;
	public Transform swarm4;
	
	private float timerswarm1;
	private float timerswarm2;
	private float timerswarm3;
	private float timerswarm4;

	public float timeToCatch = 3.5f;
	public float distanceToCatch = 0.4f;

	private int swarm1_count = 0; 
	private int swarm2_count = 0;
	private int swarm3_count = 0;
	private int swarm4_count = 0;

	private bool emptyswarm1 = false;
	private bool emptyswarm2 = false;
	private bool emptyswarm3 = false;
	private bool emptyswarm4 = false;

	private bool creature_user1 = false;	
	private bool creature_user2 = false;	

	private bool case2 = false;

	public float userHeight;

	public bool huntEnabled = true; //per evitar caçar insectes mentre es fan altres interaccions
	
	protected override void OnStart () 
	{
		Messenger.RegisterListener(new Listener("Creature_created", gameObject, "stop_catching"));

		Messenger.RegisterListener(new Listener("Creature_created", gameObject, "count_insects"));
		
		Messenger.RegisterListener(new Listener("Texture_changed", gameObject, "count_insects"));
	}

	
	void Awake ()
	{
		myTransform = transform;
	}
	
	public void count_insects(Message m)
	{
		Messenger.SendToListeners(new Insects_swarm(gameObject, "Insects_of_swarm", "", new Vector4(swarm1_count,swarm2_count,swarm3_count,swarm4_count)));
		swarm1_count = 0;
		swarm2_count = 0;
		swarm3_count = 0;
		swarm4_count = 0;

	}

	public void stop_catching(Message m)
	{
		if (m.MessageValue == "user1_swarm")
		{
			creature_user1 = true;	
		}else if (m.MessageValue == "user2_swarm")
		{
			creature_user2 = true;	
		}
	}
	
	void Update () 
	{
		
		myTransform.position = new Vector3(myTransform.position.x, userHeight, myTransform.position.z);


		/*
		 * hunt is only enabled when:
		 * ·creature1_ or creature2_ do not exist
		 * ·for player1: creature1 do not exist or exists and it's in one of following states: Initial,Idle,Long_idle,Walking,Running,Going_to_other_player,Pointing
		 * ·for player2: creature2 do not exist or exists and it's in one of following states: Initial,Idle,Long_idle,Walking,Running,Going_to_other_player,Pointing
		*/



		GameObject creature1 = GameObject.FindGameObjectWithTag("creature1");


		GameObject creature2 = GameObject.FindGameObjectWithTag("creature1");


		if ( GameObject.FindGameObjectWithTag("creature1_") != null ||
			 GameObject.FindGameObjectWithTag("creature2_") != null )
		{
			huntEnabled = false;

		}else if( gameObject.tag == "Player" &&  (creature1 == null ||
			creature1.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Initial ||
			creature1.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Idle ||
			creature1.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Long_idle ||
			creature1.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Walking ||
			creature1.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Running ||
			creature1.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Going_to_other_player ||
			creature1.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Pointing))
		{
			huntEnabled = true;

		}else if( gameObject.tag == "Player2" &&  (creature2== null ||
			creature2.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Initial ||
			creature2.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Idle ||
			creature2.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Long_idle ||
			creature2.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Walking ||
			creature2.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Running ||
			creature2.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Going_to_other_player ||
			creature2.GetComponent<CreatureController>().myCharState == CreatureController.CharState.Pointing))
		{
			huntEnabled = true;
		}else
		{
			huntEnabled = false;
		}


		if(huntEnabled)
			CheckCloseToSwarmsToHunt();

		if (gameObject.tag == "Player" && (	GameObject.FindGameObjectWithTag("creature2") != null))
		{
			creature2 = GameObject.FindGameObjectWithTag("creature2");

			if(Vector3.Distance(myTransform.position, creature2.transform.position) < distbetweenplayers)
			{
				Messenger.SendToListeners(new Message(gameObject, "playersarenear",""));	
			}
		}
		if (gameObject.tag == "Player2" && (GameObject.FindGameObjectWithTag("creature1") != null))
		{
			creature1 = GameObject.FindGameObjectWithTag("creature1");
			
			if(Vector3.Distance(myTransform.position, creature1.transform.position) < distbetweenplayers)
			{
				Messenger.SendToListeners(new Message(gameObject, "playersarenear",""));	
			}
		}
	}

	private void CheckCloseToSwarmsToHunt()
	{
		if (gameObject.name == "Player1")
		{
			GameObject userSwarm1 = (GameObject) GameObject.FindGameObjectWithTag("SwarmPlayer1");
			SwarmBehavior userSwarm1Behavior = userSwarm1.GetComponent<SwarmBehavior>();
			if(userSwarm1Behavior.updatingList)
			{
				return;
			}

			GameObject creature1 = (GameObject) GameObject.FindGameObjectWithTag("creature1");
			if(creature1 != null)
			{
				CreatureController creature1Controller = creature1.GetComponent<CreatureController>();
				if(creature1Controller.myCharState == CreatureController.CharState.Celebrating ||
				   creature1Controller.myCharState == CreatureController.CharState.Going_to_merge ||
				   creature1Controller.myCharState == CreatureController.CharState.Going_to_manipulate ||
				   creature1Controller.myCharState == CreatureController.CharState.Manipulating ||
				   creature1Controller.myCharState == CreatureController.CharState.Merging ||
				   creature1Controller.myCharState == CreatureController.CharState.Celebrating ||
				   creature1Controller.myCharState == CreatureController.CharState.Waiting ||
				   creature1Controller.myCharState == CreatureController.CharState.Pointing)
				{
					return;
				}
			}
		}

		if (gameObject.name == "Player2")
		{
			GameObject userSwarm2 = (GameObject) GameObject.FindGameObjectWithTag("SwarmPlayer2");
			SwarmBehavior userSwarm2Behavior = userSwarm2.GetComponent<SwarmBehavior>();
			if(userSwarm2Behavior.updatingList)
			{
				return;
			}

			GameObject creature2 = (GameObject) GameObject.FindGameObjectWithTag("creature2");
			if(creature2 != null)
			{
				CreatureController creature2Controller = creature2.GetComponent<CreatureController>();
				if(creature2Controller.myCharState == CreatureController.CharState.Celebrating ||
				   creature2Controller.myCharState == CreatureController.CharState.Going_to_merge ||
				   creature2Controller.myCharState == CreatureController.CharState.Going_to_manipulate ||
				   creature2Controller.myCharState == CreatureController.CharState.Manipulating ||
				   creature2Controller.myCharState == CreatureController.CharState.Merging ||
				   creature2Controller.myCharState == CreatureController.CharState.Celebrating ||
				   creature2Controller.myCharState == CreatureController.CharState.Waiting ||
				   creature2Controller.myCharState == CreatureController.CharState.Pointing)
				{
					return;
				}
			}
		}

		if ((swarm1.position - myTransform.position).sqrMagnitude < distanceToCatch*distanceToCatch && emptyswarm1 == false)
		{
			if(timerswarm1 == 0)
			{
				// Just got close to swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player1", "free_swarm1", swarm1));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player2", "free_swarm1", swarm1));
				}
			}

			if (timerswarm1 > timeToCatch)
			{
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player1","free_swarm1",swarm1));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player2","free_swarm1",swarm1));
				}
				timerswarm1 = -1;
				swarm1_count += 2; 	
			}
			
			timerswarm1 += Time.deltaTime;
		}
		else
		{
			if(timerswarm1 != 0)
			{
				// Just got away from the swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player1", "free_swarm1", swarm1));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player2", "free_swarm1", swarm1));
				}
			}
			
			timerswarm1 = 0;
		}
		
		if ((swarm2.position - myTransform.position).sqrMagnitude < distanceToCatch*distanceToCatch && emptyswarm2 == false)
		{
			if(timerswarm2 == 0)
			{
				// Just got close to swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player1", "free_swarm2", swarm2));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player2", "free_swarm2", swarm2));
				}
			}

			if (timerswarm2 > timeToCatch)
			{
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player1","free_swarm2",swarm2));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player2","free_swarm2",swarm2));
				}
				timerswarm2 = -1;	
				swarm2_count += 2; 	
			}
			
			timerswarm2 += Time.deltaTime;
		}
		else 
		{
			if(timerswarm2 != 0)
			{
				// Just got away from the swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player1", "free_swarm2", swarm2));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player2", "free_swarm2", swarm2));
				}
			}
			
			timerswarm2 = 0;
		}	
		
		if ((swarm3.position - myTransform.position).sqrMagnitude < distanceToCatch*distanceToCatch && emptyswarm3 == false)
		{
			if(timerswarm3 == 0)
			{
				// Just got close to swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player1", "free_swarm3", swarm3));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player2", "free_swarm3", swarm3));
				}
			}

			if (timerswarm3 > timeToCatch)
			{
				if (gameObject.tag == "Player")// && creature_user1 == false)
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player1","free_swarm3",swarm3));
				}
				if (gameObject.tag == "Player2")// && creature_user2 == false)
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player2","free_swarm3",swarm3));
				}
				timerswarm3 = 0;	
				swarm3_count += 2; 	
			}
			
			timerswarm3 += Time.deltaTime;
		}
		else
		{
			if(timerswarm3 != 0)
			{
				// Just got away from the swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player1", "free_swarm3", swarm3));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player2", "free_swarm3", swarm3));
				}
			}
			
			timerswarm3 = 0;
		}	
		
		if ((swarm4.position - myTransform.position).sqrMagnitude < distanceToCatch*distanceToCatch && emptyswarm4 == false)
		{
			if(timerswarm4 == 0)
			{
				// Just got close to swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player1", "free_swarm4", swarm4));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Close_to_swarm_player2", "free_swarm4", swarm4));
				}
			}

			if (timerswarm4 > timeToCatch)
			{
				if (gameObject.tag == "Player")// && creature_user1 == false)
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player1","free_swarm4",swarm4));
				}
				if (gameObject.tag == "Player2")// && creature_user2 == false)
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Insect_catched_player2","free_swarm4",swarm4));
				}
				timerswarm4 = 0;	
				swarm4_count += 2; 	
			}
			
			timerswarm4 += Time.deltaTime;
		}
		else
		{
			if(timerswarm4 != 0)
			{
				// Just got away from the swarm
				if (gameObject.tag == "Player")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player1", "free_swarm4", swarm4));
				}
				if (gameObject.tag == "Player2")
				{
					Messenger.SendToListeners(new Message_transform(gameObject, "Away_from_swarm_player2", "free_swarm4", swarm4));
				}
			}
			
			timerswarm4 = 0;
		}
	}

	private void updatePlayerPosition()
	{
		//if(Input.GetKey(KeyCode.M))
		//{
			if (gameObject.tag == "Player2")
			{
				if (Input.GetKey(KeyCode.A))
				{	
					transform.position = new Vector3 (transform.position.x - 1 * Time.deltaTime, transform.position.y, transform.position.z);
				}
				if (Input.GetKey(KeyCode.D))
				{
					transform.position = new Vector3 (transform.position.x + 1 * Time.deltaTime, transform.position.y, transform.position.z);
				}
				if (Input.GetKey(KeyCode.S))
				{
					transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - 1 * Time.deltaTime);
				}
				if (Input.GetKey(KeyCode.W))
				{
					transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 1 * Time.deltaTime);
				}
			}
			
		/*}
		else
		{*/
			if (gameObject.tag == "Player")
			{
			if (Input.GetKey(KeyCode.LeftArrow))
				{	
					transform.position = new Vector3 (transform.position.x - 1 * Time.deltaTime, transform.position.y, transform.position.z);
				}
			if (Input.GetKey(KeyCode.RightArrow))
				{
					transform.position = new Vector3 (transform.position.x + 1 * Time.deltaTime, transform.position.y, transform.position.z);
				}
			if (Input.GetKey(KeyCode.DownArrow))
				{
					transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z - 1 * Time.deltaTime);
				}
			if (Input.GetKey(KeyCode.UpArrow))
				{
					transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 1 * Time.deltaTime);
				}
			}
		//}
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, distanceToCatch);
	}

}