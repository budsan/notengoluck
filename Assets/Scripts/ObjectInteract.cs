using UnityEngine;
using System.Collections;

public class ObjectInteract : MonoBehaviour {

	const float THROW_FORCE = 7.5f;

	Grabable grabbing = null;
	Rigidbody grabbedRB = null;
	Collider grabbedCol = null;
	public Transform holder;
	public Animator anim;

	bool firstFrame = false;
	PlayerMovement playerMov;

    void Start()
    {
		playerMov = transform.parent.GetComponent<PlayerMovement>();
    }

	void Update() {
		if (grabbing != null) {
			grabbedRB.MovePosition (holder.position);
			grabbedRB.MoveRotation (holder.rotation);

			if (Input.GetButtonDown ("X" + playerMov.getPlayerId().ToString()) || Input.GetButtonDown ("Y" + playerMov.getPlayerId().ToString())) {
				if (firstFrame) {
					firstFrame = false;
				} else {
					Throw ();
				}
			}
		}

		anim.SetBool ("hold", grabbing != null || Input.GetButton ("X" + playerMov.getPlayerId().ToString()));
		anim.SetBool ("trow", grabbing == null && Input.GetButton ("Y" + playerMov.getPlayerId().ToString()));
	}

	public void Throw() {
		if (grabbing != null) {
			grabbedRB.velocity = transform.parent.forward * THROW_FORCE;
			grabbedRB.useGravity = true;
			grabbedRB.isKinematic = false;
            Extingish ext = grabbing.GetComponentInParent<Extingish>();
            if (ext != null) ext.setGrab(false, null);

			Physics.IgnoreCollision (grabbedCol, transform.parent.GetComponent<Collider> (), false);

			grabbing = null;
		}
	}
	
	void OnTriggerStay(Collider col) {
		if (grabbing == null && Input.GetButtonDown ("X" + playerMov.getPlayerId().ToString())) {
			Grabable g = col.gameObject.GetComponentInChildren<Grabable> ();
			if (g != null && g.enabled) {
				grabbing = g;
				grabbedRB = grabbing.GetComponentInParent<Rigidbody> ();
                Extingish ext = grabbing.GetComponentInParent<Extingish>();
                if (ext != null) ext.setGrab(true, transform.parent);
                grabbedRB.useGravity = false;
				firstFrame = true;
				holder.rotation = col.transform.rotation;
				grabbedCol = col;
				Physics.IgnoreCollision (grabbedCol, transform.parent.GetComponent<Collider> (), true);
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
