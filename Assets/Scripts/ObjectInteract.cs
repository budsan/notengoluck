using UnityEngine;
using System.Collections;

public class ObjectInteract : MonoBehaviour {

	const float THROW_FORCE = 5f;

	Grabable grabbing = null;
	Rigidbody grabbedRB = null;
	public Transform holder;
	public Animator anim;

	bool firstFrame = false;
    int playerId;

    void Start()
    {
        playerId = transform.parent.GetComponent<PlayerMovement>().getPlayerId();
    }

	void Update() {
		if (grabbing != null) {
			grabbedRB.MovePosition (holder.position - grabbing.transform.localPosition);
			grabbedRB.MoveRotation (holder.rotation);

			if (Input.GetButtonDown ("X" + playerId.ToString())) {
				Throw ();
			}
		}

		anim.SetBool ("hold", grabbing != null || Input.GetButton ("X" + playerId.ToString()));
		anim.SetBool ("trow", grabbing == null && Input.GetButton ("Y" + playerId.ToString()));
	}

	public void Throw() {
		if (grabbing != null) {
			if (firstFrame) {
				firstFrame = false;
			} else {
				grabbedRB.velocity = transform.forward * THROW_FORCE;
				grabbedRB.useGravity = true;
				grabbedRB.isKinematic = false;
                Extingish ext = grabbing.GetComponentInParent<Extingish>();
                if (ext != null) ext.setGrab(false);
                grabbing = null;
			}
		}
	}
	
	void OnTriggerStay(Collider col) {
		if (grabbing == null && Input.GetButtonDown ("X" + playerId.ToString())) {
			Grabable g = col.gameObject.GetComponentInChildren<Grabable> ();
			if (g != null) {
				grabbing = g;
				grabbedRB = grabbing.GetComponentInParent<Rigidbody> ();
                Extingish ext = grabbing.GetComponentInParent<Extingish>();
                if (ext != null) ext.setGrab(true);
                grabbedRB.useGravity = false;
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
