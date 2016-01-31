using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	public GameObject fire;

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.layer != Logic.ins.fireLayer && !col.gameObject.CompareTag("OnFire")) {
            if (Random.Range(0f, 1f) >= 0.8f) return;
            GameObject insFire = (GameObject)Instantiate(fire, col.transform.position, fire.transform.rotation);
            insFire.transform.localScale = Vector3.one;
            insFire.transform.SetParent(col.transform, true);

			if (col.gameObject.layer != Logic.ins.playersLayer) {
				col.gameObject.layer = Logic.ins.fireLayer;
			}
			else {
				col.gameObject.tag = "OnFire";
			}

			if (col.transform.root.gameObject.layer == Logic.ins.playersLayer) {
				col.transform.root.gameObject.GetComponent<PlayerMovement> ().SetOnFire (true);
			}
		}
	}
}
