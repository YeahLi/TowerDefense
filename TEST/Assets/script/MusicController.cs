using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {
	AudioSource music;
	bool playing=false;
	// Use this for initialization
	void Start () {
		music = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Settings.Instance.music) {
			if(!playing){
				playing=true;
				music.GetComponent<AudioSource>().Play();
			}
		}else{
			music.GetComponent<AudioSource>().Stop();
			playing=false;
		}
	}
}
