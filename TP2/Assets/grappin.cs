using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappin : MonoBehaviour {
	ConfigurableJoint joint;
	Vector3 targetPos;
	RaycastHit hit;
	public float distance=10f;
	public float distanceMin=2f;
	public LayerMask mask;
	public LineRenderer line;
	public bool canMoveHorizontal = true;
	Vector3 hand;
	SoftJointLimit JLimNull= new SoftJointLimit();

	GameObject player;

	// Use this for initialization
	void Start () {
		JLimNull.limit=Mathf.Infinity;
		joint = GetComponent<ConfigurableJoint> ();
		joint.linearLimit = JLimNull;
		line.enabled = false;

		player =GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
		hand = transform.position;
		hand.y = hand.y + 0.5f;

		if(Input.GetButtonDown("Grappin")){
			line.SetPosition(0,transform.position);
			var mousePos = Input.mousePosition;
			mousePos.z = 10;
			targetPos= Camera.main.ScreenToWorldPoint(mousePos);
			targetPos.x=transform.position.x;
			Debug.Log ("x="+targetPos.x+"y="+targetPos.y+"z="+targetPos.z);

			Physics.Raycast (transform.position, targetPos - transform.position, out hit, distance, mask);

		
			if(hit.collider!=null )

			{
				Debug.Log ("grappin active");
				Vector3 shot = new Vector3 (transform.position.x, hit.point.y, hit.point.z);
				Vector3 connectPoint =shot - new Vector3(transform.position.x,hit.collider.transform.position.y,hit.collider.transform.position.z);
				connectPoint.z = connectPoint.z / hit.collider.transform.localScale.z;
				connectPoint.y = connectPoint.y / hit.collider.transform.localScale.y;
				connectPoint.x = connectPoint.x / hit.collider.transform.localScale.x;

				float distanceMax = Vector3.Distance(transform.position,shot);

				if(distanceMax>distanceMin){
				canMoveHorizontal = false;
				SoftJointLimit JLim= new SoftJointLimit();
				JLim.limit=distanceMax;
				joint.linearLimit = JLim;

				joint.connectedBody=hit.collider.gameObject.GetComponent<Rigidbody>();
				joint.connectedAnchor = connectPoint;
				//joint.connectedAnchor = hit.point - new Vector3(hit.collider.transform.position.x,hit.collider.transform.position.y,hit.collider.transform.position.z);


				line.enabled=true;
				line.SetPosition(0,hand);

				line.SetPosition(1,joint.connectedBody.transform.TransformPoint( joint.connectedAnchor));
				Debug.Log ("connect"+connectPoint);
				Debug.Log ("hit point"+hit.point);
				Debug.Log ("conneted anchor"+joint.connectedAnchor);
				Debug.Log ("rope target"+joint.connectedBody.transform.TransformPoint( joint.connectedAnchor));
			}
			}
		}

		if (Input.GetButton ("Grappin")) {
			line.SetPosition(0,hand);
		}


		if (Input.GetButtonUp("Grappin")) {
			player.GetComponent<PlayerControlerDashGrappin> ()._Grounded = true;
			player.GetComponent<PlayerControlerDashGrappin> ()._Anim.SetBool ("Grounded", true);
			joint.linearLimit = JLimNull;
			line.enabled=false;
			canMoveHorizontal = true;
		}

	}
}