using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionalPointsDistributionState : GameState {

	private InputField inputField;
	private int currentRound;

	public AdditionalPointsDistributionState(GameSystem gameSystem, int round) : base(gameSystem){
		currentRound = round;
		inputField = gameSystem.getInputField();
		inputField.placeholder.GetComponent<Text> ().text = "Puncte adiționale pentru echipa " + (currentTeam + 1);		
		inputField.GetComponent<CanvasGroup> ().alpha = 1f;
		inputField.GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	public override void stateSolver(object k){
		string s;
		if (k is StringWrapper)
			s = ((StringWrapper)k).getString ();
		else
			return;
			//throw new UnityException ("Wrong type of object in AdditionalPointsState");
		//Phase 3, part 2 - adding points to teams
		gameSystem.getTeam(currentTeam).setAdditionalPoints(int.Parse (s));
		gameSystem.getTeamPointsText(currentTeam).text = "Puncte ad.: " + s;
		inputField.text = "";
		currentTeam++;
		if (currentTeam == gameSystem.getNumberOfTeams()) {
			inputField.GetComponent<CanvasGroup> ().alpha = 0f;
			inputField.GetComponent<CanvasGroup> ().blocksRaycasts = false;
			gameSystem.setGameState (new AttackingState (gameSystem, currentRound));
		}

		else inputField.placeholder.GetComponent<Text> ().text = "Puncte adiționale pentru echipa " + (currentTeam + 1);
	}
}
