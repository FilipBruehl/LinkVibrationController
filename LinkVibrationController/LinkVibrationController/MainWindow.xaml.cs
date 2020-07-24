using LinkVibrationController.BluetoothLE;
using LinkVibrationController.EspModels;
using LinkVibrationController.WebsocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebSocketSharp.Server;

namespace LinkVibrationController
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Private Members

        /// <summary>
        /// Used BleDeviceWatcher to find nearby ble devices <see cref="BleDeviceWatcher"/>
        /// </summary>
        private BleDeviceWatcher watcher;

        /// <summary>
        /// BleDevice to hold the found ble server <see cref="BleDevice"/>
        /// </summary>
        private BleDevice espServer;

        /// <summary>
        /// Used WebSocketServer <see cref="WebSocketServer"/>
        /// </summary>
        private WebSocketServer wsServer;

        /// <summary>
        /// Used WebSocketBehavior for the websocket server <see cref="VibrationWebsocket"/> <seealso cref="WebSocketBehavior"/>
        /// </summary>
        private VibrationWebsocket wsVibration;

        #endregion

        #region Constructor

        /// <summary>
        /// Public Standard Constructor
        /// Initializes the UI, watcher, wsServer, wsVibration and corresponding callback methods
        /// Searches for ble devices and the server
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            watcher = new BleDeviceWatcher();
            wsServer = new WebSocketServer("ws://127.0.0.1");

            // Intercept ServerFound event from watcher
            watcher.ServerFound += (device) =>
            {
                // save server to espServer
                espServer = device;

                // Use Dispatcher to acces UI thread
                Dispatcher.Invoke(() =>
                {
                    blestatus.Foreground = Brushes.Green;
                    blestatus.Text = $"Verbunden mit {espServer.Name}";
                });

                // Stop watcher because server is found
                watcher.Stop();

                // Use Dispatcher to acces UI thread
                Dispatcher.Invoke(() =>
                {
                    wssstatus.Foreground = Brushes.Yellow;
                    wssstatus.Text = "Starte Websocket Server...";
                });

                wsVibration = new VibrationWebsocket(device);

                // Intercept ClientConnected event from wsVibration
                wsVibration.ClientConnected += () =>
                {
                    // Use Dispatcher to access UI thread
                    Dispatcher.Invoke(() =>
                    {
                        wssstatus.Text = "Client ist verbunden";
                        DisableTest();
                    });
                };

                // Intercept ClientDisconnected event from wsVibration
                wsVibration.ClientDisconnected += () =>
                {
                    // User Dispatcher to access UI thread
                    Dispatcher.Invoke(() =>
                    {
                        wssstatus.Text = "Client nicht verbunden";
                        EnableTest();
                    });
                };

                // Add wsVibration service to the wsServer
                wsServer.AddWebSocketService("/vibrate", () => { return wsVibration; });

                wsServer.Start();

                // If server was successfully started
                if(wsServer.IsListening)
                {
                    // Use Dispatcher to access UI thread
                    Dispatcher.Invoke(() =>
                    {
                        wssstatus.Foreground = Brushes.Green;
                        wssstatus.Text = $"Websocket hört auf {wsServer.Address}:{wsServer.Port}";

                        DisableConnect();
                        EnableTest();
                    });
                } 
                // If server wasn't successfully started
                else
                {
                    // Use Dispatcher to access UI thread
                    Dispatcher.Invoke(() =>
                    {
                        wssstatus.Foreground = Brushes.Red;
                        wssstatus.Text = "Es ist ein Fehler aufgetreten";
                    });
                }
            };

        }

        #endregion

        #region Private Callbacks

        /// <summary>
        /// Fired when connect-Button is clicked
        /// Changes UI and starts ble watcher
        /// </summary>
        /// <param name="sender">The clicked button</param>
        /// <param name="e">Args</param>
        private void connect_Click(object sender, RoutedEventArgs e)
        {
            blestatus.Foreground = Brushes.Yellow;
            blestatus.Text = "Suche nach Server...";
            watcher.Start();
        }

        /// <summary>
        /// Fired when disconnect-button is clicked
        /// Cleares found ble devices and server, stops websocket server and changes UI
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            // Disconnect and clear all found ble devices including server
            watcher.ClearDevices();

            // reset esp server
            espServer = null;

            // Change UI
            blestatus.Foreground = Brushes.Red;
            blestatus.Text = "Nicht verbunden";

            // Stop websocket server
            wsServer.Stop();

            // If wsServer was successfully stopped
            if(!wsServer.IsListening)
            {
                // Change UI
                wssstatus.Foreground = Brushes.Red;
                wssstatus.Text = "Nicht verbunden";

                EnableConnect();
                DisableTest();
            }
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 1
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test1_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(0);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 2
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test2_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(1);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 3
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test3_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(2);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 4
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test4_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(3);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 5
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test5_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(4);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 6
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test6_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(5);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 7
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test7_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(6);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 8
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test8_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(7);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 9
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test9_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(8);
        }

        /// <summary>
        /// Fired with button click
        /// Vibrates motor 10
        /// </summary>
        /// <param name="sender">Clicked button</param>
        /// <param name="e">Args</param>
        private void test10_Click(object sender, RoutedEventArgs e)
        {
            Vibrate(9);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Enables connect-button and disables disconnect-button
        /// </summary>
        private void EnableConnect()
        {
            connect.IsEnabled = true;
            disconnect.IsEnabled = false;
        }

        /// <summary>
        /// Disables connect-button and enables disconnect-button
        /// </summary>
        private void DisableConnect()
        {
            connect.IsEnabled = false;
            disconnect.IsEnabled = true;
        }

        /// <summary>
        /// Enables all test buttons
        /// </summary>
        private void EnableTest()
        {
            test1.IsEnabled = true;
            test2.IsEnabled = true;
            test3.IsEnabled = true;
            test4.IsEnabled = true;
            test5.IsEnabled = true;
            test6.IsEnabled = true;
            test7.IsEnabled = true;
            test8.IsEnabled = true;
            test9.IsEnabled = true;
            test10.IsEnabled = true;
        }

        /// <summary>
        /// Disables all test buttons
        /// </summary>
        private void DisableTest()
        {
            test1.IsEnabled = false;
            test2.IsEnabled = false;
            test3.IsEnabled = false;
            test4.IsEnabled = false;
            test5.IsEnabled = false;
            test6.IsEnabled = false;
            test7.IsEnabled = false;
            test8.IsEnabled = false;
            test9.IsEnabled = false;
            test10.IsEnabled = false;
        }

        /// <summary>
        /// Vibrates motor
        /// </summary>
        /// <param name="id">Id of motor to vibrate</param>
        private async void Vibrate(int id)
        {
            // If wsVibration was instantiated correctly
            if(wsVibration != null)
            {
                // Create new Vibration object
                Vibration vib = new Vibration();
                vib.Duration = 500;
                vib.Id = (VibratorId)id;

                // Vibrate using wsVibration
                wsVibration.VibrateAsync(vib);
            } 
            // If wsVibration wasn't instantiated correclty
            else
            {
                // Create message to send
                string msg = $"vibrate {id} 500";

                //send message directly to the server
                espServer.SendData(msg);
            }
        }

        #endregion
    }
}
