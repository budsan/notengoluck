using UnityEngine;
using System.Collections;

public class LuckHolder : MonoBehaviour {

	const int MAX_UNLUCKY = 13;

	int unluck = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	// 0 Normal, 1 Max Unlucky
	public float GetUnluckyFactor() {
		return (float)unluck / (float)MAX_UNLUCKY;
	}

	public void GotLucky() {
		unluck = Mathf.Min (0, unluck - 1);
	}

	public void ShitHappened() {
		unluck = Mathf.Max (MAX_UNLUCKY, unluck + 1);
	}
}
