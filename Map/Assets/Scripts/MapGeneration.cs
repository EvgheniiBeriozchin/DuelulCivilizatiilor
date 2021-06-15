using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneration : MonoBehaviour {

	#region Variable Declarations
	public GameObject hexagon;
	public GameObject GameSystem;

	public int NumColumns = 15;
	public int NumRows = 10;
	public int maxTer = 40;
	public int numOfCapitals = 2;
	protected int numberOfAdjacentTerritories = 5;
	private int searchRadius = 5;

	private int numberOfTerrainHexes = 0;
	private int nr = 0;

	public Material[] Mats;
	public Material WaterMaterial;
	public Material LandMaterial;

	private Hexagon[,] hexes;
	protected Hexagon[] capitals;
	private List<Hexagon> terrainHexes;
	private List<GameObject>[] capitalTerritories;
	private Dictionary<Hexagon, GameObject> hexesGO;


	private bool[] terrainHexIsCapital;

	public Canvas MainMenu;
	public Canvas SecondMenu;

	#endregion

	public Hexagon GetHexagonAt(int q, int r){
		return hexes [q, r];
	}

	void Awake () {
		
		DontDestroyOnLoad (this.gameObject);

}

	virtual	public void GenerateMap(){
		foreach (Transform child in this.transform) {
			Destroy (child.gameObject);
		}
		numberOfTerrainHexes = 0;
		hexes = new Hexagon[NumColumns, NumRows];
		hexesGO = new Dictionary<Hexagon,GameObject>();
		terrainHexes = new List<Hexagon>();
		capitals = new Hexagon[numOfCapitals];
		capitalTerritories = new List<GameObject>[numOfCapitals];
		MainMenu.transform.GetComponent<CanvasGroup> ().alpha = 0;
		MainMenu.transform.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		SecondMenu.transform.GetComponent<CanvasGroup> ().alpha = 1;
		SecondMenu.transform.GetComponent<CanvasGroup> ().blocksRaycasts = true;
	
	
	// Map Generation
		for (int i = 0; i < NumColumns; i++) {

			for (int j = 0; j < NumRows; j++) {


				Hexagon hexagonScript = new Hexagon (i, j); 
				hexagonScript.Elevation = -0.5f;
				hexes [i, j] = hexagonScript;
				GameObject hex = (GameObject)Instantiate (hexagon, hexagonScript.Position (), Quaternion.identity);
				hex.GetComponentInChildren<Hex> ().territory = hex;
				hexesGO [hexagonScript] = hex;
				hexesGO [hexagonScript].GetComponentInChildren<Hex> ().hexagon = hexagonScript;
				hex.name = "Hex_" + i + "_" + j;
				// Parent hexGO to Map
				hex.transform.SetParent (this.transform);

				// Create array of HexObjects
				
				hex.GetComponentInChildren<Hex> ().row = j;
				hex.GetComponentInChildren<Hex> ().column = i;
				hex.GetComponentInChildren<TextMesh>().text = string.Format ("{0},{1}", i, j);

			//Set default material to the Hex, in our case it is water

		}
	}

		UpdateHexagonVisuals();
	//Optimization for Batches
	StaticBatchingUtility.Combine (this.gameObject); 
	
}

	public void UpdateHexagonVisuals(){
		for (int i = 0; i < NumColumns; i++) {
			for (int j = 0; j < NumRows; j++) {
				
				Hexagon h = GetHexagonAt (i, j);
				GameObject hGO = hexesGO [h];

				MeshRenderer mr = hGO.transform.GetComponentInChildren<MeshRenderer> ();
				if (h.Elevation >= 0) {
					mr.material = LandMaterial;
					terrainHexes.Add(h);
					numberOfTerrainHexes++;
					hexesGO [h].GetComponentInChildren<Hex> ().team = -1;
				} else {
					mr.material = WaterMaterial;
				
				}

			}
		}

	
	} 

	public void SetCapitals(){
			terrainHexIsCapital = new bool[numberOfTerrainHexes];
			for (int i = 0; i < numberOfTerrainHexes; i++) {
				terrainHexIsCapital [i] = false;
			}

			//Set Capitals randomly from the terrain hexes
			Hexagon[] neighbours;
			for (int i = 0; i < numOfCapitals; i++) {
				int j;
			bool nextToCapital = false;
				do {	
					j = Random.Range (0, numberOfTerrainHexes);
					neighbours = GetHexagonsWithinRadius(terrainHexes[j], 1);
					foreach (Hexagon k in neighbours){
						foreach (Hexagon h in capitals) {
						if (k == h)
							nextToCapital = true;
						} 	
					}
			} while(terrainHexIsCapital [j] || nextToCapital);
				terrainHexIsCapital[j] = true;
				capitals[i] = terrainHexes[j];
				hexesGO[capitals[i]].GetComponentInChildren<SpriteRenderer> ().enabled = true; 
				capitalTerritories [i] = setTerritoriesForCapital (hexesGO [capitals [i]]);		
			}
	}

	public List<GameObject> setTerritoriesForCapital(GameObject capital){
		List<GameObject> territories = new List<GameObject>();
		int territoriesDefined = 0;
		int i = 0;
		while (territoriesDefined < numberOfAdjacentTerritories && i < searchRadius) {
			i++;
			Hexagon[] candis = GetHexagonsWithinRadius (capital.GetComponentInChildren<Hex> ().hexagon, i);
			List<Hexagon> candidates = new List<Hexagon> ();
			for (int j = 0; j < candis.Length; j++) {
				candidates.Add (candis [j]);
			}
			while (territoriesDefined < numberOfAdjacentTerritories && candidates.Count > 0) {
				int j = Random.Range(0, candidates.Count);
				if (!isTerritoryCapital (candidates [j]) && hexesGO [candidates [j]].GetComponentInChildren<Hex> ().team == -1) {
					bool connected = false;
					int k = 0;
					Hexagon[] neighbours = GetHexagonsWithinRadius (capital.GetComponentInChildren<Hex> ().hexagon, 1);
					while (!connected && k < neighbours.Length) {
						connected = territories.Contains(hexesGO[neighbours[k]]) || hexesGO[neighbours[k]] == capital;
						k++;
					}
					if (connected) {
						territoriesDefined++;
						territories.Add (hexesGO [candidates [j]]);
						hexesGO [candidates [j]].GetComponentInChildren<Hex> ().team = -3;
					}
				}
				candidates.Remove (candidates[j]);
			}
		}

		if (territoriesDefined < numberOfAdjacentTerritories) {
			Debug.Log ("Not enough territories in radius");
			return null;
		}
		if (territoriesDefined > numberOfAdjacentTerritories) {
			List<GameObject> ters = new List<GameObject> ();
			for (int j = 0; j < numberOfAdjacentTerritories; j++)
				ters.Add (territories[j]);
			return ters;
		}

		return territories;
	}

	public Hexagon[] GetHexagonsWithinRadius(Hexagon centerHex, int radius){

		List<Hexagon> hexagonsWithinRadius = new List<Hexagon> ();

		for (int dx = -radius; dx <= radius; dx++) 
		{
			for (int dy = Mathf.Max(-radius, -dx-radius); dy <= Mathf.Min(radius, -dx+radius); dy++) 
			{
				if (centerHex.Q + dx >= 0 && centerHex.R + dy >= 0 && centerHex.Q + dx < NumColumns && centerHex.R + dy < NumRows)
				hexagonsWithinRadius.Add (hexes[centerHex.Q + dx, centerHex.R + dy]);
			

			}

		}
	
		return hexagonsWithinRadius.ToArray ();
	}

	public GameObject[] getGOWithinRadius(GameObject g, int radius){
		Hexagon[] hexagons = GetHexagonsWithinRadius (g.GetComponentInChildren<Hex> ().hexagon, radius);
		GameObject[] GOs = new GameObject[hexagons.Length];
		for (int i = 0; i < hexagons.Length; i++) {
			GOs [i] = hexesGO [hexagons [i]];
		}
		return GOs;
	}

	public List<GameObject> getAdjacentTerritories(GameObject territory){
		Hexagon h = territory.GetComponentInChildren<Hex> ().hexagon;
		for (int i = 0; i < numOfCapitals; i++)
			if (capitals [i] == h)
				return capitalTerritories [i];
		Debug.Log ("Territory is not capital");
		return null;
	}

	public GameObject GetGOFromHexagon(Hexagon h){
		return hexesGO [h];
	}

	public bool isTerritoryCapital(Hexagon h){
		for (int i = 0; i < numOfCapitals; i++) {
			if (capitals [i] == h) {
				return true;
			}
		}
		return false;
	}

	public void changeSettings(Image settings){
		string capitals = settings.transform.GetChild (0).transform.GetChild (2).GetComponent<Text>().text;
		string territories = settings.transform.GetChild (1).transform.GetChild (2).GetComponent<Text>().text;
		int capitalsNr, territoriesNr; 
		if (int.TryParse (capitals, out capitalsNr) && capitalsNr < 9 && capitalsNr > 0)
			numOfCapitals = capitalsNr;
		if (int.TryParse (territories, out territoriesNr) && territoriesNr >= 0) {
			numberOfAdjacentTerritories = territoriesNr;
			searchRadius = (int)(territoriesNr / 2) + 1;
		}
	}

	public int getNumberOfTeams(){
		return numOfCapitals;
	}

	public int getNumberOfAdjacentTerritories(){
		return numberOfAdjacentTerritories;
	}

	public List<int> getLocationsOfCapitals(){
		List<int> locations = new List<int> ();
		for (int i = 0; i < numOfCapitals; i++) {
			locations.Add(hexesGO [capitals [i]].GetComponentInChildren<Hex> ().team);
		}
		return locations;
	}

}
