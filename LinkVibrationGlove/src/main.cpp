#include "Arduino.h"

#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>

#include "vibration.h"
#include "pin.h"

#define SERVICE_UUID        "6E400002-B5A3-F393-E0A9-E50E24DCCA9E"
#define CHARACTERISTIC_UUID "6E400002-B5A3-F393-E0A9-E50E24DCCA9E"


BLEServer* pServer = NULL;
BLEService* pService = NULL;
BLECharacteristic* pCharacteristic;
bool deviceConnected = false;
std::string message;

std::vector<Pin*> pins{new Pin(16), new Pin(17), new Pin(5), new Pin(21), new Pin(3), new Pin(14), new Pin(27), new Pin(33), new Pin(32), new Pin(35)};

void vibrateCode(void* pvParameters) {
    Vibration vib = *(Vibration*) pvParameters;
    Pin* pin = pins[vib.get_id()];
    if(pin->getRunning() == true) {
        Serial.printf("Pin {%id} already vibrating!\n", pin->getPin());
    } else {
        Serial.printf("Starting Vibration {%i} {%i} on pin {%i}\n", vib.get_id(), vib.get_duration(), pin->getPin());
        pin->setRunning(true);
        digitalWrite(pin->getPin(), HIGH);
        delay(vib.get_duration());
        digitalWrite(pin->getPin(), LOW);
        pin->setRunning(false);
        Serial.printf("Vibration {%i} finished\n", vib.get_id());
    }
    vTaskDelete(NULL);
}

class ServerCallbacks: public BLEServerCallbacks {
  void onConnect(BLEServer* pServer) {
    deviceConnected = true;
    Serial.printf("Device connected: %i \n", pServer->getConnId());
  }
  void onDisconnect(BLEServer* pServer) {
    deviceConnected = false;
    Serial.println("Device disconnected.");
  }
};

class Callbacks: public BLECharacteristicCallbacks {
  void onWrite(BLECharacteristic* pCharacteristic) {
    message = pCharacteristic->getValue();
    if(message.length() > 0) {
      Serial.print("Message received: ");
      for(int i = 0; i < message.length(); i++) {
        Serial.print(message[i]);
      }
      Serial.println();
      if(message.rfind("vibrate") == 0) {
        Commands commands = split(message);
        Vibration* vib = new Vibration(commands);
        Serial.printf("Vibration {%i} {%i}\n", vib->get_id(), vib->get_duration());
        TaskHandle_t task;
        xTaskCreatePinnedToCore(
          &vibrateCode,
          "Vibrate",
          2048,
          (void*) vib,
          1,
          &task,
          0);
      }
    }
  }
};



void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  
  Serial.println("init pins...");

  for(const Pin* pin: pins) {
    pinMode(pin->getPin(), OUTPUT);
    digitalWrite(pin->getPin(), LOW);
  }

  Serial.println("init pins finished");

  Serial.println("Starting Server...");

  BLEDevice::init("LinkGlove");

  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new ServerCallbacks());

  pService = pServer->createService(SERVICE_UUID);
  pCharacteristic = pService->createCharacteristic(CHARACTERISTIC_UUID, BLECharacteristic::PROPERTY_READ | BLECharacteristic::PROPERTY_WRITE);
  pCharacteristic->setCallbacks(new Callbacks());
  pService->start();
  pServer->getAdvertising()->start();
  Serial.println("Started...");
}

void loop() {
  // put your main code here, to run repeatedly:
  delay(200);
}