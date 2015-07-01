using UnityEngine;
using System.Collections;

using UnityEditor;

public class PathTool : ScriptableObject {
	static PathNode m_parent=null;
	//Menu to setParent
	[MenuItem("PathTool/Set Parent %q")]
	static void SetParent(){
		//if not select any objects or select the number of object more than one, return
		if(!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length>1){
			Debug.LogError("Set Parent Failed! Only can select one object!");
			return;
		}
		//if the obj tag is pathnode
		if(Selection.activeGameObject.tag.CompareTo("pathnode")==0){
			//save this node
			m_parent=Selection.activeGameObject.GetComponent<PathNode>();
			Debug.Log("Set Parent Success");
		}else{
			Debug.LogError("Set Parent Failed");
		}
	}
	//Menu to set Next
	[MenuItem("PathTool/Set Next %w")]
	static void SetNextChild(){
		//if not select any objects or select the number of object more than one, return
		if(!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length>1 || m_parent==null){
			Debug.LogError("Set Next Failed!");
			return;
		}

		if(Selection.activeGameObject.tag.CompareTo("pathnode")==0){
			//save this node
			m_parent.SetNext(Selection.activeGameObject.GetComponent<PathNode>());
			m_parent=null;
			Debug.Log("Set Next Success");
		}

	}
}
