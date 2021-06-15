using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState {

	protected int currentTeam;
	protected GameSystem gameSystem;

	public GameState(GameSystem gameSystem){
		this.gameSystem = gameSystem;
		currentTeam = 0;
	}

	public abstract void stateSolver(object k);



}
