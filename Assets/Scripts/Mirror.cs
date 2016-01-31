using UnityEngine;
using System.Collections;

public class Mirror : MonoBehaviour {

	const float TORQUE = -20000f;

	Rigidbody rb;

	void Start() {
		rb = GetComponentInChildren<Rigidbody> ();
	}

	public void Fall() {
		rb.isKinematic = false;
		rb.useGravity = true;

		rb.AddRelativeTorque (new Vector3 (TORQUE, 0f, 0f));
		gameObject.tag = "Unluck";
	}

	void OnCollisionEnter(Collision col) {
		if (col.relativeVelocity.magnitude > 1f) {
			
		}
	}
}
