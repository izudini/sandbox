#ifndef SENSOR_H
#define SENSOR_H

#include <string>
#include "SensorState.h"

class Sensor {
private:
    std::string location;
    std::string name;
    SensorState currentState;

public:
    Sensor(const std::string& name);
    Sensor(const std::string& name, const std::string& location);
    
    const std::string& getName() const;
    const std::string& getLocation() const;
    SensorState getCurrentState() const;
    
    void setCurrentState(SensorState state);
};

#endif // SENSOR_H