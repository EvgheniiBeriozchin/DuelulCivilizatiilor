using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamingState : GameState {

	private InputField inputField;

	public NamingState(GameSystem gameSystem) : base(gameSystem) {
		this.inputField = gameSystem.getInputField();
		inputField.placeholder.GetComponent<Text> ().text = "Echipa " + (currentTeam + 1) + ", alegeți-vă civilizația";
		inputField.GetComponent<CanvasGroup> ().alpha = 1f;
		inputField.GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}
	
	// Update is called once per frame
	public override void stateSolver (object k) {
		string name;
		if (k is StringWrapper)
			name = ((StringWrapper)k).getString ();
		else
			//throw new UnityException ("Wrong type of object in AdditionalPointsState");
			return;
		Debug.Log (name);
		gameSystem.getTeam(currentTeam).setTeamName(name);
		gameSystem.setTeamName(currentTeam, name);
		//The text from the InputField is deleted
		inputField.text = "";

		currentTeam++;
		if (currentTeam == gameSystem.getNumberOfTeams()) {
			gameSystem.setGameState (new AdditionalPointsDistributionState(gameSystem, 0));
		} 
		else 
			//Go to the next team
			inputField.placeholder.GetComponent<Text> ().text = "Echipa " + (currentTeam + 1) + ", alegeți-vă civilizația";
	}
}
