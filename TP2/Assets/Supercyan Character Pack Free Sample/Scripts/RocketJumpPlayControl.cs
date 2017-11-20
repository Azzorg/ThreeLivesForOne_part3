using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketJumpPlayControl : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);
    private static GameObject RocketLine;
    private LineRenderer lr;
    private bool doRocket;
    private bool didRocket;
    private float angleRocket;
    private float timeRocketStart;
    private float RocketDelay = 0f;

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
    float JumpForce = 10f;

    [SerializeField]
    float RocketForce = 20f;

    [SerializeField]
    float RocketTime = 1f;

    [SerializeField]
    float RocketDuration = 5f;

    [SerializeField]
    LayerMask WhatIsGround;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    void Awake()
    {
        _Anim = GetComponent<Animator>();
        _Rb = GetComponent<Rigidbody>();
        _MainCamera = Camera.main;
        RocketLine = new GameObject();
    }

    // Utile pour régler des valeurs aux objets
    void Start()
    {
        _Grounded = false;
        _Flipped = false;
        doRocket = false;
        didRocket = false;
        angleRocket = 0.0f;
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        CheckRocket(horizontal);
        if (!doRocket)
        {
            HorizontalMove(horizontal * MoveSpeed);
            FlipCharacter(horizontal * MoveSpeed);
            CheckJump();
        }
    }

    void CheckRocket(float horizontal)
    {
        if (Input.GetKey(KeyCode.X) && _Grounded)
        {
            doRocket = true;
            _Rb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
            _Anim.SetFloat("MoveSpeed", 0.0f);

            if (RocketDelay < RocketTime)
            {
                angleRocket += horizontal * 0.01f;
                RocketLine.transform.position = this.transform.position;
                RocketLine.AddComponent<LineRenderer>();
                lr = RocketLine.GetComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
                lr.startColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
                lr.endColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
                lr.SetPosition(0, RocketLine.transform.position);
                lr.SetPosition(1, RocketLine.transform.position + new Vector3(0.0f, 2f * Mathf.Sin(3.14f / 2f + angleRocket), 2f * Mathf.Sin(angleRocket)));
                RocketDelay += Time.deltaTime;
            }
            else
            {
                _Rb.AddForce(Vector3.Normalize(lr.GetPosition(1) - lr.GetPosition(0)) * RocketForce, ForceMode.VelocityChange);
                _Grounded = false;
                _Anim.SetBool("Grounded", false);
                timeRocketStart = 0.0f;
                angleRocket = 0.0f;
                RocketDelay = 0f;
                LineRenderer.Destroy(lr);
                didRocket = true;
            }
        }
        else
        {
            if (didRocket)
            {
                timeRocketStart += Time.deltaTime;
                doRocket = (timeRocketStart < RocketDuration && !_Grounded);
                didRocket = (timeRocketStart < RocketDuration);
            }
            else
            {
                doRocket = false;
            }
            angleRocket = 0.0f;
            RocketDelay = 0f;

            LineRenderer.Destroy(lr);
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
