#include "vibration.h"

Vibration::Vibration(const Commands& commands): _id(convert_string_int(commands[1])), _duration(convert_string_int(commands[2])) { }

Vibration::~Vibration() {}

void Vibration::set_id(const int& id) {
    this->_id = id;
}

void Vibration::set_duration(const int& duration) {
    this->_duration = duration;
}

const int& Vibration::get_id() const {
    return this->_id;
}

const int& Vibration::get_duration() const {
    return this->_duration;
}