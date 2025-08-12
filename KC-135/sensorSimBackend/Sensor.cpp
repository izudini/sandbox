#include "Sensor.h"

Sensor::Sensor(const std::string& name)
    : name(name), location(""), currentState(SensorState::Off) {
}

Sensor::Sensor(const std::string& name, const std::string& location)
    : name(name), location(location), currentState(SensorState::Off) {
}

const std::string& Sensor::getName() const {
    return name;
}

const std::string& Sensor::getLocation() const {
    return location;
}

SensorState Sensor::getCurrentState() const {
    return currentState;
}

void Sensor::setCurrentState(SensorState state) {
    currentState = state;
}