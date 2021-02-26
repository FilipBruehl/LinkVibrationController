#include "pin.h"

Pin::Pin(const int& pin): _pin(pin), _running(false) { };
Pin::~Pin() { };

void Pin::setRunning(const bool running) {
    this->_running = running;
}

const int& Pin::getPin() const { 
    return this->_pin;
}

const bool& Pin::getRunning() const {
    return this->_running;
}