using UnityEngine;
using System.Collections;

public class UnluckyDeath : MonoBehaviour {

	
    bool dead = false;

    public void deathChance(float probability)
    {
        if (dead) return;
        float chance = Random.Range(0f, 1f);
        if(chance < probability)
        {
            dead = true;
            GetComponent<PlayerMovement>().Fall();
            GetComponent<PlayerMovement>().enabled = false;
            GetComponentInChildren<ObjectInteract>().enabled = false;
        }
    }
}
