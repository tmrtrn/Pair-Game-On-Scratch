using UnityEngine;
using System.Collections;

public class LevelDesign : MonoBehaviour {

	[SerializeField]
	public LevelProfile profile; // Container with level information
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
	
	public LevelProfile GetClone() {
		LevelProfile clone = new LevelProfile ();
		clone.level = level;

		clone.width = width;
		clone.height = height;
		clone.chipCount = chipCount;

		clone.firstStarScore = firstStarScore;
		clone.secondStarScore = secondStarScore;
		clone.thirdStarScore = thirdStarScore;

		clone.duraction = duraction;

		return clone;
	}
}
