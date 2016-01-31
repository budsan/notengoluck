using UnityEngine;
using System.Collections;

public class BookShelf : MonoBehaviour {

	const float TORQUE = -20000f;

	Rigidbody rb;

	Vector3 pos;
	Quaternion rot;


	void Start() {
		rb = GetComponentInChildren<Rigidbody> ();

		pos = rb.transform.position;
		rot = rb.transform.rotation;
	}

	public void Fall() {
		rb.isKinematic = false;

		rb.AddRelativeTorque (new Vector3 (TORQUE, 0f, 0f));
		gameObject.tag = "Unluck";
	}

	public void Reset() {
		rb.MovePosition(pos);
		rb.MoveRotation(rot);

		rb.GetComponent<Collider> ().isTrigger = false;
		Debug.Log ("derp");
	}

	public void Disable() {
		//GetComponent<Collider> ().isTrigger = true;
		//rb.useGravity = false;
	}
}
