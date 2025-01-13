using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Main : MessageBehaviour {

	private int insects_player1 = 0;
	private int insects_player2 = 0;

	private bool player1creature = false;
	private bool player2creature = false;
	
	private int insects_swarm1;
	private int insects_swarm2;
	private int insects_swarm3;
	private int insects_swarm4;

	public int DroneHuntedForCreature;
	
	//---------------

	public GameObject player1;
	public GameObject player2;
	
	public GameObject zoneWater;
	public GameObject zoneIce;
	public GameObject zoneLava;

	//private AudioSource audio;

	//private AudioSource[] audios;

	private Vector3 Creature1MergePos;
	
	private Vector3 Creature2MergePos;	

	private List<string> names = new List<string>();
	
	private List<Texture> current_textures= new List<Texture>();
	
	private List<string> anim_names= new List<string>();
	
	private SkinnedMeshRenderer[] childRenderer;
	
	private float creature_scale;
	private int counter_scale;
	
	private GameObject new_creature;
	
	private float coord0;
	private float coord1;
	private float coord2;

	public float creaturesHeight;

	private int zone;

	private int count2;

	private bool firstIdle1 = false;
	private bool firstIdle2 = false;
	
	public GameObject explosion;

	private string prevCreature1;
	private string prevCreature2;

	private string currentCreature1;
	private string currentCreature2;	
	
	protected override void OnStart () 	
	{

		Random.seed = System.DateTime.Now.Second;

		//audios = GetComponents<AudioSource>();

		//audio = audios [0];

		Messenger.RegisterListener(new Listener("Insect_created", gameObject, "update_insect_lists"));

		Messenger.RegisterListener(new Listener("List_updated_creature", gameObject, "createCreature"));

		Messenger.RegisterListener(new Listener("playersarenear", gameObject, "creaturesinteraction"));
		
		Messenger.RegisterListener(new Listener("creaturesarenear", gameObject, "creaturesmerge"));

		Messenger.RegisterListener(new Listener("MergeNewCreature1", gameObject, "creature1_after_merge"));
		
		Messenger.RegisterListener(new Listener("MergeNewCreature2", gameObject, "creature2_after_merge"));

		Messenger.RegisterListener(new Listener("first_idle_1", gameObject, "FirstIdle1"));
		Messenger.RegisterListener(new Listener("first_idle_2", gameObject, "FirstIdle2"));
		
		//Cursor.visible = false;

		loadnamelists();

		if (Camera.allCameras.Length > 1) 
		{
			Camera camera0Obj = Camera.allCameras [0];
			Camera camera1Obj = Camera.allCameras [1];
			
			camera0Obj.cullingMask &= ~(1 << LayerMask.NameToLayer ("Blending1"));

			camera1Obj.cullingMask &= ~(1 << LayerMask.NameToLayer ("Blending0"));
		}
	}

	void FirstIdle1()
	{
		firstIdle1 = true;
	}
	void FirstIdle2()
	{
		firstIdle2 = true;
	}


	void creature1_after_merge()
	{
		
		int rnd = Random.Range(0,13);
		while (names[rnd] == prevCreature1 || names[rnd] == prevCreature2 || names[rnd] == currentCreature2)
		{
			rnd = Random.Range(0,13);
		}

		loadcreature(names[rnd], player1, player2, Creature1MergePos);
		insects_player1 = 0;
		Messenger.SendToListeners(new Message(gameObject, "Creature_created","user1_swarm"));
		player1creature = true;
		currentCreature1 = GameObject.FindGameObjectWithTag ("creature1").name.Replace ("(Clone)", "");
	
	}

	void creature2_after_merge()
	{
		int rnd = Random.Range(0,13);
		while (names[rnd] == prevCreature1 || names[rnd] == prevCreature2 || names[rnd] == currentCreature1) 
		{
			rnd = Random.Range(0,13);
		}

		loadcreature(names[rnd], player2, player1, Creature2MergePos);
		insects_player2 = 0;
		Messenger.SendToListeners(new Message(gameObject, "Creature_created","user2_swarm"));
		player2creature = true;
		currentCreature2 = GameObject.FindGameObjectWithTag ("creature2").name.Replace ("(Clone)", "");
	}
		

	/// <summary>
	/// Method called by each creature when they have already prepared each other by placing themselves
	/// at a certain distance from each other
	/// </summary>
	void creaturesmerge()
	{
		count2+=1;
		if (count2 == 2)
		{
			GameObject creature1 = GameObject.FindGameObjectWithTag("creature1");
			GameObject creature2 = GameObject.FindGameObjectWithTag("creature2");
			Creature1MergePos = creature1.transform.position;
			Creature2MergePos = creature2.transform.position;
			loadcreature(GameObject.FindGameObjectWithTag("creature1").name.Replace("(Clone)",""), player1, player2,creature1.transform.position,"creature1_",creature1.GetComponent<CreatureController>().textureCount);
			loadcreature(GameObject.FindGameObjectWithTag("creature2").name.Replace("(Clone)",""), player2, player1,creature2.transform.position,"creature2_",creature2.GetComponent<CreatureController>().textureCount);
			count2 = 0;

			Debug.Log("[Main]: We moved to initial positions for merge animation.");

			Messenger.SendToListeners(new Message(gameObject, "Creature_merge", ""));
		}
	}	
	
	void creaturesinteraction()
	{
		
		GameObject creature1 = GameObject.FindGameObjectWithTag("creature1");
		GameObject creature2 = GameObject.FindGameObjectWithTag("creature2");
		
			if (player1creature == false || player2creature == false)
			{
					if (player1creature == true && creature1.GetComponent<CreatureController>().listening == true)
					{
						loadcreature(GameObject.FindGameObjectWithTag("creature1").name.Replace("(Clone)",""), player2, player1,new Vector3(player1.transform.position.x,creaturesHeight,player1.transform.position.z),
									"creature2",creature1.GetComponent<CreatureController>().textureCount);
						insects_player2 = 0;
						Messenger.SendToListeners(new Message(gameObject, "Creature_created", "user2_swarm"));
						player2creature = true;

						Messenger.SendToListeners(new Message(gameObject, "Creature_shared", "player1"));
						
						GameObject.FindGameObjectWithTag("creature2").GetComponent<CreatureController>().shared = true;
					
					}else if (player2creature == true && creature2.GetComponent<CreatureController>().listening == true)
					{
						loadcreature(GameObject.FindGameObjectWithTag("creature2").name.Replace("(Clone)",""), player1, player2, new Vector3(player2.transform.position.x,creaturesHeight,player2.transform.position.z),
									"creature1",creature2.GetComponent<CreatureController>().textureCount);
						insects_player1 = 0;
						Messenger.SendToListeners(new Message(gameObject, "Creature_created", "user1_swarm"));
						player1creature = true;

						Messenger.SendToListeners(new Message(gameObject, "Creature_shared", "player2"));

						GameObject.FindGameObjectWithTag("creature1").GetComponent<CreatureController>().shared = true;
					}
			}
	}

	void loadnamelists()
	{
			anim_names.Add("appear");
			anim_names.Add("idle");
			anim_names.Add("long_idle");
			anim_names.Add("look_up");
			anim_names.Add("walk");
			anim_names.Add("run");
			anim_names.Add("celebrate1");
			anim_names.Add("celebrate2");
			anim_names.Add("point_at");
			anim_names.Add("brief_greeting");
			anim_names.Add("effusive_greeting");
			anim_names.Add("manipulate");
			anim_names.Add("merge");
			
			names.Add("Sea_dragon");
			names.Add("Crab_man");
			names.Add("Coral_girl");
			names.Add("Octopus_man");
			names.Add("Water_man");
			names.Add("Yeti");
			names.Add("Golem");
			names.Add("Lava_man");
			names.Add("Dwarf");
			names.Add("Biped_dragon");
			names.Add("Tree_man");
			names.Add("Quad_dragon");
			names.Add("Faunus");
			names.Add("Fairy_boy");			
	}

	public void update_insect_lists (Message_transform m)
	{
		if ( m.MessageSource.name == "user1_swarm" )
		{
			insects_player1 += 1;
		}
		if ( insects_player1 == DroneHuntedForCreature && player1creature==false)
		{
			Messenger.SendToListeners(new Message(gameObject, "Creature_created", "user1_swarm"));
		}
	
		if ( m.MessageSource.name == "user2_swarm" )
		{
			insects_player2 += 1;
		}
		if ( insects_player2 == DroneHuntedForCreature && player2creature==false)
		{
			Messenger.SendToListeners(new Message(gameObject, "Creature_created","user2_swarm"));	
		}
	}

	public void createCreature(Message m)
	{
		if ( m.MessageValue == "user1_swarm" && player1creature==false )
		{
			string creatureName;
			
			zone = zoneCatched(m.MessageSource.transform);
			
			if (zone == 1)
			{
				creatureName = names[Random.Range(0,3)];
				loadcreature(creatureName, player1, player2);
			}else if (zone == 2)
			{
				creatureName = names[Random.Range(3,6)];
				loadcreature(creatureName, player1, player2);
			}else if (zone == 3)
			{
				creatureName = names[Random.Range(6,10)];
				loadcreature(creatureName, player1, player2);				
			}else 
			{
				creatureName = names[Random.Range(10,14)];
				loadcreature(creatureName, player1, player2);				
			}
			
			insects_player1 = 0;
			
			player1creature = true;
			
			Debug.Log("["+Time.time+"] Player1 has CREATURE!.");
		}
		if ( m.MessageValue == "user2_swarm" && player2creature==false )
		{
			string creatureName;
			
			zone = zoneCatched(m.MessageSource.transform);
			
			if (zone == 1)
			{
				creatureName = names[Random.Range(0,3)];
				loadcreature(creatureName, player2, player1);
			}else if (zone == 2)
			{
				creatureName = names[Random.Range(3,6)];
				loadcreature(creatureName, player2, player1);
			}else if (zone == 3)
			{
				creatureName = names[Random.Range(6,10)];
				loadcreature(creatureName, player2, player1);				
			}else 
			{
				creatureName = names[Random.Range(10,14)];
				loadcreature(creatureName, player2, player1);				
			}
			
			insects_player2 = 0;
			
			
			player2creature = true;
			
			Debug.Log("["+Time.time+"] Player2 has CREATURE!.");
		}
	}
		
	
	int zoneCatched ( Transform swarm)
	{
		
		if(zoneWater.GetComponent<Collider>().bounds.Contains(swarm.position) == true)
		{
			return 1;

		}else if(zoneIce.GetComponent<Collider>().bounds.Contains(swarm.position) == true)
		{
			return 2;

		}else if(zoneLava.GetComponent<Collider>().bounds.Contains(swarm.position) == true)
		{
			return 3;

		}else
		{
			return 4;		
		}	

	}	

	public void loadcreature ( string creature, GameObject myplayer, GameObject otherplayer, Vector3 initPos = default(Vector3), string Tag = default(string), int textureCount = default(int))
	{



		string path = "Creatures/" + creature + "/" + creature; //+ ".FBX";
		GameObject inputCreature = (GameObject)Resources.Load(path, typeof(GameObject));		


		string[] filelines = File.ReadAllLines("assets/Resources/Creatures/" + creature + "/Scale.txt");
		for(int i = 0; i < filelines.Length; i++)
		{
			string[] coord = filelines[i].Split(";"[0]);
			coord0 = float.Parse(coord[0]) * 0.01f; //changed by /100 already
			coord1 = float.Parse(coord[1]) * 0.01f; //changed by /100 already
			coord2 = float.Parse(coord[2]) * 0.01f; //changed by /100 already
		}
		
		inputCreature.transform.localScale = new Vector3(coord0,coord1,coord2);	
		
		
		new_creature = (GameObject) GameObject.Instantiate(inputCreature);

		new_creature.AddComponent<Footsteps>();

		new_creature.AddComponent<CreatureController>();	
		
		//new_creature.layer = 8;
		
		CreatureController cc = new_creature.GetComponent<CreatureController>();
		
		cc.target = myplayer.transform ;
		
		cc.target2 = otherplayer.transform;

		cc.creaturesHeight = creaturesHeight;

		if (textureCount == 1) 
		{
			cc.changeMesh = true;	
			cc.textureCount = 1;
		}

		if (initPos == new Vector3(0,0,0))
		{
			new_creature.transform.position = new Vector3(myplayer.transform.position.x, creaturesHeight, myplayer.transform.position.z);
		}else
		{
			new_creature.transform.position = initPos;
		}
		
		new_creature.transform.rotation = Quaternion.LookRotation ( myplayer.transform.position - new_creature.transform.position);
		
		childRenderer = new_creature.GetComponentsInChildren<SkinnedMeshRenderer>();
		
		childRenderer[0].material.color = new Color (1,1,1);
		
		string path_texture1 = "Creatures/" + creature + "/" + creature + "_1";// + ".png";
		string path_texture2 = "Creatures/" + creature + "/" + creature + "_2";// + ".png";
		string path_texture3 = "Creatures/" + creature + "/" + creature + "_3";// + ".png";
		string path_texture4 = "Creatures/" + creature + "/" + creature + "_4";// + ".png";
		
		current_textures.Clear();
		
		current_textures.Add( Resources.Load<Texture>(path_texture1));
		current_textures.Add( Resources.Load<Texture>(path_texture2));		
		current_textures.Add( Resources.Load<Texture>(path_texture3));		
		current_textures.Add( Resources.Load<Texture>(path_texture4));		
		
		if( textureCount == default(int))
		{
			childRenderer[0].material.mainTexture = current_textures[0];
			
		}else
		{
			childRenderer[0].material.mainTexture = current_textures[textureCount];
		}
		
		string path_controller = "Creatures/" + creature + "/" + creature;// + ".controller";
		
		Animator anim1 = new_creature.GetComponent<Animator>();
		
		anim1.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>(path_controller);

		new_creature.AddComponent<AudioSource>();	
		new_creature.AddComponent<AudioSource>();	
		
		if(Tag == default(string))
		{
			if (myplayer.name == "Player1")
			{
				new_creature.tag = "creature1";
			}else
			{
				new_creature.tag = "creature2";
			}
		}else
		{
			new_creature.tag = Tag;
		}



		creature_scale = new_creature.transform.localScale.x * 0.05f;
		counter_scale = 100;
		
	}

	void Update ()
	{

		if (Input.GetKeyDown(KeyCode.F1))
		{
			if (player1creature==false)
			{
				string creatureName;
				
				zone = zoneCatched(player1.transform);
				
				if (zone == 1)
				{
					creatureName = names[Random.Range(0,3)];
					loadcreature(creatureName, player1, player2);
				}else if (zone == 2)
				{
					creatureName = names[Random.Range(3,6)];
					loadcreature(creatureName, player1, player2);
				}else if (zone == 3)
				{
					creatureName = names[Random.Range(6,10)];
					loadcreature(creatureName, player1, player2);				
				}else 
				{
					creatureName = names[Random.Range(10,14)];
					loadcreature(creatureName, player1, player2);				
				}
				
				insects_player1 = 0;
				
				Messenger.SendToListeners(new Message(gameObject, "Creature_created","user1_swarm"));
				
				player1creature = true;	
			}
			Messenger.SendToListeners (new Message (gameObject, "Key_pressed", "F1"));
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			if (player2creature==false)
			{
				string creatureName;
				
				zone = zoneCatched(player2.transform);
				
				if (zone == 1)
				{
					creatureName = names[Random.Range(0,3)];
					loadcreature(creatureName, player2, player1);
				}else if (zone == 2)
				{
					creatureName = names[Random.Range(3,6)];
					loadcreature(creatureName, player2, player1);
				}else if (zone == 3)
				{
					creatureName = names[Random.Range(6,10)];
					loadcreature(creatureName, player2, player1);				
				}else 
				{
					creatureName = names[Random.Range(10,14)];
					loadcreature(creatureName, player2, player1);				
				}
				
				insects_player2 = 0;

				Messenger.SendToListeners(new Message(gameObject, "Creature_created","user2_swarm"));
				
				player2creature = true;	
			}
			Messenger.SendToListeners (new Message (gameObject, "Key_pressed", "F2"));
		}
		
		if (Input.GetKeyDown(KeyCode.F3))
		{
			Messenger.SendToListeners (new Message (gameObject, "Texture_changed", "user1_swarm"));
			Messenger.SendToListeners (new Message (gameObject, "Key_pressed", "F3"));
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			Messenger.SendToListeners (new Message (gameObject, "Texture_changed", "user2_swarm"));
			Messenger.SendToListeners (new Message (gameObject, "Key_pressed", "F4"));
		}

		if (Input.GetKeyDown(KeyCode.B))
		{
			Messenger.SendToListeners (new Message (gameObject, "Key_pressed", "B"));
			//string path_clip = "Sounds/Beep";
			//audio.clip = Resources.Load<AudioClip> (path_clip);
			//audio.Play ();
		}
//		if (Input.GetKeyDown(KeyCode.UpArrow))
//		{
//			new_creature.transform.localScale = new Vector3(new_creature.transform.localScale.x + creature_scale, new_creature.transform.localScale.y + creature_scale, new_creature.transform.localScale.z + creature_scale);
//			counter_scale += 5;
//		}
//		if (Input.GetKeyDown(KeyCode.DownArrow))
//		{
//			new_creature.transform.localScale = new Vector3(new_creature.transform.localScale.x - creature_scale, new_creature.transform.localScale.y - creature_scale, new_creature.transform.localScale.z - creature_scale);
//			counter_scale -= 5;
//		}
		if (GameObject.FindGameObjectWithTag ("creature1") != null) 
		{
			prevCreature1 = GameObject.FindGameObjectWithTag ("creature1").name.Replace ("(Clone)", "");
		}
		if (GameObject.FindGameObjectWithTag ("creature2") != null) 
		{
			prevCreature2 = GameObject.FindGameObjectWithTag ("creature2").name.Replace ("(Clone)", "");
		}
	}

	void OnGUI() 
	{
		//GUI.Label(new Rect(10, 30, 200, 20), "creature scale : " + counter_scale.ToString() + "%");
		//GUI.Label(new Rect(10, 50, 200, 20), "animation : " + anim_names[j-1]);
		
	}
}
