using UnityEngine;
using System.Collections;

public class PathNode : MonoBehaviour {
	//The fomer node
	public PathNode m_parent;
	//The next node
	public PathNode m_next;

	public void SetNext(PathNode node){
		if(m_next!=null){
			m_next.m_parent=null;
		}
		m_next=node;
		node.m_parent=this;
	}
	
	//debug icon
	void OnDrawGizmos(){
		Gizmos.DrawIcon(this.transform.position,"Node.tif");
	}
}
