using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuFunctions : MonoBehaviour {
	public Image settings;
	public GameObject map;

	public void Exit(){
		Application.Quit ();
	}
	public void SetGame(){
	
	}
	public void StartGame(){
		SceneManager.LoadScene ("Map");
	}
	public void EnterSettings(){
		settings.GetComponent<CanvasGroup> ().alpha = 1f;
		settings.GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	public void ExitSettings(){
		settings.GetComponent<CanvasGroup> ().alpha = 0f;
		settings.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		map.GetComponent<MapGeneration_Continent> ().changeSettings (settings);
	}

}
