using LinkVibrationController.Client;
using UnityEngine;

namespace LinkVibrationController
{
    /// <summary>
    /// Unity class for using the LinkVibrationControllerClient
    /// Asign this script to a gameobject and extend it or derive a new script from this base class
    /// </summary>
    public class LinkVibration : MonoBehaviour
    {
        protected LinkVibrationControllerClient client;

        // Start is called before the first frame update
        void Start()
        {
            client = LinkVibrationControllerClient.Instance;
        }

        // Update is called once per frame
        void Update()
        {

        }

        // OnApplicationQuit is called when the application quits
        // Needs to be implemented to close the connection to server
        void OnApplicationQuit()
        {
            if (client.IsConnected)
            {
                client.Disconnect();
            }

        }
    }
}

