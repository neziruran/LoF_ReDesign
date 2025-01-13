using UnityEngine;
using System.Collections;

public class DoorController : MessageBehaviour {


	//----------------------

	private float pointingDistance = 0.7f; //changed by /100 already

	//----------------------

	private float prevDistance1;
	private bool pointingbool1 = true;
	private float timer1;
	private float prevDistance2;
	private bool pointingbool2 = true;
	private float timer2;

	private int countmessage = 0;

	private CharState myCharState;

	private Animator animator;

	private float timer;

	private GameObject player1;
	private GameObject player2;

	private GameObject creature1;
	private GameObject creature2;

	private GameObject keyHead;
	private GameObject keyBody;

	private Listener PropManipulated;

	private float initialVolume;
	private AudioSource audioSource;

	public bool wigglingDoor;

	public GameObject particlesEffect;

	enum CharState
	{
		Idle,
		Manipulated,

	};
		
	// Use this for initialization
	protected override void OnStart () 
	{
		myCharState = CharState.Idle;	

		animator = GetComponent<Animator>();

		if (!wigglingDoor) {
			animator.speed = 0;
		}

		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");

		timer = 0;

		PropManipulated = new Listener("PropManipulated", gameObject, "propManipulated");

		Messenger.RegisterListener(PropManipulated);

		audioSource = GetComponent<AudioSource>();
		initialVolume = audioSource.volume;
		audioSource.volume = 0.1f;
	

	}

	// Update is called once per frame
	void Update () 
	{
		if (GameObject.FindGameObjectWithTag("creature1") != null)
		{
			creature1 = GameObject.FindGameObjectWithTag("creature1");
		}
		if (GameObject.FindGameObjectWithTag("creature2") != null)
		{
			creature2 = GameObject.FindGameObjectWithTag("creature2");
		}

		if (GameObject.Find("Llave_mango(Clone)") != null)
		{
			keyHead = GameObject.Find ("Llave_mango(Clone)");
		}
		if (GameObject.Find("Llave_cuerpo(Clone)") != null)
		{
			keyBody = GameObject.Find("Llave_cuerpo(Clone)");
		}

		switch (myCharState)
		{
		case CharState.Idle:			

			if (creature1 != null && creature2 != null && keyHead != null && keyBody != null )
			{
				if (Vector3.Distance (creature1.transform.position, transform.position) < pointingDistance &&  
					Vector3.Distance (creature2.transform.position, transform.position) < pointingDistance &&
					creature1.GetComponent<CreatureController>().listening == true && 
					creature2.GetComponent<CreatureController>().listening == true &&
					keyHead.GetComponent<KeyController>().iHaveBeenPicked == true &&
					keyBody.GetComponent<KeyController>().iHaveBeenPicked == true)
				{
					Debug.Log("[DoorController]: Creatures are close to me, and have all keys, and mounted, they should open me.");

					Messenger.SendToListeners(new Message(gameObject, "manipulate_prop",""));
				}	
			}
			if (GameObject.FindGameObjectWithTag("creature1") != null)
			{
				if (Vector3.Distance (creature1.transform.position, transform.position) > pointingDistance && Vector3.Distance (creature1.transform.position, transform.position) < pointingDistance+0.1f)
				{
					if (pointingbool1 == true)
					{
						if (prevDistance1 > Vector3.Distance (creature1.transform.position, transform.position))
						{
							Messenger.SendToListeners(new Message(gameObject, "pointing_prop","creature1"));
							timer1=0;
							pointingbool1 = false;
						}	    
					}
				}
				timer1 += Time.deltaTime;
				if (timer1 > 10)
				{
					pointingbool1 = true;
				}	
				prevDistance1 = Vector3.Distance (creature1.transform.position, transform.position);	
			}

			if (GameObject.FindGameObjectWithTag("creature2") != null)
			{
				if (Vector3.Distance (creature2.transform.position, transform.position) > pointingDistance && Vector3.Distance (creature2.transform.position, transform.position) < pointingDistance+0.1f)
				{
					if (pointingbool2 == true)
					{
						if (prevDistance2 > Vector3.Distance (creature2.transform.position, transform.position))
						{
							Messenger.SendToListeners(new Message(gameObject, "pointing_prop","creature2"));
							timer2=0;
							pointingbool2 = false;
						}	    
					}
				}
				timer2 += Time.deltaTime;
				if (timer2 > 10)
				{
					pointingbool2 = true;
				}	
				prevDistance2 = Vector3.Distance (creature2.transform.position, transform.position);
			}

			break;

		case CharState.Manipulated:

			if(audioSource.volume < initialVolume)
			{
				audioSource.volume += 0.01f;
			}

			if (timer == 0)
			{	
				if (!wigglingDoor) {
					animator.speed = 1;
				}

				animator = GetComponent<Animator>();
				animator.SetTrigger("manipulated");

				string path = "Effects/PropManipulated"; 
				GameObject Particles = (GameObject)Resources.Load(path, typeof(GameObject));	
				Particles.transform.position = new Vector3 (transform.position.x, 0.6f, transform.position.z);
				GameObject NewCreatureAnimation = (GameObject) GameObject.Instantiate(Particles);

				string path_clip = "Props/" + gameObject.name.Replace("(Clone)","") + "/" + gameObject.name.Replace("(Clone)","");



			}
			if (timer > 2)
			{	
				particlesEffect.SetActive(true);

			}


			//transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation ( new Vector3(0,0,0) - transform.position ), 10 * Time.deltaTime);				
			timer += Time.deltaTime;

			if ( timer > 6)//animator.GetCurrentAnimatorStateInfo(0).length || timer > 20)
			{
				//Messenger.SendToListeners(new Message(gameObject, "prop_eliminated", ""));	
				//Messenger.RemoveListener(PropManipulated);
				//Destroy(gameObject);
				//timer = 0;

				GameObject fader;
				if (GameObject.Find("Fader") != null)
				{
					fader = GameObject.Find("Fader");
					fader.GetComponent<ScreenFader> ().fadeIn = false;
				}

				//particlesEffect.SetActive(false);

			}

			break;			
		}

		if (Input.GetKeyDown (KeyCode.Alpha9)) {
			myCharState = CharState.Manipulated;
			timer=0;
			countmessage = 0;
			Messenger.SendToListeners (new Message (gameObject, "Key_pressed", "9"));
			audioSource.Play();
		}
	}

	void propManipulated(Message m)
	{
		if (gameObject.tag == m.MessageValue)
		{
			countmessage += 1;
			if (countmessage == 2)
			{
				myCharState = CharState.Manipulated;
				timer=0;
				countmessage = 0;
				audioSource.Play();

			}
		}	
	}
}
