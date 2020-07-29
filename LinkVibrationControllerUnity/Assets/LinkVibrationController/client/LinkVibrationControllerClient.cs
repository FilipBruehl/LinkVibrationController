using System;
using LinkVibrationController.models;
using Newtonsoft.Json;
using UnityEngine;
using WebSocketSharp;

namespace LinkVibrationController.Client
{
    /// <summary>
    /// Singleton class to provide access to the websocket server
    /// </summary>
    public class LinkVibrationControllerClient
    {
        #region Private Members

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static LinkVibrationControllerClient instance = null;

        /// <summary>
        /// object for thread locking
        /// </summary>
        private static readonly object Lock = new object();

        private WebSocket websocket;

        #endregion

        #region Public Members

        /// <summary>
        /// Singleton getter for Instance Variable
        /// Locks for thread safety
        /// </summary>
        public static LinkVibrationControllerClient Instance
        {
            get
            {
                lock (Lock)
                {
                    if (instance == null)
                    {
                        instance = new LinkVibrationControllerClient();
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// Member indicating if connection is active or not
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return websocket.ReadyState == WebSocketState.Open;
            }
        }

        #endregion

        #region Private Constructor

        /// <summary>
        /// Private standard constructor for initializing the client
        /// </summary>
        private LinkVibrationControllerClient()
        {
            websocket = new WebSocket("ws://127.0.0.1/vibrate");
            websocket.Connect();
            if(IsConnected)
            {
                UnityEngine.Debug.Log("Connected to server");
            } else
            {
                UnityEngine.Debug.LogError("Couldn't connect to server");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Used for sending data to the server
        /// </summary>
        /// <param name="message">Message to be send in string form</param>
        private void Send(string message)
        {
            websocket.Send(message);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Used for vibrating a certain motor
        /// </summary>
        /// <param name="id">If of motor to vibrate</param>
        /// <param name="duration">Duration of vibration</param>
        public void Vibrate(int id, int duration)
        {
            Vibration v = new Vibration();
            v.Duration = duration;
            v.Id = (VibratorId) id;

            Vibrate(v);
        }

        /// <summary>
        /// Used for vibrating a certain motor.
        /// Data gets serialized before sending it
        /// </summary>
        /// <param name="vibration">Vibration object <see cref="Vibration"/></param>
        public void Vibrate(Vibration vibration)
        {
            Send(JsonConvert.SerializeObject(vibration));
        }

        public bool Disconnect()
        {
            websocket.Close();
            return !IsConnected;
        }

        #endregion
    }
}

