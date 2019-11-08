using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JohnsonCodeHK.UIControllerExamples.CSS;

public class HeartPathFollower : MonoBehaviour {

	public Transform [] PathNode;
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
	bool isAnimStarted;

	// Use this for initialization
	public void StartHeartAnimation () {
		isAnimStarted = true;
		CurrentNode = 0;
		CheckNode ();

	}/// <summary>
	/// we will make a function to check current Node and move to it. by save the node position to CurrenPositionHolder
	/// </summary>
	/// 
	void CheckNode(){
		if (CurrentNode <= PathNode.Length - 1) {
			Timer = 0;
			CurrentPositionHolder = PathNode [CurrentNode].position;
			// we will hold the currentNode position to CurrenPosHolder.
			CurrentNode++;

		} else {
			isAnimStarted = false;
			iTween.ScaleTo (this.gameObject, new Vector3 (2, 2, 0), 1.0f);
			iTween.FadeTo (this.gameObject, 0.0f, 1.0f);
			Invoke ("DestroySelf", 1.0f);
		}
	}
	void DrawLine(){
		//for (int i = 0; i < PathNode.Length; i++) {
		//	//we will paint from PathNode[0] to 1 , 1 to 2 and like this to end of Pathnode
		//	if (i < PathNode.Length - 1) {
		//		Debug.DrawLine (PathNode [i].position, PathNode [i + 1].position, Color.green);
		//	} else {
		//		Debug.DrawLine (PathNode [i].position, PathNode [0].position, Color.green);
		//	}
		//}
	}
	// Update is called once per frame
	void Update () {
		if (isAnimStarted) {
			DrawLine ();
			//Debug.Log (CurrentNode);
			Timer = Time.deltaTime * MoveSpeed;
			//this will make the path moving

			float dist = Vector3.Distance(this.transform.position, CurrentPositionHolder);

			if (dist < 1) {
				//if player position not equal Node position we will move the player to node
				CheckNode ();

			} else {

                this.transform.position = Vector3.Lerp (this.transform.position, CurrentPositionHolder, Timer);
//				if (CurrentNode <= PathNode.Length - 1) {
//					//if it equal lthe node we will go next node
//					CurrentNode++;
//					//here 
//					CheckNode ();
//				}
			}
		}
	}
	void DestroySelf(){
		Destroy (this.gameObject);
	}
}
