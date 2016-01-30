using UnityEngine;
using System.Collections;

public class BookShelf : MonoBehaviour {

	const float TORQUE = -20000f;

	public void Fall() {
		Rigidbody rb = GetComponentInChildren<Rigidbody> ();
		rb.isKinematic = false;

		rb.AddRelativeTorque (new Vector3 (TORQUE, 0f, 0f));
		gameObject.tag = "Unluck";
	}
}
