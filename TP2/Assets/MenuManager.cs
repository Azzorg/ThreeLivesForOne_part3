using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /* Lance la scène de niveau de test */
    public void onClickLevel()
    {
        Application.LoadLevel("gym_competences_level");
    }

    /* Lance la scène d'arbre de talents */
    public void onClickSkill()
    {
        Application.LoadLevel("gym_competences_arbre");
    }
}
