using UnityEngine;
using System.Collections;

[System.Serializable]
//Use this to ensure that MapData class can be serialized in order to set initial value in Editor 
//Since MapData does not extend MonoBehaviour
public class MapData{
	public enum FieldTypeID{
		GuardPosition, //Can put defender
		CanNotStand    //Can't put defender
	}
	//usually can put defender
	public FieldTypeID fieldtype = FieldTypeID.GuardPosition;
}

public class GridNode : MonoBehaviour {
	public MapData _mapData;

	//show the icon on scene editor
	void OnDrawGizmos(){
		if (_mapData.fieldtype == MapData.FieldTypeID.CanNotStand) {
			Gizmos.DrawIcon (this.transform.position, "gridnode.tif");
		} else {
			Gizmos.DrawIcon (this.transform.position, "blue.png");
		}
	}
}
