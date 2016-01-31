using UnityEngine;
using System.Collections;

public class UnluckOnTrigger : MonoBehaviour {

    bool isActive = true;

    public void setActive(bool act)
    {
        isActive = act;
    }

    void OnTriggerEnter(Collider c)
    {
        LuckHolder luckyOne = c.GetComponent<LuckHolder>();
        if (luckyOne != null && isActive)
        {
            luckyOne.ShitHappened();
        }
    }
}
