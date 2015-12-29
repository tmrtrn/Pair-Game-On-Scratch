using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(LevelDesign))]
public class LevelDesignEditor : Editor {

	LevelProfile profile;
	LevelDesign design;

	Rect rect;
	enum EditMode {Slot, Chip, PowerUp, Block, Generator, Wall};
	EditMode currentMode = EditMode.Slot;
	AnimBool parametersFade = new AnimBool(true);
	string toolId = "";
	Vector2 teleportId = -Vector2.right;

	static int cellSize = 40;

	static Color defaultColor;
	static Color transparentColor = new Color (0, 0, 0, 0f);
	static Color unpressedColor = new Color (0.7f, 0.7f, 0.7f, 1);
	static Color[] chipColor = {new Color(1,0.6f,0.6f,1),
		new Color(0.6f,1,0.6f,1),
		new Color(0.6f,0.8f,1,1),
		new Color(1,1,0.6f,1),
		new Color(1,0.6f,1,1),
		new Color(1,0.8f,0.6f,1)};
	static Color stoneColor = new Color(0.5f,0.5f,0.5f,1);
	static string[] alphabet = {"A", "B", "C", "D", "E", "F"};

	static GUIStyle mSlotStyle;
	static GUIStyle slotStyle {
		get{
			if(mSlotStyle == null){
				mSlotStyle = new GUIStyle(GUI.skin.button);
				mSlotStyle.wordWrap = true;

				mSlotStyle.normal.background = Texture2D.whiteTexture;
				mSlotStyle.focused.background = mSlotStyle.normal.background;
				mSlotStyle.active.background = mSlotStyle.normal.background;

				mSlotStyle.normal.textColor = Color.black;
				mSlotStyle.focused.textColor = mSlotStyle.normal.textColor;
				mSlotStyle.active.textColor = mSlotStyle.normal.textColor;

				mSlotStyle.fontSize = 8;
				mSlotStyle.margin = new RectOffset ();
				mSlotStyle.padding = new RectOffset ();
			}
			return mSlotStyle;
		}
	}

	static GUIStyle mIconStyle;
	static GUIStyle iconStyle {
		get {
			if (mIconStyle == null) {
				mIconStyle = new GUIStyle (GUI.skin.button);
				mIconStyle.wordWrap = true;
				
				mIconStyle.normal.background = Texture2D.whiteTexture;
				
				mIconStyle.normal.textColor = Color.white;
				
				mIconStyle.fontSize = 8;
				
				mIconStyle.border = new RectOffset ();
				mIconStyle.margin = new RectOffset ();
				mIconStyle.padding = new RectOffset ();
			}
			return mIconStyle;
		}
	}

	public LevelDesignEditor () {
		parametersFade.valueChanged.AddListener (Repaint);
	}

	override public bool UseDefaultMargins() {
		return false;
	}

	public override void OnInspectorGUI () {
		design = (LevelDesign)target;
		profile = design.profile;

		if (profile.levelId == 0 || profile.levelId != target.GetInstanceID ()) {
			if(profile.levelId != target.GetInstanceID())
				profile = profile.GetClone();
			profile.levelId = target.GetInstanceID();
		}

		design.name = (design.transform.GetSiblingIndex () + 1).ToString();

		parametersFade.target = GUILayout.Toggle(parametersFade.target, "Level Parameters", EditorStyles.foldout);

		if (EditorGUILayout.BeginFadeGroup (parametersFade.faded)) {
			profile.width = Mathf.RoundToInt(EditorGUILayout.Slider("Width",1f * profile.width,5f,12f));
			profile.height = Mathf.RoundToInt (EditorGUILayout.Slider ("Height", 1f * profile.height, 5f, 12f));
			profile.chipCount = Mathf.RoundToInt (EditorGUILayout.Slider ("Count of Possible Colors", 1f * profile.chipCount, 3f, 6f));

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Score Stars", GUILayout.ExpandWidth(true));
			profile.firstStarScore = Mathf.Max(EditorGUILayout.IntField (profile.firstStarScore, GUILayout.ExpandWidth(true)), 1);
			profile.secondStarScore = Mathf.Max(EditorGUILayout.IntField (profile.secondStarScore, GUILayout.ExpandWidth(true)), profile.firstStarScore+1);
			profile.thirdStarScore = Mathf.Max(EditorGUILayout.IntField (profile.thirdStarScore, GUILayout.ExpandWidth(true)), profile.secondStarScore+1);
			EditorGUILayout.EndHorizontal ();

			profile.limitation = (Limitation) EditorGUILayout.EnumPopup ("Limitation", profile.limitation);
			switch (profile.limitation) {
			case Limitation.Time:
				profile.duraction = Mathf.Max(0, EditorGUILayout.IntField("Session duration", profile.duraction));
				break;
			}
			EditorGUILayout.EndFadeGroup();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, GUILayout.ExpandWidth(true));

	}
}












