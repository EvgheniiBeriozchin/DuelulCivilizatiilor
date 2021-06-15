using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon {
	public Hexagon(int q, int r){
		this.Q = q;
		this.R = r;
		this.S = -(q + r);
	}

	public readonly int Q;
	public readonly int R;
	public readonly int S;

	public float Elevation;
	public float Moisture;

	static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt (3) / 2;

	public Vector3 Position(){

		float radius = 1f;
		float height = radius * 2;
		float width = height * WIDTH_MULTIPLIER;

		float vert = height * 0.75f;
		float horiz = width;
	
		return new Vector3 (
			horiz * (this.Q + this.R / 2f), 
			0, 
			vert * this.R
		);
	}

	public static float Distance(Hexagon a, Hexagon b){
	
		return Mathf.Max (
			Mathf.Abs(a.Q - b.Q),
			Mathf.Abs(a.R - b.R),
			Mathf.Abs(a.S - b.S)
		);
	}



}
