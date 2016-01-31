using UnityEngine;
using System.Collections;

public class Mirror : MonoBehaviour {

	const float TORQUE = -20000f;

	Rigidbody rb;

	public GameObject brokenMirror;

	void Start() {
		rb = GetComponentInChildren<Rigidbody> ();
	}


	public void Fall() {
		if (enabled) {
			rb.isKinematic = false;
			rb.useGravity = true;

			rb.AddRelativeTorque (new Vector3 (TORQUE, 0f, 0f));
			gameObject.tag = "Unluck";
		}
	}

	void OnCollisionEnter(Collision col) {
		if (col.relativeVelocity.magnitude > 1f && rb.velocity.magnitude > 0.5f) {
			Break ();
		}
	}

	public void Break() {
		GameObject broken = (GameObject)Instantiate (brokenMirror, transform.position, transform.rotation);

		foreach (Rigidbody r in broken.GetComponentsInChildren<Rigidbody>()) {
			r.velocity = rb.velocity;
		}

		Collider c = GetComponent<Collider> ();

		c.enabled = false;
		Destroy (gameObject);
		this.enabled = false;
	}
}
