using UnityEngine;
using System.Collections;

public class GridMap : MonoBehaviour {
	static public GridMap Instance=null;
	//show the debug info?
	public bool m_debug=false;
	//the map size
	public int MapSizeX=14;
	public int MapSizeY=10;
	//the line layer
	public float layer = 1.5f;

	[HideInInspector]
	//2-demesion array to save the map info
	public MapData[,] m_map;

	float rate = 0;

	void Awake(){
		Instance = this;
		rate = bgController.Instance.rate;

		//initial the map info
		this.BuildMap ();
	}
	

	//Build Map
	[ContextMenu("BuildMap")]
	public void BuildMap(){
		if (rate <= 0) {
			rate=1.0f;
		}
		//create the array
		m_map = new MapData[MapSizeX, MapSizeY];
		  //init every element!!!
		for (int i=0; i<MapSizeX; i++) {
			for (int j = 0; j < MapSizeY; j++) {
				m_map[i,j]=new MapData();
			}
		}
		//get all gridnodes
		GameObject[] nodes=(GameObject[])GameObject.FindGameObjectsWithTag("gridnode");
		foreach (GameObject nodeobj in nodes) {
			//first get the new zero point's position
//			float newZeroX = (1 - rate) * this.transform.position.x;
//			Debug.Log ("New zero point position:" + newZeroX);
			//second get the new cell width
//			float newCellWidth = 1.0f * rate;
//			Debug.Log ("New cell width:" + newCellWidth);

			//third get the node position
			GridNode node=nodeobj.GetComponent<GridNode>();
			Vector3 nodePos = node.transform.position;
			//Debug.Log("Node position x:"+nodePos.x);

			//forth get the new index on X axis
//			float newIndexX=(nodePos.x-newZeroX)/newCellWidth;
//			Debug.Log("New index x:"+newIndexX);

//			float newX=(posIni.x+(rate-1.0f)*this.transform.position.x)/rate;
//			Debug.Log(newX);
//			int inewX=(int)(newX*10.0f+0.5f)/10;\

			Vector3 pos = new Vector3(nodePos.x,nodePos.y,0);
			//if the node beyond the scene, omit it
			if((int)pos.x>=MapSizeX||(int)pos.y>=MapSizeY){
				continue;
			}
			//set the type of node
			m_map[(int)pos.x,(int)pos.y].fieldtype=node._mapData.fieldtype;
		}
	}
	//Draw the map info
	void OnDrawGizmos(){
		if (!m_debug || m_map == null) {
			return;
		}
		if (rate <= 0) {
			rate=1.0f;
		}
		//the line color
		Gizmos.color = Color.blue;

		//Draw grid
		  //vertical
		for (int i=0; i<MapSizeX; i++) {
			float posI=i*rate-(rate-1.0f)*this.transform.position.x;
			Gizmos.DrawLine(new Vector3(posI,0,layer), new Vector3(posI,MapSizeY,layer));
		}
		  //horizontal
		for (int j=0; j<MapSizeY; j++) {
			Gizmos.DrawLine(new Vector3(0*rate-(rate-1.0f)*this.transform.position.x,j,layer), 
			                new Vector3(MapSizeX*rate-(rate-1.0f)*this.transform.position.x,j,layer));
		}

		//change color
		Gizmos.color = Color.red;
		for (int i=0; i<MapSizeX; i++) {
			for(int j=0; j< MapSizeY;j++){
				if(m_map[i,j].fieldtype==MapData.FieldTypeID.CanNotStand){
					Gizmos.color=new Color(1,0,0,0.5f);
					//Draw the red square
					float posIniX=i*rate-(rate-1.0f)*this.transform.position.x;
					Gizmos.DrawCube(new Vector3(posIniX+0.5f*rate,j+0.5f,layer),
					                new Vector3(1*rate,1,layer+0.1f));
				}
			}
		}
	}
}
