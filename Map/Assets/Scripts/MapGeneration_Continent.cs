using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration_Continent : MapGeneration {

	override public void GenerateMap(){
		base.GenerateMap ();

		int numContinents = 1;
		int continentSpacing = NumColumns / numContinents;

		//Do for all continents
		for (int c = 0; c < numContinents; c++) {
		//Generate a random amount of continent splats
		int NumSplats = Random.Range (5, 8);

		for (int i = 0; i < NumSplats; i++) {
			int radius = Random.Range (3,8);
			int y = Random.Range (radius, NumRows - radius - 1);
				int x = Random.Range (radius, radius + 10) + (c * continentSpacing);

			ElevateArea (x, y, radius);
		}
		}



		//Add Perlin Noise

		float noiseResolution = 0.1f;
		float noiseScale = 2f;
		Vector2 noiseOffset = new Vector2 (Random.Range(0f, 1f), Random.Range(0f, 1f) );

		for (int i = 0; i < NumColumns; i++) {
			for (int j = 0; j < NumRows; j++) {
				
				Hexagon h = GetHexagonAt (i, j);
				float noise = Mathf.PerlinNoise (
					(float) i / Mathf.Max(NumColumns, NumRows) / noiseResolution + noiseOffset.x, 
					(float) j / Mathf.Max(NumColumns, NumRows) / noiseResolution + noiseOffset.y) 
					- 0.5f;
			h.Elevation += noise * noiseScale;
			} 
			}

		UpdateHexagonVisuals ();
		SetCapitals();
		/*
		for (int i = 0; i < numOfCapitals; i++) {
			int connectedTerritories = 0;
			while (connectedTerritories < numberOfAdjacentTerritories) {
								
			
			}
		}
		*/
	}

	void ElevateArea(int q, int r, int radius){

		Hexagon centerHex = GetHexagonAt (q, r);

		Hexagon[] areaHexes = GetHexagonsWithinRadius(centerHex, radius);
		foreach (Hexagon h in areaHexes) 
		{
			h.Elevation += 1f * Mathf.Lerp(1f, 0.25f, Hexagon.Distance(centerHex, h) / radius);

		}
	
	
	}

}
