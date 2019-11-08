using UnityEngine;
using System.Collections;

public class PathFollower : MonoBehaviour {
	Node [] PathNode;
	 GameObject [] Player;
	//the object who move along the path.
	public float MoveSpeed;
	//the speed when moving along the path
	float Timer;
	//default time
	//so i forgot make a current to hold current node
	int CurrentNode;
	//this will hold current node 
	static Vector3 CurrentPositionHolder;
	//the vector3 hold Node position 

	// Use this for initialization
	void Start () {
		Player = GameObject.FindGameObjectsWithTag ("Player");
		PathNode = GetComponentsInChildren<Node> ();
		CheckNode ();
	
	}/// <summary>
	/// we will make a function to check current Node and move to it. by save the node position to CurrenPositionHolder
	/// </summary>
	/// 
	void CheckNode(){
		if (CurrentNode < PathNode.Length - 1) {
			Timer = 0;
			CurrentPositionHolder = PathNode [CurrentNode].transform.position;
			// we will hold the currentNode position to CurrenPosHolder.
	
		} else {
			CurrentNode = 0;
			CurrentPositionHolder = PathNode [CurrentNode].transform.position;
		}
	}
	void DrawLine(){
		for (int i = 0; i < PathNode.Length; i++) {
		//we will paint from PathNode[0] to 1 , 1 to 2 and like this to end of Pathnode
			if (i < PathNode.Length - 1) {
				Debug.DrawLine (PathNode [i].transform.position, PathNode [i + 1].transform.position, Color.green);
			} else {
				Debug.DrawLine (PathNode [i].transform.position, PathNode [0].transform.position, Color.green);
			}
		}
	}
	// Update is called once per frame
	void Update () {
		DrawLine ();
		Debug.Log (CurrentNode);
		Timer += Time.deltaTime * MoveSpeed;
		//this will make the path moving
		foreach (GameObject g in Player) {
			if (g.transform.position != CurrentPositionHolder) {
				//if player position not equal Node position we will move the player to node
				g.transform.position = Vector3.Lerp (g.transform.position, CurrentPositionHolder, Timer);

			} else {
				if (CurrentNode < PathNode.Length - 1) {
					//if it equal lthe node we will go next node
					CurrentNode++;
					//here 
					CheckNode ();
				}
			}

		}
	}
}
