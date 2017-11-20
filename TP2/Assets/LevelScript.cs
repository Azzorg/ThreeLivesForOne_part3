using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour {

	[SerializeField] Button niv1;
	[SerializeField] Button niv2;
	[SerializeField] Button niv3;
	[SerializeField] Button nivBoss;
	[SerializeField] Text affNiv1;
	[SerializeField] Text affNiv2;
	[SerializeField] Text affNiv3;
	[SerializeField] Text affNivBoss;
	[SerializeField] Text affichage;
	[SerializeField] Sprite doorOpened;
	[SerializeField] Sprite doorClosed;


	int nbLevDone = 0;


	// Use this for initialization
	void Start () {
		nivBoss.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		nbLevDone = 0;
		if (niv1Script.isDone) {
			nbLevDone++;
			niv1.GetComponent<Image> ().sprite = doorOpened;
			affNiv1.color = Color.green;
		}
		if (niv2Script.isDone) {
			nbLevDone++;
			niv2.GetComponent<Image> ().sprite = doorOpened;
			affNiv2.color = Color.green;
		}
		if (niv3Script.isDone) {
			nbLevDone++;
			niv3.GetComponent<Image> ().sprite = doorOpened;
			affNiv3.color = Color.green;
		}
		if (nivBossScript.isDone) {
			nivBoss.GetComponent<Image> ().sprite = doorOpened;
			affNivBoss.color = Color.green;
		}

		if (nbLevDone < 2) { 
			affichage.color = Color.red;
			affichage.text = "Vous devez encore effectuer " + (2 - nbLevDone) + " niveau(x) avant de pouvoir faire le boss";
		} else {
			nivBoss.enabled = true;
			affichage.color = Color.green;
			affichage.text = "Vous pouvez affronter le boss !!!!";
		}
	}

	//Lancer le niveau 1
	public void niv1OnClick(){
		Debug.Log("niv1 Lancé");
		Debug.Log("Nb level done : " + nbLevDone);
		Application.LoadLevel ("gym_accesBoss_level1");
	}

	//Lancer le niveau 2
	public void niv2OnClick(){
		Debug.Log("niv2 Lancé");
		Debug.Log("Nb level done : " + nbLevDone);
		Application.LoadLevel ("gym_accesBoss_level2");
	}

	//Lancer le niveau 3
	public void niv3OnClick(){
		Debug.Log("niv3 Lancé");
		Debug.Log("Nb level done : " + nbLevDone);
		Application.LoadLevel ("gym_accesBoss_level3");
	}

	//Lancer le niveau boss
	public void nivBossOnClick(){
		if (nbLevDone < 2) {
			Debug.Log ("Nb level done : " + nbLevDone);
			Debug.Log ("Le nombre de niveau fini n'est pas suffisant");
		} else {
			Debug.Log("Nb level done : " + nbLevDone);
			Debug.Log ("nivBoss Lancé");
			Application.LoadLevel ("gym_accesBoss_levelBoss");
		}
	}

}
