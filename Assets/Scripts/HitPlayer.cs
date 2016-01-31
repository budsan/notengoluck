using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HitPlayer : MonoBehaviour {

	public UnityEvent hitCallback;

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.layer == Logic.ins.playersLayer) {

			if (col.relativeVelocity.magnitude > 1f) {
				Use (col.transform.root.gameObject);
			}
		}
	}

	public void Use(GameObject g) {
		g.GetComponent<PlayerMovement> ().Fall ();
		hitCallback.Invoke ();
		this.enabled = false;
	}
}
