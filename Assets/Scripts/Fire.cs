using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	public GameObject fire;

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.layer != Logic.ins.fireLayer) {
			((GameObject)Instantiate (fire, col.transform.position, fire.transform.rotation)).transform.parent = col.transform;
			col.gameObject.layer = Logic.ins.fireLayer;
		}
	}
}
