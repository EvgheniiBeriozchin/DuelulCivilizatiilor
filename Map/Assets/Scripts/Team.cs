using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team  {


	private string teamName;
	private int additionalPoints = 0;
	private int armies;
	private List<GameObject> teamTerritories = new List<GameObject>();

	public Team(int nr){
		armies = (nr + 1) * 2;
	}

	public void appendTerritory(GameObject territory){
		teamTerritories.Add (territory);
	}

	public void removeTerritory(int index){
		teamTerritories.RemoveAt(index);
	}

	public void addArmies(int a){
		armies += a;
	}

	public void setTeamName(string name){
		teamName = name;
	}

	public void setAdditionalPoints(int points){
		additionalPoints = points;
	}

	public int getNumberOfTeamTerritories(){
		return teamTerritories.Count;
	}

	public int getAdditionalPoints(){
		return additionalPoints;
	}

	public int getArmies(){
		return armies;
	}

	public GameObject getTerritory(int i){
		return teamTerritories [i];
	}
		

}
