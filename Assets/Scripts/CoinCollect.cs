using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    void Start()
    {
        //************* Instantiate the OSC Handler...
        OSCHandler.Instance.Init();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerCoin", "ready");
        //*************
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Coin")) {
            other.gameObject.SetActive(false);

            //************* Send the message to the client...
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerCoin", 1); // value of 1 sent to pd triggers bang object
            //*************
        }
    }
}
