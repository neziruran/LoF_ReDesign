using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {

	public float walkRepeatingTime=0.8f;
	public float runRepeatingTime=0.4f;

	private bool isRuning=false;
	private bool isWalking=false;
	private bool waitForNextStep=false;

	private AudioSource[] audios;
	
	private AudioSource _stepSoundPlayer;

	private AudioClip[] _randomGrassStepSounds;
	private AudioClip[] _randomWaterStepSounds;
	private AudioClip[] _randomIceStepSounds;
	private AudioClip[] _randomLavaStepSounds;

	public GameObject zoneWater;
	public GameObject zoneIce;
	public GameObject zoneLava;

	// Use this for initialization
	void Start () {

		audios = GetComponents<AudioSource>();

		_stepSoundPlayer=audios [1];

		_stepSoundPlayer.volume = 0.2f;
		_stepSoundPlayer.playOnAwake = false;
		_stepSoundPlayer.minDistance = 600;

		zoneWater = GameObject.Find ("zoneWater");
		zoneIce = GameObject.Find ("zoneIce");
		zoneLava = GameObject.Find ("zoneLava");

		_randomGrassStepSounds = new AudioClip[]{(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Herba1") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Herba2") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Herba3") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Herba4") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Herba5") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Herba6") as AudioClip};
		_randomWaterStepSounds = new AudioClip[]{(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Aigua1") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Aigua2") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Aigua3") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Aigua4") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Aigua5") as AudioClip,
												 (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Aigua6") as AudioClip};
		_randomIceStepSounds = new AudioClip[]{(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Gel1") as AudioClip,
											   (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Gel2") as AudioClip,
											   (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Gel3") as AudioClip,
											   (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Gel4") as AudioClip,
											   (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Gel5") as AudioClip,
											   (AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Gel6") as AudioClip};
		_randomLavaStepSounds = new AudioClip[]{(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Terra1") as AudioClip,
												(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Terra2") as AudioClip,
												(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Terra3") as AudioClip,
												(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Terra4") as AudioClip,
												(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Terra5") as AudioClip,
												(AudioClip)Resources.Load("Sounds/Footsteps/Footstep.Terra6") as AudioClip};
	}
	
	// Update is called once per frame
	void Update () {

		GameObject creature = gameObject;
		CreatureController playerScript = creature.GetComponent<CreatureController>();
	
		if(playerScript.myCharState == CreatureController.CharState.Walking)
		{
			isWalking=true;
		}
		if(playerScript.myCharState != CreatureController.CharState.Walking)
		{
			isWalking=false;
		}
		if(playerScript.myCharState == CreatureController.CharState.Running)
		{
			isRuning=true;
		}
		if(playerScript.myCharState != CreatureController.CharState.Running)
		{
			isRuning=false;
		}
		if(isWalking && waitForNextStep==false)
		{
			if(zoneWater.GetComponent<Collider>().bounds.Contains(transform.position) == true)
			{
				StartCoroutine(WaitForWaterPlay(walkRepeatingTime));
			}else if(zoneIce.GetComponent<Collider>().bounds.Contains(transform.position) == true)
			{
				StartCoroutine(WaitForIcePlay(walkRepeatingTime));
			}else if(zoneLava.GetComponent<Collider>().bounds.Contains(transform.position) == true)
			{
				StartCoroutine(WaitForLavaPlay(walkRepeatingTime));	
			}else{
				StartCoroutine(WaitForGrassPlay(walkRepeatingTime));		
			}

			waitForNextStep=true;
		}
		if(isRuning && waitForNextStep==false)
		{
			if(zoneWater.GetComponent<Collider>().bounds.Contains(transform.position) == true)
			{
				StartCoroutine(WaitForWaterPlay(runRepeatingTime));
			}else if(zoneIce.GetComponent<Collider>().bounds.Contains(transform.position) == true)
			{
				StartCoroutine(WaitForIcePlay(runRepeatingTime));
			}else if(zoneLava.GetComponent<Collider>().bounds.Contains(transform.position) == true)
			{
				StartCoroutine(WaitForLavaPlay(runRepeatingTime));	
			}else{
				StartCoroutine(WaitForGrassPlay(runRepeatingTime));		
			}

			waitForNextStep=true;
		}


	}

	IEnumerator WaitForGrassPlay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		_stepSoundPlayer.clip=_randomGrassStepSounds[Random.Range(0,_randomGrassStepSounds.Length)];
		waitForNextStep=false;
		_stepSoundPlayer.Play();
	}
	IEnumerator WaitForWaterPlay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		_stepSoundPlayer.clip=_randomWaterStepSounds[Random.Range(0,_randomWaterStepSounds.Length)];
		waitForNextStep=false;
		_stepSoundPlayer.Play();
	}
	IEnumerator WaitForIcePlay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		_stepSoundPlayer.clip=_randomIceStepSounds[Random.Range(0,_randomIceStepSounds.Length)];
		waitForNextStep=false;
		_stepSoundPlayer.Play();
	}
	IEnumerator WaitForLavaPlay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		_stepSoundPlayer.clip=_randomLavaStepSounds[Random.Range(0,_randomLavaStepSounds.Length)];
		waitForNextStep=false;
		_stepSoundPlayer.Play();
	}
}
