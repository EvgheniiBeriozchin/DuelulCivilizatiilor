using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapitalChoosingState : GameState {



	public CapitalChoosingState (GameSystem gameSystem) : base(gameSystem) {
	}

	public override void stateSolver(object k){  
		GameObject hex;
		if (k is GameObject)
			hex = (GameObject) k;
		else 
			throw new UnityException("The selected object is not a gameObject");
		if (!gameSystem.getMap().GetComponentInChildren<MapGeneration>().isTerritoryCapital(hex.GetComponentInChildren<Hex>().hexagon) || hex.GetComponentInChildren<Hex>().team != - 1){
			return;
		}
		
		//The clicked hex changes its material(in our case just color) to the material, which corresponds to the currently active team
		gameSystem.appendObjectToTeam(hex, currentTeam);
		List<GameObject> adjacentTerritories = gameSystem.getMap().GetComponentInChildren<MapGeneration>().getAdjacentTerritories (hex);
		for (int i = 0; i < adjacentTerritories.Count; i++) {
			gameSystem.appendObjectToTeam (adjacentTerritories [i], currentTeam);
		}
		//The capital for the current team is chosen, we go to the next team
		currentTeam++;
		gameSystem.getCapitalImage().transform.GetComponentInChildren<Text> ().text = "Echipa " + (currentTeam + 1) + ", alegeți-vă capitala!";

		if (currentTeam == gameSystem.getNumberOfTeams()){
			gameSystem.deactivateImage(gameSystem.getCapitalImage());
			gameSystem.setGameState(new NamingState(gameSystem));	

		}
	}


}
