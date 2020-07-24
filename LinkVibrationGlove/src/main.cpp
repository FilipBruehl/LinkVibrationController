#include <Arduino.h>

#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>

#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

#include <sstream>
#include <iostream>
#include <vector>


#define SERVICE_UUID        "6E400002-B5A3-F393-E0A9-E50E24DCCA9E"
#define CHARACTERISTIC_UUID "6E400002-B5A3-F393-E0A9-E50E24DCCA9E"


BLEServer* pServer = NULL;
BLEService* pService = NULL;
BLECharacteristic* pCharacteristic;
bool deviceConnected = false;
std::string message;

std::vector<int> pins{15, 2, 4, 5, 18};

std::vector<std::string> split(const std::string& string) {
  std::vector<std::string> strings;
  std::istringstream f(string);
  std::string s;
  while(std::getline(f, s, ' ')) {
    strings.push_back(s);
  }
  return strings;
};

int convert_string_int(const std::string& string) {
    std::stringstream s(string);
    int value;
    s >> value;
    return value;
};

class Vibration {
    private:
        int id;
        int duration;

    public:
        Vibration(const std::vector<std::string>& commands) {
          this->id = convert_string_int(commands[1]);
          this->duration = convert_string_int(commands[2]);
        };
        ~Vibration() {};

        int const& get_id() const { return this->id; };
        int const& get_duration() const { return this->duration; };
};

void VibrateCode(void* pvParameters) {
  Vibration vib = *(Vibration*) pvParameters;
  Serial.printf("Starting Vibration {%i} {%i}\n", vib.get_id(), vib.get_duration());
  digitalWrite(pins[vib.get_id()], HIGH);
  delay(vib.get_duration());
  digitalWrite(pins[vib.get_id()], LOW);
  Serial.printf("Vibration {%i} finished\n", vib.get_id());
  vTaskDelete(NULL);
};

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
        std::vector<std::string> commands = split(message);
        Vibration* vib = new Vibration(commands);
        Serial.printf("Vibration {%i} {%i}\n", vib->get_id(), vib->get_duration());
        TaskHandle_t task;
        xTaskCreatePinnedToCore(
          &VibrateCode,
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

  for(const auto& pin: pins) {
    pinMode(pin, OUTPUT);
    digitalWrite(pin, LOW);
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