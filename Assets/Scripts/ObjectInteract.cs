using UnityEngine;
using System.Collections;

public class ObjectInteract : MonoBehaviour {

	const float THROW_FORCE = 5f;

	Grabable grabbing = null;
	Rigidbody grabbedRB = null;
	public Transform holder;
	public Animator anim;

	bool firstFrame = false;

	void Update() {
		if (grabbing != null) {
			grabbedRB.MovePosition (holder.position - grabbing.transform.localPosition);
			grabbedRB.MoveRotation (holder.rotation);

			if (Input.GetButtonDown ("Fire1")) {
				Throw ();
			}
		}

		anim.SetBool ("hold", grabbing != null);
		anim.SetBool ("trow", grabbing == null && Input.GetButton ("Fire1"));
	}

	public void Throw() {
		if (grabbing != null) {
			if (firstFrame) {
				firstFrame = false;
			} else {
				grabbedRB.velocity = transform.forward * THROW_FORCE;
				grabbing = null;
			}
		}
	}
	
	void OnTriggerStay(Collider col) {
		if (grabbing == null && Input.GetButtonDown ("Fire1")) {
			Grabable g = col.gameObject.GetComponentInChildren<Grabable> ();
			if (g != null) {
				grabbing = g;
				grabbedRB = grabbing.GetComponentInParent<Rigidbody> ();
				firstFrame = true;
			}
			else {
				LuckTrigger lt = col.gameObject.GetComponentInParent<LuckTrigger> ();
				if (lt != null && lt.activated) {
					lt.resetCallback.Invoke ();
				}
			}
		}
	}
}
