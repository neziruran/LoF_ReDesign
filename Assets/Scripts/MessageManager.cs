// This script maintains a list of listeners
// and the messages that they are interested
// in receiving, it then forwards on any
// messages it receives to the listener
// methods that are interested in that
// particular message type


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;




public class Message {
	
	public GameObject MessageSource 	{get; set;}
	public string MessageName   		{get; set;}
	public string MessageValue			{get; set;}
	
	public Message(GameObject s, string n, string v)
	{
		MessageSource = s;
		MessageName = n;
		MessageValue = v;
	}

}

// we are using inheritence here instead of an
// interface because the generic Message class
// is a valid object on its own

// now that we have our base class, we can
// inherit any number of specialized message
// classes that contain additional information

public class Message_transform: Message {
	
	public Transform Swarm_transform {get; set;}
	
	public Message_transform(GameObject s, string n, string v, Transform r) : base(s,n,v)
	{
		Swarm_transform = r;
	}
}

public class Insects_swarm : Message {
	
	public Vector4 Insects_swarms	{get; set;}
	
	public Insects_swarm(GameObject s, string n, string v, Vector4 p) : base(s,n,v)
	{
		Insects_swarms = p;
	}
}

public class DistanceMessage : Message {
	
	public float Distance	{ get; set;}
	
	public DistanceMessage(GameObject s, string n, string v, float d) : base (s,n,v)
	{
		Distance = d;
	}
}

public class HuntedInsectsPositions : Message {

	public Vector3 positionInsect1 {get; set;}
	public Vector3 positionInsect2 {get; set;}

	public HuntedInsectsPositions(GameObject s, string n, string v, Vector3 p1, Vector3 p2) : base (s,n,v)
	{
		positionInsect1 = p1;
		positionInsect2 = p2;
	}
}


// DEFINE ANY NUMBER OF MESSAGE CLASSES HERE
// as long as they inherit from Message, then
// you can subscribe to them and publish them

// we need a listener class that defines who
// is interested in which types of messages

public class Listener {
	
	public string ListenFor;
	public GameObject ForwardToObject;
	public string ForwardToMethod;
	
	public Listener(string lf, GameObject fo, string fm)
	{
		ListenFor = lf;
		ForwardToObject = fo;
		ForwardToMethod = fm;
	}
	
}

public class MessageBehaviour : MonoBehaviour {
	
	// base class from which all classes using
	// the MessageManager can inherit
	
	protected MessageManager Messenger;
	
	public void Start()
	{
		
		Messenger = GameObject.Find("World").GetComponent<MessageManager>();
		if(!Messenger) Debug.LogError("World.MessageManager could not be found.  Insure there is a World object with a MessageManager script attached.");
		OnStart();
	}
	
	// child classes must use the OnStart()
	// method instead of Start() like this:
	// protected override void OnStart ()
	
	// inheriting from MessageBehaviour like
	// this is purely done out of convenience
	// to easily get your Messenger reference.
	// you could instead find this reference
	// in the Start() method of every other
	// class 
	
	protected virtual void OnStart()
	{
	}
	
}


public class MessageManager : MonoBehaviour {
	
	private List<Listener> listeners = new List<Listener>();
	
	public void RegisterListener(Listener l)
	{
		listeners.Add(l);
	}
	public void RemoveListener(Listener l)
	{
		listeners.Remove (l);
	}

	// we only ever need access to the base Message
	// class attributes for our forwarding work
	
	public void SendToListeners(Message m)
	{
		//Debug.Log ("["+Time.time+"] Sending to listeners: "+m.MessageName);
		//Debug.Log (" Sending to listeners: "+m.MessageName);
		foreach (var f in listeners.FindAll(l => l.ListenFor == m.MessageName))  
		{    
			f.ForwardToObject.BroadcastMessage(f.ForwardToMethod,m,SendMessageOptions.DontRequireReceiver);
		}
	}
	
}
