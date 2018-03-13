using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour {

    // Players interaction notification
    public GameObject playerAction;
    GameObject instance;

    void Start()
    {
        playerAction = this.transform.Find("Action").gameObject;
    }

    void Update()
    {
        instance.transform.position = playerAction.transform.position;
    }

    // ---------- Trigger Interactions ----------
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Item")
           instance = Instantiate(Resources.Load("PlayerAction/PickUp_Open"), playerAction.transform.position, Quaternion.identity) as GameObject;
        else if (collider.tag == "Object")
            instance = Instantiate(Resources.Load("PlayerAction/Examine"), playerAction.transform.position, Quaternion.identity) as GameObject;
        else if (collider.tag == "NPC")
            instance = Instantiate(Resources.Load("PlayerAction/Talk"), playerAction.transform.position, Quaternion.identity) as GameObject;
        else if (collider.tag == "Door")
            instance = Instantiate(Resources.Load("PlayerAction/Enter_Exit"), playerAction.transform.position, Quaternion.identity) as GameObject;
    }

    void OnTriggerExit(Collider collider)
    {
        Destroy(instance);
    }
}
