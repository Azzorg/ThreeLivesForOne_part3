using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerControllerGlide : MonoBehaviour
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
    bool _IsGliding { get; set; }
    float _duration { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

    [SerializeField]
    float GlideForceZ = 3.0f;

    [SerializeField]
    float GlideForceY = -0.5f;

    [SerializeField]
    float GlideDuration = 3f;


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
        _IsGliding = false;
        _duration = GlideDuration;
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
        if(_IsGliding == false){ HorizontalMove(horizontal); }
        FlipCharacter(horizontal);
        CheckJump();
        CheckGlide();
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
    
    /* Gère le vol plané */
    void CheckGlide()
    {
        if (_Grounded == false)
        {
            if (Input.GetKeyDown(KeyCode.F) && _IsGliding == false)
            {
                _Rb.velocity = new Vector3(_Rb.velocity.x, 0, 0); // Enlève toute vitesse en Y et en Z
                _Rb.AddForce(new Vector3(0, -GlideForceY, 0), ForceMode.VelocityChange); // Applique une force verticale
                Debug.Log("Vol Plané");
                _IsGliding = true;
                _Grounded = false;
                _Anim.SetBool("Grounded", false);
                _Anim.SetBool("Jump", true);
            }
        }
        else
        {
            _IsGliding = false;
            _duration = GlideDuration;
        }

        if (_IsGliding)
        {
            _duration -= Time.deltaTime;
            float coef = _duration / GlideDuration;
            if(coef >= 0) {
                if (_Flipped) { _Rb.velocity = new Vector3(0, _Rb.velocity.y, -10 * coef); }
                else { _Rb.velocity = new Vector3(0, _Rb.velocity.y, 10 * coef); }
            }
            else { _Rb.velocity = new Vector3(0, _Rb.velocity.y, 0); }

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

