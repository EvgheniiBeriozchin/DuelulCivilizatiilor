using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FortifyState : GameState {

	private GameObject fromTerritory;
	private GameObject toTerritory;
	private int currentRound;
	private InputField inputField;
	private GameState attackingState;

	public FortifyState(GameSystem gameSystem, GameState aState) : base(gameSystem){
		attackingState = aState;
		currentTeam = ((AttackingState)attackingState).getTeam ();
		Image capital = gameSystem.getCapitalImage ();
		capital.transform.GetComponentInChildren<Text> ().text = "Echipa " + (currentTeam + 1) + ", fortificați!";
		gameSystem.activateImage (capital);
		inputField = gameSystem.getInputField();
		inputField.placeholder.GetComponent<Text> ().text = "Numărul de armate fortificate";
	}

	public override void stateSolver(object k){
		if (k is GameObject) {
			if (fromTerritory == null) {
				fromTerritory = (GameObject)k;
				if (fromTerritory.GetComponentInChildren<Hex> ().team == currentTeam && fromTerritory.GetComponentInChildren<Hex> ().armies > 1) {
					gameSystem.getCapitalImage ().transform.GetComponentInChildren<Text> ().text = "Alegeți teritoriul fortificat!";
				} else
					fromTerritory = null;
			} else if (fromTerritory != (GameObject)k && ((GameObject)k).GetComponentInChildren<Hex> ().team == currentTeam) {
				toTerritory = (GameObject)k;
				inputField.GetComponent<CanvasGroup> ().alpha = 1f;
				inputField.GetComponent<CanvasGroup> ().blocksRaycasts = true;
			}
		}
		else if (k is StringWrapper){
			Debug.Log ("In State: " + ((StringWrapper)k).getString());
			Debug.Log (fromTerritory);
			Debug.Log (toTerritory);
			int armies;
			if (int.TryParse (((StringWrapper)k).getString (), out armies) && armies < fromTerritory.GetComponentInChildren<Hex> ().getArmies ()) {
				Debug.Log ("Parsed");
				fromTerritory.GetComponentInChildren<Hex> ().setArmies (fromTerritory.GetComponentInChildren<Hex>().getArmies() - armies);
				toTerritory.GetComponentInChildren<Hex> ().setArmies (toTerritory.GetComponentInChildren<Hex> ().getArmies () + armies);
				inputField.GetComponent<CanvasGroup> ().alpha = 0f;
				inputField.GetComponent<CanvasGroup> ().blocksRaycasts = false;
				fromTerritory = null;
				toTerritory = null;
				inputField.GetComponentsInChildren<Text> () [1].text = "";
				Image capital = gameSystem.getCapitalImage ();
				capital.transform.GetComponentInChildren<Text> ().text = "Echipa " + (currentTeam + 1) + ", fortificați!";
				Debug.Log (fromTerritory);
				Debug.Log (toTerritory);
			}
		}
	}

	public int getRound(){
		return currentRound;
	}

	public GameState getAttackingState(){
		return attackingState;
	}
}
