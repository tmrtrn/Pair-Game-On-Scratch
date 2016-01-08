using UnityEngine;
using System.Collections;

public class SessionAssistant : MonoBehaviour {

	public static SessionAssistant main;

	void  Awake (){
		main = this;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Starting a new game session
	public void StartSession(FieldTarget sessionType, Limitation limitationType) {
	}
}
