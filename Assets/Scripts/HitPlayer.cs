using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HitPlayer : MonoBehaviour {

	public UnityEvent hitCallback;

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.layer == Logic.ins.playersLayer) {

			if (col.relativeVelocity.magnitude > 1f) {
				col.gameObject.GetComponent<PlayerMovement> ().Fall ();
				hitCallback.Invoke ();
			}
		}
	}
}
