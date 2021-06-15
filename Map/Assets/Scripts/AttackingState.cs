using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackingState : GameState {

	private int currentRound;
	private GameObject attackingTerritory;
	private GameObject defendingTerritory;

	public AttackingState(GameSystem gameSystem, int round) : base(gameSystem){
		currentRound = round;
		gameSystem.activateButtons ();
		if ((currentRound + 1) % 2 == 1)
			currentTeam = 0;
		else
			currentTeam = gameSystem.getNumberOfTeams() - 1;
		Image capital = gameSystem.getCapitalImage ();
		capital.transform.GetComponentInChildren<Text> ().text = "Echipa " + (currentTeam + 1) + ", atacați!";
		gameSystem.activateImage (capital);
	}


	public override void stateSolver(object k){
		Debug.Log (currentTeam);
		if (attackingTerritory == null) {
			attackingTerritory = (GameObject)k;
			if (attackingTerritory.GetComponentInChildren<Hex> ().team == currentTeam && attackingTerritory.GetComponentInChildren<Hex> ().armies > 1) {
				gameSystem.getCapitalImage ().transform.GetComponentInChildren<Text> ().text = "Alegeți teritoriul atacat!";
			} else
				attackingTerritory = null;
		}
		else {
			defendingTerritory = (GameObject)k;
			if (defendingTerritory.GetComponentInChildren<Hex>().team == currentTeam) {
				return;
			}
			if (!checkIfNeighbour (attackingTerritory))
				return;
			int defendingRandom, attackingRandom, defendingNumber, attackingNumber;
			Hex defendingHexObject = defendingTerritory.GetComponentInChildren<Hex> ();
			Hex attackingHexObject = attackingTerritory.GetComponentInChildren<Hex> ();
			//The points of each team (attackers, defenders) are calculated according to the formula
			attackingRandom = Random.Range (1, 7);
			gameSystem.setAttackingRandom("" + attackingRandom);
			gameSystem.activateText (gameSystem.getAttackingRandom());
			attackingNumber = attackingRandom + gameSystem.getTeam(attackingHexObject.team).getAdditionalPoints() + attackingHexObject.armies;
			gameSystem.setAttackingInput("" + attackingNumber);
			gameSystem.activateText (gameSystem.getAttackingInput());
			defendingRandom = Random.Range (1, 7);
			//In case the defending territory is neutral
			if (defendingHexObject.team == -1)
				defendingNumber = defendingRandom + 2;
			else
				defendingNumber = defendingRandom + gameSystem.getTeam (defendingHexObject.team).getAdditionalPoints () + defendingHexObject.armies;
			gameSystem.setDefendingRandom("" + defendingRandom);
			gameSystem.setDefendingInput("" + defendingNumber);
			gameSystem.activateText (gameSystem.getDefendingRandom());
			gameSystem.activateText (gameSystem.getDefendingInput());
			//Wait some time before showing the result
			System.Threading.Thread.Sleep (500);

			if (attackingNumber > defendingNumber) {
				gameSystem.getCapitalImage ().transform.GetComponentInChildren<Text> ().text = "Echipa atacatoare a cîștigat: " + attackingNumber + " vs " + defendingNumber;
				//Transfering the captured hex to the attacking team
				int transferArmies = attackingHexObject.getArmies() - 1;
				Debug.Log ("Transfer Armies: " + transferArmies);
				attackingHexObject.setArmies(1);

				if (defendingHexObject.team > -1) {
					int lostArmies = defendingHexObject.armies;
					gameSystem.getTeam (defendingHexObject.team).addArmies (-lostArmies);
					gameSystem.setArmiesText (defendingHexObject.team, "Armate: " + gameSystem.getTeam (defendingHexObject.team).getArmies ());
					gameSystem.getTeam (defendingHexObject.team).removeTerritory (defendingHexObject.positionInTheTeam);
					for (int i = defendingHexObject.positionInTheTeam; i < gameSystem.getTeam (defendingHexObject.team).getNumberOfTeamTerritories (); i++) {
						gameSystem.getTeam (defendingHexObject.team).getTerritory (i).GetComponentInChildren<Hex> ().positionInTheTeam--;
					}
				}
				defendingHexObject.setArmies (transferArmies);
				gameSystem.getTeam (attackingHexObject.team).appendTerritory (defendingTerritory);
				defendingHexObject.positionInTheTeam = gameSystem.getTeam (attackingHexObject.team).getNumberOfTeamTerritories () - 1;
				defendingTerritory.GetComponentInChildren<MeshRenderer> ().material = gameSystem.getTeamMaterial (attackingHexObject.team);
				defendingHexObject.team = attackingHexObject.team;

			} else {
				//Deleted all armies, but one from the attacking hex because it lost
				int lostArmies = attackingHexObject.armies - 1;
				gameSystem.getTeam (attackingHexObject.team).addArmies (-lostArmies);
				gameSystem.setArmiesText (attackingHexObject.team, "Armate: " + gameSystem.getTeam (attackingHexObject.team).getArmies ());
				attackingHexObject.setArmies(1);
				attackingTerritory.GetComponentsInChildren<TextMesh> () [1].text = "*";
				gameSystem.getCapitalImage ().transform.GetComponentInChildren<Text> ().text = "Echipa care se apără a cîștigat: " + attackingNumber + " vs " + defendingNumber;
			}

			System.Threading.Thread.Sleep (2000);

			attackingTerritory = null;

			gameSystem.getCapitalImage ().transform.GetComponentInChildren<Text> ().text = "Echipa " + (currentTeam + 1) + ", atacați!";
		
		}
	}


	public void incrementTeam(){
		//gameSystem.deactivateText (defendingInput);
		//gameSystem.deactivateText (attackingInput);		
		if ((currentRound + 1) % 2 == 1)
			currentTeam++;
		else
			currentTeam--;
		if ((currentTeam == gameSystem.getNumberOfTeams () && (currentRound + 1) % 2 == 1) || (currentTeam == -1 && (currentRound + 1) % 2 == 0)) { 
			gameSystem.deactivateButtons ();
			gameSystem.deactivateImage (gameSystem.getCapitalImage ());
			if (currentRound + 1 == gameSystem.getNumberOfRounds ())
				gameSystem.setGameState (new EndGameState (gameSystem));
			else {
				gameSystem.calculateArmiesForTeams ();
				gameSystem.setGameState (new AdditionalPointsDistributionState (gameSystem, currentRound + 1));
			}
		}
		else {
			gameSystem.getCapitalImage().transform.GetComponentInChildren<Text> ().text = "Echipa " + (currentTeam + 1) + ", atacați!";
		}
	}

	private bool checkIfNeighbour(GameObject go){
		GameObject[] neighbours = gameSystem.getMap ().GetComponent<MapGeneration_Continent> ().getGOWithinRadius (go, 1);
		foreach (GameObject g in neighbours) {
			if (g == defendingTerritory)
				return true;
		}
		return false;
	}

	public int getTeam(){
		return currentTeam;
	}
}
