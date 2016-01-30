using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	public GameObject fire;

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.layer != Logic.ins.fireLayer && !col.gameObject.CompareTag("OnFire")) {
			((GameObject)Instantiate (fire, col.transform.position, fire.transform.rotation)).transform.parent = col.transform;
			if (col.gameObject.layer != Logic.ins.playersLayer) {
				col.gameObject.layer = Logic.ins.fireLayer;
			}
			else {
				col.gameObject.tag = "OnFire";
			}

		}
	}
}
