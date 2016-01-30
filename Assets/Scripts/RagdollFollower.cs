using UnityEngine;
using System.Collections;

public class RagdollFollower : MonoBehaviour {

	public Transform toFollow;
	public PlayerMovement mov;

	Rigidbody rb;

	void Start() {
		rb = GetComponent<Rigidbody> ();
	}

	// Update is called once per frame
	void LateUpdate () {
		rb.MovePosition(toFollow.position);
	}
}
