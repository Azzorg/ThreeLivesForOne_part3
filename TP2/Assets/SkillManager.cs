using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillManager : MonoBehaviour
{

    static public int SkillPoints = 0;

    [SerializeField]
    public Text SkillPointsText;

    /* Boutons pour chaque skill débloquable */
    [SerializeField]
    public Button button_redPass1_1;
    [SerializeField]
    public Button button_redPass1_2;
    [SerializeField]
    public Button button_redPass1_3;
    [SerializeField]
    public Button button_redPass2_1;
    [SerializeField]
    public Button button_redPass2_2;
    [SerializeField]
    public Button button_redPass3_1;
    [SerializeField]
    public Button button_redPass3_2;
    [SerializeField]
    public Button button_redPass4_1;
    [SerializeField]
    public Button button_redAct1_1;

    // Etat de chaque niveau de skill
    public static bool redPass1_1 = false;
    public static bool redPass1_2 = false;
    public static bool redPass1_3 = false;
    public static bool redPass2_1 = false;
    public static bool redPass2_2 = false;
    public static bool redPass3_1 = false;
    public static bool redPass3_2 = false;
    public static bool redPass4_1 = false;
    public static bool redAct1_1 = false;


    //Red Skills
    public static float SpeedBoost = 1.0f; // Coefficient de vitesse
    public static float timeChargeBoost = 0; // Coefficient de durée chargement (non utilisé dans la démo Skill)
    public static int wallJumpBoost = 0; // Quantité de wallJumps faisables en plus (non utilisé dans la démo Skill)
    public static bool invisibility = false; // Capacité d'être invisible (non utilisé dans la démo Skill)
    public static bool grapnel = false; // Capacité à utiliser le grappin (non utilisé dans la démo Skill)



    // Use this for initialization
    void Start()
    {
        /* Désactivation des skills non activable pour le moment (les niveaux inférieurs n'ont pas encore été débloqués) */
        if(redPass1_1 == false)button_redPass1_2.GetComponent<Button>().interactable = false;
        if(redPass1_2 == false)button_redPass1_3.GetComponent<Button>().interactable = false;
        if(redPass2_1 == false)button_redPass2_2.GetComponent<Button>().interactable = false;
        if(redPass3_1 == false)button_redPass3_2.GetComponent<Button>().interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        SkillPointsText.text = SkillPoints.ToString();
    }

    /* OnClick sur le bouton du passif rouge 1 niveau 1 */
    public void onClick_redPassif1_1()
    {
        /* Si on a assez de points de Skill et que ce niveau n'est pas déjà débloqué */
        if(SkillPoints >= 300 && redPass1_1 == false)
        {
            SkillPoints -= 300;
            SpeedBoost = 1.25f;
            button_redPass1_2.GetComponent<Button>().interactable = true;
            redPass1_1 = true;
        }
    }

    /* OnClick sur le bouton du passif rouge 1 niveau 2 */
    public void onClick_redPassif1_2()
    {
        /* Si on a assez de points de Skill et que ce niveau n'est pas déjà débloqué */
        if (SkillPoints >= 500 && redPass1_2 == false)
        {
            SkillPoints -= 500;
            SpeedBoost = 1.5f;
            button_redPass1_3.GetComponent<Button>().interactable = true;
            redPass1_2 = true;
        }
    }

    /* OnClick sur le bouton du passif rouge 1 niveau 3*/
    public void onClick_redPassif1_3()
    {
        /* Si on a assez de points de Skill et que ce niveau n'est pas déjà débloqué */
        if (SkillPoints >= 800 && redPass1_3 == false)
        {
            SkillPoints -= 800;
            SpeedBoost = 1.75f;
            redPass1_3 = true;
        }
    }

    /* Onclick sur le bouton retournant au menu principal */
    public void onClick_returnMenu()
    {
        Application.LoadLevel("gym_competences_menu");
    }
}
