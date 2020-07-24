using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace LinkVibrationController.BluetoothLE
{
    /// <summary>
    /// Wrapperclass for DeviceWatcher <see cref="DeviceWatcher"/>
    /// </summary>
    public class BleDeviceWatcher
    {
        #region Private Members

        /// <summary>
        /// Underlying DeviceWatcher <see cref="DeviceWatcher"/>
        /// </summary>
        private readonly DeviceWatcher mDeviceWatcher;

        /// <summary>
        /// List of properties to look up by the watcher
        /// </summary>
        private readonly string[] mRequestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

        /// <summary>
        /// Object for locking due to thread safety
        /// </summary>
        private readonly object mThreadLock = new object();

        #endregion

        #region Public Members

        /// <summary>
        /// Dictionary of found BLE devices <see cref="BleDevice"/>
        /// </summary>
        public static readonly Dictionary<string, BleDevice> Devices = new Dictionary<string, BleDevice>();

        /// <summary>
        /// Indicates id watcher is watching
        /// </summary>
        public bool Watching => mDeviceWatcher.Status == DeviceWatcherStatus.Started;

        #endregion

        #region Public Events

        /// <summary>
        /// Is fired when a new device gets added
        /// </summary>
        public event Action<BleDevice> DeviceAdded = (device) => { };

        /// <summary>
        /// Is fired when a device gets updated
        /// </summary>
        public event Action<string> DeviceUpdated = (id) => { };

        /// <summary>
        /// Is fired when a device gets removed
        /// </summary>
        public event Action<string> DeviceRemoved = (id) => { };

        public event Action<BleDevice> ServerFound = (device) => { };

        /// <summary>
        /// Is fired when watcher stopps
        /// </summary>
        public event Action Stopped = () => { };

        /// <summary>
        /// Is fired when watcher enumeration finishes
        /// </summary>
        public event Action EnumCompleted = () => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public BleDeviceWatcher()
        {
            mDeviceWatcher = DeviceInformation.CreateWatcher(
                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                mRequestedProperties,
                DeviceInformationKind.AssociationEndpoint);

            mDeviceWatcher.Added += Added;
            mDeviceWatcher.Updated += Updated;
            mDeviceWatcher.Removed += Removed;

            mDeviceWatcher.EnumerationCompleted += (sender, args) => 
            {
                EnumCompleted();
                mDeviceWatcher.Stop();
            };
            mDeviceWatcher.Stopped += (sender, args) => { Stopped(); };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Callback for when a device is removed
        /// </summary>
        /// <param name="sender">The Watcher</param>
        /// <param name="args">Arguments</param>
        private void Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Locking for thread safety
            lock(mThreadLock)
            {
                /// Remove device from dict
                Devices.Remove(args.Id);

                // Inform observers
                DeviceRemoved(args.Id);
            }
        }

        /// <summary>
        /// Callback for when a device is updated
        /// </summary>
        /// <param name="sender">The watcher</param>
        /// <param name="args">Arguments</param>
        private void Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // Locking for thread safety
            lock(mThreadLock)
            {
                // Update device properties
                Devices[args.Id].Properties = args.Properties;

                // Inform observers
                DeviceUpdated(args.Id);
            }
        }

        /// <summary>
        /// Callback for when a device is added/found
        /// </summary>
        /// <param name="sender">The watcher</param>
        /// <param name="args">Arguments</param>
        private async void Added(DeviceWatcher sender, DeviceInformation args)
        {
            BleDevice device = null;

            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(args.Id);

            // Locking for thread safety
            lock(mThreadLock)
            {
                // Construction new BleDevice
                device = new BleDevice(
                    id: args.Id,
                    name: args.Name,
                    properties: args.Properties,
                    paired: args.Pairing.IsPaired,
                    device: bluetoothLeDevice);

                // Add device to dictionary
                Devices[args.Id] = device;

                // Inform oberservers
                DeviceAdded(device);

                if(device.Server)
                {
                    ServerFound(device);
                    Stop();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to start the watcher
        /// </summary>
        public void Start()
        {
            if(mDeviceWatcher.Status == DeviceWatcherStatus.Started)
            {
                return;
            }

            mDeviceWatcher.Start();
        }

        /// <summary>
        /// Method to stop the watcher
        /// </summary>
        public void Stop()
        {
            if(mDeviceWatcher.Status == DeviceWatcherStatus.Stopped)
            {
                return;
            }

            mDeviceWatcher.Stop();
        }

        /// <summary>
        /// Method to close connection to all ble devices
        /// </summary>
        public void ClearDevices()
        {
            foreach(KeyValuePair<string, BleDevice> device in Devices)
            {
                device.Value.Disconnect();
            }

            Devices.Clear();
        }

        #endregion
    }
}
