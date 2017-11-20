using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangerPersoPlayControl : MonoBehaviour
{

    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);

    private bool estVoleur;
    private bool estMage;
    private bool estGuerrier;
    private float jumpForceCurrent;
    private float moveSpeedCurrent;


    // Déclaration des variables
    bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    Animator[] _ListAnim { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }

    // Valeurs exposées
    [SerializeField]
    float MoveSpeedVoleur = 5.0f;

    [SerializeField]
    float MoveSpeedMage = 5.0f;

    [SerializeField]
    float MoveSpeedGuerrier = 5.0f;

    [SerializeField]
    float JumpForceVoleur = 10f;

    [SerializeField]
    float JumpForceMage = 10f;

    [SerializeField]
    float JumpForceGuerrier = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

    [SerializeField]
    LayerMask VoleurMask;

    [SerializeField]
    LayerMask GuerrierMask;

    [SerializeField]
    LayerMask MageMask;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _ListAnim = GetComponentsInChildren<Animator>();
        _Rb = GetComponent<Rigidbody>();
        _MainCamera = Camera.main;
        _MainCamera.cullingMask = VoleurMask;
        estVoleur = true;
        jumpForceCurrent = JumpForceVoleur;
        moveSpeedCurrent = MoveSpeedVoleur;
        estMage = false;
        estGuerrier = false;
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
        var horizontal = Input.GetAxis("Horizontal");
        CheckChangePerso();
        HorizontalMove(horizontal * moveSpeedCurrent);
        FlipCharacter(horizontal);
        CheckJump();
    }

    void CheckChangePerso()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !estVoleur)
        {
            estVoleur = true;
            jumpForceCurrent = JumpForceVoleur;
            moveSpeedCurrent = MoveSpeedVoleur;
            estMage = false;
            estGuerrier = false;
            _MainCamera.cullingMask = VoleurMask;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2) && !estMage)
        {
            estVoleur = false;
            estMage = true;
            jumpForceCurrent = JumpForceMage;
            moveSpeedCurrent = MoveSpeedMage;
            estGuerrier = false;
            _MainCamera.cullingMask = MageMask;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3) && !estGuerrier)
        {
            estVoleur = false;
            estMage = false;
            estGuerrier = true;
            jumpForceCurrent = JumpForceGuerrier;
            moveSpeedCurrent = MoveSpeedGuerrier;
            _MainCamera.cullingMask = GuerrierMask;
        }
    }

    // Gère le mouvement horizontal
    void HorizontalMove(float horizontal)
    {
        _Rb.velocity = new Vector3(_Rb.velocity.x, _Rb.velocity.y, horizontal);
        foreach (Animator anim in _ListAnim)
        {
            anim.SetFloat("MoveSpeed", Mathf.Abs(horizontal));
        }        
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    void CheckJump()
    {
        if (_Grounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                _Rb.AddForce(new Vector3(0, jumpForceCurrent, 0), ForceMode.Impulse);
                _Grounded = false;
                foreach (Animator anim in _ListAnim)
                {
                    anim.SetBool("Grounded", false);
                    anim.SetBool("Jump", true);
                }
                
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

        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << coll.gameObject.layer)) == 0)
            return;

        // Évite une collision avec le plafond
        if (coll.relativeVelocity.y > 0)
        {
            _Grounded = true;
            foreach (Animator anim in _ListAnim)
            {
                anim.SetBool("Grounded", _Grounded);
            }
        }
    }
}
