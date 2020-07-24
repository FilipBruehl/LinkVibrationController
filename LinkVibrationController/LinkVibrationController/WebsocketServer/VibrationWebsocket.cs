using LinkVibrationController.BluetoothLE;
using LinkVibrationController.EspModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;


namespace LinkVibrationController.WebsocketServer
{
    /// <summary>
    /// Websocket class for the vibration tasks
    /// </summary>
    class VibrationWebsocket: WebSocketBehavior
    {
        #region Private Members

        /// <summary>
        /// esp server used for communication
        /// </summary>
        private readonly BleDevice mServer;

        #endregion

        #region Public Events

        /// <summary>
        /// Fired when a client connects to the websocket server
        /// </summary>
        public event Action ClientConnected = () => { };

        /// <summary>
        /// Fired when a client disconnects from the websocket server
        /// </summary>
        public event Action ClientDisconnected = () => { };

        #endregion

        #region Constructor 

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public VibrationWebsocket() : this(null) { }

        /// <summary>
        /// Constructor with server to initialize communication
        /// </summary>
        /// <param name="server">Esp Server <see cref="BleDevice"/></param>
        public VibrationWebsocket(BleDevice server)
        {
            mServer = server;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Callback fired with every message received
        /// Sends message to the esp server
        /// </summary>
        /// <param name="e">Args <see cref="MessageEventArgs"/></param>
        protected override async void OnMessage(MessageEventArgs e)
        {
            string message = e.Data;
            Vibration vibration = JsonConvert.DeserializeObject<Vibration>(message);
            Console.WriteLine($"Received Message: {message}");
            if(vibration != null)
            {
                Send(await VibrateAsync(vibration) ? "OK" : "ERR");
            }
        }

        /// <summary>
        /// Callback executed when a client connects
        /// </summary>
        protected override void OnOpen()
        {
            ClientConnected();
        }

        /// <summary>
        /// Callback executed when a client closes the connection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClose(CloseEventArgs e)
        {
            ClientDisconnected();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sends vibration message to esp server to vibrate a single motor
        /// </summary>
        /// <param name="vibration">Vibration object <see cref="Vibration"/></param>
        /// <returns>Bool if data is send</returns>
        public async Task<bool> VibrateAsync(Vibration vibration)
        {
            string data = $"vibrate {(int) vibration.Id} {vibration.Duration}";
            return await mServer.SendData(data);
        }

        #endregion
    }
}
