using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlerDashGrappin : MonoBehaviour
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);

	private int currDashFrames=0;

    // Déclaration des variables
	int nbDash=0;
    public bool _Grounded { get; set; }
    bool _Flipped { get; set; }
    public Animator _Anim { get; set; }
    Rigidbody _Rb { get; set; }
    Camera _MainCamera { get; set; }
	GameObject player;

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

	[SerializeField]
	int nbDashMax=2;

	[SerializeField]
	float dashPower=20f;

	[SerializeField]
	float dashFrames=20;

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
        _Grounded = true;
        _Flipped = false;
		player =GameObject.FindGameObjectWithTag ("Player");
    }

    // Vérifie les entrées de commandes du joueur
    void Update()
    {
		
		if (player.GetComponent<grappin> ().canMoveHorizontal && currDashFrames == 0) {
			var horizontal = Input.GetAxis ("Horizontal") * MoveSpeed;
			HorizontalMove (horizontal);
			FlipCharacter (horizontal);
			CheckJump ();
			checkDash ();
		} else if (!player.GetComponent<grappin> ().canMoveHorizontal) {
			_Grounded = false;
			_Anim.SetFloat ("MoveSpeed", 0f);
			_Anim.SetBool ("Grounded", false);
		} else if (!(currDashFrames == 0)) {
			currDashFrames = currDashFrames + 1;
			if (currDashFrames > dashFrames)
				currDashFrames = 0;
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
		if (_Grounded) {
			if (Input.GetButtonDown ("Jump")) {
				_Rb.AddForce (new Vector3 (0, JumpForce, 0), ForceMode.Impulse);
				_Grounded = false;
				_Anim.SetBool ("Grounded", false);
				_Anim.SetBool ("Jump", true);
			}
		} 
    }

	void checkDash()
	{
		if (!_Grounded) {
			if (Input.GetButtonDown("Dash") && nbDash<nbDashMax)
			{
				_Rb.AddRelativeForce (new Vector3(0f,0f,1f)*dashPower,ForceMode.Impulse);

				currDashFrames = 1;
				nbDash = nbDash + 1;
			}
		}
		else

			nbDash = 0;
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
			player.GetComponent<grappin> ().canMoveHorizontal = true;
        }
    }
}
