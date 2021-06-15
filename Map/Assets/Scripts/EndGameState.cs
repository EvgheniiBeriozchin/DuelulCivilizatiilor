using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameState : GameState {

	public EndGameState(GameSystem gameSystem) : base(gameSystem) {
		Image results = gameSystem.getEndGameImage();
		int[] teamScores = new int[gameSystem.getNumberOfTeams ()];
		for (int i = 0; i < gameSystem.getNumberOfTeams (); i++) {
			teamScores [i] = gameSystem.getTeam (i).getNumberOfTeamTerritories ();
		}
		List<int> loc = gameSystem.getMap ().GetComponentInChildren<MapGeneration_Continent> ().getLocationsOfCapitals ();
		foreach (int i in loc) {
			teamScores [loc[i]]++;
		}
		string s = "Scoruri: \n";
		for (int i = 0; i < gameSystem.getNumberOfTeams (); i++) {
			s += "Echipa " + (i + 1) + ": " + teamScores[i] + " puncte\n";
		}
		results.GetComponentInChildren<Text>().text = s;
		gameSystem.activateImage(results);

	}

	public override void stateSolver(object k){
		
	}
}
