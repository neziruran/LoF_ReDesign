using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class LogManager : MessageBehaviour
{ 
	FileStream fileStream;


	string path;

	public enum PlayerState
	{
		Explorer,
		Hunter,
		Character_Creator,
		Passive,
		Character_Keeper
	};

	public PlayerState player1State;
	public PlayerState player2State;

	void Awake ()
	{
		System.DateTime saveNow = System.DateTime.Now;

		string theDate = System.DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss");
		path = ".\\logs\\" + theDate + ".txt";

		Debug.Log (path);

		if (!File.Exists(path)) 
		{
			// Create a file to write to. 
			File.Create(path);
		}
	}
	
	// inicialitzacio
	protected override void OnStart () 
	{    
		Messenger.RegisterListener (new Listener("Insect_created", gameObject, "insectCreated"));
		Messenger.RegisterListener (new Listener("Creature_created", gameObject, "creatureGoingToBeCreated"));
		Messenger.RegisterListener (new Listener("List_updated_creature", gameObject, "creatureCreated"));
		Messenger.RegisterListener (new Listener("List_updated_texture", gameObject, "creatureTextureChanged"));
		Messenger.RegisterListener (new Listener ("Creature_points_object", gameObject, "creaturePointsObject"));
		Messenger.RegisterListener (new Listener ("Creature_looks_up", gameObject, "creatureLooksUp"));
		Messenger.RegisterListener (new Listener ("Creature_greets", gameObject, "creatureGreeting"));
		Messenger.RegisterListener (new Listener ("Creature_shared", gameObject, "creatureShared"));
		Messenger.RegisterListener (new Listener ("Creature_merge", gameObject, "creatureMerged"));
		Messenger.RegisterListener (new Listener ("manipulate_prop", gameObject, "manipulateProp"));
		Messenger.RegisterListener (new Listener ("Key_pressed", gameObject, "keyPressed"));

		player1State = PlayerState.Explorer;
		player2State = PlayerState.Explorer;

		fileStream = new FileStream(path, FileMode.Open);

		string start = (Mathf.Round(Time.time * 100f) / 100f) +";System;start;0;0";

		byte[] info = new UTF8Encoding(true).GetBytes(start+"\n");
		fileStream.Write(info, 0, info.Length);

		GameObject p1 = GameObject.Find ("Player1");
		string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;explorer;" + p1.transform.position.x + ";" + p1.transform.position.z;
		byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
		fileStream.Write(infoP1, 0, infoP1.Length);
		
		GameObject p2 = GameObject.Find ("Player2");
		string player2Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;explorer;" + p2.transform.position.x + ";" + p2.transform.position.z;
		byte[] infoP2 = new UTF8Encoding(true).GetBytes(player2Position+"\n");
		fileStream.Write(infoP2, 0, infoP2.Length);
	}
	
	// actualitzacio per cada frame
	void Update ()
	{
		GameObject p1 = GameObject.Find ("Player1");
		string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;pos;" + p1.transform.position.x + ";" + p1.transform.position.z;
		byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
		fileStream.Write(infoP1, 0, infoP1.Length);

		GameObject p2 = GameObject.Find ("Player2");
		string player2Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;pos;" + p2.transform.position.x + ";" + p2.transform.position.z;
		byte[] infoP2 = new UTF8Encoding(true).GetBytes(player2Position+"\n");
		fileStream.Write(infoP2, 0, infoP2.Length);
	}

	public void insectCreated (Message_transform m)
	{
		if ( m.MessageSource.name == "user1_swarm" )
		{
			GameObject p1 = GameObject.Find ("Player1");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;hunted_insect;" + p1.transform.position.x + ";" + p1.transform.position.z;
			byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoP1, 0, infoP1.Length);

			if(player1State == PlayerState.Explorer)
			{
				string player1S = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;hunter;" + p1.transform.position.x + ";" + p1.transform.position.z;
				byte[] infoP1S = new UTF8Encoding(true).GetBytes(player1S+"\n");
				fileStream.Write(infoP1S, 0, infoP1S.Length);
				player1State = PlayerState.Hunter;
			}
		}
		
		if ( m.MessageSource.name == "user2_swarm" )
		{
			GameObject p2 = GameObject.Find ("Player2");
			string player2Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;hunted_insect;" + p2.transform.position.x + ";" + p2.transform.position.z;
			byte[] infoP2 = new UTF8Encoding(true).GetBytes(player2Position+"\n");
			fileStream.Write(infoP2, 0, infoP2.Length);

			if(player2State == PlayerState.Explorer)
			{
				string player2S = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;hunter;" + p2.transform.position.x + ";" + p2.transform.position.z;
				byte[] infoP2S = new UTF8Encoding(true).GetBytes(player2S+"\n");
				fileStream.Write(infoP2S, 0, infoP2S.Length);
				player2State = PlayerState.Hunter;
			}
		}
	}

	public void creatureGoingToBeCreated (Message m)
	{
		if ( m.MessageValue == "user1_swarm" )
		{
			GameObject p1 = GameObject.Find ("Player1");
			if(player1State == PlayerState.Hunter)
			{
				string player1S = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;character_creator;" + p1.transform.position.x + ";" + p1.transform.position.z;
				byte[] infoP1S = new UTF8Encoding(true).GetBytes(player1S+"\n");
				fileStream.Write(infoP1S, 0, infoP1S.Length);
				player1State = PlayerState.Character_Creator;
			}
		}
		
		if ( m.MessageValue == "user2_swarm" )
		{
			GameObject p2 = GameObject.Find ("Player2");
			if(player2State == PlayerState.Hunter)
			{
				string player2S = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;character_creator;" + p2.transform.position.x + ";" + p2.transform.position.z;
				byte[] infoP2S = new UTF8Encoding(true).GetBytes(player2S+"\n");
				fileStream.Write(infoP2S, 0, infoP2S.Length);
				player2State = PlayerState.Character_Creator;
			}
		}
	}

	public void creatureCreated (Message m)
	{
		if ( m.MessageSource.name == "user1_swarm" )
		{
			GameObject p1 = GameObject.Find ("Player1");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;creature_created;" + p1.transform.position.x + ";" + p1.transform.position.z;
			byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoP1, 0, infoP1.Length);

			if(player1State == PlayerState.Character_Creator)
			{
				string player1S = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;character_keeper;" + p1.transform.position.x + ";" + p1.transform.position.z;
				byte[] infoP1S = new UTF8Encoding(true).GetBytes(player1S+"\n");
				fileStream.Write(infoP1S, 0, infoP1S.Length);
				player1State = PlayerState.Character_Keeper;
			}
		}
		
		if ( m.MessageSource.name == "user2_swarm" )
		{
			GameObject p2 = GameObject.Find ("Player2");
			string player2Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;creature_created;" + p2.transform.position.x + ";" + p2.transform.position.z;
			byte[] infoP2 = new UTF8Encoding(true).GetBytes(player2Position+"\n");
			fileStream.Write(infoP2, 0, infoP2.Length);

			if(player2State == PlayerState.Character_Creator)
			{
				string player2S = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;character_keeper;" + p2.transform.position.x + ";" + p2.transform.position.z;
				byte[] infoP2S = new UTF8Encoding(true).GetBytes(player2S+"\n");
				fileStream.Write(infoP2S, 0, infoP2S.Length);
				player2State = PlayerState.Character_Keeper;
			}
		}
	}

	public void creatureTextureChanged (Message m)
	{
		if ( m.MessageSource.name == "user1_swarm" )
		{
			GameObject p1 = GameObject.Find ("Player1");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;creature_changed;" + p1.transform.position.x + ";" + p1.transform.position.z;
			byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoP1, 0, infoP1.Length);
		}
		
		if ( m.MessageSource.name == "user2_swarm" )
		{
			GameObject p2 = GameObject.Find ("Player2");
			string player2Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;creature_changed;" + p2.transform.position.x + ";" + p2.transform.position.z;
			byte[] infoP2 = new UTF8Encoding(true).GetBytes(player2Position+"\n");
			fileStream.Write(infoP2, 0, infoP2.Length);
		}
	}

	public void creaturePointsObject (Message m)
	{
		if ( m.MessageSource.tag == "creature1" )
		{
			GameObject c1 = GameObject.FindGameObjectWithTag ("creature1");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Creature1;point_at;" + c1.transform.position.x + ";" + c1.transform.position.z;
			byte[] infoC1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoC1, 0, infoC1.Length);
		}
		
		if ( m.MessageSource.tag == "creature2" )
		{
			GameObject c2 = GameObject.FindGameObjectWithTag ("creature2");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Creature2;point_at;" + c2.transform.position.x + ";" + c2.transform.position.z;
			byte[] infoC2 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoC2, 0, infoC2.Length);
		}
	}

	public void creatureLooksUp (Message m)
	{
		if ( m.MessageSource.tag == "creature1" )
		{
			GameObject c1 = GameObject.FindGameObjectWithTag ("creature1");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Creature1;look_up;" + c1.transform.position.x + ";" + c1.transform.position.z;
			byte[] infoC1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoC1, 0, infoC1.Length);
		}
		
		if ( m.MessageSource.tag == "creature2" )
		{
			GameObject c2 = GameObject.FindGameObjectWithTag ("creature2");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Creature2;look_up;" + c2.transform.position.x + ";" + c2.transform.position.z;
			byte[] infoC2 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoC2, 0, infoC2.Length);
		}
	}

	public void creatureGreeting (Message m)
	{
		if ( m.MessageSource.tag == "creature1" )
		{
			GameObject c1 = GameObject.FindGameObjectWithTag ("creature1");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Creature1;greeting;" + c1.transform.position.x + ";" + c1.transform.position.z;
			byte[] infoC1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoC1, 0, infoC1.Length);
		}
		
		if ( m.MessageSource.tag == "creature2" )
		{
			GameObject c2 = GameObject.FindGameObjectWithTag ("creature2");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Creature2;greeting;" + c2.transform.position.x + ";" + c2.transform.position.z;
			byte[] infoC2 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoC2, 0, infoC2.Length);
		}
	}

	public void creatureShared (Message m)
	{
		if ( m.MessageValue == "player1" )
		{
			GameObject p1 = GameObject.Find ("Player1");
			string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player1;share_creature;" + p1.transform.position.x + ";" + p1.transform.position.z;
			byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
			fileStream.Write(infoP1, 0, infoP1.Length);
		}
		
		if ( m.MessageValue == "player2" )
		{
			GameObject p2 = GameObject.Find ("Player2");
			string player2Position = (Mathf.Round(Time.time * 100f) / 100f) + ";Player2;share_creature;" + p2.transform.position.x + ";" + p2.transform.position.z;
			byte[] infoP2 = new UTF8Encoding(true).GetBytes(player2Position+"\n");
			fileStream.Write(infoP2, 0, infoP2.Length);
		}
	}

	public void creatureMerged (Message m)
	{
		string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";System;creature_merge;0;0";
		byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
		fileStream.Write(infoP1, 0, infoP1.Length);
	}

	public void manipulateProp (Message m)
	{
		Vector3 propPos = m.MessageSource.transform.position;
		string player1Position = (Mathf.Round(Time.time * 100f) / 100f) + ";" + m.MessageSource.tag + ";manipulate_prop;" + propPos.x + ";" + propPos.z;
		byte[] infoP1 = new UTF8Encoding(true).GetBytes(player1Position+"\n");
		fileStream.Write(infoP1, 0, infoP1.Length);
	}

	void OnApplicationQuit() 
	{
		string start = (Mathf.Round(Time.time * 100f) / 100f) +";System;end;0;0";
		
		byte[] info = new UTF8Encoding(true).GetBytes(start+"\n");
		fileStream.Write(info, 0, info.Length);
	}

	public void keyPressed(Message m) 
	{
		string message = m.MessageValue;

		if (m.MessageValue == "F1") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;creature_created_player1;0;0";
		}

		if (m.MessageValue == "F2") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;creature_created_player2;0;0";
		}

		if (m.MessageValue == "F3") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;creature_changed_player1;0;0";
		}

		if (m.MessageValue == "F4") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;creature_changed_player2;0;0";
		}

		if (m.MessageValue == "1") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;manipulated_prop1;0;0";
		}

		if (m.MessageValue == "2") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;manipulated_prop2;0;0";
		}

		if (m.MessageValue == "3") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;manipulated_prop3;0;0";
		}

		if (m.MessageValue == "4") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;manipulated_prop4;0;0";
		}
		if (m.MessageValue == "9") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;door_opened;0;0";
		}
		if (m.MessageValue == "B") {
			message = (Mathf.Round(Time.time * 100f) / 100f) + ";System;start_biometrics;0;0";
			gameObject.GetComponent<AudioSource>().Play();
		}
		
		byte[] info = new UTF8Encoding(true).GetBytes(message+"\n");
		fileStream.Write(info, 0, info.Length);
	}
}