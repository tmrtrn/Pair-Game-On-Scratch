using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelDesign : MonoBehaviour {

	[SerializeField]
	public LevelProfile profile; // Container with level information

	public static Dictionary<int, LevelDesign> all = new Dictionary<int, LevelDesign> (); // Dictionary of all levels. int - the number of levels, LevelDesign 

	private Text label;

	void Awake()
	{
		Button btn = GetComponent<Button> ();
		profile.level = GetNumber ();
		all.Add (profile.level, this);
		if (btn != null)
			btn.onClick.AddListener(() => OnClick());
		label = GetComponentInChildren<Text> ();
		gameObject.name = "LEVEL "+ GetNumber ().ToString ();
		label.text = gameObject.name;
	}

	public void OnClick () {
		if (CPanel.uiAnimation > 0) return; 
		LoadLevel ();
	}

	// Play this level
	public void LoadLevel () {
		LevelProfile.main = profile;
		FieldAssistant.main.CreateField ();
		SessionAssistant.main.StartSession (LevelProfile.main.target, LevelProfile.main.limitation);
		FindObjectOfType<FieldCamera>().ShowField();
		UIServer.main.ShowPage ("Field");
	}

	// determination the level number
	public int GetNumber () {
		return transform.GetSiblingIndex () + 1;
	}

}

[System.Serializable]
public class LevelProfile {
	public static LevelProfile main; //current level
	const int maxSize = 12; // maximal playing field size

	public int levelId = 0; // Level Id
	public int level = 0; //Level number
	//field size
	public int width = 9;
	public int height = 9;
	public int chipCount = 6; // count of chip colors
	public int targetColorCount = 30; //Count of target color in Color mode

	public int firstStarScore = 100; // number of score points needed to get a first stars
	public int secondStarScore = 200; // number of score points needed to get a second stars
	public int thirdStarScore = 300; // number of score points needed to get a third stars

	public Limitation limitation = Limitation.Time;
	// Session duration in time limitation mode = duration value (sec);
	public int duraction = 10;

	public FieldTarget target = FieldTarget.Score; // Playing rules
	// Target score in Score mode = firstStarScore;
	
	// Slot
	public bool[] slot = new bool[maxSize * maxSize];
	public bool GetSlot(int x, int y) {
		return slot [y * maxSize + x];
	}
	public void SetSlot(int x, int y, bool v) {
		slot[y * maxSize + x] = v;
	}
	
	// Generators
	public bool[] generator = new bool[maxSize * maxSize];
	public bool GetGenerator(int x, int y) {
		return generator [y * maxSize + x];
	}
	public void SetGenerator(int x, int y, bool v) {
		generator[y * maxSize + x] = v;
	}
	
	// Teleports
	public int[] teleport = new int[maxSize * maxSize];
	public int GetTeleport(int x, int y) {
		return teleport [y * maxSize + x];
	}
	public void SetTeleport(int x, int y, int v) {
		teleport[y * maxSize + x] = v;
	}

	// Chip
	public int[] chip = new int[maxSize * maxSize];
	public int GetChip(int x, int y) {
		return chip [y * maxSize + x];
	}
	public void SetChip(int x, int y, int v) {
		chip[y * maxSize + x] = v;
	}

	// Powerup
	public int[] powerup = new int[maxSize * maxSize];
	public int GetPowerup(int x, int y) {
		return powerup [y * maxSize + x];
	}
	public void SetPowerup(int x, int y, int v) {
		powerup[y * maxSize + x] = v;
	}
	
	public LevelProfile GetClone() {
		LevelProfile clone = new LevelProfile ();
		clone.level = level;

		clone.width = width;
		clone.height = height;
		clone.chipCount = chipCount;

		clone.target = target;

		clone.firstStarScore = firstStarScore;
		clone.secondStarScore = secondStarScore;
		clone.thirdStarScore = thirdStarScore;

		clone.duraction = duraction;

		return clone;
	}
}
