using UnityEngine;
using System.Collections;

public class KeyController : MessageBehaviour {

	private float catchingDistance = 0.8f; //changed by /100 already

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

	private Listener PropManipulated;
	private AudioSource audio;

	private bool pickedByCreature1;
	private bool pickedByCreature2;

	public bool iHaveBeenPicked;

	enum CharState
	{
		Idle,
		Picked
	};



	// Use this for initialization
	protected override void OnStart () 
	{

		myCharState = CharState.Idle;	

		animator = GetComponent<Animator>();

		player1 = GameObject.FindGameObjectWithTag("Player");
		player2 = GameObject.FindGameObjectWithTag("Player2");

		timer = 0;

		PropManipulated = new Listener("PropManipulated", gameObject, "propManipulated");

		Messenger.RegisterListener(PropManipulated);

		audio = GetComponent<AudioSource>();
		audio.playOnAwake = false;
		audio.minDistance = 6; //changed by /100 already
		audio.volume = 0.3f;

		pickedByCreature1 = false;
		pickedByCreature2 = false;

		iHaveBeenPicked = false;

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


		switch (myCharState)
		{
		case CharState.Idle:

			if (creature1 != null && creature2 != null )
			{
				float distCreature1 = (creature1.transform.position.x - transform.position.x) * (creature1.transform.position.x - transform.position.x) + (creature1.transform.position.z - transform.position.z) * (creature1.transform.position.z - transform.position.z);
				Debug.Log ("Creature 1: " + distCreature1);
				if (distCreature1 < catchingDistance*catchingDistance &&
					creature1.GetComponent<CreatureController>().listening == true &&
					creature1.GetComponent<CreatureController>().hasKeyPart == false)
				{
					pickedByCreature1 = true;
					iHaveBeenPicked = true;
					myCharState = CharState.Picked;
					creature1.GetComponent<CreatureController> ().hasKeyPart = true;
					Messenger.SendToListeners(new Message(gameObject, "prop_eliminated",gameObject.tag));

					string path = "Effects/CreatureAppears"; 
					GameObject Particles = (GameObject)Resources.Load (path, typeof(GameObject));	
					Particles.transform.position = new Vector3 (transform.position.x, 0.6f, transform.position.z); //changed by /100 already
					GameObject NewCreatureAnimation = (GameObject)GameObject.Instantiate (Particles);

					string path_clip = "Props/" + gameObject.name.Replace("(Clone)","") + "/" + gameObject.name.Replace("(Clone)","");
					audio.clip = Resources.Load<AudioClip>(path_clip);
					audio.Play();

					break;
				}	

				float distCreature2 = (creature2.transform.position.x - transform.position.x) * (creature2.transform.position.x - transform.position.x) + (creature2.transform.position.z - transform.position.z) * (creature2.transform.position.z - transform.position.z);
				Debug.Log ("Creature 2: " + distCreature2);
				if (distCreature2 < catchingDistance*catchingDistance &&
					creature2.GetComponent<CreatureController>().listening == true &&
					creature2.GetComponent<CreatureController>().hasKeyPart == false)
				{
					pickedByCreature2 = true;
					iHaveBeenPicked = true;
					myCharState = CharState.Picked;
					creature2.GetComponent<CreatureController> ().hasKeyPart = true;
					Messenger.SendToListeners(new Message(gameObject, "prop_eliminated",gameObject.tag));

					string path = "Effects/CreatureAppears"; 
					GameObject Particles = (GameObject)Resources.Load (path, typeof(GameObject));	
					Particles.transform.position = new Vector3 (transform.position.x, 0.6f, transform.position.z); //changed by /100 already
					GameObject NewCreatureAnimation = (GameObject)GameObject.Instantiate (Particles);

					string path_clip = "Props/" + gameObject.name.Replace("(Clone)","") + "/" + gameObject.name.Replace("(Clone)","");
					audio.clip = Resources.Load<AudioClip>(path_clip);
					audio.Play();

					break;
				}	
			}

			break;

		case CharState.Picked:

			Vector3 newPos = new Vector3 (0,0,0);

			if (this.gameObject.name == "Llave_cuerpo(Clone)") {
				newPos = new Vector3 (0, 0, -0.6f);
			} else {
				//newPos = new Vector3 (0.85f, -2f, -.85f);
			}

			if (pickedByCreature1 && creature1 != null) {
				newPos += creature1.transform.position;
			}

			if (pickedByCreature2 && creature2 != null)  {
				newPos += creature2.transform.position;
			}

			transform.position = newPos;

			break;			
		}
	}
}
