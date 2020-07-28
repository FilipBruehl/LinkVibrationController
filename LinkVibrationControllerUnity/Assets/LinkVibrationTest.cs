using LinkVibrationController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkVibrationTest : MonoBehaviour
{
    private LinkVibrationControllerClient client;

    // Start is called before the first frame update
    void Start()
    {
        client = LinkVibrationControllerClient.Instance;
        Debug.Log(client.IsConnected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        client.Disconnect();
    }

    public void VibrateShort()
    {
        client.Vibrate(0, 500);
    }

    public void VibrateLong()
    {
        client.Vibrate(0, 1000);
    }
}
