using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HitPlayer : MonoBehaviour {

	public UnityEvent hitCallback;

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.layer == Logic.playersLayer) {

			Rigidbody own = transform.root.GetComponentInChildren<Rigidbody> ();
			if (col.relativeVelocity.magnitude > 1f && (own == null || own.velocity.magnitude > 1f)) {
				Use (col.transform.root.gameObject);
			}
		}
	}

	public void Use(GameObject g) {
		PlayerMovement pm = g.GetComponent<PlayerMovement> ();
		if (pm != null) {
			pm.Fall ();
			hitCallback.Invoke ();
			this.enabled = false;
		}
	}
}
