using LinkVibrationController.Client;
using UnityEngine;

namespace LinkVibrationController
{
    /// <summary>
    /// Unity class for using the LinkVibrationControllerClient
    /// Asign this script to a gameobject and use Vibrate function.
    /// If you want to extend the functionality please use this as a base class and derive from it
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

        /// <summary>
        /// Public function to vibrate a certain motor for a given duration
        /// </summary>
        /// <param name="id">Id of the motor</param>
        /// <param name="duration">Duration</param>
        public void Vibrate(int id, int duration)
        {
            client.Vibrate(id, duration);
        }
    }
}

