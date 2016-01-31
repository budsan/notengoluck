using UnityEngine;
using System.Collections;

public class UnluckyDeath : MonoBehaviour {


	bool _dead = false;

	public PlayerMovement playerMov;

	public Grabable head;

	public bool dead {
		get { return _dead; }
	}

    public void deathChance(float probability)
    {
        if (dead) return;
        float chance = Random.Range(0f, 1f);
        if(chance < probability)
        {
            _dead = true;
			playerMov.Fall();
			playerMov.enabled = false;
			playerMov.enabled = false;

			GetComponentInChildren<SkinnedMeshRenderer> ().material.color = Color.gray;
			head.enabled = true;

			Logic.ins.ImDead(playerMov.getPlayerId());
        }
    }
}
