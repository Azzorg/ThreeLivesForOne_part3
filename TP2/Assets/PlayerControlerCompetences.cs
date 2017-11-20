using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControlerCompetences : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);


    // Déclaration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }
    int _ChargePoint { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

    [SerializeField]
    LayerMask WhatIsCapsules;

    [SerializeField]
    Text SkillPointsText;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
        _MainCamera = Camera.main;
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        _Grounded = false;
        _Flipped = false;
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed * SkillManager.SpeedBoost;
        HorizontalMove(horizontal);
        FlipCharacter(horizontal);
        CheckJump();
        SkillPointsText.text = SkillManager.SkillPoints.ToString();
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, horizontal);
        _Anim.SetFloat("MoveSpeed", Mathf.Abs(horizontal));
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);
                _Grounded = false;
                _Anim.SetBool("Grounded", false);
                _Anim.SetBool("Jump", true);
            }
        }
    }

    // Gère l'orientation du joueur et les ajustements de la camera
    void FlipCharacter(float horizontal)
    {
        if (horizontal < 0 && !_Flipped)
        {
            _Flipped = true;
            transform.Rotate(FlipRotation);
            _MainCamera.transform.Rotate(-FlipRotation);
            _MainCamera.transform.localPosition = InverseCameraPosition;
        }
        else if (horizontal > 0 && _Flipped)
        {
            _Flipped = false;
            transform.Rotate(-FlipRotation);
            _MainCamera.transform.Rotate(FlipRotation);
            _MainCamera.transform.localPosition = CameraPosition;
        }
    }


    // Collision avec le sol, capsules et fin de niveau
    void OnCollisionEnter(Collision coll)
    {
        /* Capsules Charge */
        if (coll.gameObject.layer == 12)
        {
            coll.gameObject.transform.position = new Vector3(-100, 0, 0);
            Destroy(coll.gameObject);
            _Rb.velocity = new Vector3(0, _Rb.velocity.y, _Rb.velocity.z);
            if (coll.gameObject.GetComponent<ChargeCapsuleScript>().hasBeenTriggered == false)
            {
                _ChargePoint += 100;
                coll.gameObject.GetComponent<ChargeCapsuleScript>().hasBeenTriggered = true;
            }

        }

        /* Capsules skill */
        if (coll.gameObject.layer == 9)
        {
            coll.gameObject.transform.position = new Vector3(-100, 0, 0);
            Destroy(coll.gameObject);
            _Rb.velocity = new Vector3(0, _Rb.velocity.y, _Rb.velocity.z);
            if (coll.gameObject.GetComponent<SkillCapsuleScript>().hasBeenTriggered == false)
            {
                SkillManager.SkillPoints += 200;
                coll.gameObject.GetComponent<SkillCapsuleScript>().hasBeenTriggered = true;
            }

        }

        //End level
        if (coll.gameObject.layer == 10)
        {
            coll.gameObject.transform.position = new Vector3(-100, 0, 0);
            Destroy(coll.gameObject);
            _Rb.velocity = new Vector3(0, _Rb.velocity.y, _Rb.velocity.z);
            if (coll.gameObject.GetComponent<SkillEndScript>().hasBeenTriggered == false)
            {
                SkillManager.SkillPoints += 600;
                coll.gameObject.GetComponent<SkillEndScript>().hasBeenTriggered = true;
            }
            Application.LoadLevel("gym_competences_menu");
        }


        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
            _Anim.SetBool("Grounded", _Grounded);
        }
    }

}
