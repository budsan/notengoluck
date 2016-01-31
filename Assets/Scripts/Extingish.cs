using UnityEngine;
using System.Collections;

public class Extingish : MonoBehaviour {


    bool isGrabbed = false;
    LuckHolder myPlayer;

    public void setGrab(bool grab, Transform player)
    {
        isGrabbed = grab;
        myPlayer = (player != null) ? player.GetComponent<LuckHolder>() : null;

    }

    public GameObject extinguisher;
    float counter = .1f;

	// Update is called once per frame
	void Update () {
	    if(isGrabbed)
        {
            counter -= Time.deltaTime;
            if(counter <= 0)
            {
                counter += .1f;
                float unlucky = (myPlayer != null) ? myPlayer.GetUnluckyFactor() : 1f;

                if (Random.Range(0f, 1f) > unlucky) Instantiate(extinguisher, transform.position, extinguisher.transform.rotation);
            }
        }
	}
}
