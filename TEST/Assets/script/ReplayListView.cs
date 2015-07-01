using UnityEngine;
using System.Collections;
using System.IO;
public class ReplayListView : MonoBehaviour {

	public ArrayList listItems;
	Vector2 scrollPosition;
	float listX=250f;
	float listY=180f;
	float listW=300f;
	float listH=280f;
	float rateW=1.0f;
	float rateH=1.0f;
	public string filename;
	UpgradeLableController ReplayNameLable;
	void Start(){
		ReplayNameLable=GameObject.Find("ReplayName").gameObject.GetComponent<UpgradeLableController>();

		scrollPosition = Vector2.zero;
		listItems = new ArrayList ();
		//get the filename
		//Debug.Log (Application.persistentDataPath);
		DirectoryInfo theFolder = new DirectoryInfo(Application.persistentDataPath);
		FileInfo[] fileInfo = theFolder.GetFiles();
		foreach (FileInfo NextFile in fileInfo)  //遍历文件
		{
			string name=NextFile.Name;
			//Debug.Log(name);
			string check=name.Substring(0,2);
			if(check=="Op"){
				listItems.Add(name);
			}
		}
		if (listItems.Count == 0) {
			filename = "No Replay File";
			GameObject.Find("Play").SetActive(false);
		} else {
			filename = (string)listItems [0];
		}

		rateW = (Screen.width * 1.0f) / 800;
		rateH = (Screen.height * 1.0f) / 600;
		listW = listW * rateW;
		listH = listH * rateH;
		listX = (Screen.width - listW) / 2;
		listY = (listY * Screen.height) / 600;
	}
	void Update(){
		ReplayNameLable.text = filename;
	}
	void OnGUI () {
		GUILayout.BeginArea(new Rect(listX,listY, listW, listH), GUI.skin.window);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true); 
		GUILayout.BeginVertical(GUI.skin.box);
		
		foreach (string item in listItems)
		{
			if(GUILayout.Button(item, GUI.skin.box, GUILayout.ExpandWidth(true))){
				filename=item;
				//Debug.Log(resoluton);
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
	}
}
