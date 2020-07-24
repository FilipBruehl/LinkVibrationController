using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace LinkVibrationController.BluetoothLE
{
    /// <summary>
    /// Wrapper for BluetoothLEDevice <see cref="BluetoothLEDevice"/>
    /// </summary>
    public class BleDevice
    {
        #region Private Properties

        /// <summary>
        /// Underlying BluetoothLEDevice <see cref="BluetoothLEDevice"/>
        /// </summary>
        private readonly BluetoothLEDevice mBluetoothLEDevice;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of esp32 server
        /// </summary>
        public static readonly string ServerName = "ESP32 Server";

        /// <summary>
        /// The Uuid of the server service and characteristic
        /// </summary>
        public static readonly Guid ServerUuid = new Guid("6E400002-B5A3-F393-E0A9-E50E24DCCA9E");

        /// <summary>
        /// Id of the BLE device
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Name of BLE device
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Properties of BLE device
        /// </summary>
        public IReadOnlyDictionary<string, object> Properties { set; get; }

        /// <summary>
        /// Pairingstatus of BLE device
        /// </summary>
        public bool Paired { get; }

        /// <summary>
        /// Indicates if the device found is the server
        /// </summary>
        public bool Server { get; }

        public GattCharacteristic Characteristic { private set; get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Bluetooth Id for the device</param>
        /// <param name="name">Name of the BLE device</param>
        /// <param name="properties">Properties of the BLE device</param>
        /// <param name="paired">Pairingstatus</param>
        /// <param name="device">BluetoothBLEDevice property <see cref="BluetoothLEDevice"/></param>
        public BleDevice(string id, string name, IReadOnlyDictionary<string, object> properties, bool paired, BluetoothLEDevice device)
        {
            Id = id;
            Name = name;
            Properties = properties;
            Paired = paired;
            mBluetoothLEDevice = device;
            Server = IsServer().Result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines if the device is the server
        /// </summary>
        /// <returns></returns>
        private async Task<bool> IsServer()
        {
            GattCharacteristic characteristic = await GetCharacteristic(BleDevice.ServerUuid);
            if(characteristic != null)
            {
                Characteristic = characteristic;
                return true;
            } else
            {
                characteristic.Service.Dispose();
                return false;
            }
        }

        /// <summary>
        /// Gets characteristic for uuid
        /// </summary>
        /// <param name="uuid">Uuid for characteristic</param>
        /// <returns></returns>
        private async Task<GattCharacteristic> GetCharacteristic(Guid uuid)
        {
            GattCharacteristic characteristic = null;

            GattDeviceServicesResult servicesResult = await mBluetoothLEDevice.GetGattServicesForUuidAsync(uuid);
            if(servicesResult.Status == GattCommunicationStatus.Success)
            {
                GattCharacteristicsResult characteristicsResult = await servicesResult.Services[0].GetCharacteristicsForUuidAsync(uuid);
                if(characteristicsResult.Status == GattCommunicationStatus.Success)
                {
                    if(characteristicsResult.Characteristics.Count == 1)
                    {
                        characteristic = characteristicsResult.Characteristics[0];
                    }
                }
            }
            return characteristic;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Better toString for logging
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} {Id} {Paired}";
        }

        /// <summary>
        /// Function to send data to the device
        /// </summary>
        /// <param name="message">Message to be send</param>
        /// <returns>Bool if data was send</returns>
        public async Task<bool> SendData(string message)
        {
            if(Characteristic != null)
            {
                DataWriter writer = new DataWriter();
                writer.WriteString(message);

                Console.WriteLine("Sending");
                GattCommunicationStatus result = await Characteristic.WriteValueAsync(writer.DetachBuffer());
                if (result == GattCommunicationStatus.Success)
                {
                    return true;
                }
                return false;
            }
            return false;
            
        }

        /// <summary>
        /// Disconnect from the ble device and close open characteristics
        /// </summary>
        public void Disconnect()
        {
            if(Characteristic != null)
            {
                Characteristic.Service.Dispose();
            }
            mBluetoothLEDevice?.Dispose();
        }

        #endregion
    }
}
