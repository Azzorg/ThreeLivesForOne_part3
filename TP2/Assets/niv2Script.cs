using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class niv2Script : MonoBehaviour {

	public static bool isDone = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Le niveau a été fait
	public void hasBeenDone(){
		isDone = true;
	}
}
