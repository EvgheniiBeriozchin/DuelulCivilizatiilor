using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class GameSystem : MonoBehaviour {

	#region Variable Declarations
	private GameObject Map;

	private GameState gameState;

	public  InputField inputField;

	public Image capitalImage;
	public Image endGameImage;

	public Button endAttacksButton;
	public Button attackButton;

	public Text defendingRandom;
	public Text attackingRandom;
	public Text defendingInput;
	public Text attackingInput;
	public Text[] TeamNames;
	public Text[] TeamPoints;
	public Text[] TeamRoundArmies;
	public Text[] teamTotalArmies;

	private int numberOfTeams = 2;
	private int numberOfRounds = 8;
	private int maxConnectedTerritories = 0;
	private int numberOfConnectedTerritories = 0;

	private bool[] isTeamInGame;
	private bool[] isTerritoryVisited;

	public Material[] teamMaterials;
	private Team[] currentTeams;
	#endregion

	#region StartFunction
	void Start () {

		//Connecting the GameSystem with the current Map	
		Map = GameObject.Find ("Map");
		numberOfTeams = Map.GetComponent<MapGeneration_Continent> ().getNumberOfTeams ();

		//Initialization of the arrays
		currentTeams = new Team[numberOfTeams];
		isTeamInGame = new bool[numberOfTeams];


		//Initialization of each existent team
		//TO DO: Fix number of Armies at the beginning
		for (int i = 0; i < numberOfTeams; i++){
			currentTeams [i] = new Team (Map.GetComponent<MapGeneration_Continent>().getNumberOfAdjacentTerritories());
			isTeamInGame [i] = true;
			teamTotalArmies [i].text = "Armate: " + currentTeams[i].getArmies();
			TeamRoundArmies [i].text = "Puncte ter.: 0";
		}

		//Starting the game process
		//In Phase 1 each team or PLAYER chooses a capital city for its team (NOW from the 8 possible capitals)
		//The PLAYER does it by clicking on a hex on the map. After the hex is clicked, it changes its color to the color of the active team 
		//The initiation of Phase 1 takes place by activating the Image+Text (capitalImage) Element (which informs the PLAYER about what he has to do)
		//Initially in the game this UI Element is activated, so we just write the suitable text in it
		capitalImage.transform.GetComponentInChildren<Text>().text = "Echipa 1, alegeți-vă capitala!";
		gameState = new CapitalChoosingState (this);
	}

	#endregion


	void Update () {
		//The function Update mainly tracks the position of the mouse and detects the objects which are clicked by the PLAYER.
		//Continuously Rays are casted from the point of the mouse to the game world/objects beneath
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		// = If the ray hits an object
		if (Physics.Raycast (ray, out hitInfo)) {
			//Initialize HitHex as the object hit by the ray
			GameObject hitHex = hitInfo.collider.transform.parent.gameObject;
			//The Water hexagons have their team parameter (in the Hex Script attached to the GO) set to 100, while all the terrain hexes have it set to either 10 (neutral hex) or 0-7 (the corresponding team)
			//There are no operations related to the Water hexagons during the game, so we eliminate the Rayhits on them. 
			if (Input.GetMouseButtonDown (0) && hitHex.GetComponentInChildren<Hex>().team > -2){
				if (gameState == null)	
				gameState = new AttackingState (this, 0);
					gameState.stateSolver(hitHex);
			}
		}
	}
		
	//Called when Enter is pressed on the InputField
	public void getInput(){
		Debug.Log ("From input: " + inputField.text);
		Debug.Log (gameState);
		gameState.stateSolver (new StringWrapper(inputField.text));
	}



	//When the button EndAttacks is clicked
	public void EndAttacksClick(){
		if (gameState is AttackingState)
			gameState = new FortifyState (this, gameState);
		else if (gameState is FortifyState) {
			gameState = ((FortifyState)gameState).getAttackingState ();
			((AttackingState)gameState).incrementTeam ();
		}
	}


	//When the button Attack is clicked
	public void Attack(){
		capitalImage.transform.GetComponentInChildren<Text>().text = "Alegeți teritoriul de atac!";
		activateImage (capitalImage);
	}



	//Number of additional armies for each team is calculated according to
	//the biggest connected chain of terrritories
	//This is calculated using DFS
	#region ArmiesFunctions
	public void calculateArmiesForTeams(){
		for (int i = 0; i < numberOfTeams; i++) {
			maxConnectedTerritories = 0;
			isTerritoryVisited = new bool[currentTeams [i].getNumberOfTeamTerritories()];
			for (int j = 0; j < currentTeams[i].getNumberOfTeamTerritories(); j++) {
				isTerritoryVisited [j] = false;	
			}
			for (int j = 0; j < currentTeams[i].getNumberOfTeamTerritories(); j++) {
				if (!isTerritoryVisited [j]) {
					numberOfConnectedTerritories = 0;
					DFS (i,j);
					if (numberOfConnectedTerritories > maxConnectedTerritories)
						maxConnectedTerritories = numberOfConnectedTerritories;
				

				}
	
			}

			//The additional armies are distributed randomly through the territories
			DistributeArmies(i, (int)(maxConnectedTerritories / 2));
			currentTeams [i].addArmies ((int)(maxConnectedTerritories / 2));
			teamTotalArmies [i].text = "Armate: " + currentTeams [i].getArmies ();
			TeamRoundArmies [i].text = "Puncte ter.: " + (int)(maxConnectedTerritories / 2);
		}

	}
		
	public void DFS(int i, int j){
		isTerritoryVisited [j] = true;
		numberOfConnectedTerritories++;
		Hexagon[] Neighbours = Map.GetComponent<MapGeneration_Continent>().GetHexagonsWithinRadius (currentTeams [i].getTerritory(j).GetComponentInChildren<Hex>().hexagon, 1);
		foreach (Hexagon h in Neighbours) {
			int PositionOfTheHexagon = Map.GetComponent<MapGeneration_Continent>().GetGOFromHexagon (h).GetComponentInChildren<Hex> ().positionInTheTeam;
			if (Map.GetComponent<MapGeneration_Continent>().GetGOFromHexagon (h).GetComponentInChildren<Hex>().team == i && !isTerritoryVisited[PositionOfTheHexagon]) DFS(i, PositionOfTheHexagon);	
		}


	}

	void DistributeArmies(int team, int armies){
		//Armies are randomly added to the team's territories
		for (int i = 0; i < armies; i++) {

			GameObject Hex = currentTeams [team].getTerritory(Random.Range (0, currentTeams [team].getNumberOfTeamTerritories()));

			Hex.GetComponentInChildren<Hex>().addArmies(1);
		}


	}
	#endregion

	public void appendObjectToTeam(GameObject gameObject, int team){
		currentTeams [team].appendTerritory(gameObject);
		gameObject.GetComponentInChildren<MeshRenderer> ().material = teamMaterials[team];
		//The info about the team to which each hex belongs is stored inside each hex (in the Hex Script)
		gameObject.transform.GetComponentInChildren<Hex> ().team = team;
		//As this is phase 1, the current hex becomes the team's capital and the info about this hex is stored inside the Team Script which corresponds to the currently active team (the currentTeams array)
		//Inside the hex (the Hex Script) we store the position of this hex in the array(list)
		gameObject.transform.GetComponentInChildren<Hex> ().positionInTheTeam = currentTeams[team].getNumberOfTeamTerritories() - 1;
	}

	//Some helper functions to shorten the main code 
	//These Activate/Deactivate the corresponding UI Element, when called
	#region HelperFunctions
	public void activateText(Text text){
		text.GetComponent<CanvasGroup> ().alpha = 1f;
	}

	public void deactivateText(Text text){
		text.GetComponent<CanvasGroup> ().alpha = 0f;
	}
	public void activateButtons(){
		endAttacksButton.GetComponent<CanvasGroup> ().alpha = 1f;
		endAttacksButton.GetComponent<CanvasGroup> ().blocksRaycasts = true;
		attackButton.GetComponent<CanvasGroup> ().alpha = 1f;
		attackButton.GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	public void deactivateButtons(){
		endAttacksButton.GetComponent<CanvasGroup> ().alpha = 0f;
		endAttacksButton.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		attackButton.GetComponent<CanvasGroup> ().alpha = 0f;
		attackButton.GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}
	public void activateImage(Image image){
		image.GetComponent<CanvasGroup> ().alpha = 1f;
		image.GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	public void deactivateImage(Image image){
		image.GetComponent<CanvasGroup> ().alpha = 0f;
		image.GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}
	#endregion

	#region Getters
	public Team getTeam(int team){
		return currentTeams[team];
	}

	public Image getCapitalImage(){
		return capitalImage;
	}

	public Image getEndGameImage(){
		return endGameImage;
	}

	public int getNumberOfTeams(){
		return numberOfTeams;
	}

	public int getNumberOfRounds(){
		return numberOfRounds;
	}

	public InputField getInputField(){
		return inputField;
	}

	public Text getTeamPointsText(int team){
		return TeamPoints [team];
	}

	public Text getAttackingRandom(){
		return attackingRandom;
	}

	public Text getDefendingRandom(){
		return defendingRandom;
	}

	public Text getAttackingInput(){
		return attackingInput;
	}

	public Text getDefendingInput(){
		return defendingInput;
	}

	public Material getTeamMaterial(int i){
			return teamMaterials[i];
	}

	public GameObject getMap(){
		return Map;
	}

	#endregion

	#region Setters
	public void setGameState(GameState gameState){
		this.gameState = gameState;	
	}

	public void setTeamName(int team, string name){
		TeamNames [team].text = name; 
	}

	public void setAttackingRandom(string s){
		attackingRandom.text = s;
	}

	public void setDefendingRandom(string s){
		defendingRandom.text = s;
	}

	public void setAttackingInput(string s){
		attackingInput.text = s;
	}

	public void setDefendingInput(string s){
		defendingInput.text = s;
	}


	public void setArmiesText(int i, string s){
		teamTotalArmies [i].text = s;
	}


	#endregion
}
