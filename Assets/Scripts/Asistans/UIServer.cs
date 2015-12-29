using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIServer : MonoBehaviour {

	public static UIServer main;

	public string defaultPage; // name of starting page
	public Page[] pages; // Page list

	private Dictionary<string, CPanel> dPanels = new Dictionary<string, CPanel>(); // Dictionary panels. It is formed automatically from the child objects
	private Dictionary<string, Dictionary<string, bool>> dPages = new Dictionary<string, Dictionary<string, bool>>(); // Dictionary pages. It is based on an array of "pages"
	
	private string currentPage; // Current page name
	private string previousPage; // Previous page name

	void Start () {
		ArraysConvertation(); // filling dictionaries
		ShowPage (defaultPage); // Showing of starting page
	}
	
	void Awake () {
		main = this;
	}

	//filling dictionaries
	void ArraysConvertation()
	{
		foreach (CPanel gm in GetComponentsInChildren<CPanel>(true)) {
			dPanels.Add(gm.name, gm);
		}
		// filling pages dictionary of "pages" arrays element
		foreach (Page pg in pages) {
			Dictionary<string, bool> p = new Dictionary<string, bool>();
			for (int i = 0; i < pg.panels.Length; i++)
				p.Add(pg.panels[i], true);
			dPages.Add(pg.name, p);
		}
	}

	// displaying a page with a given name
	public void ShowPage (string p) {
		if (CPanel.uiAnimation > 0) return;
		if (currentPage == p) return;
		previousPage = currentPage;
		currentPage = p;
		foreach (string key in dPanels.Keys) {
			if (dPages[p].ContainsKey("?" + key)) continue;
			dPanels[key].SetActive(dPages[p].ContainsKey(key));
		}
	}

	// Class information about the page
	[System.Serializable]
	public struct Page {
		public string name; // page name
		public string[] panels; // a list of names of panels in this page
	}
}
