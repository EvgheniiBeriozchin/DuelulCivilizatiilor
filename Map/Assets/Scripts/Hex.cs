using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex : MonoBehaviour   {

	public int column;
	public int row;
	public int team = -2;
	public int armies = 2;
	public int numberOfChildren = 2;
	public int positionInTheTeam;
	public Hexagon hexagon;
	public GameObject territory;



	public void addArmies(int nr){
		Debug.Log (armies + " " + nr);
		armies += nr;
		Debug.Log (armies);
		TextMesh tm = territory.GetComponentsInChildren<TextMesh> () [1];
		Debug.Log ("armies: " + armies + " " + tm.text);
		string s = "*";
		for (int i = 0; i < armies - 1; i++) {
			s = s + " *";
			Debug.Log ("armies: " + armies + " " + s);
		}
		tm.text = s;
	}

	public int getArmies(){
		return armies;
	}

	public void setArmies(int nr){
		Debug.Log (armies + " " + nr);
		armies = nr;
		Debug.Log (armies);
		TextMesh tm = territory.GetComponentsInChildren<TextMesh> () [1];
		Debug.Log ("armies: " + armies + " " + tm.text);
		string s = "*";
		for (int i = 0; i < armies - 1; i++) {
			s = s + " *";
			Debug.Log ("armies: " + armies + " " + s);
		}
		tm.text = s;
	}
	


}
