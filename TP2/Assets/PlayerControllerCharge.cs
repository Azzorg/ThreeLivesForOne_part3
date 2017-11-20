using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControllerCharge : MonoBehaviour
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
    bool _ChargeReady { get; set; } // Si le sort chargé peut être utilise ==> true sinon ==> false
    bool _ChargeOnLoad { get; set; } // Si un chargement a été initié ==> true sinon ==> false
    Outline _BarChargeOutline { get; set; } // Contours de la barre de chargement
    float _TargetTime { get; set; } // Compteur de temps pour que le sort chargé soit prêt

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

    [SerializeField]
    Text ChargePointsText;

    [SerializeField]
    Image imageBarCharge;

    [SerializeField]
    int ChargeCost = 200;

    [SerializeField]
    float chargeDuration = 2.0f;

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
        _ChargeReady = false;
        _BarChargeOutline = imageBarCharge.GetComponentInChildren<Outline>();
        _BarChargeOutline.enabled = false;
        _TargetTime = chargeDuration;
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        if(_ChargeOnLoad == false) { HorizontalMove(horizontal); }
        FlipCharacter(horizontal);
        CheckJump();
        CheckChargeSpell();
        ChargePointsText.text = chargeBarScript.chargePoints.ToString(); // Indication du nombre de points de chargement en chiffre
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

    //Gère l'activation du sort chargé
    void CheckChargeSpell()
    {
        if (_Grounded)
        {
            /* Si la touche de chargement (ctrl) est appuyée et qu'on a assez de points de chargement */
            if (Input.GetKey(KeyCode.LeftControl) && chargeBarScript.chargePoints >= ChargeCost)
            {
                _TargetTime -= Time.deltaTime; // compteur du chargement
                _ChargeOnLoad = true; // indique qu'un chargement a lieu (désactive les mouvements horizontaux)
                _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, 0); // Stoppe le personnage
                _Anim.SetBool("Grounded", false);
                _Anim.SetBool("Jump", true);

                /* Si 2 sec ==> chargement pret */
                if (_TargetTime <= 0 && _ChargeReady == false)
                {
                    _ChargeReady = true;
                }
                /* Affiche que le chargement est pret en mettant un contour à la barre de chargement */
                if (_ChargeReady == true) { _BarChargeOutline.enabled = true; }
                else { _BarChargeOutline.enabled = false; }
            }
            /* Si Ctrl n'est pas appuyé mais qu'un chargement est pret alors on fait le sort chargé */
            else if (!Input.GetKey(KeyCode.LeftControl) && _ChargeReady == true)
            {
                chargeBarScript.chargePoints -= ChargeCost; // retire 200 pts de chargement
                _ChargeReady = false; // la charge a été utilisé ==> false
                _TargetTime = chargeDuration; // remise à la valeur initiale du compteur de chargement
                _ChargeOnLoad = false; // Un chargement n'est plus en train d'être éffectué
                _BarChargeOutline.enabled = false; // On enlève les contours de la barre de chargement

                /* Gestion du saut chargé */
                _Rb.AddForce(new Vector3(0, JumpForce * 1.3f, 0), ForceMode.Impulse); // Ajout d'une grande force de saut (sort chargé)
                _Grounded = false;
                _Anim.SetBool("Grounded", false);
                _Anim.SetBool("Jump", true);
            }
            /* Si Ctrl n'est pas appuyé et qu'un chargement n'est pas pret*/
            else
            {
                _ChargeOnLoad = false;
                _TargetTime = chargeDuration;
                _Anim.SetBool("Grounded", true);
                _Anim.SetBool("Jump", false);
            }
        }
        else { _ChargeOnLoad = false; }
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


    // Collision avec le sol et capsules de chargement
    void OnCollisionEnter(Collision coll)
    {
        /* Contact avec une capsules de chargement */
        if (coll.gameObject.layer == 12)
        {
            coll.gameObject.transform.position = new Vector3(-100, 0, 0); // déplace l'objet dans un autre plan pour éviter les changements de trajectoires du joueur
            Destroy(coll.gameObject); // Destruction de l'objet
            _Rb.velocity = new Vector3(0, _Rb.velocity.y, _Rb.velocity.z); // Enleve tout vitesse dans l'axe x due à la collision pour éviter les changements de trajectoires du joueur

            /* Si la capsule n'a pas déja été collisionnée (empêche les mutliples collisions) */
            if (coll.gameObject.GetComponent<ChargeCapsuleScript>().hasBeenTriggered == false)
            {
                /* Ajout de 100 pts de chargement */
                if (chargeBarScript.chargePoints < chargeBarScript.chargePointsMax - 100)
                {
                    Debug.Log("+100 < 400");
                    chargeBarScript.chargePoints += 100;
                    coll.gameObject.GetComponent<ChargeCapsuleScript>().hasBeenTriggered = true;
                }
                /* Ajout jusqu'au maximum de pts de chargement */
                else if (chargeBarScript.chargePoints < chargeBarScript.chargePointsMax)
                {
                    Debug.Log("+100 < 500");
                    chargeBarScript.chargePoints = chargeBarScript.chargePointsMax;
                    coll.gameObject.GetComponent<ChargeCapsuleScript>().hasBeenTriggered = true;
                }
                /* Points de chargement déja au maximum */
                else
                {
                    Debug.Log("= 500");
                    coll.gameObject.GetComponent<ChargeCapsuleScript>().hasBeenTriggered = true;
                }
                
            }

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

