using UnityEngine;
using System.Collections;

public class ResolutionListView : MonoBehaviour {
	public ArrayList listItems;
	Vector2 scrollPosition;
	public float listX=250f;
	public float listY=180f;
	public float listW=300f;
	public float listH=150f;
	float rateW=1.0f;
	float rateH=1.0f;
	public string resoluton;
	UpgradeLableController resolutionLable;
	void Start(){
		resolutionLable=GameObject.Find("resolutionlable").gameObject.GetComponent<UpgradeLableController>();
		resoluton = Screen.width + "x" + Screen.height;
		scrollPosition = Vector2.zero;
		listItems = new ArrayList ();
		Resolution[] resolutions = Screen.resolutions;
		foreach (Resolution res in resolutions) {
			string lable=res.width + "x" + res.height;
			listItems.Add(lable);
		}

		rateW = (Screen.width * 1.0f) / 800;
		rateH = (Screen.height * 1.0f) / 600;
		listW = listW * rateW;
		listH = listH * rateH;
		listX = (Screen.width - listW) / 2;
		listY = (listY * Screen.height) / 600;
	}
	void Update(){
		resolutionLable.text = resoluton;
	}
	void OnGUI () {
		GUILayout.BeginArea(new Rect(listX,listY, listW, listH), GUI.skin.window);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true); 
		GUILayout.BeginVertical(GUI.skin.box);
		
		foreach (string item in listItems)
		{
			if(GUILayout.Button(item, GUI.skin.box, GUILayout.ExpandWidth(true))){
				resoluton=item;
				//Debug.Log(resoluton);
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();

	}
}
