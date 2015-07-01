using UnityEngine;
using System.Collections;

public class starController : MonoBehaviour {
	GameObject firstStar;
	GameObject secondStar;
	GameObject thirdStar;
	// Use this for initialization
	void Start () {
		firstStar = transform.FindChild ("firstStar").gameObject;
		secondStar = transform.FindChild ("secondStar").gameObject;
		thirdStar = transform.FindChild ("thirdStar").gameObject;

		if (GameManager.Instance.m_result > GameManager.Instance.LowMarks) {
			firstStar.SetActive(true);
		}
		if (GameManager.Instance.m_result > GameManager.Instance.mediumMarks) {
			secondStar.SetActive(true);
		}
		if (GameManager.Instance.m_result > GameManager.Instance.highMarks) {
			thirdStar.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
