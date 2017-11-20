using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class chargeBarScript : MonoBehaviour {

    public static int chargePointsMax;
    public static int chargePoints;

    Image imageChargeBar;

	// Use this for initialization
	void Start () {
        imageChargeBar = GetComponent<Image>();
        chargePoints = 0;
        chargePointsMax = 500;
	}
	
	// Update is called once per frame
	void Update () {

        /* Mise à jour de la barre de chargement (taille fonction des points possedés) */
        float percentage = (float)((float)chargePoints /(float)chargePointsMax);
        imageChargeBar.fillAmount = percentage;
		
	}
}
