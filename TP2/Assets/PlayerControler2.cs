using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler2 : MonoBehaviour
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

	int charge = 0;
	bool wasCharged = false;
	bool isOnAWall = false;
	bool isInWallJump = false;

    // Valeurs exposées
    [SerializeField]
    float MoveSpeed = 5.0f;

    [SerializeField]
    float JumpForce = 10f;

    [SerializeField]
    LayerMask WhatIsGround;

	[SerializeField]
	LayerMask WhatIsWall;

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
		
		// Glisser sur le mur
		if (isOnAWall){
			_Rb.velocity = new Vector3 (0, -1, 0);
		}

		Debug.Log ("Input axis : " + Input.GetAxis ("Horizontal"));
			
        var horizontal = Input.GetAxis("Horizontal") * MoveSpeed;
		if (!isInWallJump) {
			HorizontalMove (horizontal);
		}
		FlipCharacter(horizontal);		

        CheckJump();
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
		// Charge du saut mural
		if (Input.GetKey (KeyCode.LeftShift) && isOnAWall) {
			wasCharged = true;
			if(charge < 200){
				charge += 2;
			}
		}
		else {
			wasCharged = false;
			charge = 0;
		}

		if (isOnAWall) {
			_Anim.SetBool("Grounded", false);
			_Anim.SetBool("Jump", true);

			if (Input.GetButtonDown("Jump")) {
				Debug.Log ("IsOnAWall + Jump + Value charge : " + charge);
				_Rb.velocity = new Vector3 (0, 0, 0);
				if (!_Flipped) {
					FlipCharacter (-1);
					isInWallJump = true;
					_Rb.AddForce (new Vector3 (0, charge/20, -charge/10), ForceMode.Impulse);
					//_Rb.AddForce (new Vector3 (0, 0, -JumpForce + (charge)), ForceMode.Impulse);

				} else {
					FlipCharacter (1);
					isInWallJump = true;
					_Rb.AddForce (new Vector3 (0, charge/20, charge/10), ForceMode.Impulse);
					//_Rb.AddForce (new Vector3 (0, 0, JumpForce + (charge)), ForceMode.Impulse);
				}
				_Grounded = false;
			} 
		}
			
        if (_Grounded)
        {
			if (Input.GetButtonDown("Jump"))
            {
				Debug.Log ("Saut");
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
		if (horizontal < 0 && !_Flipped) {
			_Flipped = true;
			transform.Rotate (FlipRotation);
			_MainCamera.transform.Rotate (-FlipRotation);
			_MainCamera.transform.localPosition = InverseCameraPosition;
		} else if (horizontal > 0 && _Flipped) {
			_Flipped = false;
			transform.Rotate (-FlipRotation);
			_MainCamera.transform.Rotate (FlipRotation);
			_MainCamera.transform.localPosition = CameraPosition;
		}
			
    }

    // Collision avec le sol et fin de niveau
    void OnCollisionEnter(Collision coll)
	{
		isInWallJump = false;
		// Sur un mur
		if (coll.gameObject.layer == 10) {
			_Grounded = false;
			isOnAWall = true;
		} else {
			isOnAWall = false;
		}

		// Fin de niveau
		if(coll.gameObject.layer == 9)
		{
			Debug.Log ("Collision fin de niveau");
			//Application.Quit;
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

	void OnCollisionExit(Collision coll){
		// Quand le personnage n'est plus sur le mur
		if (coll.gameObject.layer == 10) {
			isOnAWall = false;
		}

		if (coll.gameObject.layer == 8) {
			_Grounded = false;
		}
	}
}
