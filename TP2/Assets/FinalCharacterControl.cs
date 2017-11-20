using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalCharacterControl : MonoBehaviour
{

    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);

    private bool didCharge;
    private bool chargeCharge;
    private float chargeDelay = 0.0f;
    private float timeChargeStart;
    private bool chargeAble;


    // Déclaration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float ChargeSpeed = 10.0f;

    [SerializeField]
    float ChargeDuration = 1.0f;

    [SerializeField]
    float ChargeTime = 1.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;


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
        chargeCharge = false;
        didCharge = false;
        chargeAble = false;

    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        CheckCharge();
        if (!chargeCharge && !didCharge)
        {
            HorizontalMove(horizontal * MoveSpeed);
            FlipCharacter(horizontal * MoveSpeed);
            CheckJump();
        }
    }

    void CheckCharge()
    {
        if (Input.GetKey(KeyCode.X) && _Grounded && !didCharge && chargeAble)
        {
            chargeCharge = true;
            _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, 0.0f);
            _Anim.SetFloat("MoveSpeed", 0.0f);

            if (chargeDelay < ChargeTime)
            {
                chargeDelay += Time.deltaTime;
            }
            else
            {
                timeChargeStart = 0.0f;
                chargeDelay = 0.0f;
                didCharge = true;
                chargeCharge = false;

            }
        }
        else
        {
            chargeCharge = false;
        }
        if (didCharge)
        {
            timeChargeStart += Time.deltaTime;
            didCharge = (timeChargeStart < ChargeDuration);
            if (_Flipped)
            {
                ChargeSpeed *= -1;
            }
            _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, ChargeSpeed);
            _Anim.SetFloat("MoveSpeed", Mathf.Abs(ChargeSpeed));
            ChargeSpeed = Mathf.Abs(ChargeSpeed);
        }
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

    // Collision avec le sol
    void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.name == "Collectible")
        {
            Destroy(coll.gameObject);
            chargeAble = true;
        }

        if (didCharge && coll.gameObject.tag == "Destructible")
        {
            Destroy(coll.gameObject);
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
