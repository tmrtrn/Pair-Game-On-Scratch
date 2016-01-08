using UnityEngine;
using System.Collections;

public class FieldCamera : MonoBehaviour {

	public static FieldCamera main;
	float defaultFOV = 2;

	void Awake() {
		main = this;
		transform.position = new Vector3 (0, 0.7f, -10);
		GetComponent<Camera> ().orthographicSize = defaultFOV;
	}

	public void ShowField()
	{

	}

}
