using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CreatureController : MessageBehaviour
{ 
	
//-----
	//changed by /100 already
	public float walkDistance=0.35f; 			//distància a partir de la qual la criatura comença a caminar
	public float runDistance=0.55f;			//distància a partir de la qual la criatura comença a correr
	public float greetingDistance = 1.5f;	//distància a la que les criatures es saluden
	public int insectsTextureChange = 6;	//quantitat d'insectes a caçar per canviar de textura
	public float mergeDistance = 1.0f;		//distància a partir de la qual les criatures es fusionen
	public int mergeTime = 25;				//temps que ha de passar perquè les criatures es fusionin
	public int pasivetime1 = 10;
	public int pasivetime2 = 20;
	public int pasivetime3 = 30;


//-----

	public bool listening;
	public bool hasKeyPart;

	public float stopDistance=0.1f; //changed by /100 already
			
	public Transform target;
	public Transform target2;
	private Vector3 targetFloor;
	private Vector3 targetFloor2;
	public int rotationSpeed=15;
	public float walkSpeed=0.3f; //changed by /100 already
	public float runSpeed=0.9f; //changed by /100 already
	public float timer;
	private float timerToGreetAgain = 0;
	private float timerLookingUp;
	private float timerLongIdle;
	
	private float creaturesHight = 0.2f; //changed by /100 already

	private int inactivity;

	private Transform myTransform;
	public CharState myCharState;
	private int cond = 0;

	int idleStateHash = Animator.StringToHash("Base Layer.idle");
	int longidleStateHash = Animator.StringToHash("Base Layer.long_idle");

	private AudioSource[] audios;

	private AudioSource audio;

	private Animator animator;

	private AnimatorStateInfo previousstate;

	public float creaturesHeight;

	private SkinnedMeshRenderer[] childRenderer;

	private List <Texture> current_textures= new List<Texture>();
	
	private int insectCount;

	public int textureCount;

	private Listener Insect_catched_player1;

	private Listener Insect_catched_player2;

	private Listener Merge1;
	
	private Listener Merge2;

	private Listener List_updated;	

	private Listener manipulateProp;
	private Listener pointingProp;
	private Listener celebrateProp;

	private bool greetingbool = true;

	private Vector3 prop_position;
	private string prop_tag;

	private Vector3 pointing_prop_position;

	private GameObject otherCreature;

	private float prevDistance;

	public float mergeTimer;
	private bool case2;

	private Vector3 otherPosition;

 	private int firstIdle;

	public bool shared = false;

	public bool changeMesh = false;


	public enum CharState
	{
		Initial,
		Idle,
		Long_idle,
		Walking,
		Running,
		Going_to_other_player,
		Pointing,
		Going_to_manipulate,
		Manipulating,
		Waiting,
		Celebrating,
		Brief_greeting,
		Effusive_greeting,
		Looking_up,
		Going_to_merge,
		Merging,
		Destroy,
	};

	void Awake ()
	{
		myTransform = transform;
		
	}
	
	// inicialitzacio
	protected override void OnStart () 
	{    
		
		timer = 0; 
	
		animator = GetComponent<Animator>();
	
		myCharState = CharState.Initial;

		audios = GetComponents<AudioSource>();

		audio = audios [0];
		audio.playOnAwake = false;
		audio.minDistance = 6; //changed by /100 already

		
		childRenderer = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		childRenderer[0].material.color = new Color (1,1,1);

		string path_texture1 = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + gameObject.name.Replace("(Clone)","") + "_1";
		string path_texture2 = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + gameObject.name.Replace("(Clone)","") + "_2";
		string path_texture3 = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + gameObject.name.Replace("(Clone)","") + "_3";
		string path_texture4 = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + gameObject.name.Replace("(Clone)","") + "_4";
		
		current_textures.Clear();
		
		current_textures.Add( Resources.Load<Texture>(path_texture1));
		current_textures.Add( Resources.Load<Texture>(path_texture2));		
		current_textures.Add( Resources.Load<Texture>(path_texture3));		
		current_textures.Add( Resources.Load<Texture>(path_texture4));

		if (gameObject.tag == "creature1")
		{

			Insect_catched_player1 = new Listener("Insect_created", gameObject, "insect_catched");
			Messenger.RegisterListener(Insect_catched_player1);

			Merge1 = new Listener("Merge1", gameObject, "mergeDestroy");
			Messenger.RegisterListener(Merge1);
	
		}
		
		if (gameObject.tag == "creature2")
		{
			Insect_catched_player2 = new Listener("Insect_created", gameObject, "insect_catched");
			Messenger.RegisterListener(Insect_catched_player2);

			Merge2 = new Listener("Merge2", gameObject, "mergeDestroy");
			Messenger.RegisterListener(Merge2);		
		}

		List_updated = new Listener("List_updated_texture", gameObject, "ChangeTexture");
		Messenger.RegisterListener(List_updated);

		manipulateProp = new Listener("manipulate_prop", gameObject, "manipulate");
		Messenger.RegisterListener(manipulateProp);	

		pointingProp = new Listener("pointing_prop", gameObject, "PointingProp");
		Messenger.RegisterListener(pointingProp);	
		
		celebrateProp = new Listener("prop_eliminated", gameObject, "CelebrateProp");
		Messenger.RegisterListener(celebrateProp);	

		if (gameObject.tag == "creature1_" || gameObject.tag == "creature2_") {
			animator.SetBool ("clone", true);
			myCharState = CharState.Merging;

			string path = "Effects/CreatureDuplicated"; 
			GameObject Particles = (GameObject)Resources.Load (path, typeof(GameObject));	
			Particles.transform.position = new Vector3 (transform.position.x, 0.6f, transform.position.z); //changed by /100 already
			GameObject NewCreatureAnimation = (GameObject)GameObject.Instantiate (Particles);

            Debug.LogWarning(Particles.transform);

            string path_clip = "Sounds/CriaturaDesdobla";
			audio.clip = Resources.Load<AudioClip> (path_clip);
			audio.Play ();

		} else if (shared == false) {
			animator.SetBool ("clone", false);

			string path = "Effects/CreatureAppears"; 
			GameObject Particles = (GameObject)Resources.Load (path, typeof(GameObject));	
			Particles.transform.position = new Vector3 (transform.position.x, 0.6f, transform.position.z); //changed by /100 already
			Particles.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			GameObject NewCreatureAnimation = (GameObject)GameObject.Instantiate (Particles);

			string path_clip = "Sounds/CriaturaFusiona";
			audio.clip = Resources.Load<AudioClip> (path_clip);
			audio.Play ();
		} else 
		{
			string path = "Effects/CreatureDuplicated"; 
			GameObject Particles = (GameObject)Resources.Load (path, typeof(GameObject));	
			Particles.transform.position = new Vector3 (transform.position.x, 0.6f, transform.position.z); //changed by /100 already
			GameObject NewCreatureAnimation = (GameObject)GameObject.Instantiate (Particles);
			
			string path_clip = "Sounds/CriaturaDesdobla";
			audio.clip = Resources.Load<AudioClip> (path_clip);
			audio.Play ();

			animator.SetBool ("clone", true);
			shared = false;
			myCharState = CharState.Walking;
		}


		mergeTimer = 0;

		hasKeyPart = false;


		
	}
	
	// actualitzacio per cada frame
	void Update ()
	{
		
		if (gameObject.tag == "creature1" || gameObject.tag == "creature1_"  )
		{
			if (GameObject.FindGameObjectWithTag("creature2") != null)
			{
				otherCreature = GameObject.FindGameObjectWithTag("creature2");
			}		
		}
		
		if (gameObject.tag == "creature2" || gameObject.tag == "creature2_" )
		{
			if (GameObject.FindGameObjectWithTag("creature1") != null)
			{
				otherCreature = GameObject.FindGameObjectWithTag("creature1");
			}
		}


		targetFloor = new Vector3 ( target.position.x, creaturesHeight, target.position.z);
		targetFloor2 = new Vector3 ( target2.position.x, creaturesHeight, target2.position.z);

		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		
		/*if (Input.GetKeyDown(KeyCode.Z))
		{
			myCharState = CharState.Pointing;
			timer=0;
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			myCharState = CharState.Manipulating;
			timer=0;
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			myCharState = CharState.Effusive_greeting;
			timer=0;
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			myCharState = CharState.Brief_greeting;
			timer=0;
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			myCharState = CharState.Looking_up;
			timer=0;
		}*/

		if (otherCreature != null)
		{
			mergeTimer += Time.deltaTime;

			if (listening == true)
			{
				if (Vector3.Distance (otherCreature.transform.position, myTransform.position) > greetingDistance && Vector3.Distance (otherCreature.transform.position, myTransform.position) < greetingDistance+0.1f)
				{
					if (greetingbool == true)
					{
						if (prevDistance > Vector3.Distance (otherCreature.transform.position, myTransform.position))
						{
							myCharState = CharState.Brief_greeting;
							timer=0;
							timerToGreetAgain=0;
						}	    
					}
				}

				timerToGreetAgain += Time.deltaTime;

				if (timerToGreetAgain > 10)
				{
					greetingbool = true;
				}	

				prevDistance = Vector3.Distance (otherCreature.transform.position, myTransform.position);

				if(mergeTimer > mergeTime && otherCreature.GetComponent<CreatureController>().listening == true)
				{
					//Debug.Log("[CreatureController]: Enough time has passed to merge again and we are ready.");

					if(Vector3.Distance(myTransform.position, otherCreature.transform.position) < mergeDistance)
					{
						bool canWeMerge = true;

						// Check here distance with other props for both creatures
						foreach(PropController prop in (PropController[]) FindObjectsOfType<PropController>())
						{
							if(Vector3.Distance(myTransform.position, prop.transform.position) < 0.5f ||
								Vector3.Distance(otherCreature.transform.position, prop.transform.position) < 0.5f)
							{
								Debug.Log("[CreatureController]: We want to merge but we are close to a prop.");
								canWeMerge = false;
							}
						}

						//solució provisional per no fusionar-se un cop tenen la clau
						if (GameObject.Find("Llave_mango(Clone)") != null && GameObject.Find("Llave_cuerpo(Clone)") != null)
						{
							canWeMerge = false;
						}



						if(canWeMerge)
						{
							Debug.Log("[CreatureController]: We are close enough for merging and we are going to merge.");

							mergeTimer = 0;
							myCharState = CharState.Going_to_merge;
							timer = 0;
							otherCreature.GetComponent<CreatureController>().myCharState = CharState.Going_to_merge;
							otherCreature.GetComponent<CreatureController>().mergeTimer = 0;
							otherCreature.GetComponent<CreatureController>().timer = 0;

							Messenger.SendToListeners(new Message(gameObject, "GoingToMerge",""));	
						}
					}
				}
			}
		} 
		

		switch (myCharState)
		{
			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Initial:
	
				Messenger.SendToListeners(new Message(gameObject, "FogFollowingCreatures",""));					
				
				listening = false;
			
				if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.idle")== true )
				{
					myCharState = CharState.Idle;
				}   				
				
				myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (targetFloor - myTransform.position), 1 * Time.deltaTime);				
			
			break;
	
			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Idle:

				Messenger.SendToListeners(new Message(gameObject, "FogFollowingPlayers",""));					
			
				listening = true;
			
				animator.SetBool("long_idle",false);
				animator.SetBool("walk",false);
				
				if (Vector3.Distance (targetFloor, myTransform.position) > walkDistance)
				{		
					myCharState = CharState.Walking;
					break;
				}
	
				timer += Time.deltaTime;			
			
				if (Mathf.Abs(timer-pasivetime3) < 1 && Vector3.Distance (targetFloor, myTransform.position) > 50)
				{	
					myCharState = CharState.Going_to_other_player;
					timer = 0;
					break;
				}else if (Mathf.Abs(timer-pasivetime2) < 0.5  )
				{	
					myCharState = CharState.Looking_up;
					break;
				}else if (Mathf.Abs(timer-pasivetime1) < 1 )
				{	
					timerLongIdle=0;		
					myCharState = CharState.Long_idle;
					break;
				}

			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Walking:
			
				listening = true;
	
				animator.SetBool("run",false);
				animator.SetBool("walk",true);				
				
				if (Vector3.Distance (targetFloor, myTransform.position) > runDistance)
				{
					myCharState = CharState.Running;
					break;
				}else
				{
					if (Vector3.Distance (targetFloor, myTransform.position) < stopDistance) 
					{
						myCharState = CharState.Idle;
						break;
					}else
					{
						myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (targetFloor - myTransform.position), 10 * Time.deltaTime);				
						myTransform.position += myTransform.forward * walkSpeed * Time.deltaTime;
					}
				}
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Running:
				
				listening = false;
				
				animator.SetBool("run",true);
			
				if (Vector3.Distance (targetFloor, myTransform.position) < runDistance) 
				{
					myCharState = CharState.Walking;
					break;
				}else
				{
					myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (targetFloor - myTransform.position), 3 * Time.deltaTime);				
					myTransform.position += myTransform.forward * runSpeed * Time.deltaTime;
				}
				
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------

			case CharState.Long_idle:
			
				listening = true;
				
				timer += Time.deltaTime;			

				if (timerLongIdle == 0)
				{
					animator.SetTrigger("long_idle");
				}
				timerLongIdle += Time.deltaTime;

				if ( timerLongIdle > animator.GetCurrentAnimatorStateInfo(0).length)
				{
					myCharState = CharState.Idle;	
					timerLongIdle=0;
					break;		
				}   
				if (Vector3.Distance (targetFloor, myTransform.position) > walkDistance)
				{		
					myCharState = CharState.Walking;
					animator.SetBool("long_idle",false);
					timerLongIdle=0;		
				}
				
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Looking_up:

				listening = false;
			
				timer += Time.deltaTime;			
				
				if (timerLookingUp == 0)
				{
					animator.SetTrigger("look_up");
				}
				timerLookingUp += Time.deltaTime;
				if ( timerLookingUp > animator.GetCurrentAnimatorStateInfo(0).length)
				{
					if (Vector3.Distance (targetFloor, myTransform.position) > walkDistance)
					{		
						myCharState = CharState.Walking;	
						timerLookingUp=0;								
						break;
					}	
					myCharState = CharState.Idle;	
					timerLookingUp=0;	
				} 
				
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Going_to_other_player:

				listening = false;
			
				timer += Time.deltaTime;			
				animator.SetBool("walk",true);

				if (Vector3.Distance (targetFloor, myTransform.position) < 0.5f) //changed by /100 already
				{
					myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (targetFloor2 - myTransform.position), rotationSpeed * Time.deltaTime);
					myTransform.position += myTransform.forward * walkSpeed * Time.deltaTime;
				}
				else
				{
					myCharState = CharState.Walking;
				}

			break;
			
			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Pointing:

				listening = false;
			
				myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (pointing_prop_position - myTransform.position), rotationSpeed * Time.deltaTime);				
							
				if (timer == 0)
				{
					animator.SetTrigger("point_at");
				}
				timer += Time.deltaTime;
				
				if ( timer > animator.GetCurrentAnimatorStateInfo(0).length )
				{
					if (Vector3.Distance (targetFloor, myTransform.position) > walkDistance)
					{		
						myCharState = CharState.Walking;	
						timer=0;
						break;
					}	
					myCharState = CharState.Idle;	
					timer=0;
				} 
				
			
			break;
			
			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Going_to_manipulate:
			
				listening = false;
							
				animator.SetBool("long_idle",false);
				animator.SetBool("walk",false);
				
				if (Vector3.Distance (new Vector3( prop_position.x,creaturesHight,prop_position.z), myTransform.position) < 0.5f) //changed by /100 already
				{
					myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (new Vector3( prop_position.x,creaturesHight,prop_position.z) - myTransform.position), 10 * Time.deltaTime);				
					myTransform.position -= myTransform.forward * walkSpeed * Time.deltaTime;
				}else
				{
					Debug.Log("[CreatureController]: I am already looking the prop, so I am going to manipulate it.");
					myCharState = CharState.Manipulating;	
				}
			
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Manipulating:

				listening = false;
				
				myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (prop_position - myTransform.position), rotationSpeed * Time.deltaTime);				

				if (timer == 0)
				{
					animator.SetTrigger("manipulate");
				}

				timer += Time.deltaTime;
				if ( timer > animator.GetCurrentAnimatorStateInfo(0).length)
				{
					myCharState = CharState.Waiting;	
					Messenger.SendToListeners(new Message(gameObject, "PropManipulated",prop_tag));	
					timer=0;	
				}   
			
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Waiting:
			
				listening = false;
	
			
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Celebrating:
			
				listening = false;
				if (timer == 0)
				{
					float j = Random.Range(0,2);
						
					if ( j == 0 )
					{
						animator.SetTrigger("celebrate1");
					}else
					{
						animator.SetTrigger("celebrate2");	
					}
				}
				timer += Time.deltaTime;

				//transform.localScale = Vector3.Scale(new Vector3(1.0005f,1.0005f,1.0005f), transform.localScale );

				if ( timer > animator.GetCurrentAnimatorStateInfo(0).length)
				{	
					if (Vector3.Distance (targetFloor, myTransform.position) > walkDistance)
					{		
						myCharState = CharState.Walking;	
						timer=0;
						break;
					}	
					myCharState = CharState.Idle;	
					timer=0;
				}   
			
			break;
			
			//-----------------------------------------------------------------------------------------------------------------------------------

			case CharState.Brief_greeting:

				listening = false;

				myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (otherCreature.transform.position - myTransform.position), 
														 rotationSpeed * Time.deltaTime);				
				greetingbool = false;

				if (timer == 0)
				{
					animator.SetTrigger("brief_greeting");
				}
				timer += Time.deltaTime;
				
				if ( timer > animator.GetCurrentAnimatorStateInfo(0).length )
				{
					if (Vector3.Distance (targetFloor, myTransform.position) > walkDistance)
					{		
						myCharState = CharState.Walking;	
						timer=0;
						break;
					}	
					myCharState = CharState.Idle;	
					timer=0;	
				}  
			
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------

			case CharState.Effusive_greeting:

				listening = false;
			
				myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (otherCreature.transform.position - myTransform.position), 
				                                         rotationSpeed * Time.deltaTime);				
				greetingbool = false;

				if (timer == 0)
				{
					animator.SetTrigger("effusive_greeting");
				}
				timer += Time.deltaTime;

				if ( timer > animator.GetCurrentAnimatorStateInfo(0).length)
				{
					if (Vector3.Distance (targetFloor, myTransform.position) > walkDistance)
					{		
						myCharState = CharState.Walking;	
						timer=0;
						break;
					}	
					myCharState = CharState.Idle;	
					timer=0;		
				} 
			
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Going_to_merge:
			
				listening = false;
				
				animator.SetBool("long_idle",false);
				animator.SetBool("walk",false);

				if (Vector3.Distance (otherCreature.transform.position, myTransform.position) < 1.8f) //changed by /100 already
				{
					if (Vector3.Distance (new Vector3 (0,0,0), myTransform.position) <= 2.6f) //changed by /100 already
					{
						myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (otherCreature.transform.position - myTransform.position), 10 * Time.deltaTime);				
						myTransform.position -= myTransform.forward * walkSpeed * Time.deltaTime;

					}else if (Vector3.Distance (new Vector3 (0,0,0), otherCreature.transform.position) >= 2.6f) //changed by /100 already
					{
						Messenger.SendToListeners(new Message(gameObject, "creaturesarenear",""));	
						myCharState = CharState.Waiting;
						break;
					}
				}else
				{
					Messenger.SendToListeners(new Message(gameObject, "creaturesarenear",""));	
					myCharState = CharState.Waiting;	
				}
			
			
			break;

			//-----------------------------------------------------------------------------------------------------------------------------------

			case CharState.Merging:

				listening = false;
				
				animator.SetBool("walk",true);				
			
				if (Vector3.Distance (otherCreature.transform.position, myTransform.position) > 0.1f) //changed by /100 already
				{
					myTransform.rotation = Quaternion.Slerp (myTransform.rotation, Quaternion.LookRotation (otherCreature.transform.position - myTransform.position), rotationSpeed * Time.deltaTime);			
					myTransform.position += myTransform.forward * walkSpeed * Time.deltaTime;
				}
				else
				{					
					if (gameObject.tag == "creature1_")
					{
						Messenger.SendToListeners(new Message(gameObject, "Merge2",""));
					} 
					if (gameObject.tag == "creature2_")
					{
						Messenger.SendToListeners(new Message(gameObject, "Merge1",""));
					}

					mergeDestroy();
				}
			
			break;
			
			//-----------------------------------------------------------------------------------------------------------------------------------
			
			case CharState.Destroy:

				listening = false;
			
				if (timer == 0)
				{
					animator.SetTrigger("merge");
				}
				timer += Time.deltaTime;

				if ( timer > animator.GetCurrentAnimatorStateInfo(0).length)
				{
					
					if (gameObject.tag == "creature1_") 
					{
						Messenger.SendToListeners(new Message(gameObject, "MergeNewCreature2",""));
					}
					if (gameObject.tag == "creature2_") 
					{
						Messenger.SendToListeners(new Message(gameObject, "MergeNewCreature1",""));
					}
				
					Messenger.RemoveListener(Insect_catched_player1);
					Messenger.RemoveListener(Insect_catched_player2);
					Messenger.RemoveListener(Merge1);
					Messenger.RemoveListener(Merge2);
					Messenger.RemoveListener(List_updated); // FATLTAVA AQUEST RemoveListener. Quan enviava per texture changed, petava el iterador de listeners
					Messenger.RemoveListener(manipulateProp);
					Messenger.RemoveListener(pointingProp);
					Messenger.RemoveListener(celebrateProp);					
				
					Destroy(gameObject);
				}

			break;	

			//-----------------------------------------------------------------------------------------------------------------------------------
			
		}// SWITCH END

		
		if (previousstate.nameHash != animator.GetCurrentAnimatorStateInfo(0).nameHash)
		{
			if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.look_up") == true )
			{
				string path_clip = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + "look_up";
				audio.clip = Resources.Load<AudioClip>(path_clip);
				audio.Play();

				Messenger.SendToListeners(new Message(gameObject, "Creature_looks_up",""));
			}  

			if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.point_at") == true )
			{
				string path_clip = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + "point_at";
				audio.clip = Resources.Load<AudioClip>(path_clip);
				audio.Play();

				Messenger.SendToListeners(new Message(gameObject, "Creature_points_object",""));
			}  

			if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.brief_greeting") == true )
			{
				string path_clip = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + "brief_greeting";
				audio.clip = Resources.Load<AudioClip>(path_clip);
				audio.Play();

				Messenger.SendToListeners(new Message(gameObject, "Creature_greets",""));
			}  

			if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.effusive_greeting") == true )
			{
				string path_clip = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + "effusive_greeting";
				audio.clip = Resources.Load<AudioClip>(path_clip);
				audio.Play();

				Messenger.SendToListeners(new Message(gameObject, "Creature_greets",""));
			}  

			if ( animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.manipulate") == true )
			{
				string path_clip = "Creatures/" + gameObject.name.Replace("(Clone)","") + "/" + "manipulate";
				audio.clip = Resources.Load<AudioClip>(path_clip);
				audio.Play();
			}  
		}

		previousstate = animator.GetCurrentAnimatorStateInfo(0);

		myTransform.Rotate(new Vector3(-transform.eulerAngles.x,0,-transform.eulerAngles.z));

		if (myCharState != CharState.Long_idle)
		{
			timerLongIdle=0;			
		}	

	
	}

	public void mergeDestroy()
	{
		//animator.SetTrigger("merge");
		timer = 0;
		myCharState = CharState.Destroy;	
	}

	public void manipulate(Message m)
	{
		Debug.Log("[CreatureController]: We are going to manipulate.");

		prop_position = m.MessageSource.transform.position;
		prop_tag = m.MessageSource.tag; 
		//animator.SetTrigger("manipulate");
		timer = 0;
		mergeTimer = 0;
		myCharState = CharState.Going_to_manipulate;
		Messenger.SendToListeners(new Message(gameObject, "FogFollowingCreatures",""));
	}

	public void PointingProp(Message m)
	{
		if (listening == true)
		{
			if (m.MessageValue == gameObject.tag)
			{
				pointing_prop_position = m.MessageSource.transform.position;
				timer = 0;
				myCharState = CharState.Pointing;
			}
		}	
	}

	public void CelebrateProp(Message m)
	{
		timer = 0;
		myCharState = CharState.Celebrating;
		Messenger.SendToListeners(new Message(gameObject, "FogFollowingPlayers",""));		
	}

	public void insect_catched(Message m)
	{
		if (m.MessageSource.name == "user1_swarm" && gameObject.tag == "creature1") 
		{			
			insectCount += 1;
			if (insectCount == insectsTextureChange) 
			{
				Messenger.SendToListeners (new Message (gameObject, "Texture_changed", "user1_swarm"));
				insectCount = 0;
			}
		}

		if (m.MessageSource.name == "user2_swarm" && gameObject.tag == "creature2") 
		{	
			insectCount += 1;
			if (insectCount == insectsTextureChange) 
			{
				Messenger.SendToListeners (new Message (gameObject, "Texture_changed", "user2_swarm"));
				insectCount = 0;
			}
		}
	}

	public void ChangeTexture(Message m)
	{
		if (!changeMesh) 
		{
			if (m.MessageSource.name == "user1_swarm" && gameObject.tag == "creature1") 
			{	
				Messenger.RemoveListener(Insect_catched_player1);
				Messenger.RemoveListener(Insect_catched_player2);
				Messenger.RemoveListener(Merge1);
				Messenger.RemoveListener(Merge2);
				Messenger.RemoveListener(List_updated);
				Messenger.RemoveListener(manipulateProp);
				Messenger.RemoveListener(pointingProp);
				Messenger.RemoveListener(celebrateProp);

				GameObject.Find("Scenario").GetComponentInChildren<Main>().loadcreature(gameObject.name.Replace("(Clone)",""),
				                                                                        GameObject.Find("Player1"),GameObject.Find("Player2"),transform.position,null,1);
				textureCount =1;
				/*
				childRenderer [0].material.mainTexture = current_textures [1];

				string path = "Effects/CreatureDuplicated"; 
				GameObject Particles = (GameObject)Resources.Load (path, typeof(GameObject));	
				Particles.transform.position = new Vector3 (transform.position.x, 60, transform.position.z);
				GameObject NewCreatureAnimation = (GameObject)GameObject.Instantiate (Particles);
				*/
				Destroy(gameObject);

			}
			if (m.MessageSource.name == "user2_swarm" && gameObject.tag == "creature2") 
			{	
				Messenger.RemoveListener(Insect_catched_player1);
				Messenger.RemoveListener(Insect_catched_player2);
				Messenger.RemoveListener(Merge1);
				Messenger.RemoveListener(Merge2);
				Messenger.RemoveListener(List_updated);
				Messenger.RemoveListener(manipulateProp);
				Messenger.RemoveListener(pointingProp);
				Messenger.RemoveListener(celebrateProp);

				GameObject.Find("Scenario").GetComponentInChildren<Main>().loadcreature(gameObject.name.Replace("(Clone)",""),
				                                                                        GameObject.Find("Player2"),GameObject.Find("Player2"),transform.position,null,1);
	
				/*
				childRenderer [0].material.mainTexture = current_textures [1];

				string path = "Effects/CreatureDuplicated"; 
				GameObject Particles = (GameObject)Resources.Load (path, typeof(GameObject));	
				Particles.transform.position = new Vector3 (transform.position.x, 60, transform.position.z);
				GameObject NewCreatureAnimation = (GameObject)GameObject.Instantiate (Particles);
				 */
				Destroy(gameObject);

			}



		} else 
		{
			if (m.MessageSource.name == "user1_swarm" && gameObject.tag == "creature1") 
			{	
				Messenger.RemoveListener(Insect_catched_player1);
				Messenger.RemoveListener(Insect_catched_player2);
				Messenger.RemoveListener(Merge1);
				Messenger.RemoveListener(Merge2);
				Messenger.RemoveListener(List_updated);
				Messenger.RemoveListener(manipulateProp);
				Messenger.RemoveListener(pointingProp);
				Messenger.RemoveListener(celebrateProp);
				if (gameObject.name.Contains("02"))
				{
					GameObject.Find("Scenario").GetComponentInChildren<Main>().loadcreature(gameObject.name.Replace("02(Clone)",""),
					                                                                        GameObject.Find("Player1"),GameObject.Find("Player2"),transform.position);	
				}else
				{
					GameObject.Find("Scenario").GetComponentInChildren<Main>().loadcreature(gameObject.name.Replace("(Clone)","02"),
					                                                                        GameObject.Find("Player1"),GameObject.Find("Player2"),transform.position);	
				}
				Destroy(gameObject);
			}
			if (m.MessageSource.name == "user2_swarm" && gameObject.tag == "creature2") 
			{		
				Messenger.RemoveListener(Insect_catched_player1);
				Messenger.RemoveListener(Insect_catched_player2);
				Messenger.RemoveListener(Merge1);
				Messenger.RemoveListener(Merge2);
				Messenger.RemoveListener(List_updated);
				Messenger.RemoveListener(manipulateProp);
				Messenger.RemoveListener(pointingProp);
				Messenger.RemoveListener(celebrateProp);
				if (gameObject.name.Contains("02"))
				{
					GameObject.Find("Scenario").GetComponentInChildren<Main>().loadcreature(gameObject.name.Replace("02(Clone)",""),
					                                                                        GameObject.Find("Player2"),GameObject.Find("Player1"),transform.position);	
				}else
				{
					GameObject.Find("Scenario").GetComponentInChildren<Main>().loadcreature(gameObject.name.Replace("(Clone)","02"),
					                                                                        GameObject.Find("Player2"),GameObject.Find("Player1"),transform.position);	
				}
				Destroy(gameObject);
			}
		}
	}
	
	
		
		
	void OnGUI() 
	{
		/*if (gameObject.tag == "creature1")
		{
			GUI.Label(new Rect(10, 90, 400, 20), "creature1 state : " + myCharState.ToString());
			GUI.Label(new Rect(10, 110, 200, 20), "timer : " + timer.ToString());
			
		}
		if (gameObject.tag == "creature2")
		{
			GUI.Label(new Rect(10, 140, 200, 20), "creature2 state : " + myCharState.ToString());
			GUI.Label(new Rect(10, 160, 200, 20), "timer : " + timer.ToString());
			
		}	*/	
	}
}