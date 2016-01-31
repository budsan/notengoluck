using UnityEngine;
using System.Collections;

public class LuckHolder : MonoBehaviour {

	const int MAX_UNLUCKY = 13;
	public GameObject shadowPrefab;

	int unluck = 0;

	// Use this for initialization
	void Start () {
		GameObject g = (GameObject) Instantiate(shadowPrefab, this.transform.position, this.transform.rotation);
        g.name = "Shadow";
		g.GetComponent<PlayerShadow>().player = GetComponent<LuckHolder>();
		g.GetComponent<RagdollFollower>().toFollow = this.GetComponent<PlayerMovement>().ragdollChest.transform;
		g.GetComponent<RagdollFollower>().mov = this.GetComponent<PlayerMovement>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	// 0 Normal, 1 Max Unlucky
	public float GetUnluckyFactor() {
		return (float)unluck / (float)MAX_UNLUCKY;
	}

	public void GotLucky() {
		unluck = Mathf.Max (0, unluck - 1);
	}

	public void ShitHappened() {
		unluck = Mathf.Min (MAX_UNLUCKY, unluck + 1);
	}

	void OnControllerColliderHit(ControllerColliderHit col) {
		HitPlayer hp = col.gameObject.GetComponent<HitPlayer> ();
		if (hp && hp.enabled) {
			hp.Use (gameObject);
		}
	}

	void OnDrawGizmos() {
		float l = GetUnluckyFactor ();
		Gizmos.color = new Color (Mathf.Min (1f, l * 2f), 1f - Mathf.Max(0f, l - 0.5f) * 2f, 0f);
		Gizmos.DrawCube (transform.position + new Vector3(0,1.5f,0f), new Vector3 (l, 0.2f, 0.1f));
	}
}