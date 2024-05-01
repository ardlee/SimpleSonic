using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityOSC;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class DrowningTimer : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private float maxTime = 5;
    private float timer = 0;
    private bool isUnderwater = false;

    void Start()
    {
        //************* Instantiate the OSC Handler...
        OSCHandler.Instance.Init();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerDrown", "ready");
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempoDrown", "ready");
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerDeath", "ready");

        //*************
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerDrown", 0); // toggles OFF bang object for drowning sound


        slider.maxValue = maxTime;
    }
/* TODO: can use the update fcn to alter drowning sound tempo */
    void Update()
    {
        // slider should follow sonic
        slider.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
        slider.value = maxTime - timer;

        // slider should show up if underwater
        slider.gameObject.SetActive(isUnderwater);
        // timer should count if underwater
        if (isUnderwater) {
            timer += Time.deltaTime;
            //************* Send the message to the client...
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempoDrown", 500/(timer+1)); // sets tempo of drowning sound
            if (timer > maxTime) {
                // Death
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerDeath", 1); // value of 1 sent to pd triggers bang object

                //if (timer > maxTime + 0.3) { OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerDeath", 0); } // value of 1 sent to pd triggers bang object

                gameObject.SetActive(false);
            }
        }
   

    }

    

    void FixedUpdate()
    {
        //************* Routine for receiving the OSC...
        OSCHandler.Instance.UpdateLogs();
        Dictionary<string, ServerLog> servers = OSCHandler.Instance.Servers;

        foreach (KeyValuePair<string, ServerLog> item in servers)
        {
            // If we have received at least one packet,
            // show the last received from the log in the Debug console
            if (item.Value.log.Count > 0)
            {
                int lastPacketIndex = item.Value.packets.Count - 1;

                //get address and data packet
                Debug.Log(item.Value.packets[lastPacketIndex].Data[0].ToString());

            }
        }
        //*************
    }

    /* TODO: can use trigger enter/exit fcns to activate/deactivate drowning sound tempo */
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Water")) {
            isUnderwater = true;
            //************* Send the message to the client...
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempoDrown", 500); // resets tempo of drowning sound to default (500)
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerDrown", 1); // toggles ON bang object for drowning sound
            //*************
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.CompareTag("Water")) {
            isUnderwater = false;
            timer = 0;
            //************* Send the message to the client...
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/tempoDrown", 500);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/triggerDrown", 0); // toggles OFF bang object for drowning sound
            //*************
        }
    }
}
