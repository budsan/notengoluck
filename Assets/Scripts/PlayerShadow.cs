using UnityEngine;
using System.Collections;

public class PlayerShadow : MonoBehaviour {
	public LuckHolder player;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		float unluck = player.GetUnluckyFactor();
		ParticleSystem sys = GetComponent<ParticleSystem>();
		ParticleSystem.EmissionModule em = sys.emission;
		em.rate = new ParticleSystem.MinMaxCurve(100*unluck);
	}
}
