﻿using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	Queue<GameEvent> events;
	Queue<Command> commands;

	// Use this for initialization
	void Start () {
		events = new Queue<GameEvent>();
		commands = new Queue<Command>();
	}
	
	// Update is called once per frame
	void Update () {
		this.tick();
	}

	public void enqueueEvent(GameEvent ge){
		this.events.Enqueue(ge);
	}

	public void enqueueCommand(Command c){
		this.commands.Enqueue(c);
	}

	public void tick(){

		while(events.Count>0)
		{
			GameEvent ge = events.Dequeue();
			broadcastEvent(ge);
			while(commands.Count>0)
				commands.Dequeue().run();
		}

	}

	private void broadcastEvent(GameEvent ge){
		foreach(Map map in MapManager.getInstance().getMapList())
		{
			map.broadcastEvent(ge);
		}
	}
}
