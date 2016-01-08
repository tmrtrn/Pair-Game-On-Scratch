using UnityEngine;
using System.Collections;

public class FieldAssistant : MonoBehaviour {

	public static FieldAssistant main;

	void  Awake (){
		main = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Field generator
	public void  CreateField (){
	}
}

public enum Limitation {
	Time
}

public enum FieldTarget {
	Score = 0
}
