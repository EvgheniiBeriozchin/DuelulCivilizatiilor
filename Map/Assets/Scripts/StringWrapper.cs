using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringWrapper {

	private string s;

	public StringWrapper(string n){
		s = n;
	}

	public string getString(){
		return s;
	}

	public void setString(string s){
		this.s = s;
	}
}
