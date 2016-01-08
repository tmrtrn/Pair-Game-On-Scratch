using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(LevelDesign))]
public class LevelDesignEditor : Editor {

	LevelProfile profile;
	LevelDesign design;

	Rect rect;
	enum EditMode {Slot, Chip, PowerUp, Generator};
	EditMode currentMode = EditMode.Slot;
	AnimBool parametersFade = new AnimBool(true);
	string toolID = "";
	Vector2 teleportID = -Vector2.right;

	static int cellSize = 40;
	static int slotOffect = 4;

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

			profile.target = (FieldTarget) EditorGUILayout.EnumPopup ("Target", profile.target);

			EditorGUILayout.EndFadeGroup();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, GUILayout.ExpandWidth(true));

		defaultColor = GUI.color;
		GUI.color = currentMode == EditMode.Slot ? unpressedColor : defaultColor;
		if (GUILayout.Button("Slot", EditorStyles.toolbarButton, GUILayout.Width(40)))
			currentMode = EditMode.Slot;
		GUI.color = currentMode == EditMode.Chip ? unpressedColor : defaultColor;
		if (GUILayout.Button("Chip", EditorStyles.toolbarButton, GUILayout.Width(40)))
			currentMode = EditMode.Chip;
		GUI.color = currentMode == EditMode.PowerUp ? unpressedColor : defaultColor;
		if (GUILayout.Button("PowerUp", EditorStyles.toolbarButton, GUILayout.Width(70)))
			currentMode = EditMode.PowerUp;

		GUI.color = defaultColor;
		
		GUILayout.FlexibleSpace ();
		
		if (GUILayout.Button ("Reset", EditorStyles.toolbarButton, GUILayout.Width (40)))
			profile = new LevelProfile ();
		
		EditorGUILayout.EndVertical ();

		// Slot modes
		if (currentMode == EditMode.Slot) {
			EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			
			defaultColor = GUI.color;
			
			GUI.color = toolID == "Slots" ? unpressedColor : defaultColor;
			if (GUILayout.Button("Slots", EditorStyles.toolbarButton, GUILayout.Width(40)))
				toolID = "Slots";
			
			GUI.color = toolID == "Generators" ? unpressedColor : defaultColor;
			if (GUILayout.Button("Generators", EditorStyles.toolbarButton, GUILayout.Width(70)))
				toolID = "Generators";
			
			GUI.color = toolID == "Teleports" ? unpressedColor : defaultColor;
			if (GUILayout.Button("Teleports", EditorStyles.toolbarButton, GUILayout.Width(70)))
				toolID = "Teleports";

			
			GUI.color = defaultColor;		
			GUILayout.FlexibleSpace ();
			
			EditorGUILayout.EndHorizontal ();
		}

		// Chip modes
		if (currentMode == EditMode.Chip) {
			EditorGUILayout.BeginHorizontal (EditorStyles.toolbar, GUILayout.ExpandWidth(true));
			
			string  key;
			defaultColor = GUI.color;
			
			GUI.color = toolID == "Random" ? unpressedColor : defaultColor;
			if (GUILayout.Button("Random", EditorStyles.toolbarButton, GUILayout.Width(50)))
				toolID = "Random";
			
			for (int i = 0; i < profile.chipCount; i++) {
				key = "Color " + alphabet[i];
				GUI.color = toolID == key ? unpressedColor * chipColor[i] : defaultColor * chipColor[i];
				if (GUILayout.Button(key, EditorStyles.toolbarButton, GUILayout.Width(50)))
					toolID = key;
			}
			
			GUI.color = toolID == "Stone" ? unpressedColor : defaultColor;
			if (GUILayout.Button("Stone", EditorStyles.toolbarButton, GUILayout.Width(40)))
				toolID = "Stone";
			
			GUI.color = defaultColor;		
			GUILayout.FlexibleSpace ();
			
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.BeginVertical (EditorStyles.inspectorDefaultMargins);

		
		rect = GUILayoutUtility.GetRect (profile.width * (cellSize + slotOffect), profile.height * (cellSize + slotOffect));
		rect.x += slotOffect; 
		rect.y += slotOffect;
		
		EditorGUILayout.BeginHorizontal ();
		DrawModeTools ();
		EditorGUILayout.EndHorizontal ();
		
		EditorGUILayout.EndVertical ();
		
		switch (currentMode) {
		case EditMode.Slot: DrawSlot(); break;
	//	case EditMode.Chip: DrawChip(); break;
	//	case EditMode.PowerUp: DrawPowerUp(); break;
		}
		
		design.profile = profile; 
		EditorUtility.SetDirty (design);
	}

	bool DrawSlotButton(int x, int y, Rect r, LevelProfile lp)
	{
		defaultColor = GUI.backgroundColor;
		Color color = Color.white;
		string label = "";
		bool btn = false;
		int chip = lp.GetChip (x, y);
		if (!lp.GetSlot (x, y))
			color *= 0;

		GUI.backgroundColor = color;
		btn = GUI.Button(new Rect(r.xMin + x * (cellSize + slotOffect), r.yMin + y * (cellSize + slotOffect), cellSize, cellSize), label, slotStyle);

		float cursor = -2;

		if (lp.GetSlot(x, y) && lp.GetGenerator (x, y)) {
			GUI.backgroundColor = Color.black;
			GUI.Box(new Rect(r.xMin + x * (cellSize + slotOffect) + cursor, r.yMin + y * (cellSize + slotOffect) - 2, 10, 10), "G", iconStyle);
			cursor += 10 + 2;
		}
		if (lp.GetSlot(x, y) && lp.GetTeleport (x, y) > 0) {
			GUI.backgroundColor = Color.black;
			GUI.Box(new Rect(r.xMin + x * (cellSize + slotOffect) + cursor, r.yMin + y * (cellSize + slotOffect) - 2, cellSize - 12, 10), "T:" + lp.GetTeleport (x, y).ToString(), iconStyle);
		}
		
		if (lp.GetSlot (x, y)) {
			GUI.backgroundColor = transparentColor;
			GUI.Box (new Rect (r.xMin + x * (cellSize + slotOffect), r.yMin + y * (cellSize + slotOffect) + cellSize - 10, 20, 10), (y * 12 + x + 1).ToString (), slotStyle);
		}
		
		GUI.backgroundColor = defaultColor;

		return btn;
	}



	void DrawSlot () {
		for (int x = 0; x < profile.width; x++) {
			for (int y = 0; y < profile.height; y++) {
//				if (teleportID != -Vector2.right) {
//					if (DrawSlotButtonTeleport(x, y, rect, profile)) {
//						if (x == teleportID.x && y == teleportID.y)
//							profile.SetTeleport(Mathf.CeilToInt(teleportID.x), Mathf.CeilToInt(teleportID.y), 0);
//						else
//							profile.SetTeleport(Mathf.CeilToInt(teleportID.x), Mathf.CeilToInt(teleportID.y), y * 12 + x + 1);
//						teleportID = -Vector2.right;
//					}
//					continue;
//				}
				
				
				if (DrawSlotButton(x, y, rect, profile)) {
					switch (toolID) {
					case "Slots": 
						profile.SetSlot(x, y, !profile.GetSlot(x,y));
						break;
					case "Generators": 
						profile.SetGenerator(x, y, !profile.GetGenerator(x,y));
						break;
					case "Teleports": 
						teleportID = new Vector2(x, y);
						break;
						
					}
				}
			}
		}
	}


	void DrawModeTools ()
	{
		switch (currentMode) {
		case EditMode.Slot:
			if (GUILayout.Button("Reset", GUILayout.Width(70))) 
				ResetSlots();		
			break;
		case EditMode.Chip:
			if (GUILayout.Button("Clear", GUILayout.Width(50))) 
				SetAllChips(-1);
			if (GUILayout.Button("Randomize", GUILayout.Width(90))) 
				SetAllChips(0);
			break;
		case EditMode.PowerUp:
			if (GUILayout.Button("Clear", GUILayout.Width(50))) 
				PowerUpClear();
			break;
		}
	}


	void ResetSlots () {
		
		if (toolID == "Slots")
			for (int x = 0; x < 12; x++)
				for (int y = 0; y < 12; y++)
					profile.SetSlot(x, y, true);
		
		if (toolID == "Generators")
			for (int x = 0; x < 12; x++)
				for (int y = 0; y < 12; y++)
					profile.SetGenerator(x, y, y == 0);
		
		if (toolID == "Teleports")
			for (int x = 0; x < 12; x++)
				for (int y = 0; y < 12; y++)
					profile.SetTeleport(x, y, 0);

	}

	void SetAllChips (int c) {
		for (int x = 0; x < 12; x++)
			for (int y = 0; y < 12; y++)
				profile.SetChip(x, y, c);
	}

	void PowerUpClear ()
	{
		for (int x = 0; x < 12; x++)
			for (int y = 0; y < 12; y++)
				profile.SetPowerup(x, y, 0);
	}



}












