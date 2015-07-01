using UnityEngine;
using System.Collections;

public class PlayerInfoController : MonoBehaviour {
	public static PlayerInfoController Instance;
	public string playerName="Player Name";
	public string testTime="1";
	float rateW=0;
	float rateH=0;
	int fontsize=30;

	void Awake(){
		Instance = this;
	}

	void Start(){
		rateW = (Screen.width*1.0f)/ 800;
		rateH = (Screen.height * 1.0f) / 600;
		fontsize = (int)(30 * rateH);
		playerName = "Player Name";
		testTime="1";
	}

	void OnGUI(){
		GUI.skin.textField.fontSize =fontsize;
		GUI.skin.textField.alignment=TextAnchor.MiddleCenter;
		playerName = GUI.TextField(new Rect(240*rateW,520*rateH,200*rateW,50*rateH),playerName,15);//15为最大字符串长度 
		testTime = GUI.TextField(new Rect(480*rateW,520*rateH,80*rateW,50*rateH),testTime,2);//15为最大字符串长度 
	}
}
