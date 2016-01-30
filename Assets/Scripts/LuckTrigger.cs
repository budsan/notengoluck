﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class LuckTrigger : MonoBehaviour {

	public UnityEvent callback;

	const float MIN_ENTROPY = 0.1f;
	const float TENSION_DECAY = 0.1f;

	const float MAX_PERIOD = 2f;
	const float MIN_PERIOD = 0.25f;

	[Range(0f, 1f)]
	public float maxTensionPerSecond = 0.3f;

	float tension = 0f; // Entre 0 y 1
	float lastTension = 0f;
	float tensionTime = 0f;

	[Header("Gizmos")]
	public Vector3 gizmoCenter;

	void Update() {
		// Si hay tension y no hay nada que este tensandonos
		if (tension > 0f && tension == lastTension) {
			// tranquilizamos
			tension = Mathf.Max(0f, tension - TENSION_DECAY*Time.deltaTime);
		}

		if (tension > 0f && tensionTime > 0f) {
			tensionTime -= Time.deltaTime;

			if (tensionTime < 0f) {
				ConsiderTriggering ();
				tensionTime = Mathf.Lerp (MAX_PERIOD, MIN_PERIOD, tension);
			}
		}

		lastTension = tension;
	}

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.layer == Logic.ins.playersLayer) {
			LuckHolder lh = col.gameObject.GetComponent<LuckHolder> ();
			if (lh != null) {
				ConsiderTriggering (lh);

				tensionTime = Mathf.Lerp (MAX_PERIOD, MIN_PERIOD, lh.GetUnluckyFactor ());
			}
		}
	}

	void OnTriggerStay(Collider col) {
		if (col.gameObject.layer == Logic.ins.playersLayer) {
			LuckHolder lh = col.gameObject.GetComponent<LuckHolder> ();
			if (lh != null) {
				// Un easing lineal de
				tension = Mathf.Min(1f,
					tension +
					(TENSION_DECAY + // Tension que siempre augmenta, por el echo de estar en el trigger, random muy pequenya
					 Random.Range (MIN_ENTROPY, maxTensionPerSecond) * lh.GetUnluckyFactor () // Tension relativa (linealmente) al unlucky factor
					) * Time.fixedDeltaTime); 
			}
		}
	}

	void ConsiderTriggering(LuckHolder lh = null) {
		float threshold = tension;
		if (lh != null) { // Case on Trigger Enter
			float unluck = lh.GetUnluckyFactor ();

			threshold = unluck * 0.5f + tension * 0.5f;
		}

		if (Random.Range (0f, 1f) < threshold) {
			callback.Invoke ();
			this.enabled = false;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color (Mathf.Min (1f, tension * 2f), 1f - Mathf.Max(0f, tension - 0.5f) * 2f, 0f);
		Gizmos.DrawCube (transform.position + gizmoCenter, new Vector3 (tension, 0.2f, 0.1f));
	}
}
